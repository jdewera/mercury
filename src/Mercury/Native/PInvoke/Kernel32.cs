using System.Runtime.InteropServices;
using Mercury.Native.Structs;
using Microsoft.Win32.SafeHandles;

namespace Mercury.Native.PInvoke;

internal static partial class Kernel32
{
    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ReadProcessMemory(SafeProcessHandle processHandle, nint address, out byte bytes, nint size, out nint bytesReadCount);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    internal static partial nint VirtualQueryEx(SafeProcessHandle processHandle, nint address, out MemoryBasicInformation64 memoryInformation, nint size);
}