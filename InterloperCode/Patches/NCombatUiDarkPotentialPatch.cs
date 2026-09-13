using Godot;
using HarmonyLib;
using Interloper.InterloperCode.Field;
using Interloper.InterloperCode.Potential;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Runs;
using InterloperCharacter = Interloper.InterloperCode.Character.Interloper;

namespace Interloper.InterloperCode.Patches;

/* =====================================================================================
 * Discovery spike: how the base game enqueues a networked player action from a UI button.
 *
 * A GameAction (MegaCrit.Sts2.Core.GameActions) is a thin wrapper around an async Task
 * that runs in response to player input, INSIDE the game's deterministic action pipeline.
 * The End Turn button follows this pattern: it enqueues an EndPlayerTurnAction via
 * ActionQueueSynchronizer.RequestEnqueue(GameAction) (also see DevConsole.ProcessCommand,
 * which enqueues a ConsoleCmdGameAction the same way for networked console commands).
 *
 * To subclass GameAction you implement:
 *   - OwnerId                 -> the acting Player's NetId
 *   - ActionType              -> GameActionType.CombatPlayPhaseOnly defers the action until
 *                                the local player's play phase (else the synchronizer holds it)
 *   - ExecuteAction()         -> the actual logic, run inside the pipeline with a PlayerChoiceContext
 *   - ToNetAction()           -> an INetAction used to serialize/broadcast the action to all peers
 *
 * INetAction is a [GenerateSubtypes] interface whose subtype registry is baked at compile
 * time, so a mod cannot ship a brand-new net action type. We therefore reuse the built-in
 * NetConsoleCmdGameAction (which reconstructs to a ConsoleCmdGameAction): on the peer that
 * pressed the button, our custom DarkPotentialClearAction runs DarkPotentialCmd.Clear directly;
 * the networked copy reconstructs as a ConsoleCmdGameAction and runs the registered
 * DarkPotentialClearConsoleCmd (auto-discovered via ReflectionHelper.GetSubtypesInMods<AbstractConsoleCmd>),
 * which performs the SAME DarkPotentialCmd.Clear with a GameActionPlayerChoiceContext.
 * Both paths converge on the same deterministic logic - never a raw UI callback.
 *
 * Enqueue point (button press):
 *   RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new DarkPotentialClearAction(player));
 * ===================================================================================== */

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
internal class NCombatUiDarkPotentialPatch
{
    [HarmonyPostfix]
    private static void Postfix(NCombatUi __instance, CombatState state)
    {
        var me = LocalContext.GetMe(state);
        if (me == null)
            return;

        if (me.Character is not InterloperCharacter)
            return;

        var bar = DarkPotentialNode.NDarkPotentialBar[__instance];
        bar.Initialize(me);
        bar.Reparent(__instance._energyCounter);
        bar.Position = new Vector2(-24f, -100f);

        var existingButton = __instance._energyCounter.GetNodeOrNull<Button>("DarkPotentialClearButton");
        if (existingButton != null)
        {
            __instance._energyCounter.RemoveChild(existingButton);
            existingButton.QueueFree();
        }

        var clearButton = new Button
        {
            Name = "DarkPotentialClearButton",
            Text = "\u2715",
            Size = new Vector2(44f, 44f),
            Position = new Vector2(146f, 40f)
        };
        __instance._energyCounter.AddChild(clearButton);
        clearButton.Pressed += () => RequestClear(me);
    }

    private static void RequestClear(Player player)
    {
        RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new DarkPotentialClearAction(player));
    }
}

/// <summary>
/// Custom networked player action for the clear button. Runs <see cref="DarkPotentialCmd.Clear"/>
/// inside the deterministic action pipeline with a <see cref="GameActionPlayerChoiceContext"/>.
/// <see cref="ToNetAction"/> reuses the built-in NetConsoleCmdGameAction because mods cannot
/// register new <c>[GenerateSubtypes]</c> net action types; remote peers reconstruct that net
/// action as a <see cref="ConsoleCmdGameAction"/> running <see cref="DarkPotentialClearConsoleCmd"/>.
/// </summary>
public class DarkPotentialClearAction : GameAction
{
    private readonly Player _player;

    public override ulong OwnerId => _player.NetId;

    public override GameActionType ActionType => GameActionType.CombatPlayPhaseOnly;

    public DarkPotentialClearAction(Player player)
    {
        _player = player;
    }

    protected override Task ExecuteAction()
    {
        return DarkPotentialCmd.Clear(new GameActionPlayerChoiceContext(this), _player);
    }

    public override INetAction ToNetAction()
    {
        return new NetConsoleCmdGameAction
        {
            cmd = DarkPotentialClearConsoleCmd.Command,
            inCombat = true
        };
    }

    public override string ToString()
    {
        return $"DarkPotentialClearAction for player {_player.NetId}";
    }
}

/// <summary>
/// Backs the networked (remote-peer / reconstructed) path of <see cref="DarkPotentialClearAction"/>.
/// Auto-registered with the dev console via <c>ReflectionHelper.GetSubtypesInMods&lt;AbstractConsoleCmd&gt;</c>;
/// performs the same <see cref="DarkPotentialCmd.Clear"/> as the custom action.
/// </summary>
public class DarkPotentialClearConsoleCmd : AbstractConsoleCmd
{
    public const string Command = "interloper_darkpotential_clear";

    public override string CmdName => Command;

    public override string Args => "";

    public override string Description => "Clear the player's Dark Potential, activating the highest reached level.";

    public override bool IsNetworked => true;

    public override bool DebugOnly => false;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (issuingPlayer == null)
            return new CmdResult(success: false, "No issuing player for Dark Potential clear.");

        var runningAction = RunManager.Instance.ActionExecutor.CurrentlyRunningAction
            ?? new ConsoleCmdGameAction(issuingPlayer, Command, inCombat: true);
        var ctx = new GameActionPlayerChoiceContext(runningAction);
        return new CmdResult(DarkPotentialCmd.Clear(ctx, issuingPlayer), success: true, "Dark Potential cleared.");
    }
}