using System.Numerics;

namespace ZlibngDotNet;

public unsafe partial class Zlibng
{
    /// <summary>
    ///   Compresses the source buffer into the destination buffer.  sourceLen is<br/>
    /// the byte length of the source buffer.  Upon entry, destLen is the total size<br/>
    /// of the destination buffer, which must be at least the value returned by<br/>
    /// compressBound(sourceLen).  Upon exit, destLen is the actual size of the<br/>
    /// compressed data.  compress() is equivalent to compress2() with a level<br/>
    /// parameter of Z_DEFAULT_COMPRESSION.<br/>
    /// <br/>
    ///   compress returns Z_OK if success, Z_MEM_ERROR if there was not<br/>
    /// enough memory, Z_BUF_ERROR if there was not enough room in the output<br/>
    /// buffer.
    /// </summary>
    public delegate* unmanaged<void*, ref nint, void*, nint, ZlibngCompressionResult> CompressFunctionPointer { get; }

    /// <inheritdoc cref="CompressFunctionPointer" />
    public int Compress<T1, T2>(nint dest, T1 destLen, nint source, T2 sourceLen)
        where T1 : IBinaryInteger<T1> where T2 : IBinaryInteger<T2>
    {
        return Compress(dest.ToPointer(), destLen, source.ToPointer(), sourceLen);
    }

    /// <inheritdoc cref="CompressFunctionPointer" />
    public int Compress<T1, T2>(void* dest, T1 destLen, void* source, T2 sourceLen)
        where T1 : IBinaryInteger<T1> where T2 : IBinaryInteger<T2>
    {
        return Compress(new Span<byte>(dest, int.CreateChecked(destLen)),
            new ReadOnlySpan<byte>(source, int.CreateChecked(sourceLen)));
    }

    /// <inheritdoc cref="CompressFunctionPointer" />
    public int Compress(byte[] dest, int destOffset, int destLen,
        byte[] source, int sourceOffset, int sourceLen)
    {
        return Compress(new Span<byte>(dest, destOffset, destLen),
            new ReadOnlySpan<byte>(source, sourceOffset, sourceLen));
    }

    /// <inheritdoc cref="CompressFunctionPointer" />
    public int Compress(Span<byte> dest, ReadOnlySpan<byte> source)
    {
        fixed (byte* destPtr = dest)
        fixed (byte* sourcePtr = source)
        {
            IntPtr destLen = (nint)dest.Length;
            IntPtr sourceLen = (nint)source.Length;
            ZlibngCompressionResult result = CompressFunctionPointer(destPtr, ref destLen, sourcePtr, sourceLen);
            return Util.GetReturnValue(result, destLen);
        }
    }

    /// <inheritdoc cref="CompressFunctionPointer" />
    public ZlibngCompressionResult Compress<T1, T2>(nint dest, ref T1 destLen, nint source, T2 sourceLen)
        where T1 : IBinaryInteger<T1> where T2 : IBinaryInteger<T2>
    {
        return Compress(dest.ToPointer(), ref destLen, source.ToPointer(), sourceLen);
    }

    /// <inheritdoc cref="CompressFunctionPointer" />
    public ZlibngCompressionResult Compress<T1, T2>(void* dest, ref T1 destLen, void* source, T2 sourceLen)
        where T1 : IBinaryInteger<T1> where T2 : IBinaryInteger<T2>
    {
        return Compress(new Span<byte>(dest, int.CreateChecked(destLen)),
            new ReadOnlySpan<byte>(source, int.CreateChecked(sourceLen)),
            out destLen);
    }

    /// <inheritdoc cref="CompressFunctionPointer" />
    public ZlibngCompressionResult Compress(byte[] dest, int destOffset, ref int destLen,
        byte[] source, int sourceOffset, int sourceLen)
    {
        return Compress(new Span<byte>(dest, destOffset, destLen),
            new ReadOnlySpan<byte>(source, sourceOffset, sourceLen),
            out destLen);
    }

    /// <inheritdoc cref="CompressFunctionPointer" />
    public ZlibngCompressionResult Compress<T>(Span<byte> dest, ReadOnlySpan<byte> source, out T bytesWritten)
        where T : IBinaryInteger<T>
    {
        fixed (byte* destPtr = dest)
        fixed (byte* sourcePtr = source)
        {
            IntPtr destLen = (nint)dest.Length;
            IntPtr sourceLen = (nint)source.Length;
            ZlibngCompressionResult result = CompressFunctionPointer(destPtr, ref destLen, sourcePtr, sourceLen);
            bytesWritten = T.CreateChecked(destLen);
            return result;
        }
    }
}
