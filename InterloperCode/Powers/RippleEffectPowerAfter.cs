using BaseLib.Abstracts;
using Interloper.InterloperCode.Cards.Common;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Interloper.InterloperCode.Powers;
public class RippleEffectPowerAfter() : CustomTemporaryPowerModelWrapper<RippleEffect, StrengthPower>
{
    public override PowerType Type =>
        PowerType.Buff;
    
    protected override bool InvertInternalPowerAmount => false;
}