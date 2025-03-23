using System;
using System.Runtime.InteropServices;
using Il2CppInterop.Common.Host;
using Il2CppInterop.Common.XrefScans;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.Runtime;
using Il2CppInterop.Runtime.XrefScans;
using MonoMod.Core;

namespace Il2CppInterop.Runtime.Startup;

public record RuntimeConfiguration
{
    public required Version UnityVersion { get; init; }
    public IDetourFactory? DetourFactory { get; init; } = null;

    public string GameAssemblyName { get; init; } = "GameAssembly";

    public string GameAssemblyPostfix { get; init; } = ".dll";

    public IntPtr Il2CppHandle { get; init; } = IntPtr.Zero;
}

public sealed class Il2CppInteropRuntime : BaseHost
{
    private Il2CppInteropRuntime()
    {
    }

    public static Il2CppInteropRuntime Instance => GetInstance<Il2CppInteropRuntime>();

    public Version UnityVersion { get; private init; }

    public IDetourFactory? DetourFactory { get; private init; }

    public string GameAssemblyName { get; private init; }

    public string GameAssemblyPostfix { get; private init; }

    internal static string GameAssemblyFullName => $"{Instance.GameAssemblyName}{Instance.GameAssemblyPostfix}";

    private IntPtr _Il2cppHandle { get; set; }

    public IntPtr Il2CppHandle
    {
        get
        {
            if (_Il2cppHandle != IntPtr.Zero)
            {
                return _Il2cppHandle;
            }

            return _Il2cppHandle = NativeLibrary.Load(GameAssemblyName, typeof(Il2CppInteropRuntime).Assembly, null);
        }
    }

    public static Il2CppInteropRuntime Create(RuntimeConfiguration configuration)
    {
        var res = new Il2CppInteropRuntime
        {
            UnityVersion = configuration.UnityVersion,
            DetourFactory = configuration.DetourFactory,
            GameAssemblyName = configuration.GameAssemblyName,
            GameAssemblyPostfix = configuration.GameAssemblyPostfix,
            _Il2cppHandle = configuration.Il2CppHandle,
        };
        SetInstance(res);
        res.AddXrefScanner<Il2CppInteropRuntime, XrefScanImpl>();
        return res;
    }

    public override void Start()
    {
        UnityVersionHandler.RecalculateHandlers();
        base.Start();
    }
}
