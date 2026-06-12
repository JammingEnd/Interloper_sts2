using Interloper.InterloperCode.Cards.Glyph;
using Interloper.InterloperCode.Keywords;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Interloper.InterloperCode.Powers;

public class LastWordsPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Keywords.Contains(InterloperKeywords.Consumed))
        {
            var mouthCard = CombatState.CreateCard<GlyphMouth>(this.Owner.Player);
            await CardPileCmd.AddGeneratedCardToCombat(mouthCard, PileType.Hand, this.Owner.Player);
        }
    }
}