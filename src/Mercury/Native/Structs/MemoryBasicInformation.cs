using System.Runtime.InteropServices;
using Mercury.Native.Enums;

namespace Mercury.Native.Structs;

[StructLayout(LayoutKind.Explicit, Size = 48)]
internal readonly record struct MemoryBasicInformation64
{
    [field: FieldOffset(0x0)]
    internal long BaseAddress { get; init; }

    [field: FieldOffset(0x18)]
    internal nint RegionSize { get; init; }

    [field: FieldOffset(0x20)]
    internal PageState State { get; init; }

    [field: FieldOffset(0x24)]
    internal PageProtection Protect { get; init; }
}