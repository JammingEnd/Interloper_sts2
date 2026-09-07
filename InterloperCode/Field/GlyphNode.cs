using BaseLib.Utils;
using Interloper.InterloperCode.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Interloper.InterloperCode.Field;

public static class GlyphNode
{
    public static readonly AddedNode<NCombatUi, NGlyphArch> NGlyphArch = new(ui =>
    {
        var arch = new NGlyphArch();
        ui.AddChild(arch);
        return arch;
    });
}