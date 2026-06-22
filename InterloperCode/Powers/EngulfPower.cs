using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Interloper.InterloperCode.Powers;

public class EngulfPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    
}