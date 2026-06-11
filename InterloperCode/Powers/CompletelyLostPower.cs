using Interloper.InterloperCode.Keywords;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace Interloper.InterloperCode.Powers;

public class CompletelyLostPower() : InterloperPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        var pile = PileType.Exhaust.GetPile(Owner.Player);
        var consumedCards = pile.Cards
            .Where(c => c.Keywords.Contains(ConsumedKeyword.Consumed))
            .ToList();

        if (consumedCards.Count == 0)
            return;

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var selected = await CardSelectCmd.FromSimpleGrid(
            null, consumedCards, Owner.Player, prefs);

        if (selected != null)
        {
            foreach (var card in selected)
            {
                var deckVar = card.DeckVersion;
                await CardPileCmd.RemoveFromDeck(deckVar);
            }

        }
    }
}