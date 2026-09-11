namespace Interloper.InterloperCode.Telemetry;

public static class TelemetryConfig
{
    public const string SupabaseUrl = "https://tpvveklazigaeeoxqjmz.supabase.co";
    public const string SupabaseAnonKey = "sb_publishable_Fk-Y8_wW1l00FQ9GsrssZQ_AXeKJgv6";

    public static bool IsConfigured =>
        SupabaseUrl.StartsWith("https://") && !SupabaseUrl.Contains("YOUR_PROJECT")
        && !SupabaseAnonKey.Contains("YOUR_ANON_KEY");
}