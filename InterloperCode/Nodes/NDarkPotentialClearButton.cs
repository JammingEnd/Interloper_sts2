using Godot;

namespace Interloper.InterloperCode.Nodes;

public partial class NDarkPotentialClearButton : TextureButton
{
    private const float RotationSpeed = 0.4f;
    private static readonly Color HoverColor = new(0.7f, 0.7f, 0.7f);

    public override void _Ready()
    {
        PivotOffset = Size * 0.5f;
        MouseEntered += OnHovered;
        MouseExited += OnUnhovered;
    }

    public override void _Process(double delta)
    {
        Rotation -= RotationSpeed * (float)delta;
    }

    private void OnHovered()
    {
        SelfModulate = HoverColor;
    }

    private void OnUnhovered()
    {
        SelfModulate = Colors.White;
    }
}