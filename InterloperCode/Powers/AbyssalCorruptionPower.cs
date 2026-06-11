using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Interloper.InterloperCode.Powers;

public class AbyssalCorruptionPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Debuff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    
}