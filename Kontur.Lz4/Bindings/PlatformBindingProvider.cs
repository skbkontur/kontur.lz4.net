using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace Kontur.Lz4.Bindings
{
    internal static class PlatformBindingProvider
    {
        public static ILz4Bindings GetBinding()
        {
            var is64Bit = Environment.Is64BitProcess;
            var platformId = Environment.OSVersion.Platform;

            Exception error = null;

            for (var attempt = 0; attempt < 10; attempt++)
            {
                try
                {
                    var binging = GetBinding(platformId, is64Bit);
                    binging.CompressBound(42);
                    return binging;
                }
                catch (Exception e)
                {
                    error = e;
                    Thread.Sleep(50);
                }
            }

            throw error ?? new Exception("No attempts were made.");
        }

        private static ILz4Bindings GetBinding(PlatformID platformId, bool is64Bit)
        {
            string dllPath, assemblyName;

            switch (platformId)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32Windows:
                    assemblyName = (is64Bit ? Lz4BindingsX64.Name : Lz4BindingsX86.Name) + ".dll";
                    (dllPath, _) = BinariesUnpacker.UnpackAssemblyFromResource(assemblyName, assemblyName);
                    var libHandle = LoadLibraryW(dllPath);
                    if (libHandle == IntPtr.Zero)
                    {
                        var lastWin32Error = Marshal.GetLastWin32Error();
                        throw new Win32Exception($"Failed with code {lastWin32Error}. x64={IntPtr.Size == 8}", new Win32Exception(lastWin32Error));
                    }
                    return CreateBindings(is64Bit);
                case PlatformID.Unix:
                    assemblyName = (is64Bit ? Lz4BindingsX64.Name : Lz4BindingsX86.Name) + ".so";
                    bool created;
                    (dllPath, created) = BinariesUnpacker.UnpackAssemblyFromResource(assemblyName, "lib" + assemblyName);
                    var error = UnixFilePermissionHelper.TrySet755(dllPath);
                    if (created && error != null)
                        throw error;
                    return is64Bit ? (ILz4Bindings)new Lz4BindingsX64() : new Lz4BindingsX86();
                default:
                    throw new InvalidOperationException($"{platformId} is not supported");
            }
        }

        private static ILz4Bindings CreateBindings(bool is64Bit) => 
            is64Bit ? (ILz4Bindings) new Lz4BindingsX64() : new Lz4BindingsX86();


        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibraryW(string lpFileName);
    }
}