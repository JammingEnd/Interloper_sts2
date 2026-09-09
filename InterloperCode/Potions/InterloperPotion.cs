using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using Interloper.InterloperCode.Character;
using Interloper.InterloperCode.Extensions;

namespace Interloper.InterloperCode.Potions;

[Pool(typeof(InterloperPotionPool))]
public abstract class InterloperPotion : CustomPotionModel
{
    public override string? CustomPackedImagePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionImagePath();
            return ResourceLoader.Exists(path) ? path : null;
        }
    }

    public override string? CustomPackedOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".PotionImagePath();
            return ResourceLoader.Exists(path) ? path : null;
        }
    }
}