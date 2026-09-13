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
 * Networking: single path for the Dark Potential clear button.
 *
 * A GameAction (MegaCrit.Sts2.Core.GameActions) is a thin wrapper around an async Task
 * that runs in response to player input, INSIDE the game's deterministic action pipeline.
 * The base game's pattern for a networked UI button is to enqueue a ConsoleCmdGameAction
 * directly via ActionQueueSynchronizer.RequestEnqueue(GameAction) (see DevConsole.ProcessCommand).
 *
 * Button press:
 *   RequestClear -> RequestEnqueue(new ConsoleCmdGameAction(me, Command, inCombat: true))
 *   -> ActionQueueSynchronizer broadcasts it to every peer (serialized as NetConsoleCmdGameAction)
 *   -> on each peer the reconstructed ConsoleCmdGameAction runs DarkPotentialClearConsoleCmd
 *      (auto-discovered via ReflectionHelper.GetSubtypesInMods<AbstractConsoleCmd>)
 *   -> DarkPotentialClearConsoleCmd.Process runs the SAME DarkPotentialCmd.Clear with a
 *      GameActionPlayerChoiceContext wrapping the currently-running action.
 * Every peer runs the same deterministic logic - never a raw UI callback.
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
            Text = "X",
            Size = new Vector2(44f, 44f),
            Position = new Vector2(146f, 40f)
        };
        __instance._energyCounter.AddChild(clearButton);
        clearButton.Pressed += () => RequestClear(me);
    }

    private static void RequestClear(Player player)
    {
        RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(
            new ConsoleCmdGameAction(player, DarkPotentialClearConsoleCmd.Command, inCombat: true));
    }
}

/// <summary>
/// Backs the networked clear path. Auto-registered with the dev console via
/// <c>ReflectionHelper.GetSubtypesInMods&lt;AbstractConsoleCmd&gt;</c>; runs on every peer when
/// the clear button enqueues a <see cref="ConsoleCmdGameAction"/>. Performs the
/// <see cref="DarkPotentialCmd.Clear"/> with a <see cref="GameActionPlayerChoiceContext"/> wrapping
/// the currently-running action (the <see cref="ConsoleCmdGameAction"/> the executor is running).
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

        var runningAction = RunManager.Instance.ActionExecutor.CurrentlyRunningAction;
        if (runningAction == null)
            return new CmdResult(success: false, "No running action to perform Dark Potential clear.");

        var ctx = new GameActionPlayerChoiceContext(runningAction);
        return new CmdResult(DarkPotentialCmd.Clear(ctx, issuingPlayer), success: true, "Dark Potential cleared.");
    }
}