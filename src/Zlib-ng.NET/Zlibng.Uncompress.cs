using System.Numerics;

namespace ZlibngDotNet;

public unsafe partial class Zlibng
{
    /// <summary>
    ///   Decompresses the source buffer into the destination buffer.  sourceLen is
    /// the byte length of the source buffer.  Upon entry, destLen is the total size
    /// of the destination buffer, which must be large enough to hold the entire
    /// uncompressed data.  (The size of the uncompressed data must have been saved
    /// previously by the compressor and transmitted to the decompressor by some
    /// mechanism outside the scope of this compression library.) Upon exit, destLen
    /// is the actual size of the uncompressed data.
    /// 
    ///   uncompress returns Z_OK if success, Z_MEM_ERROR if there was not
    /// enough memory, Z_BUF_ERROR if there was not enough room in the output
    /// buffer, or Z_DATA_ERROR if the input data was corrupted or incomplete.  In
    /// the case where there is not enough room, uncompress() will fill the output
    /// buffer with the uncompressed data up to that point.
    /// </summary>
    public delegate* unmanaged<void*, ref nint, void*, nint, ZlibngCompressionResult> UncompressFunctionPointer { get; }

    /// <inheritdoc cref="UncompressFunctionPointer" />
    public int Uncompress<T1, T2>(void* dest, T1 destLen, void* source, T2 sourceLen)
        where T1 : IBinaryInteger<T1> where T2 : IBinaryInteger<T2>
    {
        return Uncompress(new Span<byte>(dest, int.CreateChecked(destLen)),
            new ReadOnlySpan<byte>(source, int.CreateChecked(sourceLen)));
    }

    /// <inheritdoc cref="UncompressFunctionPointer" />
    public int Uncompress<T1, T2>(nint dest, T1 destLen, nint source, T2 sourceLen)
        where T1 : IBinaryInteger<T1> where T2 : IBinaryInteger<T2>
    {
        return Uncompress(dest.ToPointer(), destLen, source.ToPointer(), sourceLen);
    }

    /// <inheritdoc cref="UncompressFunctionPointer" />
    public int Uncompress(byte[] dest, int destOffset, int destLen, byte[] source,
        int sourceOffset, int sourceLen)
    {
        return Uncompress(new Span<byte>(dest, destOffset, destLen),
            new ReadOnlySpan<byte>(source, sourceOffset, sourceLen));
    }

    /// <inheritdoc cref="UncompressFunctionPointer" />
    public int Uncompress(Span<byte> dest, ReadOnlySpan<byte> source)
    {
        fixed (byte* destPtr = dest)
        fixed (byte* sourcePtr = source)
        {
            IntPtr destLen = (nint)dest.Length;
            IntPtr sourceLen = (nint)source.Length;
            ZlibngCompressionResult result = UncompressFunctionPointer(destPtr, ref destLen, sourcePtr, sourceLen);
            return Util.GetReturnValue(result, destLen);
        }
    }

    /// <inheritdoc cref="UncompressFunctionPointer" />
    public ZlibngCompressionResult Uncompress<T1, T2>(void* dest, ref T1 destLen, void* source, T2 sourceLen)
        where T1 : IBinaryInteger<T1> where T2 : IBinaryInteger<T2>
    {
        return Uncompress(new Span<byte>(dest, int.CreateChecked(destLen)),
            new ReadOnlySpan<byte>(source, int.CreateChecked(sourceLen)),
            out destLen);
    }

    /// <inheritdoc cref="UncompressFunctionPointer" />
    public ZlibngCompressionResult Uncompress<T1, T2>(nint dest, ref T1 destLen, nint source, T2 sourceLen)
        where T1 : IBinaryInteger<T1> where T2 : IBinaryInteger<T2>
    {
        return Uncompress(dest.ToPointer(), ref destLen, source.ToPointer(), sourceLen);
    }

    /// <inheritdoc cref="UncompressFunctionPointer" />
    public ZlibngCompressionResult Uncompress(byte[] dest, int destOffset, ref int destLen,
        byte[] source, int sourceOffset, int sourceLen)
    {
        return Uncompress(new Span<byte>(dest, destOffset, destLen),
            new ReadOnlySpan<byte>(source, sourceOffset, sourceLen),
            out destLen);
    }

    /// <inheritdoc cref="UncompressFunctionPointer" />
    public ZlibngCompressionResult Uncompress<T>(Span<byte> dest, ReadOnlySpan<byte> source, out T bytesWritten)
        where T : IBinaryInteger<T>
    {
        fixed (byte* destPtr = dest)
        fixed (byte* sourcePtr = source)
        {
            IntPtr destLen = (nint)dest.Length;
            IntPtr sourceLen = (nint)source.Length;
            ZlibngCompressionResult result = UncompressFunctionPointer(destPtr, ref destLen, sourcePtr, sourceLen);
            bytesWritten = T.CreateChecked(destLen);
            return result;
        }
    }
}
