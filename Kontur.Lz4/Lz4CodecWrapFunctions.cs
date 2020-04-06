using System;

namespace Kontur.Lz4
{
    internal static class Lz4CodecWrapFunctions
    {
        // based on code from https://github.com/MiloszKrajewski/lz4net  (c) 2013-2017, Milosz Krajewski

        private const int WRAP_OFFSET_0 = 0;
        private const int WRAP_OFFSET_4 = sizeof(int);
        private const int WRAP_OFFSET_8 = 2 * sizeof(int);
        private const int WRAP_LENGTH = WRAP_OFFSET_8;

        /// <summary>Sets uint32 value in byte buffer.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        private static void Poke4(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte) value;
            buffer[offset + 1] = (byte) (value >> 8);
            buffer[offset + 2] = (byte) (value >> 16);
            buffer[offset + 3] = (byte) (value >> 24);
        }

        /// <summary>Gets uint32 from byte buffer.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The value.</returns>
        private static uint Peek4(byte[] buffer, int offset)
        {
            // NOTE: It's faster than BitConverter.ToUInt32 (suprised? me too)
            return
                // ReSharper disable once RedundantCast
                (uint) buffer[offset] |
                ((uint) buffer[offset + 1] << 8) |
                ((uint) buffer[offset + 2] << 16) |
                ((uint) buffer[offset + 3] << 24);
        }

        /// <summary>Compresses and wraps given input byte buffer.</summary>
        /// <param name="inputBuffer">The input buffer.</param>
        /// <param name="inputOffset">The input offset.</param>
        /// <param name="inputLength">Length of the input.</param>
        /// <returns>Compressed buffer.</returns>
        /// <exception cref="System.ArgumentException">inputBuffer size of inputLength is invalid</exception>
        public static unsafe byte[] Wrap(byte[] inputBuffer, int inputOffset, int inputLength)
        {
            inputLength = Math.Min(inputBuffer.Length - inputOffset, inputLength);
            if (inputLength < 0)
                throw new ArgumentException("inputBuffer size of inputLength is invalid");
            if (inputLength == 0)
                return new byte[WRAP_LENGTH];

            var outputLength = inputLength; // MaximumOutputLength(inputLength);
            var outputBuffer = new byte[outputLength];
            fixed (void* inputBufferPtr = inputBuffer)
            fixed (void* outputBufferPtr = outputBuffer)
            {
                outputLength = Lz4CodecEncodeFunctions.Bindings.CompressDefault(new IntPtr(inputBufferPtr) + inputOffset
                    , new IntPtr(outputBufferPtr), inputLength, outputLength);
            }

            byte[] result;

            if (outputLength >= inputLength || outputLength <= 0)
            {
                result = new byte[inputLength + WRAP_LENGTH];
                Poke4(result, WRAP_OFFSET_0, (uint) inputLength);
                Poke4(result, WRAP_OFFSET_4, (uint) inputLength);
                Buffer.BlockCopy(inputBuffer, inputOffset, result, WRAP_OFFSET_8, inputLength);
            }
            else
            {
                result = new byte[outputLength + WRAP_LENGTH];
                Poke4(result, WRAP_OFFSET_0, (uint) inputLength);
                Poke4(result, WRAP_OFFSET_4, (uint) outputLength);
                Buffer.BlockCopy(outputBuffer, 0, result, WRAP_OFFSET_8, outputLength);
            }

            return result;
        }

        /// <summary>Unwraps the specified compressed buffer.</summary>
        /// <param name="inputBuffer">The input buffer.</param>
        /// <param name="inputOffset">The input offset.</param>
        /// <returns>Uncompressed buffer.</returns>
        /// <exception cref="System.ArgumentException">
        ///     inputBuffer size is invalid or inputBuffer size is invalid or has been corrupted
        /// </exception>
        public static unsafe byte[] Unwrap(byte[] inputBuffer, int inputOffset = 0)
        {
            var inputLength = inputBuffer.Length - inputOffset;
            if (inputLength < WRAP_LENGTH)
                throw new ArgumentException("inputBuffer size is invalid");

            var outputLength = (int) Peek4(inputBuffer, inputOffset + WRAP_OFFSET_0);
            inputLength = (int) Peek4(inputBuffer, inputOffset + WRAP_OFFSET_4);
            if (inputLength > inputBuffer.Length - inputOffset - WRAP_LENGTH)
                throw new ArgumentException("inputBuffer size is invalid or has been corrupted");

            byte[] result;

            if (inputLength >= outputLength)
            {
                result = new byte[inputLength];
                Buffer.BlockCopy(inputBuffer, inputOffset + WRAP_OFFSET_8, result, 0, inputLength);
            }
            else
            {
                result = new byte[outputLength];
                fixed (void* inputBufferPtr = inputBuffer)
                fixed (void* resultPtr = result)
                {
                    Lz4CodecEncodeFunctions.Bindings.DecompressSafe(new IntPtr(inputBufferPtr) + inputOffset + WRAP_OFFSET_8,
                        new IntPtr(resultPtr), inputLength, outputLength);
                }
            }

            return result;
        }
    }
}