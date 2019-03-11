using NUnit.Framework;

namespace Kontur.Lz4.Tests.Lz4NetCompatibility
{
    [TestFixture]
    public class WrapWithTheirUnwrapMithMyTest : WrapUnwrapTestBase
    {
        protected override byte[] Unwrap(byte[] compressed)
        {
            return LZ4Codec.Unwrap(compressed);
        }

        protected override byte[] Wrap(byte[] buffer)
        {
            return LZ4.LZ4Codec.Wrap(buffer);
        }
    }
}