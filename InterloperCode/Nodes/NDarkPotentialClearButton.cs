using Godot;

namespace Interloper.InterloperCode.Nodes;

public partial class NDarkPotentialClearButton : TextureButton
{
    private const float RotationSpeed = 0.4f;

    public override void _Ready()
    {
        PivotOffset = Size * 0.5f;
    }

    public override void _Process(double delta)
    {
        Rotation -= RotationSpeed * (float)delta;
    }
}