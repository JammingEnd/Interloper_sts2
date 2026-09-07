using Godot;
using HarmonyLib;
using Interloper.InterloperCode.Field;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Interloper.InterloperCode.Patches;

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
internal class NCombatUiGlyphArchPatch
{
    [HarmonyPostfix]
    private static void Postfix(NCombatUi __instance, CombatState state)
    {
        var me = LocalContext.GetMe(state);
        if (me == null)
            return;

        var arch = GlyphNode.NGlyphArch[__instance];
        arch.Initialize(me);
        arch.Reparent(__instance._energyCounter);
        arch.Position = new Vector2(-24, -100);
    }
}