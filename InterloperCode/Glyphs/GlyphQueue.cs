using Interloper.InterloperCode.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Interloper.InterloperCode.Glyphs;

public class GlyphQueue
{
    public const int ActivationThreshold = 3;

    private Player Owner { get; }

    private readonly List<GlyphModel> _glyphs = [];

    public IReadOnlyList<GlyphModel> Glyphs => _glyphs;

    public bool IsFull => _glyphs.Count >= ActivationThreshold;

    public bool HasAny => _glyphs.Count != 0;

    public GlyphQueue(Player owner)
    {
        Owner = owner;
    }

    public bool TryEnqueue(GlyphModel glyph)
    {
        if (IsFull)
            return false;

        glyph.AssertMutable();
        _glyphs.Add(glyph);
        return true;
    }

    public bool Remove(GlyphModel glyph)
    {
        return _glyphs.Remove(glyph);
    }

    public void Clear()
    {
        _glyphs.Clear();
    }

    public (int eyes, int mouths, int tails) GetCounts()
    {
        int eyes = _glyphs.Count(g => g.Type == GlyphType.EYE);
        int mouths = _glyphs.Count(g => g.Type == GlyphType.MOUTH);
        int tails = _glyphs.Count(g => g.Type == GlyphType.TAIL);
        return (eyes, mouths, tails);
    }
}