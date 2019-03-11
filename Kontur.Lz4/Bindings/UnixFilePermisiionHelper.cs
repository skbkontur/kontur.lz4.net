using System;
using System.Runtime.InteropServices;

namespace Kontur.Lz4.Bindings
{
    internal static class UnixFilePermisiionHelper
    {
        [DllImport("libc", SetLastError = true)]
        private static extern int chmod(string pathname, UnixPermissions mode);


        public static void Set755(string fileName)
        {
            var setPermissions = UnixPermissions.S_IRUSR | UnixPermissions.S_IXUSR | UnixPermissions.S_IWUSR
                                 | UnixPermissions.S_IRGRP | UnixPermissions.S_IXGRP
                                 | UnixPermissions.S_IROTH | UnixPermissions.S_IXOTH;
            if (chmod(fileName, setPermissions) != 0)
                throw new Exception("chmod failed with error = " + Marshal.GetLastWin32Error());
        }
    }
}