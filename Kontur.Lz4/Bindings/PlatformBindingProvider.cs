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
            string dllPath;

            switch (platformId)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32Windows:
                    (dllPath, _) = BinariesUnpacker.UnpackAssemblyFromResource(
                        is64Bit ? "liblz4-64.dll" : "liblz4.dll",
                        is64Bit,
                        Lz4Bindings.DllName + ".dll");
                    var libHandle = LoadLibraryW(dllPath);
                    if (libHandle == IntPtr.Zero)
                    {
                        var lastWin32Error = Marshal.GetLastWin32Error();
                        throw new Win32Exception($"Failed with code {lastWin32Error}. x64={IntPtr.Size == 8}", new Win32Exception(lastWin32Error));
                    }

                    return new Lz4Bindings();
                case PlatformID.Unix:
                    bool created;
                    (dllPath, created) = BinariesUnpacker.UnpackAssemblyFromResource(
                        is64Bit ? "liblz4-64.so" : "liblz4.so",
                        is64Bit,
                        "./lib" + Lz4Bindings.DllName + ".so");
                    var error = UnixFilePermissionHelper.TrySet755(dllPath);
                    if (created && error != null)
                        throw error;
                    return new Lz4Bindings();
                default:
                    throw new InvalidOperationException($"{platformId} is not supported");
            }
        }

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibraryW(string lpFileName);
    }
}