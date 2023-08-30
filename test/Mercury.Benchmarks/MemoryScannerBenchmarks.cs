using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;

namespace Mercury.Benchmarks;

[MemoryDiagnoser]
public class MemoryScannerBenchmarks
{
    [Params(1, 10, 100, 1000)]
    public int PatternLength { get; set; }

    private string _pattern = null!;
    private GCHandle _patternHandle;

    [Benchmark]
    public void FindPattern()
    {
        MemoryScanner.FindPattern(Process.GetCurrentProcess(), _pattern);
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _patternHandle.Free();
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        var patternBytes = RandomNumberGenerator.GetBytes(PatternLength);

        _pattern = BitConverter.ToString(patternBytes).Replace('-', ' ');
        _patternHandle = GCHandle.Alloc(patternBytes, GCHandleType.Pinned);
    }
}