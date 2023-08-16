## Mercury

A high performance memory scanner with support for wildcards

---

### Notable features

- Boyer-Moore-Horspool algorithm for fast pattern matching
- Support for wildcard bytes

---

### Getting started

The example below demonstrates a basic implementation of the library

```c#
var process = Process.GetProcessesByName("")[0];
var pattern = "7F ?? 01";
var addresses = MemoryScanner.FindPattern(process, pattern);
```

---

### MemoryScanner class

An instance for scanning process memory

```c#
public static class MemoryScanner
```

### Methods

Searches a process's memory for the specified pattern

```c#
public static ICollection<nint> FindPattern(Process process, string pattern)
```