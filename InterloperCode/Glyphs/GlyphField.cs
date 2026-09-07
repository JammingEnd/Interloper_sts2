using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Interloper.InterloperCode.Glyphs;

public static class GlyphField
{
    public static readonly SpireField<PlayerCombatState, GlyphQueue> Queue = new(() => null);
}