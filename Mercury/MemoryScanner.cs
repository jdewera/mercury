using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mercury.Extensions;
using Mercury.Native.Enums;
using Mercury.Native.PInvoke;
using Mercury.Native.Structs;

namespace Mercury;

/// <summary>
/// An instance for scanning process memory
/// </summary>
public static class MemoryScanner
{
    /// <summary>
    /// Searches a process's memory for the specified pattern
    /// </summary>
    public static ICollection<nint> FindPattern(Process process, string pattern)
    {
        var patternComponents = pattern.Split(' ');
        var patternBytes = new byte?[patternComponents.Length];

        for (var i = 0; i < patternComponents.Length; i++)
        {
            if (patternComponents[i] == "??")
            {
                patternBytes[i] = null;
            }
            else
            {
                patternBytes[i] = Convert.ToByte(patternComponents[i], 16);
            }
        }

        var shiftTable = new int[256];

        for (var i = 0; i < shiftTable.Length; i++)
        {
            shiftTable[i] = patternBytes.Length;
        }

        for (var i = 0; i < patternBytes.Length - 1; i++)
        {
            var @byte = patternBytes[i];

            if (@byte is not null)
            {
                shiftTable[@byte.Value] = patternBytes.Length - 1 - i;
            }
        }

        var occurrences = new ConcurrentBag<nint>();

        Parallel.ForEach(GetRegions(process), region =>
        {
            for (var i = patternBytes.Length - 1; i < region.Bytes.Length; i += shiftTable[region.Bytes[i]])
            {
                for (var j = patternBytes.Length - 1; patternBytes[j] is null || patternBytes[j] == region.Bytes[i - patternBytes.Length + 1 + j]; j--)
                {
                    if (j == 0)
                    {
                        occurrences.Add(region.Address + i - patternBytes.Length + 1);
                        break;
                    }
                }
            }
        });

        return occurrences.Order().ToList();
    }

    private static IEnumerable<(nint Address, byte[] Bytes)> GetRegions(Process process)
    {
        nint currentAddress = 0;

        while (true)
        {
            var status = Ntdll.NtQueryVirtualMemory(process.SafeHandle, currentAddress, MemoryInformationClass.MemoryBasicInformation, out var region, Unsafe.SizeOf<MemoryBasicInformation64>(), 0);

            if (!status.IsSuccess())
            {
                break;
            }

            if (region.State.HasFlag(PageState.Commit) && region.Protect != PageProtection.NoAccess && !region.Protect.HasFlag(PageProtection.Guard))
            {
                var regionBytes = new byte[region.RegionSize];

                status = Ntdll.NtReadVirtualMemory(process.SafeHandle, currentAddress, out regionBytes[0], regionBytes.Length, 0);

                if (!status.IsSuccess())
                {
                    throw new Win32Exception(Ntdll.RtlNtStatusToDosError(status));
                }

                yield return (currentAddress, regionBytes);
            }

            currentAddress = (nint) region.BaseAddress + region.RegionSize;
        }
    }
}