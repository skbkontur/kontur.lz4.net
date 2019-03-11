namespace Kontur.Lz4.Tests.Lz4NetCompatibility
{
    public class EncodeWithTheirDecodeWithMyTest: EncodeDecodeTestBase
    {
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