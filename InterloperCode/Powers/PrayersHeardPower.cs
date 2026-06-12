using Interloper.InterloperCode.Cards.Uncommon;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Powers;

public class PrayersHeardPower() : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => (AbstractModel)ModelDb.Card<PrayersHeard>();

    protected override bool IsPositive => false;
}