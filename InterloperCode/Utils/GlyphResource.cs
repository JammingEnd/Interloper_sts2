using BaseLib.Audio;

namespace Interloper.InterloperCode.Utils;

public static class GlyphResource
{
    public const string GlyphPlacementPath = "res://Interloper/images/ui/combat/glyphs/glyph_placement.png";
    public const string SlotHighlightPath = "res://Interloper/images/ui/combat/glyphs/glyph_highlight.png";

    public const string QueueVfxPath = "res://Interloper/scenes/Glyphs/glyph_queue_vfx.tscn";
    public const string SequenceVfxPath = "res://Interloper/scenes/Glyphs/glyph_sequence_vfx.tscn";

    public const float QueueVfxScale = 0.35f;
    public const float SequenceVfxScale = 0.35f;

    public static readonly ModSound QueueSound = new("res://Interloper/sound/glyph_queue.ogg");
    public static readonly ModSound SequenceSound = new("res://Interloper/sound/sequence.ogg");

    public const string EyeIconPath = "res://Interloper/images/powers/glyph_eye_power.png";
    public const string MouthIconPath = "res://Interloper/images/powers/glyph_mouth_power.png";
    public const string TailIconPath = "res://Interloper/images/powers/glyph_tail_power.png";
}