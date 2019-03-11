using System;
using System.IO;
using System.Reflection;

namespace Kontur.Lz4.Bindings
{
    internal static class BinariesUnpacker
    {
        public static void UnpackAssemblyFromResource(string library, string outputFile)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"{nameof(Kontur)}.{nameof(Lz4)}." + library;

            using (var input = assembly.GetManifestResourceStream(resourceName))
            {
                if (input == null)
                    throw new InvalidOperationException($"Resource with name '{resourceName}' was not found.");
                using (var outStream = File.Open(outputFile, FileMode.OpenOrCreate))
                {
                    input.CopyTo(outStream);

                }
            }
        }
    }
}