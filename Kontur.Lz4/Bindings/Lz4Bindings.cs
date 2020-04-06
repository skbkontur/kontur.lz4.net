using System;
using System.Runtime.InteropServices;

namespace Kontur.Lz4.Bindings
{
    internal class Lz4Bindings : ILz4Bindings
    {
        // Note(kungurtsev): up version on update.
        public const string DllName = "Kontur_Lz4_original_v1";

        public int CompressDefault(IntPtr source, IntPtr dest,
            int sourceSize, int maxDestSize)
        {
            return Calls.CompressDefault(source, dest, sourceSize, maxDestSize);
        }

        public int CompressBound(int sourceSize)
        {
            return Calls.CompressBound(sourceSize);
        }


        public int DecompressSafe(IntPtr source, IntPtr dest,
            int compressedSize, int maxDecompressedSize)
        {
            return Calls.DecompressSafe(source, dest, compressedSize, maxDecompressedSize);
        }

        public int DecompressFast(IntPtr source, IntPtr dest,
            int originalSize)
        {
            return Calls.DecompressFast(source, dest, originalSize);
        }

        private static class Calls
        {
            [DllImport(DllName,
                CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                EntryPoint = "LZ4_compress_default")]
            public static extern int CompressDefault(IntPtr source, IntPtr dest,
                int sourceSize, int maxDestSize);

            [DllImport(DllName,
                CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                EntryPoint = "LZ4_compressBound")]
            public static extern int CompressBound(int sourceSize);


            [DllImport(DllName,
                CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                EntryPoint = "LZ4_decompress_safe")]
            public static extern int DecompressSafe(IntPtr source, IntPtr dest,
                int compressedSize, int maxDecompressedSize);

            [DllImport(DllName,
                CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
                EntryPoint = "LZ4_decompress_fast")]
            public static extern int DecompressFast(IntPtr source, IntPtr dest,
                int originalSize);
        }
    }
}