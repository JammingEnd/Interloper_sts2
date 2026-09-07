using Godot;
using Interloper.InterloperCode.Cards;
using Interloper.InterloperCode.Glyphs;
using Interloper.InterloperCode.Helpers;
using Interloper.InterloperCode.Utils;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace Interloper.InterloperCode.Nodes;

public partial class NGlyphArch : Control
{
    private const int SlotCount = 3;

    private static readonly Vector2 ArchSize = new(176f, 116f);

    private static readonly Vector2 PlacementSize = new(80f, 80f);

    private static readonly Vector2 SlotSize = new(62f, 62f);

    private static readonly Vector2 IconSize = new(52f, 52f);

    private static readonly Vector2[] PlacementPositions =
    [
        new(-9f, 45f),
        new(48f, -9f),
        new(105f, 45f)
    ];

    private static readonly Vector2[] SlotPositions =
    [
        new(0f, 54f),
        new(57f, 0f),
        new(114f, 54f)
    ];

    private static readonly Vector2 IconOffset = new(5f, 5f);

    private Player? _player;

    private readonly List<TextureRect> _placements = [];
    private readonly List<TextureRect> _highlights = [];
    private readonly List<TextureRect> _icons = [];

    public void Initialize(Player player)
    {
        _player = player;
        Refresh();
    }

    public override void _Ready()
    {
        Size = ArchSize;
        MouseFilter = MouseFilterEnum.Ignore;

        for (int i = 0; i < SlotCount; i++)
        {
            var placement = new TextureRect
            {
                Name = $"Placement{i}",
                Texture = PreloadManager.Cache.GetCompressedTexture2D(GlyphResource.GlyphPlacementPath),
                Position = PlacementPositions[i],
                Size = PlacementSize,
                MouseFilter = MouseFilterEnum.Ignore,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
            };

            var highlight = new TextureRect
            {
                Name = $"Highlight{i}",
                Texture = PreloadManager.Cache.GetCompressedTexture2D(GlyphResource.SlotHighlightPath),
                Position = SlotPositions[i],
                Size = SlotSize,
                Visible = false,
                MouseFilter = MouseFilterEnum.Ignore,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
            };

            var icon = new TextureRect
            {
                Name = $"Icon{i}",
                Position = SlotPositions[i] + IconOffset,
                Size = IconSize,
                Visible = false,
                MouseFilter = MouseFilterEnum.Ignore,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
            };

            var slotIndex = i;
            highlight.MouseEntered += () => OnSlotHovered(highlight);
            highlight.MouseExited += () => OnSlotUnhovered(highlight);

            AddChild(placement);
            AddChild(highlight);
            AddChild(icon);

            _placements.Add(placement);
            _highlights.Add(highlight);
            _icons.Add(icon);
        }

        Visible = true;
    }

    public override void _Process(double delta)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (!IsNodeReady())
            return;

        if (_player == null)
            return;

        var queue = _player.PlayerCombatState?.GetGlyphQueue();
        int count = queue?.Glyphs.Count ?? 0;

        for (int i = 0; i < SlotCount; i++)
        {
            bool filled = i < count;
            bool isNext = i == count && count == SlotCount - 1;

            var icon = _icons[i];
            icon.Visible = filled;
            if (filled)
                icon.Texture = GetIconFor(queue!.Glyphs[i].Type);

            var highlight = _highlights[i];
            highlight.Visible = isNext;
            highlight.MouseFilter = isNext ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
        }
    }

    private Texture2D GetIconFor(GlyphType type) => type switch
    {
        GlyphType.EYE => PreloadManager.Cache.GetCompressedTexture2D(GlyphResource.EyeIconPath),
        GlyphType.MOUTH => PreloadManager.Cache.GetCompressedTexture2D(GlyphResource.MouthIconPath),
        GlyphType.TAIL => PreloadManager.Cache.GetCompressedTexture2D(GlyphResource.TailIconPath),
        _ => PreloadManager.Cache.GetCompressedTexture2D(GlyphResource.EyeIconPath)
    };

    private void OnSlotHovered(TextureRect highlight)
    {
        if (_player == null)
            return;

        var queue = _player.PlayerCombatState?.GetGlyphQueue();
        if (queue == null || queue.Glyphs.Count != SlotCount - 1)
            return;

        var desc = new LocString("static_hover_tips", "INTERLOPER-GLYPH_POSSIBLE_OUTCOMES.description");
        desc.Add("Outcomes", LocHelper.GetPossibleOutcomeLoc(_player.Creature));

        var hoverTip = new HoverTip(
            new LocString("static_hover_tips", "INTERLOPER-GLYPH_POSSIBLE_OUTCOMES.title"),
            desc);

        var set = NHoverTipSet.CreateAndShow(highlight, hoverTip, HoverTip.GetHoverTipAlignment(highlight));
        set?.SetExtraFollowOffset(new Vector2(20, -20));
        set?.SetFollowOwner();
    }

    private void OnSlotUnhovered(TextureRect highlight)
    {
        NHoverTipSet.Remove(highlight);
    }
}