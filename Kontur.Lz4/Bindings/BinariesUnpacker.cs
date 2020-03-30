using System;
using System.Collections;
using System.IO;

namespace Kontur.Lz4.Bindings
{
    internal static class BinariesUnpacker
    {
        public static (string path, bool created) UnpackAssemblyFromResource(string library, string outputFile)
        {
            var resourceName = $"{nameof(Kontur)}.{nameof(Lz4)}." + library;

            var expectedBinaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, outputFile);

            var resource = GetResource(resourceName);

            if (File.Exists(expectedBinaryPath) && ByteArraysEquals(File.ReadAllBytes(expectedBinaryPath), resource))
                return (expectedBinaryPath, false);

            File.WriteAllBytes(expectedBinaryPath, resource);

            return (expectedBinaryPath, true);
        }

        private static byte[] GetResource(string resourceName)
        {
            var assembly = typeof(BinariesUnpacker).Assembly;

            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                    throw new NotSupportedException($"Resource '{resourceName}' is missing");

                using (var memoryStream = new MemoryStream())
                {
                    resourceStream.CopyTo(memoryStream);

                    return memoryStream.ToArray();
                }
            }
        }

        private static bool ByteArraysEquals(byte[] a1, byte[] a2)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(a1, a2);
        }
    }
}