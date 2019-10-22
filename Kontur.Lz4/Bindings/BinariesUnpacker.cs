using System;
using System.IO;

namespace Kontur.Lz4.Bindings
{
    internal static class BinariesUnpacker
    {
        public static string UnpackAssemblyFromResource(string library, string outputFile)
        {
            var assembly = typeof(BinariesUnpacker).Assembly;
            var resourceName = $"{nameof(Kontur)}.{nameof(Lz4)}." + library;


            var expectedBinaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, outputFile);
            try
            {
                File.Delete(expectedBinaryPath);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }

            if (File.Exists(expectedBinaryPath))
                return expectedBinaryPath;
            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                    throw new NotSupportedException($"Resource '{resourceName}' is missing");
                using (var resultStream = new FileStream(expectedBinaryPath, FileMode.Create, FileAccess.ReadWrite,
                    FileShare.Read))
                {
                    resourceStream.CopyTo(resultStream);
                }
            }

            return expectedBinaryPath;
        }
    }
}