using System;
using System.Runtime.InteropServices;
using Il2CppInterop.Runtime.Startup;
using MonoMod.Core;

namespace Il2CppInterop.Runtime.Injection;

internal static class Detour
{
    public static IDisposable Apply(nint original, Delegate target, out nint trampoline)
    {
        var factory = Il2CppInteropRuntime.Instance.DetourFactory ?? DetourFactory.Current;
        var detour = factory.CreateNativeDetour(original, Marshal.GetFunctionPointerForDelegate(target));
        trampoline = detour.OrigEntrypoint;
        return detour;
    }

    public static IDisposable Apply<T>(nint original, T target, out T trampoline) where T : Delegate
    {
        var factory = Il2CppInteropRuntime.Instance.DetourFactory ?? DetourFactory.Current;
        var detour = factory.CreateNativeDetour(original, Marshal.GetFunctionPointerForDelegate(target));
        trampoline = Marshal.GetDelegateForFunctionPointer<T>(detour.OrigEntrypoint);
        return detour;
    }
}
