using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Ancient;

public class EndlessCycle() : CorruptionHandlerCard(20, 1, CardType.Skill, CardRarity.Ancient, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(8, ValueProp.Move),
        new PowerVar<VoidReachPower>("VoidReachPower",4),
        new PowerVar<VoidReachPower>("VoidReachPowerPerTurn",2),
    ];

    protected override async Task CorruptionConsumptionEffect(PlayerChoiceContext choiceContext, CardPlay play)
    {
       await PowerCmd.Apply<VoidReachPower>(choiceContext, Owner.Creature, DynamicVars["VoidReachPowerPerTurn"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars["VoidReachPowerPerTurn"].UpgradeValueBy(1m);
    }
}