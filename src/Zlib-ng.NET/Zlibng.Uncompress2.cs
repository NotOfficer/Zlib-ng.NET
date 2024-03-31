using System.Numerics;

namespace ZlibngDotNet;

public unsafe partial class Zlibng
{
	/// <summary>
	///   Same as uncompress, except that sourceLen is a pointer, where the
	/// length of the source is *sourceLen.  On return, *sourceLen is the number of
	/// source bytes consumed.
	/// </summary>
	public delegate* unmanaged<void*, ref nint, void*, ref nint, ZlibngCompressionResult> Uncompress2FunctionPointer { get; }

	/// <inheritdoc cref="Uncompress2FunctionPointer" />
	public ZlibngCompressionResult Uncompress2<T1, T2>(void* dest, ref T1 destLen, void* source, ref T2 sourceLen)
		where T1 : IBinaryInteger<T1> where T2 : IBinaryInteger<T2>
	{
		return Uncompress2(new Span<byte>(dest, int.CreateChecked(destLen)),
			new ReadOnlySpan<byte>(source, int.CreateChecked(sourceLen)),
			out destLen, out sourceLen);
	}

	/// <inheritdoc cref="Uncompress2FunctionPointer" />
	public ZlibngCompressionResult Uncompress2<T1, T2>(nint dest, ref T1 destLen, nint source, ref T2 sourceLen)
		where T1 : IBinaryInteger<T1> where T2 : IBinaryInteger<T2>
	{
		return Uncompress2(dest.ToPointer(), ref destLen, source.ToPointer(), ref sourceLen);
	}

	/// <inheritdoc cref="Uncompress2FunctionPointer" />
	public ZlibngCompressionResult Uncompress2(byte[] dest, int destOffset, ref int destLen,
		byte[] source, int sourceOffset, ref int sourceLen)
	{
		return Uncompress2(new Span<byte>(dest, destOffset, destLen),
			new ReadOnlySpan<byte>(source, sourceOffset, sourceLen),
			out destLen, out sourceLen);
	}

	/// <inheritdoc cref="Uncompress2FunctionPointer" />
	public ZlibngCompressionResult Uncompress2<T1, T2>(Span<byte> dest, ReadOnlySpan<byte> source,
		out T1 bytesWritten, out T2 bytesConsumed) where T1 : IBinaryInteger<T1> where T2 : IBinaryInteger<T2>
	{
		fixed (byte* destPtr = dest)
		fixed (byte* sourcePtr = source)
		{
			var destLen = (nint)dest.Length;
			var sourceLen = (nint)source.Length;
			var result = Uncompress2FunctionPointer(destPtr, ref destLen, sourcePtr, ref sourceLen);
			bytesWritten = T1.CreateChecked(destLen);
			bytesConsumed = T2.CreateChecked(sourceLen);
			return result;
		}
	}
}
