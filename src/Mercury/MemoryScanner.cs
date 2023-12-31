﻿using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
        var defaultShift = patternBytes.Length;
        var lastWildcardIndex = Array.LastIndexOf(patternBytes, "??");

        if (lastWildcardIndex != -1)
        {
            defaultShift -= lastWildcardIndex;
        }

        Array.Fill(shiftTable, defaultShift);

        for (var i = 0; i < patternBytes.Length - 1; i++)
        {
            var @byte = patternBytes[i];

            if (@byte is not null)
            {
                shiftTable[@byte.Value] = patternBytes.Length - 1 - i;
            }
        }

        var regions = GetRegions(process).ToList();
        var occurrences = new List<nint>();

        foreach (var region in regions)
        {
            var regionBytes = new byte[region.Size];

            if (!Kernel32.ReadProcessMemory(process.SafeHandle, region.Address, out regionBytes[0], regionBytes.Length, out var bytesRead))
            {
                if (Marshal.GetLastPInvokeError() == (int) SystemErrorCode.PartialCopy)
                {
                    if (bytesRead == 0)
                    {
                        continue;
                    }
                }
                else
                {
                    throw new Win32Exception();
                }
            }

            for (var i = patternBytes.Length - 1; i < bytesRead; i += shiftTable[regionBytes[i]])
            {
                for (var j = patternBytes.Length - 1; patternBytes[j] is null || patternBytes[j] == regionBytes[i - patternBytes.Length + 1 + j]; j--)
                {
                    if (j == 0)
                    {
                        occurrences.Add(region.Address + i - patternBytes.Length + 1);
                        break;
                    }
                }
            }
        }

        return occurrences;
    }

    private static IEnumerable<(nint Address, int Size)> GetRegions(Process process)
    {
        nint currentAddress = 0;

        while (true)
        {
            if (Kernel32.VirtualQueryEx(process.SafeHandle, currentAddress, out var region, Unsafe.SizeOf<MemoryBasicInformation64>()) == 0)
            {
                if (Marshal.GetLastPInvokeError() == (int) SystemErrorCode.InvalidParameter)
                {
                    break;
                }

                throw new Win32Exception();
            }

            if (region.State.HasFlag(AllocationType.Commit) && region.Protect != ProtectionType.NoAccess && !region.Protect.HasFlag(ProtectionType.Guard))
            {
                yield return (currentAddress, (int) region.RegionSize);
            }

            currentAddress = (nint) region.BaseAddress + region.RegionSize;
        }
    }
}