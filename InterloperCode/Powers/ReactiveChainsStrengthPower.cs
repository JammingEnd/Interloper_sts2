using BaseLib.Abstracts;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Powers;

public class ReactiveChainsStrengthPower() : CustomTemporaryPowerModelWrapper<ReactiveChainsPower, StrengthPower>
{
    public override PowerType Type =>
        PowerType.Buff;
    
    protected override bool InvertInternalPowerAmount => false;
}