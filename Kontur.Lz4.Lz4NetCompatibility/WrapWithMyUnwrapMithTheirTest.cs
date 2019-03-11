using NUnit.Framework;

namespace Kontur.Lz4.Tests.Lz4NetCompatibility
{
    [TestFixture]
    public class WrapWithMyUnwrapMithTheirTest : WrapUnwrapTestBase
    {
        protected override byte[] Unwrap(byte[] compressed)
        {
            return LZ4.LZ4Codec.Unwrap(compressed);
        }

        protected override byte[] Wrap(byte[] buffer)
        {
            return LZ4Codec.Wrap(buffer);
        }
    }
}