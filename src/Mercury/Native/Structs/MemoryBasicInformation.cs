using System.Runtime.InteropServices;
using Mercury.Native.Enums;

namespace Mercury.Native.Structs;

[StructLayout(LayoutKind.Explicit, Size = 48)]
internal readonly record struct MemoryBasicInformation64([field: FieldOffset(0x0)] long BaseAddress, [field: FieldOffset(0x18)] nint RegionSize, [field: FieldOffset(0x20)] PageState State, [field: FieldOffset(0x24)] PageProtection Protect);