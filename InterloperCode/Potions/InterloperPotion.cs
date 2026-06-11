using BaseLib.Abstracts;
using BaseLib.Utils;
using Interloper.InterloperCode.Character;

namespace Interloper.InterloperCode.Potions;

[Pool(typeof(InterloperPotionPool))]
public abstract class InterloperPotion : CustomPotionModel;