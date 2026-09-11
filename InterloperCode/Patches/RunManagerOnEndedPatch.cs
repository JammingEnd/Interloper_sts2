using HarmonyLib;
using MegaCrit.Sts2.Core.Runs;

namespace Interloper.InterloperCode.Patches;

[HarmonyPatch(typeof(RunManager), nameof(RunManager.OnEnded))]
internal class RunManagerOnEndedPatch
{
    [HarmonyPostfix]
    private static void Postfix(bool isVictory)
    {
        Telemetry.TelemetryManager.EndRun(isVictory);
    }
}