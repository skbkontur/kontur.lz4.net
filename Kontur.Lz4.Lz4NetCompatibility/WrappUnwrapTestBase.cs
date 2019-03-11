using System;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Kontur.Lz4.Tests.Lz4NetCompatibility
{
    public abstract class WrapUnwrapTestBase
    {
        public const string LoremIpsum =
            "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod " +
            "tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, " +
            "quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo " +
            "consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse " +
            "cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat " +
            "non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

        [Test]
        public void WrapLorem()
        {
            var longLorem = string.Concat(Enumerable.Repeat(LoremIpsum, 5));

            var buffer = Encoding.UTF8.GetBytes(longLorem);

            var compressed = Wrap(buffer);
            var decompressed = Unwrap(compressed);

            Assert.AreEqual(longLorem, Encoding.UTF8.GetString(decompressed));
        }

        [Test]
        public void WrapRandom()
        {
            var buffer = new byte[2048];
            var random = new Random(0);
            random.NextBytes(buffer);

            var compressed = Wrap(buffer);
            var decompressed = Unwrap(compressed);

            Assert.AreEqual(
                Convert.ToBase64String(buffer),
                Convert.ToBase64String(decompressed));
        }

        protected abstract byte[] Unwrap(byte[] compressed);


        protected abstract byte[] Wrap(byte[] buffer);
    }
}