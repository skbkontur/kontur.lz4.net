using System;
using Kontur.Lz4.Bindings;

namespace Kontur.Lz4
{
    internal static class Lz4CodecEncodeFunctions
    {
        internal static ILz4Bindings Bindings = LZ4Codec.Bindings;

        /// <summary>Decodes the specified input.</summary>
        /// <param name="input">The input.</param>
        /// <param name="inputOffset">The input offset.</param>
        /// <param name="inputLength">Length of the input.</param>
        /// <param name="output">The output.</param>
        /// <param name="outputOffset">The output offset.</param>
        /// <param name="outputLength">Length of the output.</param>
        /// <param name="knownOutputLength">Set it to <c>true</c> if output length is known.</param>
        /// <returns>Number of bytes written.</returns>
        public static int Decode(
            byte[] input,
            int inputOffset,
            int inputLength,
            byte[] output,
            int outputOffset,
            int outputLength,
            bool knownOutputLength)
        {
            unsafe
            {
                fixed (void* inputPtr = input)
                fixed (void* outputPtr = output)
                {
                    if (knownOutputLength)
                    {
                        int length = Bindings.DecompressFast(new IntPtr(inputPtr) + inputOffset,
                            new IntPtr(outputPtr) + outputOffset,
                            outputLength);
                        if (length != inputLength)
                            throw new ArgumentException("LZ4 block is corrupted, or invalid length has been given.");
                        return outputLength;
                    }
                    else
                    {
                        int length = Bindings.DecompressSafe(new IntPtr(inputPtr) + inputOffset,
                            new IntPtr(outputPtr) + outputOffset,
                            inputLength,
                            outputLength);
                        if (length < 0)
                            throw new ArgumentException("LZ4 block is corrupted, or invalid length has been given.");
                        return length;
                    }
                }
            }
        }

        /// <summary>Decodes the specified input.</summary>
        /// <param name="input">The input.</param>
        /// <param name="inputOffset">The input offset.</param>
        /// <param name="inputLength">Length of the input.</param>
        /// <param name="outputLength">Length of the output.</param>
        /// <returns>Decompressed buffer.</returns>
        public static byte[] Decode(byte[] input, int inputOffset, int inputLength, int outputLength)
        {
            if (inputLength < 0)
                inputLength = input.Length - inputOffset;
            if (input == null)
                throw new ArgumentNullException("input");
            if (inputOffset < 0 || inputOffset + inputLength > input.Length)
                throw new ArgumentException("inputOffset and inputLength are invalid for given input");
            var result = new byte[outputLength];
            var length = Decode(input, inputOffset, inputLength, result, 0, outputLength, false);
            if (length != outputLength)
                throw new ArgumentException("outputLength is not valid");
            return result;
        }

        public static unsafe byte[] Encode(byte[] input, int inputOffset, int inputLength)
        {
            if (inputLength < 0)
                inputLength = input.Length - inputOffset;
            if (input == null)
                throw new ArgumentNullException("input");
            if (inputOffset < 0 || inputOffset + inputLength > input.Length)
                throw new ArgumentException("inputOffset and inputLength are invalid for given input");
            var result = new byte[Bindings.CompressBound(inputLength)];
            int length;
            fixed (void* inputPtr = input)
            fixed (void* resultPtr = result)
            {
                length = Bindings.CompressDefault(
                    new IntPtr(inputPtr) + inputOffset, new IntPtr(resultPtr), inputLength, result.Length);
            }

            if (length != result.Length)
            {
                if (length < 0)
                    throw new InvalidOperationException("Compression has been corrupted");
                var buffer = new byte[length];
                Buffer.BlockCopy(result, 0, buffer, 0, length);
                return buffer;
            }

            return result;
        }
    }
}