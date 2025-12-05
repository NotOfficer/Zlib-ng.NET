using System.Numerics;

namespace ZlibngDotNet;

public unsafe partial class Zlibng
{
    /// <summary>
    ///   Compresses the source buffer into the destination buffer.  The level
    /// parameter has the same meaning as in deflateInit.  sourceLen is the byte
    /// length of the source buffer.  Upon entry, destLen is the total size of the
    /// destination buffer, which must be at least the value returned by
    /// compressBound(sourceLen).  Upon exit, destLen is the actual size of the
    /// compressed data.
    ///
    ///   compress2 returns Z_OK if success, Z_MEM_ERROR if there was not enough
    /// memory, Z_BUF_ERROR if there was not enough room in the output buffer,
    /// Z_STREAM_ERROR if the level parameter is invalid.
    /// </summary>
    public delegate* unmanaged<void*, ref nint, void*, nint, ZlibngCompressionLevel, ZlibngCompressionResult> Compress2FunctionPointer { get; }

    /// <inheritdoc cref="Compress2FunctionPointer" />
    public int Compress2<T1, T2>(nint dest, T1 destLen, nint source, T2 sourceLen, ZlibngCompressionLevel level)
        where T1 : IBinaryInteger<T1> where T2 : IBinaryInteger<T2>
    {
        return Compress2(dest.ToPointer(), destLen, source.ToPointer(), sourceLen, level);
    }

    /// <inheritdoc cref="Compress2FunctionPointer" />
    public int Compress2<T1, T2>(void* dest, T1 destLen, void* source, T2 sourceLen, ZlibngCompressionLevel level)
        where T1 : IBinaryInteger<T1> where T2 : IBinaryInteger<T2>
    {
        return Compress2(new Span<byte>(dest, int.CreateChecked(destLen)),
            new ReadOnlySpan<byte>(source, int.CreateChecked(sourceLen)), level);
    }

    /// <inheritdoc cref="Compress2FunctionPointer" />
    public int Compress2(byte[] dest, int destOffset, int destLen,
        byte[] source, int sourceOffset, int sourceLen, ZlibngCompressionLevel level)
    {
        return Compress2(new Span<byte>(dest, destOffset, destLen),
            new ReadOnlySpan<byte>(source, sourceOffset, sourceLen), level);
    }

    /// <inheritdoc cref="Compress2FunctionPointer" />
    public int Compress2(Span<byte> dest, ReadOnlySpan<byte> source, ZlibngCompressionLevel level)
    {
        fixed (byte* destPtr = dest)
        fixed (byte* sourcePtr = source)
        {
            var destLen = (nint)dest.Length;
            var sourceLen = (nint)source.Length;
            var result = Compress2FunctionPointer(destPtr, ref destLen, sourcePtr, sourceLen, level);
            return Util.GetReturnValue(result, destLen);
        }
    }

    /// <inheritdoc cref="Compress2FunctionPointer" />
    public ZlibngCompressionResult Compress2<T1, T2>(nint dest, ref T1 destLen, nint source, T2 sourceLen, ZlibngCompressionLevel level)
        where T1 : IBinaryInteger<T1> where T2 : IBinaryInteger<T2>
    {
        return Compress2(dest.ToPointer(), ref destLen, source.ToPointer(), sourceLen, level);
    }

    /// <inheritdoc cref="Compress2FunctionPointer" />
    public ZlibngCompressionResult Compress2<T1, T2>(void* dest, ref T1 destLen, void* source, T2 sourceLen, ZlibngCompressionLevel level)
        where T1 : IBinaryInteger<T1> where T2 : IBinaryInteger<T2>
    {
        return Compress2(new Span<byte>(dest, int.CreateChecked(destLen)),
            new ReadOnlySpan<byte>(source, int.CreateChecked(sourceLen)),
            level, out destLen);
    }

    /// <inheritdoc cref="Compress2FunctionPointer" />
    public ZlibngCompressionResult Compress2(byte[] dest, int destOffset, ref int destLen,
        byte[] source, int sourceOffset, int sourceLen, ZlibngCompressionLevel level)
    {
        return Compress2(new Span<byte>(dest, destOffset, destLen),
            new ReadOnlySpan<byte>(source, sourceOffset, sourceLen),
            level, out destLen);
    }

    /// <inheritdoc cref="Compress2FunctionPointer" />
    public ZlibngCompressionResult Compress2<T>(Span<byte> dest, ReadOnlySpan<byte> source, ZlibngCompressionLevel level, out T bytesWritten)
        where T : IBinaryInteger<T>
    {
        fixed (byte* destPtr = dest)
        fixed (byte* sourcePtr = source)
        {
            var destLen = (nint)dest.Length;
            var sourceLen = (nint)source.Length;
            var result = Compress2FunctionPointer(destPtr, ref destLen, sourcePtr, sourceLen, level);
            bytesWritten = T.CreateChecked(destLen);
            return result;
        }
    }
}
