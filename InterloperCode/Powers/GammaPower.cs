using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;

namespace Interloper.InterloperCode.Powers;

// void corruption now also increases the intensity of other debuffs
public class GammaPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Debuff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        base.CanonicalVars.Concat([
            new IntVar("Removal", 0),
            new IntVar("DamageIncrease", 0)
        ]);

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power != this)
            return;

        UpdateRemoval();
        UpdateDamage();
    }

    private void UpdateRemoval()
    {
        int amount = this.Amount;
        DynamicVars["Removal"].BaseValue = amount * Math.Min(amount, 75) / 100;
    }

    private void UpdateDamage()
    {
        int amount = this.Amount;
        int percent = Math.Min(amount, 75);
        DynamicVars["DamageIncrease"].BaseValue = (int)(amount * (percent * 0.02));
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
            return;

        int percent = Math.Min(this.Amount, 75);
        int removed = this.Amount * percent / 100;
        if (removed <= 0)
            return;

        await PowerCmd.Apply<GammaPower>(choiceContext, Owner, -removed, Owner, null);
    }

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target != this.Owner || !props.IsPoweredAttack())
            return 1M;
        int percent = Math.Min(this.Amount, 75);
        decimal amount1 = (decimal)(this.Amount * (percent * 0.02)); 
        if (amount1 <= 0)
        {
            return 1m;
        }
        return 1 + (amount1 / 100);
    }
}