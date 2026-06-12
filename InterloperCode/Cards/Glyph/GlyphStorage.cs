using BaseLib.Utils;
using Interloper.InterloperCode.Entries;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Glyph;

public class GlyphStorage() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power.GetType() != typeof(GlyphStorage))
            return;

        if (amount <= 0)
            return;

        var glyphsOnPlayer = Owner.Powers.OfType<GlyphPower>().ToArray();
        if (glyphsOnPlayer.Length < 3)
            return;

        int eyes = glyphsOnPlayer.Count(g => g is GlyphEyePower);
        int mouths = glyphsOnPlayer.Count(g => g is GlyphMouthPower);
        int tails = glyphsOnPlayer.Count(g => g is GlyphTailPower);

        var effectTask = (eyes, mouths, tails) switch
        {
            (3, 0, 0) => ThreeEyes(choiceContext),
            (0, 3, 0) => ThreeMouths(choiceContext),
            (0, 0, 3) => ThreeTails(choiceContext),
            (2, 1, 0) => TwoEyesOneMouth(choiceContext),
            (2, 0, 1) => TwoEyesOneTail(choiceContext),
            (1, 2, 0) => OneEyeTwoMouths(choiceContext),
            (0, 2, 1) => TwoMouthsOneTail(choiceContext),
            (1, 0, 2) => OneEyeTwoTails(choiceContext),
            (0, 1, 2) => OneMouthTwoTails(choiceContext),
            (1, 1, 1) => OneOfEach(choiceContext),
            _ => Task.CompletedTask
        };
        await effectTask;

        await PowerCmd.Remove<GlyphPower>(Owner);

        var entry = new SequenceActivatedEntry(
            2, Owner.Player, CombatState.RoundNumber,
            CombatState.CurrentSide, CombatManager.Instance.History,
            CombatState.Players);

        await PowerCmd.Remove<GlyphStorage>(Owner);
    }

    private static async Task ThreeEyes(PlayerChoiceContext choiceContext) { }
    private static async Task ThreeMouths(PlayerChoiceContext choiceContext) { }
    private static async Task ThreeTails(PlayerChoiceContext choiceContext) { }
    private static async Task TwoEyesOneMouth(PlayerChoiceContext choiceContext) { }
    private static async Task TwoEyesOneTail(PlayerChoiceContext choiceContext) { }
    private static async Task OneEyeTwoMouths(PlayerChoiceContext choiceContext) { }
    private static async Task TwoMouthsOneTail(PlayerChoiceContext choiceContext) { }
    private static async Task OneEyeTwoTails(PlayerChoiceContext choiceContext) { }
    private static async Task OneMouthTwoTails(PlayerChoiceContext choiceContext) { }
    private static async Task OneOfEach(PlayerChoiceContext choiceContext) { }
}