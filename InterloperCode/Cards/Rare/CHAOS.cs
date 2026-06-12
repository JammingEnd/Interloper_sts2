using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Helpers;
using Interloper.InterloperCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Cards.Rare;

public class CHAOS() : InterloperCard(3,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var exhaustPile = PileType.Exhaust.GetPile(Owner);
        var exhaustCards = exhaustPile.GetOldestPlayableCards(includeConsumed: true);

        var nonExhaustCards = new List<CardModel>();
        nonExhaustCards.AddRange(PileType.Hand.GetPile(Owner).Cards);
        nonExhaustCards.AddRange(PileType.Draw.GetPile(Owner).Cards);
        nonExhaustCards.AddRange(PileType.Discard.GetPile(Owner).Cards);

        foreach (var card in exhaustCards)
            await CardPileCmd.Add(card, PileType.Hand);

        foreach (var card in nonExhaustCards)
        {
            CardCmd.ApplyKeyword(card, InterloperKeywords.Consumed);
            await CardCmd.Exhaust(choiceContext, card);
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromKeyword(InterloperKeywords.Consumed)
        ];

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}