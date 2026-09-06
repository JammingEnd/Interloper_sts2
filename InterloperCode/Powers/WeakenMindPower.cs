using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Powers;

public class WeakenMindPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier,
        out decimal modifiedAmount)
    {
        if (canonicalPower.GetType() != typeof(AbyssalCorruptionPower))
        {
            modifiedAmount = amount;
            return false;
        }

        if (applier != this.Owner)
        {
            modifiedAmount = amount;
            return false;
        }

        modifiedAmount = amount * this.Amount;
        return true;
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power.GetType() != typeof(AbyssalCorruptionPower))
            return;

        if (applier != Owner)
            return;

        if (amount <= 0)
            return;

        await PowerCmd.Remove(this);
    }
}