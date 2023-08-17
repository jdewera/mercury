using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Xunit;

namespace Mercury.Tests;

public sealed class MemoryScannerTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(8)]
    [InlineData(13)]
    [InlineData(21)]
    [InlineData(34)]
    [InlineData(55)]
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

        var patternBytes = RandomNumberGenerator.GetBytes(8);
        var patternBytesHandle = GCHandle.Alloc(patternBytes, GCHandleType.Pinned);
        var pattern = string.Join(' ', BitConverter.ToString(patternBytes).Split('-').Select((@byte, i) => i is 1 or 3 or 5 ? "??" : @byte));

        // Act

        var addresses = MemoryScanner.FindPattern(Process.GetCurrentProcess(), pattern);

        // Assert

        Assert.Contains(patternBytesHandle.AddrOfPinnedObject(), addresses);
    }
}