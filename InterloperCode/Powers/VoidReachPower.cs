using Interloper.InterloperCode.Cards.Basic;
using Interloper.InterloperCode.Cards.Common;
using Interloper.InterloperCode.Cards.Uncommon;
using Interloper.InterloperCode.Helpers;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

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

        Trigger(choiceContext, player, amount);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = Owner.Player;
        if (player == null)
            return;
        var amount = player.Creature.GetPowerAmount<VoidReachPower>();
        if(amount <= 0)
            return;
        MainFile.Logger.Info($"Reached Trigger!!!!");
        Trigger(choiceContext, player, amount);
        
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
            return;
        var player = Owner.Player;
        if (player == null)
            return;
        var amount = player.Creature.GetPowerAmount<VoidReachPower>();
        Trigger(choiceContext, player, amount);
    }

    private async void Trigger(PlayerChoiceContext choiceContext, Player player, decimal amount)
    {
       
        var exhaustPile = PileType.Exhaust.GetPile(player);
        var drawnCardIds = new HashSet<string>();

        while (true)
        {
            var oldestCard = exhaustPile.GetOldestPlayableCard();
            if (oldestCard == null || !drawnCardIds.Add(oldestCard.Id.ToString()))
            {
                MainFile.Logger.Info("[VoidReach] No eligible cards in exhaust");
                break;
            }

            int cardCost = oldestCard.EnergyCost.Canonical;
            if (oldestCard.GetType() == typeof(GraspOfTheAbyss))
            {
                var b = oldestCard.EnergyCost.GetWithModifiers(CostModifiers.Local);
                cardCost = b;
            }
            if (cardCost == 0)
                cardCost = 1;

            int threshold = cardCost * 2;
            
            if (this.Amount < threshold)
            {
                MainFile.Logger.Info($"[VoidReach] Not enough ({this.Amount} < {threshold}) for {oldestCard.Title} (cost {cardCost})");
                break;
            }
            if (oldestCard.GetType() == typeof(StrikeInterloper))
            {
                bool upgraded = oldestCard.IsUpgraded;
                var trans = CombatState.CreateCard<CorruptedStrike>(oldestCard.Owner);
                oldestCard = CardCmd.Transform(oldestCard, trans, CardPreviewStyle.None).Result.Value.cardAdded;
                if (upgraded)
                     CardCmd.Upgrade(oldestCard);
            }

            if (oldestCard.GetType() == typeof(DefendInterloper))
            {
                bool upgraded = oldestCard.IsUpgraded;
                var trans = CombatState.CreateCard<CorruptedDefend>(oldestCard.Owner);
                oldestCard = CardCmd.Transform(oldestCard, trans, CardPreviewStyle.None).Result.Value.cardAdded;
                if (upgraded)
                    CardCmd.Upgrade(oldestCard);
            }
            MainFile.Logger.Info($"[VoidReach] Pulling {oldestCard.Title}, consuming {threshold} VoidReach");
            oldestCard.EnergyCost.AddUntilPlayed(-1);
            Action? handler = null;
            handler = () =>
            {
                oldestCard.EnergyCost.AddUntilPlayed(-1);
                oldestCard.EnergyCostChanged -= handler;
            };
            oldestCard.EnergyCostChanged += handler;

            if (oldestCard.GetType() == typeof(LostAndFound))
            {
                await CardPileCmd.Add(oldestCard, PileType.Hand);
            }
            else
            {
                await CardPileCmd.Add(oldestCard, PileType.Draw);
            }

            await PowerCmd.ModifyAmount(choiceContext, this, -threshold, player.Creature, null);
        }
    }
}