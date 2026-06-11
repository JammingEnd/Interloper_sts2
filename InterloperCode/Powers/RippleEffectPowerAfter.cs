using Interloper.InterloperCode.Cards.Common;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Powers;
public class RippleEffectPowerAfter() : TemporaryStrengthPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override AbstractModel OriginModel => (AbstractModel)ModelDb.Card<RippleEffect>();
}