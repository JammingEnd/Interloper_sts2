using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Helpers;
using Interloper.InterloperCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class OuterReach() : InterloperCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AnyAlly)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var targetPlayer = play.Target.Player;
        var exhaustPile = PileType.Exhaust.GetPile(targetPlayer);
        if (exhaustPile.Cards.Count == 0)
            return;

        var oldestCard = exhaustPile.GetOldestPlayableCard();
        if (oldestCard == null)
            return;

        oldestCard.EnergyCost.AddUntilPlayed(-1, true);
        oldestCard.AddKeyword(InterloperKeywords.Consumed);
        await CardPileCmd.Add(oldestCard, PileType.Hand);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromKeyword(InterloperKeywords.Consumed)
        ];

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}