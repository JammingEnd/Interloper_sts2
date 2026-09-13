using Godot;
using Interloper.InterloperCode.Potential;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using InterloperCharacter = Interloper.InterloperCode.Character.Interloper;

namespace Interloper.InterloperCode.Nodes;

public partial class NDarkPotentialBar : Control
{
    private static readonly Vector2 BarSize = new(176f, 116f);

    private const float ArcRadius = 50f;
    private const float ArcWidth = 10f;
    private const int ArcPointCount = 96;

    private const float ArcStartAngle = Mathf.Pi * 3f / 4f;
    private const float ArcSweep = Mathf.Pi * 3f / 2f;
    private const float ArcEndAngle = ArcStartAngle + ArcSweep;

    private static readonly DarkPotentialLevels Levels = new();

    private static readonly Color TrackColor = new(0.13f, 0.09f, 0.21f);
    private static readonly Color FillColor = InterloperCharacter.Color;

    private Player? _player;

    public void Initialize(Player player)
    {
        _player = player;
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
        MouseEntered += OnBarHovered;
        MouseExited += OnBarUnhovered;
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

        QueueRedraw();
    }

    private void OnBarHovered()
    {
        if (_player == null)
            return;

        var description = new LocString("static_hover_tips", "INTERLOPER-DARK_POTENTIAL.description");
        description.Add("Levels", BuildLevelsText(_player));

        var hoverTip = new HoverTip(
            new LocString("static_hover_tips", "INTERLOPER-DARK_POTENTIAL.title"),
            description);

        var set = NHoverTipSet.CreateAndShow(this, hoverTip, HoverTip.GetHoverTipAlignment(this));
        set?.SetExtraFollowOffset(new Vector2(20f, -20f));
        set?.SetFollowOwner();
    }

    private void OnBarUnhovered()
    {
        NHoverTipSet.Remove(this);
    }

    private static string BuildLevelsText(Player player)
    {
        int max = player.PlayerCombatState?.GetDarkPotentialMax() ?? 100;
        var lines = new List<string>(DarkPotentialLevels.LevelCount);
        for (int i = 1; i <= DarkPotentialLevels.LevelCount; i++)
        {
            int threshold = max * i / DarkPotentialLevels.LevelCount;
            string levelDescription = Levels.GetDescription(i);
            lines.Add(string.IsNullOrWhiteSpace(levelDescription)
                ? $"Level {i}: {threshold}/{max}"
                : $"Level {i}: {threshold}/{max} — {levelDescription}");
        }

        return string.Join("\n", lines);
    }
}