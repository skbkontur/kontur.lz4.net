using System;
namespace Kontur.Lz4.Bindings
{
    interface ILz4Bindings
    {
        int CompressDefault(IntPtr source, IntPtr dest,
            int sourceSize, int maxDestSize);

        int CompressBound(int sourceSize);

        int DecompressSafe(IntPtr source, IntPtr dest,
            int compressedSize, int maxDecompressedSize);

        int DecompressFast(IntPtr source, IntPtr dest,
            int originalSize);
    }
}