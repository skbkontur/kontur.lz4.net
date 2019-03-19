using System;
using Kontur.Lz4.Bindings;

namespace Kontur.Lz4
{
    public static class LZ4Codec
    {
        // based on code from https://github.com/MiloszKrajewski/lz4net  (c) 2013-2017, Milosz Krajewski

        internal static ILz4Bindings Bindings = PlatformBindingProvider.GetBinding();

        /// <summary>Decodes the specified input.</summary>
        /// <param name="input">The input.</param>
        /// <param name="inputOffset">The input offset.</param>
        /// <param name="inputLength">Length of the input.</param>
        /// <param name="outputLength">Length of the output.</param>
        /// <returns>Decompressed buffer.</returns>
        public static byte[] Decode(byte[] input, int inputOffset, int inputLength, int outputLength)
        {
            return Lz4CodecEncodeFunctions.Decode(input, inputOffset, inputLength, outputLength);
        }



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
            int outputLength = 0,
            bool knownOutputLength = false)
        {
            return Lz4CodecEncodeFunctions.Decode(input, inputOffset, inputLength, output, outputOffset, outputLength, knownOutputLength);
        }

        /// <summary>Encodes the specified input.</summary>
        /// <param name="input">The input.</param>
        /// <param name="inputOffset">The input offset.</param>
        /// <param name="inputLength">Length of the input.</param>
        /// <returns>Compressed buffer.</returns>
        public static byte[] Encode(byte[] input, int inputOffset, int inputLength)
        {
            return Lz4CodecEncodeFunctions.Encode(input, inputOffset, inputLength);
        }


        /// <summary>Encodes the specified input.</summary>
        /// <param name="input">The input.</param>
        /// <param name="inputOffset">The input offset.</param>
        /// <param name="inputLength">Length of the input.</param>
        /// <param name="output">The output.</param>
        /// <param name="outputOffset">The output offset.</param>
        /// <param name="outputLength">Length of the output.</param>
        /// <returns>Number of bytes written.</returns>
        public static int Encode(
            byte[] input,
            int inputOffset,
            int inputLength,
            byte[] output,
            int outputOffset,
            int outputLength)
        {
            return Lz4CodecEncodeFunctions.Encode(input, inputOffset, inputLength, output, outputOffset, outputLength);
        }


        /// <summary>Compresses and wraps given input byte buffer.</summary>
        /// <param name="inputBuffer">The input buffer.</param>
        /// <param name="inputOffset">The input offset.</param>
        /// <param name="inputLength">Length of the input.</param>
        /// <returns>Compressed buffer.</returns>
        /// <exception cref="System.ArgumentException">inputBuffer size of inputLength is invalid</exception>
        public static byte[] Wrap(byte[] inputBuffer, int inputOffset = 0, int inputLength = int.MaxValue)
        {
            return Lz4CodecWrapFunctions.Wrap(inputBuffer, inputOffset, inputLength);
        }

        /// <summary>Unwraps the specified compressed buffer.</summary>
        /// <param name="inputBuffer">The input buffer.</param>
        /// <param name="inputOffset">The input offset.</param>
        /// <returns>Uncompressed buffer.</returns>
        /// <exception cref="System.ArgumentException">
        ///     inputBuffer size is invalid or inputBuffer size is invalid or has been corrupted
        /// </exception>
        public static byte[] Unwrap(byte[] inputBuffer, int inputOffset = 0)
        {
            return Lz4CodecWrapFunctions.Unwrap(inputBuffer, inputOffset);
        }
    }
}