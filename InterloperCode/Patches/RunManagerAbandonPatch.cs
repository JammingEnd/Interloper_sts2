using HarmonyLib;
using MegaCrit.Sts2.Core.Runs;

namespace Interloper.InterloperCode.Patches;

[HarmonyPatch(typeof(RunManager), nameof(RunManager.Abandon))]
internal class RunManagerAbandonPatch
{
    [HarmonyPostfix]
    private static void Postfix()
    {
        Telemetry.TelemetryManager.EndRun(false);
    }
}