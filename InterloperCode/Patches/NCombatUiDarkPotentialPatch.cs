using Godot;
using HarmonyLib;
using Interloper.InterloperCode.Extensions;
using Interloper.InterloperCode.Field;
using Interloper.InterloperCode.Nodes;
using Interloper.InterloperCode.Potential;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Runs;
using InterloperCharacter = Interloper.InterloperCode.Character.Interloper;

namespace Interloper.InterloperCode.Patches;

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

        var clearButton = new NDarkPotentialClearButton
        {
            Name = "DarkPotentialClearButton",
            TextureNormal = PreloadManager.Cache.GetTexture2D("ui/combat/dark_potential/potential_button.png".ImagePath()),
            Size = new Vector2(40f, 40f),
            // Centered on the bar (bar center = Position + BarSize/2).
            Position = new Vector2(54f, -57f)
        };
        __instance._energyCounter.AddChild(clearButton);
        clearButton.Pressed += () => RequestClear(me);
        AttachClearHover(clearButton);
    }

    private static void AttachClearHover(NDarkPotentialClearButton button)
    {
        button.MouseEntered += () =>
        {
            var loc = new LocString("static_hover_tips", "INTERLOPER-DARK_POTENTIAL.clearButton");
            var hoverTip = new HoverTip(loc);
            var set = NHoverTipSet.CreateAndShow(button, hoverTip, HoverTip.GetHoverTipAlignment(button));
            set?.SetExtraFollowOffset(new Vector2(20f, -20f));
            set?.SetFollowOwner();
        };
        button.MouseExited += () => NHoverTipSet.Remove(button);
    }

    private static void RequestClear(Player player)
    {
        RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(
            new ConsoleCmdGameAction(player, DarkPotentialClearConsoleCmd.Command, inCombat: true));
    }
}

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