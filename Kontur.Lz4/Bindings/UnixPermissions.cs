using System;

namespace Kontur.Lz4.Bindings
{
    [Flags]
    internal enum UnixPermissions
    {
        S_IRUSR = 0x100,
        S_IWUSR = 0x80,
        S_IXUSR = 0x40,

        // group permission
        S_IRGRP = 0x20,
        S_IWGRP = 0x10,
        S_IXGRP = 0x8,

        // other permissions
        S_IROTH = 0x4,
        S_IWOTH = 0x2,
        S_IXOTH = 0x1
    }
}