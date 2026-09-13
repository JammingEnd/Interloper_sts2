using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Interloper.InterloperCode.Cards.Uncommon;

public class EmpowerCursePower : InterloperPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterBlockCleared(Creature creature)
    {
        if (creature != Owner && creature.Player == null)
            return;
        await PlayerCmd.GainEnergy(this.Amount, creature.Player);
    }
}