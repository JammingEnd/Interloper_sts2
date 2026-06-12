using Interloper.InterloperCode.Character;
using Interloper.InterloperCode.Keywords;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Interloper.InterloperCode.Powers;

public class CorruptedCreationPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        List<CardPoolModel> list1 = this.Owner.Player.UnlockState.CharacterCardPools.ToList<CardPoolModel>();
        if (list1.Count > 0)
        {
            list1.Remove(this.Owner.Player.Character.CardPool);
        }

        var cards = list1.SelectMany<CardPoolModel, CardModel>(
            p => p.AllCards.Where(
                c => c.EnergyCost.Canonical == 0)
        );
        for (int i = 0; i < this.Amount; i++)
        {
            var newcard =
                CardFactory.GetDistinctForCombat(this.Owner.Player, cards, 1, Owner.Player.RunState.Rng.CombatCardGeneration).FirstOrDefault();
            if (newcard != null)
            {
                newcard.AddKeyword(InterloperKeywords.Consumed);
                await CardPileCmd.AddGeneratedCardToCombat(newcard, PileType.Hand, Owner.Player);
            }
        }
    }
}