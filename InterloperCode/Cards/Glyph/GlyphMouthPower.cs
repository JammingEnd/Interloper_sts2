using Interloper.InterloperCode.Powers;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Interloper.InterloperCode.Cards.Glyph;

public class GlyphMouthPower() : GlyphPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    
}