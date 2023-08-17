using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Xunit;

namespace Mercury.Tests;

public sealed class MemoryScannerTests
{
    [Fact]
    public void FindPattern_FindsPattern()
    {
        // Arrange

        var patternBytes = RandomNumberGenerator.GetBytes(20);
        var patternBytesHandle = GCHandle.Alloc(patternBytes, GCHandleType.Pinned);
        var pattern = BitConverter.ToString(patternBytes).Replace('-', ' ');

        // Act

        var addresses = MemoryScanner.FindPattern(Process.GetCurrentProcess(), pattern);

        // Assert

        Assert.Contains(patternBytesHandle.AddrOfPinnedObject(), addresses);
    }

    [Fact]
    public void FindPattern_WithWildcard_FindsPattern()
    {
        // Arrange

        var patternBytes = RandomNumberGenerator.GetBytes(20);
        var patternBytesHandle = GCHandle.Alloc(patternBytes, GCHandleType.Pinned);
        var pattern = string.Join(' ', BitConverter.ToString(patternBytes).Split('-').Select((@byte, i) => i is 1 or 5 or 11 ? "??" : @byte));

        // Act

        var addresses = MemoryScanner.FindPattern(Process.GetCurrentProcess(), pattern);

        // Assert

        Assert.Contains(patternBytesHandle.AddrOfPinnedObject(), addresses);
    }
}