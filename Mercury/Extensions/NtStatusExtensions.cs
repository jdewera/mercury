using Mercury.Native.Enums;

namespace Mercury.Extensions;

internal static class NtStatusExtensions
{
    internal static bool IsSuccess(this NtStatus status)
    {
        return (int) status >= 0;
    }
}