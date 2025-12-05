using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ZlibngDotNet;

internal static class Util
{
    public static void ThrowIfNull(nint argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument == nint.Zero)
        {
            throw new ArgumentNullException(paramName);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetReturnValue(ZlibngCompressionResult result, nint destLen)
        => result == ZlibngCompressionResult.Ok ? (int)destLen : (int)result;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetReturnValue<T>(int result, T destLen) where T : IBinaryInteger<T>
        => result == 0 ? int.CreateChecked(destLen) : result;

    public static unsafe string GetStringFromPtr(byte* ptr)
    {
        ReadOnlySpan<byte> span = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(ptr);
        return span.Length == 0 ? "" : Encoding.UTF8.GetString(span);
    }
}
