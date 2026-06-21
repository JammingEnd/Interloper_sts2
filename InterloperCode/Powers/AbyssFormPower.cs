using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Interloper.InterloperCode.Powers;

public class AbyssFormPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        var pools = Owner.Player.UnlockState.CharacterCardPools.ToList();
        pools.Remove(Owner.Player.Character.CardPool);

        var exhaustCards = pools
            .SelectMany(p => p.AllCards)
            .Where(c => c.Keywords.Contains(CardKeyword.Exhaust))
            .ToList();

        if (exhaustCards.Count == 0)
            return;

        for (int i = 0; i < (int)Amount; i++)
        {
            var newCard = CardFactory.GetDistinctForCombat(
                Owner.Player, exhaustCards, 1,
                Owner.Player.RunState.Rng.CombatCardGeneration).FirstOrDefault();

            if (newCard != null)
            {
                newCard.EnergyCost.SetThisTurnOrUntilPlayed(0);
                newCard.SetStarCostThisCombat(0);
                await CardPileCmd.AddGeneratedCardToCombat(
                    newCard, PileType.Hand, Owner.Player);
            }
        }
    }
}