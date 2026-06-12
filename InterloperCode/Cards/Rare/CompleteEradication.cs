using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Rare;

public class CompleteEradication() : CorruptionHandlerCard(40, 3,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(60),
        new ExtraDamageVar(0),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(
            (_, target) => target.GetPowerAmount<AbyssalCorruptionPower>() >= 40 ? 3M : 1M)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(play.Target)
            .Execute(choiceContext);
    }

    protected override Task CorruptionConsumptionEffect(
        PlayerChoiceContext choiceContext, CardPlay play) =>
        Task.CompletedTask;

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}