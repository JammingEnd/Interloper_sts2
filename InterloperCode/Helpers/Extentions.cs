using Interloper.InterloperCode.Glyphs;
using Interloper.InterloperCode.Keywords;
using Interloper.InterloperCode.Relics;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Helpers;

public static class Extentions
{
    public static CardModel[] GetOldestPlayableCards(this CardPile pile, int amount = 0, bool includeStatus = false, bool includeCurse = false, bool includeQuest = false, bool includeConsumed = false)
    {
        var query = pile.Cards.AsEnumerable();

        if (!includeConsumed)
            query = query.Where(p => !p.Keywords.Contains(InterloperKeywords.Consumed));
        if (!includeCurse)
            query = query.Where(p => p.Type != CardType.Curse);
        if (!includeStatus)
            query = query.Where(p => p.Type != CardType.Status);
        if (!includeQuest)
            query = query.Where(p => p.Type != CardType.Quest);

        var owner = pile.Cards.Select(c => c.Owner).FirstOrDefault();
        if (owner != null && owner.Relics.Any(r => r is PocketPocketDimensionRelic))
            query = query.Where(p => !p.Keywords.Contains(CardKeyword.Ethereal));

        return query.ToArray();
    }

    public static CardModel GetOldestPlayableCard(this CardPile pile, bool includeStatus = false, bool includeCurse = false, bool includeQuest = false, bool includeConsumed = false)
    {
        var query = pile.Cards.AsEnumerable();

        if (!includeConsumed)
            query = query.Where(p => !p.Keywords.Contains(InterloperKeywords.Consumed));
        if (!includeCurse)
            query = query.Where(p => p.Type != CardType.Curse);
        if (!includeStatus)
            query = query.Where(p => p.Type != CardType.Status);
        if (!includeQuest)
            query = query.Where(p => p.Type != CardType.Quest);

        var owner = pile.Cards.Select(c => c.Owner).FirstOrDefault();
        if (owner != null && owner.Relics.Any(r => r is PocketPocketDimensionRelic))
            query = query.Where(p => !p.Keywords.Contains(CardKeyword.Ethereal));

        return query.FirstOrDefault();
    }

    public static bool TryGetPower<T>(Creature creature, out PowerModel power) where T : PowerModel
    {
        power = creature.GetPower<T>();
        return power != null;
    }
    private static readonly GlyphSequence[] Sequences =
    {
        GlyphSequence.OneOfEach,
        GlyphSequence.ThreeEyes,
        GlyphSequence.ThreeMouths,
        GlyphSequence.ThreeTails,
        GlyphSequence.TwoEyesOneMouth,
        GlyphSequence.TwoEyesOneTail,
        GlyphSequence.TwoMouthsOneTail,
        GlyphSequence.OneEyeTwoMouths,
        GlyphSequence.OneEyeTwoTails,
        GlyphSequence.OneMouthTwoTails
    };
    public static GlyphSequence GetSequenceByIndex(int index)
    {
        return Sequences[index]; 
    }
}