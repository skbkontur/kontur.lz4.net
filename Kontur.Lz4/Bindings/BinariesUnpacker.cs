using System;
using System.Collections;
using System.IO;
using System.Reflection;
// ReSharper disable AssignNullToNotNullAttribute

namespace Kontur.Lz4.Bindings
{
    internal static class BinariesUnpacker
    {
        public static (string path, bool created) UnpackAssemblyFromResource(string library, string outputFile)
        {
            var directory = GetDirectoryName();
            var expectedBinaryPath = Path.Combine(directory, outputFile);

            var resourceName = $"{nameof(Kontur)}.{nameof(Lz4)}." + library;
            var resource = GetResource(resourceName);

            if (File.Exists(expectedBinaryPath) && ByteArraysEquals(File.ReadAllBytes(expectedBinaryPath), resource))
                return (expectedBinaryPath, false);

            Directory.CreateDirectory(Path.GetDirectoryName(expectedBinaryPath));
            File.WriteAllBytes(expectedBinaryPath, resource);

            return (expectedBinaryPath, true);
        }

        private static string GetDirectoryName()
        {
            var location = Assembly.GetEntryAssembly()?.Location;
            if (location != null)
                return Path.GetDirectoryName(location);

            return AppDomain.CurrentDomain.BaseDirectory;
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