using BaseLib.Config;

namespace Interloper.InterloperCode.Utils;

public class InterloperConfig : SimpleModConfig
{
    [ConfigHoverTip]
    public static bool UploadMetrics { get; set; } = false;

    [ConfigHideInUI]
    [ConfigIgnoreRestoreDefaults]
    public static bool UploadMetricsFtueSeen { get; set; } = false;
}