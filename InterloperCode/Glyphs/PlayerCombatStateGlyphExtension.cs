using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Glyphs;

public static class PlayerCombatStateGlyphExtension
{
    public static GlyphQueue? GetGlyphQueue(this PlayerCombatState state)
    {
        return GlyphField.Queue[state];
    }

    public static (int eyes, int mouths, int tails) GetGlyphCounts(this PlayerCombatState state)
    {
        var queue = state.GetGlyphQueue();
        return queue == null ? (0, 0, 0) : queue.GetCounts();
    }
}