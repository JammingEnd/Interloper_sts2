using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.HoverTips;

namespace Interloper.InterloperCode.Cards.Rare;

public class BreakingPoint() : InterloperCard(2,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    private int _strengthLoss = 8;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(
            (_, target) =>
            {
                int after = target.GetPowerAmount<StrengthPower>() - _strengthLoss;
                return after < 0 ? (Decimal)(2 * -after) : 0M;
            })
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<BreakingPointPower>(
            choiceContext, play.Target, _strengthLoss, Owner.Creature, this);

        await CommonActions.CardAttack(this, play).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        _strengthLoss = 10;
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<BreakingPointPower>()
        ];

}
