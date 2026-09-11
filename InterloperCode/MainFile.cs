using BaseLib.Config;
using Godot;
using HarmonyLib;
using Interloper.InterloperCode.Telemetry;
using Interloper.InterloperCode.Utils;
using MegaCrit.Sts2.Core.Modding;

namespace Interloper.InterloperCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "Interloper"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        //If you want to use scripts defined in your mod for Godot scenes, uncomment the following line.
        //Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());

        ModConfigRegistry.Register(ModId, new InterloperConfig());
        TelemetrySetup.Initialize();

        Harmony harmony = new(ModId);

        harmony.PatchAll();
    }
}