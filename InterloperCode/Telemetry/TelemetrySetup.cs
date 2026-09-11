using Interloper.InterloperCode.Utils;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.Metrics;

namespace Interloper.InterloperCode.Telemetry;

public static class TelemetrySetup
{
    public static void Initialize()
    {
        if (!TelemetryConfig.IsConfigured)
            return;

        TelemetryManager.Configure(TelemetryConfig.SupabaseUrl, TelemetryConfig.SupabaseAnonKey);
        RunManager.Instance.RunStarted += OnRunStarted;
    }

    public static bool ShouldCollect => InterloperConfig.UploadMetrics && MetricUtilities.ShouldUploadMetrics();

    private static void OnRunStarted(RunState state)
    {
        if (!ShouldCollect)
            return;

        TelemetryManager.StartRun(state);
    }
}