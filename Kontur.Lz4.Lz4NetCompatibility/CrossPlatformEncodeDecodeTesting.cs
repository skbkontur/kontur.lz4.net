using System;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Kontur.Lz4.Tests.Lz4NetCompatibility
{
    [TestFixture]
    public class CrossPlatformTests
    {
        private static byte[] _originalBytes = Encoding.ASCII.GetBytes("abracadabracodabraabracadabracodaabracada bra");

        private static byte[] _encodedOriginalWin64 =
            Convert.FromBase64String("cWFicmFjYWQHABFvBwABCwAGEgADDwBQYSBicmE=");


        [Explicit]
        [Test]
        public void EncodeSampleWithTheir()
        {
            Console.Write(Convert.ToBase64String(LZ4.LZ4Codec.Encode(_originalBytes, 0, _originalBytes.Length)));
        }

        [Test]
        public void DecodeSampleFromWin64()
        {
            var decoded = LZ4Codec.Decode(_encodedOriginalWin64, 0, _encodedOriginalWin64.Length, _originalBytes.Length);
            decoded.SequenceEqual(_originalBytes).Should().BeTrue();
        }
    }
}