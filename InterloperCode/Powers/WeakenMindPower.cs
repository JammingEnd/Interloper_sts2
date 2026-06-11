using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Powers;

public class WeakenMindPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override decimal ModifyPowerAmountGivenMultiplicative(PowerModel power, Creature giver, decimal amount, Creature? target,
        CardModel? cardSource)
    {
        if (power.GetType() == typeof(AbyssalCorruptionPower) && giver == Owner)
        {
           return this.Amount;
        }
        return 1;
    }
}