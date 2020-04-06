namespace Kontur.Lz4.Tests.Lz4NetCompatibility
{
    public class EncodeWithTheirDecodeWithMyTest : EncodeDecodeTestBase
    {
        protected override int Decode(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset, int outputLength = 0,
            bool knownOutputLength = false)
        {
            return LZ4Codec.Decode(input, inputOffset, inputLength, output, outputOffset, outputLength,
                knownOutputLength);
        }

        protected override byte[] Decode(byte[] encoded, int offsetDecode, int i, int lengthForEncodeInput)
        {
            return LZ4Codec.Decode(encoded, offsetDecode, encoded.Length - offsetDecode,
                lengthForEncodeInput);
        }

        protected override byte[] Encode(byte[] original, int offsetEncode, int lengthForEncodeInput)
        {
            return LZ4.LZ4Codec.Encode(original, offsetEncode, lengthForEncodeInput);
        }
    }
}