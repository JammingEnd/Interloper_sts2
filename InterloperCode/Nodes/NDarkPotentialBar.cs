using Godot;
using Interloper.InterloperCode.Extensions;
using MegaCrit.Sts2.Core.Assets;
using Interloper.InterloperCode.Potential;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using InterloperCharacter = Interloper.InterloperCode.Character.Interloper;

namespace Interloper.InterloperCode.Nodes;

public partial class NDarkPotentialBar : Control
{
    private static readonly Vector2 BarSize = new(196f, 126f);

    private const float ArcRadius = 60f;
    private const float ArcWidth = 14f;
    private const int ArcPointCount = 96;

    private const float ArcStartAngle = Mathf.Pi * 3f / 4f;
    private const float ArcSweep = Mathf.Pi * 3f / 2f;
    private const float ArcEndAngle = ArcStartAngle + ArcSweep;

    private const float MarkerSize = 30f;
    private const float MarkerInset = 17f;
    private const float MarkerOutset = 5f;

    private const float FlameSize = 40f;
    private const float FlameScale = 1f;

    private static readonly DarkPotentialLevels Levels = new();

    private static readonly Texture2D MarkerTexture =
        PreloadManager.Cache.GetTexture2D("ui/combat/dark_potential/potential_marker.png".ImagePath());

    private static readonly Texture2D EnergyMarkerTexture =
        PreloadManager.Cache.GetTexture2D("ui/combat/dark_potential/energy_marker.png".ImagePath());

    private static readonly PackedScene FireVfxScene =
        PreloadManager.Cache.GetScene("othervfx/dark_fire_vfx.tscn".ScenePath());

    private static readonly Color TrackColor = new(0.13f, 0.09f, 0.21f);
    private static readonly Color FillColor = InterloperCharacter.Color;

    private readonly List<TextureRect> _levelMarkers = new();
    private readonly List<TextureRect> _energyMarkers = new();
    private readonly List<Node2D> _flames = new();

    private Player? _player;

    public void Initialize(Player player)
    {
        _player = player;
        if (_levelMarkers.Count == 0)
        {
            CreateMarkers();
            CreateFlames();
            for (int i = 0; i < _levelMarkers.Count; i++)
                AttachLevelHover(_levelMarkers[i], i + 1);
            for (int i = 0; i < _energyMarkers.Count; i++)
                AttachEnergyHover(_energyMarkers[i], i);
        }
        RefreshFlames();
        QueueRedraw();
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        DarkPotentialCmd.OnChanged += OnDarkPotentialChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DarkPotentialCmd.OnChanged -= OnDarkPotentialChanged;
    }

    public override void _Ready()
    {
        Size = BarSize;
        MouseFilter = MouseFilterEnum.Stop;
    }

    public override void _Draw()
    {
        if (_player?.PlayerCombatState == null)
            return;

        double progress = _player.PlayerCombatState.GetDarkPotentialProgress();

        DrawArc(Size * 0.5f, ArcRadius, ArcStartAngle, ArcEndAngle, ArcPointCount, TrackColor, ArcWidth, true);
        DrawArc(Size * 0.5f, ArcRadius, ArcStartAngle, ArcStartAngle + ArcSweep * (float)progress, ArcPointCount, FillColor, ArcWidth, true);
    }

    private void OnDarkPotentialChanged(Player player)
    {
        if (player != _player)
            return;

        RefreshFlames();
        QueueRedraw();
    }

    private void CreateFlames()
    {
        foreach (var marker in _levelMarkers)
        {
            var flame = FireVfxScene.Instantiate<Node2D>();
            flame.Position = new Vector2(MarkerSize * 0.5f - FlameSize * 0.5f, MarkerSize * 0.6f - FlameSize * 0.5f - 40f);
            flame.Scale = new Vector2(FlameScale, FlameScale);
            flame.Visible = false;
            marker.AddChild(flame);
            _flames.Add(flame);
        }
    }

    private void RefreshFlames()
    {
        if (_player?.PlayerCombatState == null || _flames.Count == 0)
            return;

        int current = _player.PlayerCombatState.GetDarkPotential();
        int max = _player.PlayerCombatState.GetDarkPotentialMax();
        for (int i = 0; i < _flames.Count; i++)
        {
            int threshold = max * (i + 1) / DarkPotentialLevels.LevelCount;
            _flames[i].Visible = current >= threshold;
        }
    }

    private void CreateMarkers()
    {
        for (int i = 1; i <= DarkPotentialLevels.LevelCount; i++)
        {
            _levelMarkers.Add(CreateMarker(MarkerTexture, i / (float)DarkPotentialLevels.LevelCount, Colors.White, false));
        }

        for (int i = 0; i < DarkPotentialCmd.EnergyThresholds.Length; i++)
        {
            _energyMarkers.Add(CreateMarker(EnergyMarkerTexture, DarkPotentialCmd.EnergyThresholds[i] / 100f, Colors.White, true));
        }
    }

    private TextureRect CreateMarker(Texture2D texture, double progress, Color tint, bool pointInward)
    {
        var marker = new TextureRect
        {
            Texture = texture,
            Size = new Vector2(MarkerSize, MarkerSize),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            Modulate = tint,
            MouseFilter = MouseFilterEnum.Stop
        };

        marker.PivotOffset = marker.Size * 0.5f;

        float angle = ArcStartAngle + ArcSweep * (float)progress;
        float radius = pointInward ? ArcRadius - MarkerInset : ArcRadius + MarkerOutset;
        var arcPoint = Size * 0.5f + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        marker.Position = arcPoint - marker.Size * 0.5f;
        // Art points up (-Y); level markers rotate to theta+90deg (outward), energy +180deg more (inward).
        marker.Rotation = angle + Mathf.Pi / 2f + (pointInward ? Mathf.Pi : 0f);

        AddChild(marker);
        return marker;
    }

    private void AttachLevelHover(TextureRect marker, int level)
    {
        marker.MouseEntered += () => ShowLevelTip(marker, level);
        marker.MouseExited += () => NHoverTipSet.Remove(marker);
    }

    private void ShowLevelTip(TextureRect marker, int level)
    {
        if (_player == null) return;
        int max = _player.PlayerCombatState?.GetDarkPotentialMax() ?? 100;
        int threshold = max * level / DarkPotentialLevels.LevelCount;
        string description = Levels.GetDescription(level);

        var title = new LocString("static_hover_tips", "INTERLOPER-DARK_POTENTIAL.levelTitle");
        title.Add("Level", level);
        var body = new LocString("static_hover_tips", "INTERLOPER-DARK_POTENTIAL.level");
        body.Add("Threshold", threshold);
        body.Add("Max", max);
        body.Add("Description", string.IsNullOrWhiteSpace(description) ? "" : $" — {description}");

        var hoverTip = new HoverTip(title, body);
        var set = NHoverTipSet.CreateAndShow(marker, hoverTip, HoverTip.GetHoverTipAlignment(marker));
        set?.SetExtraFollowOffset(new Vector2(20f, -20f));
        set?.SetFollowOwner();
    }

    private void AttachEnergyHover(TextureRect marker, int index)
    {
        marker.MouseEntered += () => ShowEnergyTip(marker, index);
        marker.MouseExited += () => NHoverTipSet.Remove(marker);
    }

    private void ShowEnergyTip(TextureRect marker, int index)
    {
        var loc = new LocString("static_hover_tips", "INTERLOPER-DARK_POTENTIAL.energy");
        loc.Add("Percent", DarkPotentialCmd.EnergyThresholds[index]);
        loc.Add("Value", DarkPotentialCmd.EnergyValues[index]);
        var hoverTip = new HoverTip(loc);
        var set = NHoverTipSet.CreateAndShow(marker, hoverTip, HoverTip.GetHoverTipAlignment(marker));
        set?.SetExtraFollowOffset(new Vector2(20f, -20f));
        set?.SetFollowOwner();
    }
}