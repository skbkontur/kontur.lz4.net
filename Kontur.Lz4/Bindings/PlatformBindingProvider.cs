using System;

namespace Kontur.Lz4.Bindings
{
    internal static class PlatformBindingProvider
    {
        public static ILz4Bindings GetBinding()
        {
            bool is64Bit = Environment.Is64BitProcess;
            var platformId = Environment.OSVersion.Platform;
            return GetBinding(platformId, is64Bit);
        }

        private static ILz4Bindings GetBinding(PlatformID platformId, bool is64Bit)
        {
            switch (platformId)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32Windows:
                    BinariesUnpacker.UnpackAssemblyFromResource(is64Bit ? "liblz4-64.dll" : "liblz4.dll",
                        Lz4Bindings.DllName + ".dll");
                    return new Lz4Bindings();
                case PlatformID.Unix:
                    var libraryFile = "./lib" + Lz4Bindings.DllName + ".so";
                    BinariesUnpacker.UnpackAssemblyFromResource(is64Bit ? "liblz4-64.so" : "liblz4.so",
                        libraryFile);
                    UnixFilePermisiionHelper.Set755(libraryFile);
                    return new Lz4Bindings();
                default:
                    throw new InvalidOperationException($"{platformId} is not supported");
            }
        }
    }
}