using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Powers;

public class EffigyOfFirePower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterCardChangedPiles(
        CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (oldPileType != PileType.Exhaust)
            return;

        var duplicate = CombatState.CreateCard(card.CanonicalInstance, Owner.Player);
        await CardPileCmd.AddGeneratedCardToCombat(duplicate, PileType.Hand, Owner.Player);

        await PowerCmd.Remove(this);
    }
}