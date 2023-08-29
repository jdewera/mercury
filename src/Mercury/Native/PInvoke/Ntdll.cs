using System.Runtime.InteropServices;
using Mercury.Native.Enums;
using Mercury.Native.Structs;
using Microsoft.Win32.SafeHandles;

namespace Mercury.Native.PInvoke;

internal static partial class Ntdll
{
    [LibraryImport("ntdll.dll")]
    internal static partial NtStatus NtQueryVirtualMemory(SafeProcessHandle processHandle, nint baseAddress, MemoryInformationClass memoryInformationClass, out MemoryBasicInformation64 memoryInformation, nint memoryInformationLength, nint returnLength);

    [LibraryImport("ntdll.dll")]
    internal static partial NtStatus NtReadVirtualMemory(SafeProcessHandle processHandle, nint baseAddress, out byte buffer, nint numberOfBytesToRead, out int numberOfBytesRead);

    [LibraryImport("ntdll.dll")]
    internal static partial int RtlNtStatusToDosError(NtStatus status);
}