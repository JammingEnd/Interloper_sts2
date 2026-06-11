using BaseLib.Utils;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Cards.Rare;

public class Unnatural() : CorruptionHandlerCard(15, 1,
    CardType.Attack, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    private int _baseDamage = 10;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(
            (_, target) =>
                target.GetPowerAmount<AbyssalCorruptionPower>() >= 15
                    ? _baseDamage * 2
                    : _baseDamage),
        new PowerVar<WeakPower>("WeakPower", 2),
        new PowerVar<VulnerablePower>("VulnerablePower", 2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(play.Target)
            .Execute(choiceContext);

        await PowerCmd.Apply<WeakPower>(
            choiceContext, play.Target,
            DynamicVars["WeakPower"].IntValue, Owner.Creature, this);

        await PowerCmd.Apply<VulnerablePower>(
            choiceContext, play.Target,
            DynamicVars["VulnerablePower"].IntValue, Owner.Creature, this);
    }

    protected override async Task CorruptionConsumptionEffect(
        PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<WeakPower>(
            choiceContext, play.Target,
            DynamicVars["WeakPower"].IntValue, Owner.Creature, this);

        await PowerCmd.Apply<VulnerablePower>(
            choiceContext, play.Target,
            DynamicVars["VulnerablePower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        _baseDamage += 5;
        DynamicVars["WeakPower"].UpgradeValueBy(1m);
        DynamicVars["VulnerablePower"].UpgradeValueBy(1m);
    }
}