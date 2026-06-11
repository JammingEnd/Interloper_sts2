using Interloper.InterloperCode.Cards.Rare;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Powers;

public class BreakingPointPower() : TemporaryStrengthPower
{
    public override AbstractModel OriginModel =>
        ModelDb.Card<BreakingPoint>();

    protected override bool IsPositive => false;
}