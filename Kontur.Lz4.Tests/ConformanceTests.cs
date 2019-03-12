using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using FluentAssertions;
using NUnit.Framework;

namespace Kontur.Lz4.Tests
{
    [TestFixture]
    public class ConformanceTests
    {
        private readonly RNGCryptoServiceProvider _cryptoRandomProvider = new RNGCryptoServiceProvider();

        [TestCase(@"Samples/EngText.txt", 0, 0)]
        [TestCase(@"Samples/RusText.txt", 0, 0)]
        [TestCase(@"Samples/EngText.txt", 9, 0)]
        [TestCase(@"Samples/RusText.txt", 9, 0)]
        [TestCase(@"Samples/EngText.txt", 0, 5)]
        [TestCase(@"Samples/EngText.txt", 6, 5)]
        [TestCase(@"Samples/RusText.txt", 0, 6)]
        public void TestEncodeDecodeOnFileFragments(string fileName, int offsetEncode, int offsetDecode)
        {
            var rand = new Random();
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                for (var i = 0; i < 100; i++)
                {
                    var maxSize = file.Length;
                    var offsetLength = rand.Next((int) maxSize * 3 / 4);
                    file.Seek(offsetLength, SeekOrigin.Begin);
                    var original = new byte[rand.Next(100, (int) maxSize - offsetLength)];
                    file.Read(original, 0, original.Length);
                    file.Seek(0, SeekOrigin.Begin);
                    var lengthForEncodeInput = original.Length - offsetEncode;
                    var encoded = LZ4Codec.Encode(original, offsetEncode, lengthForEncodeInput);
                    if (offsetDecode > 0) encoded = EnlargeArray(encoded, offsetDecode);

                    var decoded = LZ4Codec.Decode(encoded, offsetDecode, encoded.Length - offsetDecode,
                        lengthForEncodeInput);

                    Assert.Throws<ArgumentException>(() =>
                        LZ4Codec.Decode(encoded, offsetDecode, encoded.Length - offsetDecode,
                            lengthForEncodeInput - 1));

                    decoded.SequenceEqual(original.Skip(offsetEncode)).Should().BeTrue();
                }
            }
        }

        [Combinatorial()]
        [Test]
        public void TestEncodeDecodeOnFileFragments_WithExistingBuffer(
            [Values(@"Samples/EngText.txt", @"Samples/RusText.txt")]
            string fileName, [Values(0, 9)] int offsetEncode, [Values(0, 5)] int offsetDecode,
            [Values(0, 7)] int offsetInDecodedBuffer,

            [Values(true, false)] bool knownOutputSize)
        {
            var rand = new Random();
            using (var file = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                for (var i = 0; i < 100; i++)
                {
                    var maxSize = file.Length;
                    var offsetLength = rand.Next((int) maxSize * 3 / 4);
                    file.Seek(offsetLength, SeekOrigin.Begin);
                    var original = new byte[rand.Next(100, (int) maxSize - offsetLength)];
                    file.Read(original, 0, original.Length);
                    file.Seek(0, SeekOrigin.Begin);
                    var lengthForEncodeInput = original.Length - offsetEncode;
                    var encoded = LZ4Codec.Encode(original, offsetEncode, lengthForEncodeInput);
                    if (offsetDecode > 0) encoded = EnlargeArray(encoded, offsetDecode);

                    var decoded = new byte[lengthForEncodeInput + offsetInDecodedBuffer];
                    LZ4Codec.Decode(encoded, offsetDecode, encoded.Length - offsetDecode, decoded, offsetInDecodedBuffer,
                            lengthForEncodeInput, knownOutputSize)
                        .Should().BeGreaterThan(0);

                    var decodedTrash = new byte[lengthForEncodeInput + offsetInDecodedBuffer - 1];

                    Assert.Throws<ArgumentException>(() =>
                        LZ4Codec.Decode(encoded, offsetDecode, encoded.Length - offsetDecode, decodedTrash, offsetInDecodedBuffer,
                            lengthForEncodeInput - 1, knownOutputSize));

                    decoded.Skip(offsetInDecodedBuffer).SequenceEqual(original.Skip(offsetEncode)).Should().BeTrue();
                }
            }
        }

        [TestCase(0, 3)]
        [TestCase(3, 3)]
        [TestCase(0, 0)]
        [Repeat(100)]
        public void TestEncodeDecodeOnRandomData(int offsetEncode, int offsetDecode)
        {
            var rand = new Random();
            var original = new byte[rand.Next(10000, 200000)];
            _cryptoRandomProvider.GetBytes(original);

            var lengthForEncodeInput = original.Length - offsetEncode;
            var encoded = LZ4Codec.Encode(original, offsetEncode, lengthForEncodeInput);
            if (offsetDecode > 0) encoded = EnlargeArray(encoded, offsetDecode);
            var decoded = LZ4Codec.Decode(encoded, offsetDecode, encoded.Length - offsetDecode,
                lengthForEncodeInput);
            decoded.SequenceEqual(original.Skip(offsetEncode)).Should().BeTrue();
        }

        private static byte[] EnlargeArray(byte[] array, int additionalSize)
        {
            var newEncoded = new byte[array.Length + additionalSize];
            Buffer.BlockCopy(array, 0, newEncoded, additionalSize, array.Length);
            array = newEncoded;
            return array;
        }
    }
}