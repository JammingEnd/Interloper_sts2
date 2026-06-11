using Interloper.InterloperCode.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Helpers;

public static class Extentions
{
    public static CardModel[] GetOldestPlayableCards(this CardPile pile, int amount = 0, bool includeStatus = false, bool includeCurse = false, bool includeQuest = false, bool includeConsumed = false)
    {
        var query = pile.Cards.AsEnumerable();

        if (!includeConsumed)
            query = query.Where(p => !p.Keywords.Contains(ConsumedKeyword.Consumed));
        if (!includeCurse)
            query = query.Where(p => p.Type != CardType.Curse);
        if (!includeStatus)
            query = query.Where(p => p.Type != CardType.Status);
        if (!includeQuest)
            query = query.Where(p => p.Type != CardType.Quest);
        
        return query.ToArray();
        
    }
    public static CardModel GetOldestPlayableCard(this CardPile pile, bool includeStatus = false, bool includeCurse = false, bool includeQuest = false, bool includeConsumed = false)
    {
        var query = pile.Cards.AsEnumerable();

        if (!includeConsumed)
            query = query.Where(p => !p.Keywords.Contains(ConsumedKeyword.Consumed));
        if (!includeCurse)
            query = query.Where(p => p.Type != CardType.Curse);
        if (!includeStatus)
            query = query.Where(p => p.Type != CardType.Status);
        if (!includeQuest)
            query = query.Where(p => p.Type != CardType.Quest);
        
        return query.FirstOrDefault();
        
    }
}