using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Common;

// deal 10, exhaust
public class BorderOfnothing() : InterloperCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<VoidReachPower>("VoidReachPower", 3)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<VoidReachPower>(choiceContext, Owner.Creature, DynamicVars["VoidReachPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["VoidReachPower"].UpgradeValueBy(2m);
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VoidReachPower>()
    ];
}