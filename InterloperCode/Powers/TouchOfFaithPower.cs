using MegaCrit.Sts2.Core.Entities.Powers;

namespace Interloper.InterloperCode.Powers;

public class TouchOfFaithPower : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public override PowerInstanceType InstanceType =>
        PowerInstanceType.Instanced;
}