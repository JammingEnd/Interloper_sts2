using Interloper.InterloperCode.Helpers;
using Interloper.InterloperCode.Keywords;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Powers;

public class VoidReachPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        var player = applier?.Player;
        if (player != null)
        {
            int toSpend = (int)amount;
            while (toSpend > 1)
            {
                var exhaustPile = PileType.Exhaust.GetPile(player);
                var drawPile = PileType.Draw.GetPile(player);
                // we dont want cards that are status, curse or are marked with consumed
                var oldestCard = exhaustPile.GetOldestPlayableCards()[0];
                if (oldestCard == null)
                {
                    return;
                }
                // unmodified cost
                int oldExhaustCardCost = oldestCard.EnergyCost.Canonical;

                if (toSpend > oldExhaustCardCost * 2)
                {
                    break;
                }
                
                // when cost is 0, make it 1
                if (oldExhaustCardCost == 0 && power.Amount > 1)
                {
                    ExhaustIntoHand(exhaustPile, drawPile, oldestCard);
                    oldExhaustCardCost = 1;
                }
                // double the oldest cost
                else if (power.Amount >= oldExhaustCardCost * 2)
                {
                    oldExhaustCardCost *= 2;
                    ExhaustIntoHand(exhaustPile, drawPile, oldestCard);
                    
                }
                await PowerCmd.ModifyAmount(
                    choiceContext,
                    power,
                    oldExhaustCardCost,
                    applier,
                    cardSource);

                toSpend -= oldExhaustCardCost;
            }
            
        }
    }

    private void ExhaustIntoHand(CardPile exhaustPile, CardPile drawPile, CardModel card)
    {
        exhaustPile.RemoveInternal(card);
        drawPile.AddInternal(card);
        card.EnergyCost.AddUntilPlayed(-1);
    }
    
}