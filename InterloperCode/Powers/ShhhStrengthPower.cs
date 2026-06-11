using Interloper.InterloperCode.Cards.Uncommon;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Powers;

public class ShhhStrengthPower() : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => (AbstractModel)ModelDb.Card<Shhhh>();

    protected override bool IsPositive => false;
}