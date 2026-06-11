using Interloper.InterloperCode.Cards.Glyph;
using Interloper.InterloperCode.Keywords;
using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Interloper.InterloperCode.Cards;

public enum GlyphType
{
    EYE = 0,
    MOUTH = 1,
    TAIL = 2,
}

// card-type that is used in the glyph cards
public abstract class GlyphCard(int cost, CardType type, CardRarity rarity, TargetType target) : InterloperCard(cost, type, rarity, target)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // add glyph to sequence
        
        // add Consumed Keyword
        CardCmd.ApplyKeyword(this, ConsumedKeyword.Consumed);
        
        // fire effects
    }
    
}
