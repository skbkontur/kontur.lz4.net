using System;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Kontur.Lz4.Tests
{
    [TestFixture]
    public class CrossPlatformTests
    {
        private static byte[] _originalBytes = Encoding.ASCII.GetBytes("abracadabracodabraabracadabracodaabracada bra");

        private static byte[] _encodedOriginal =
            Convert.FromBase64String("cWFicmFjYWQHABFvBwABCwAGEgADDwBQYSBicmE=");

        
        [Test]
        public void DecodeSampleFromOriginal()
        {
            var decoded = LZ4Codec.Decode(_encodedOriginal, 0, _encodedOriginal.Length, _originalBytes.Length);
            decoded.SequenceEqual(_originalBytes).Should().BeTrue();
        }
        
        [Test]
        public void EncodeIsStable()
        {
            var encoded = LZ4Codec.Encode(_originalBytes, 0, _originalBytes.Length);
            encoded.SequenceEqual(_encodedOriginal).Should().BeTrue();
        }
    }
}