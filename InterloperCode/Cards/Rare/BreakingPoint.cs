using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Interloper.InterloperCode.Cards.Rare;

public class BreakingPoint() : InterloperCard(2,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(1),
        new ExtraDamageVar(2),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((Func<CardModel, Creature, Decimal>) 
                ((_, target) => (target != null ? target.GetPowerAmount<StrengthPower>() * 2 : 0))),
        new PowerVar<BreakingPointPower>("BreakingPointPower", 10)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<BreakingPointPower>(
            choiceContext, play.Target, DynamicVars["BreakingPointPower"].IntValue, Owner.Creature, this);
        decimal damage = Math.Abs(play.Target.GetPowerAmount<StrengthPower>() * 2);
        await CreatureCmd.Damage(choiceContext, play.Target, damage, ValueProp.Move, this);
        
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BreakingPointPower"].UpgradeValueBy(2);
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<BreakingPointPower>()
        ];

}
