namespace Mercury.Native.Enums;

[Flags]
internal enum PageProtection
{
    NoAccess = 0x1,
    Guard = 0x100
}