using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Xunit;

namespace Mercury.Tests;

public class MemoryScannerTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    public void FindPattern_FindsPattern(int length)
    {
        // Arrange

        var patternBytes = RandomNumberGenerator.GetBytes(length);
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

        var patternBytes = RandomNumberGenerator.GetBytes(10);
        var patternBytesHandle = GCHandle.Alloc(patternBytes, GCHandleType.Pinned);
        var pattern = string.Join(' ', BitConverter.ToString(patternBytes).Split('-').Select((@byte, i) => i is 1 or 3 or 5 ? "??" : @byte));

        // Act

        var addresses = MemoryScanner.FindPattern(Process.GetCurrentProcess(), pattern);

        // Assert

        Assert.Contains(patternBytesHandle.AddrOfPinnedObject(), addresses);
    }
}