using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Keywords;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class NeverAgain() : InterloperCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar("NeverAgainDraw", 1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<NeverAgainPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        await CardPileCmd.Draw(choiceContext, DynamicVars["NeverAgainDraw"].IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NeverAgainDraw"].UpgradeValueBy(1);
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromKeyword(InterloperKeywords.Consumed)
        ];

}