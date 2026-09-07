using HarmonyLib;
using Interloper.InterloperCode.Glyphs;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Interloper.InterloperCode.Patches;

[HarmonyPatch(typeof(PlayerCombatState), MethodType.Constructor)]
[HarmonyPatch([typeof(Player)])]
internal class PlayerCombatStateConstructorPatch
{
    [HarmonyPostfix]
    private static void Postfix(Player player, PlayerCombatState __instance)
    {
        GlyphField.Queue[__instance] = new GlyphQueue(player);
    }
}