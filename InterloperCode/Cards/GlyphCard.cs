using BaseLib.Utils;
using Interloper.InterloperCode.Cards.Glyph;
using Interloper.InterloperCode.Glyphs;
using Interloper.InterloperCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Interloper.InterloperCode.Cards;

public enum GlyphType
{
    EYE = 0,
    MOUTH = 1,
    TAIL = 2,
}

// card-type that is used in the glyph cards
[Pool(typeof(TokenCardPool))]
public abstract class GlyphCard(int cost, CardType type, CardRarity rarity, TargetType target) : InterloperCard(cost, type, rarity, target)
{
    public abstract GlyphType GlyphType { get; }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, InterloperKeywords.Consumed];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card != this)
            return;

        switch (GlyphType)
        {
            case GlyphType.EYE:
                await GlyphCmd.Produce<GlyphEyeModel>(choiceContext, Owner, this, cardPlay);
                break;
            case GlyphType.MOUTH:
                await GlyphCmd.Produce<GlyphMouthModel>(choiceContext, Owner, this, cardPlay);
                break;
            case GlyphType.TAIL:
                await GlyphCmd.Produce<GlyphTailModel>(choiceContext, Owner, this, cardPlay);
                break;
        }
    }
}
