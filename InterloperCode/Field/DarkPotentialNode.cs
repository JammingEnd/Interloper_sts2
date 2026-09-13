using BaseLib.Utils;
using Interloper.InterloperCode.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Interloper.InterloperCode.Field;

public static class DarkPotentialNode
{
    public static readonly AddedNode<NCombatUi, NDarkPotentialBar> NDarkPotentialBar = new(ui =>
    {
        var bar = new NDarkPotentialBar();
        ui.AddChild(bar);
        return bar;
    });
}