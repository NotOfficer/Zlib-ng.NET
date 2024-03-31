using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ZlibngDotNet.Tests;

public class Tests
{
	private static readonly Zlibng Zlib = new(Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
		@"Libraries\zlib-ng2.dll"));

	[Fact]
	public void CompressAndDecompress()
	{
		var randomString = GetRandomString(8192);
		var randomStringBuffer = Encoding.ASCII.GetBytes(randomString);

		var compressedBufferSize = (int)Zlib.CompressBound(randomStringBuffer.Length);
		Assert.NotEqual(0, compressedBufferSize);

		var compressedBuffer = new byte[compressedBufferSize];
		var compressionResult = Zlib.Compress(compressedBuffer, randomStringBuffer, out int compressedSize);
		Assert.Equal(ZlibngCompressionResult.Ok, compressionResult);
		Assert.NotEqual(0, compressedSize);

		var decompressedBuffer = new byte[randomStringBuffer.Length];
		var decompressionResult = Zlib.Uncompress(decompressedBuffer, compressedBuffer.AsSpan(0, compressedSize), out int decompressedSize);
		Assert.Equal(ZlibngCompressionResult.Ok, decompressionResult);
		Assert.NotEqual(0, decompressedSize);

		Assert.Equal(decompressedSize, randomStringBuffer.Length);
		Assert.True(randomStringBuffer.AsSpan().SequenceEqual(decompressedBuffer));
	}

	private static string GetRandomString(int length)
	{
		const string chars = "0123456789ABCDEF";
		var result = new string('\0', length);
		var resultReadonlySpan = result.AsSpan();
		var resultSpan = MemoryMarshal.CreateSpan(ref Unsafe.AsRef(in resultReadonlySpan.GetPinnableReference()), length);

		for (var i = 0; i < resultSpan.Length; i++)
		{
			resultSpan[i] = chars[Random.Shared.Next(chars.Length)];
		}

		return result;
	}
}
