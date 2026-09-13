using Interloper.InterloperCode.Powers;
using Interloper.InterloperCode.Potential;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Interloper.InterloperCode.Keywords;
using MegaCrit.Sts2.Core.HoverTips;

namespace Interloper.InterloperCode.Cards.Ancient;

public class TouchOfFaith() : InterloperCard(2, CardType.Power, CardRarity.Ancient, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<TouchOfFaithPower>("TouchOfFaithPower", 1),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TouchOfFaithPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        await DarkPotentialCmd.SetAutoUseThresholds(choiceContext, Owner, true);
    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Innate);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<TouchOfFaithPower>(),
        HoverTipFactory.FromKeyword(InterloperKeywords.DarkPotential)
    ];

}