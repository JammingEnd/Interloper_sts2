using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Interloper.InterloperCode.Patches;

[HarmonyPatch(typeof(NCombatCardPile), "OnRelease")]
internal static class NCombatCardPileOnReleasePatch
{
    [HarmonyPrefix]
    private static bool Prefix(NCombatCardPile __instance)
    {
        if (__instance._pile == null || __instance._pile.IsEmpty)
            return false;

        return true;
    }
}