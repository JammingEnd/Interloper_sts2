using Interloper.InterloperCode.Helpers;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

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
        if (power.GetType() != typeof(VoidReachPower))
            return;

        if (amount <= 0)
            return;

        var player = Owner.Player;
        if (player == null)
            return;

        var exhaustPile = PileType.Exhaust.GetPile(player);
        MainFile.Logger.Info($"[VoidReach] Changed by {amount}, total: {this.Amount}, exhaust count: {exhaustPile.Cards.Count}");

        while (true)
        {
            var oldestCard = exhaustPile.GetOldestPlayableCard();
            if (oldestCard == null)
            {
                MainFile.Logger.Info("[VoidReach] No eligible cards in exhaust");
                break;
            }

            int cardCost = oldestCard.EnergyCost.Canonical;
            if (cardCost == 0)
                cardCost = 1;

            int threshold = cardCost * 2;
            if (this.Amount < threshold)
            {
                MainFile.Logger.Info($"[VoidReach] Not enough ({this.Amount} < {threshold}) for {oldestCard.Title} (cost {cardCost})");
                break;
            }

            MainFile.Logger.Info($"[VoidReach] Pulling {oldestCard.Title}, consuming {threshold} VoidReach");
            await CardPileCmd.Add(oldestCard, PileType.Draw, CardPilePosition.Random);
            oldestCard.EnergyCost.AddUntilPlayed(-1);

            await PowerCmd.ModifyAmount(choiceContext, this, -threshold, applier, cardSource);
        }
    }
    
}