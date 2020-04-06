using System;
using System.Runtime.InteropServices;

namespace Kontur.Lz4.Bindings
{
    internal static class UnixFilePermissionHelper
    {
        [DllImport("libc", SetLastError = true)]
        private static extern int chmod(string pathname, UnixPermissions mode);

        public static Exception TrySet755(string fileName)
        {
            var setPermissions = UnixPermissions.S_IRUSR | UnixPermissions.S_IXUSR | UnixPermissions.S_IWUSR
                                 | UnixPermissions.S_IRGRP | UnixPermissions.S_IXGRP
                                 | UnixPermissions.S_IROTH | UnixPermissions.S_IXOTH;
            return chmod(fileName, setPermissions) != 0
                ? new Exception("chmod failed with error = " + Marshal.GetLastWin32Error())
                : null;
        }
    }
}