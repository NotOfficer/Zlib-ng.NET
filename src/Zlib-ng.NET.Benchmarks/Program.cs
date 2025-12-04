using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

using ZlibngDotNet;

BenchmarkRunner.Run<ZlibngBenchmarks>();

[MemoryDiagnoser(false)]
[SimpleJob(RuntimeMoniker.Net90, baseline: true)]
[SimpleJob(RuntimeMoniker.Net80)]
public class ZlibngBenchmarks
{
	private Zlibng _zlibng = null!;
	private byte[] _destBuffer = null!;
	private byte[] _srcBuffer = null!;

	[GlobalSetup]
	public void Setup()
	{
		_zlibng = new Zlibng(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Libraries\zlib-ng2.dll"));
		Console.WriteLine($"Zlib-ng version: {_zlibng.GetVersionString()}");

		const int count = 4_000_00;
		var randomBuffer = new byte[count * sizeof(long)]; // 3.2 MB
		var random = new Random(187);
		for (var i = 0; i < count; i += sizeof(long))
		{
			Unsafe.WriteUnaligned(ref randomBuffer[i], random.NextInt64(0, 69));
		}
		var compressionBound = (int)_zlibng.CompressBound(randomBuffer.Length);
		var tmpBuffer = ArrayPool<byte>.Shared.Rent(compressionBound);
		var compressedSize = (uint)_zlibng.Compress2(tmpBuffer.AsSpan(0, compressionBound), randomBuffer, ZlibngCompressionLevel.Best);
		var compressionRatio = (double)randomBuffer.Length / compressedSize;
		Console.WriteLine("Compression ratio: {0}", compressionRatio.ToString("0.000", CultureInfo.InvariantCulture));
		_destBuffer = randomBuffer;
		_srcBuffer = new byte[compressedSize];
		Unsafe.CopyBlockUnaligned(ref _srcBuffer[0], ref tmpBuffer[0], compressedSize);
		ArrayPool<byte>.Shared.Return(tmpBuffer);
	}

	[Benchmark(Description = "Uncompress_Zlib-ng")]
	public int Uncompress_Zlibng()
	{
		var result = _zlibng.Uncompress2(_destBuffer, _srcBuffer, out int bytesWritten, out int bytesConsumed);
		return bytesWritten;
	}

	[Benchmark]
	public int Uncompress_SystemIOCompression_Stream() => ManagedZlib.Uncompress_SystemIOCompression_Stream(_destBuffer, _srcBuffer);

	[Benchmark]
	public int Uncompress_SharpZipLib_Inflater() => ManagedZlib.Uncompress_SharpZipLib_Inflater(_destBuffer, _srcBuffer);

	[Benchmark]
	public int Uncompress_SharpZipLib_Stream() => ManagedZlib.Uncompress_SharpZipLib_Stream(_destBuffer, _srcBuffer);

	[Benchmark]
	public int Uncompress_DotnetZip_ZlibCodec() => ManagedZlib.Uncompress_DotnetZip_ZlibCodec(_destBuffer, _srcBuffer);

	[Benchmark]
	public int Uncompress_DotnetZip_Stream() => ManagedZlib.Uncompress_DotnetZip_Stream(_destBuffer, _srcBuffer);
}

public static class ManagedZlib
{
	public static int Uncompress_SystemIOCompression_Stream(byte[] dest, byte[] src)
	{
		using var srcMs = new MemoryStream(src, false);
		using var zlibStream = new System.IO.Compression.ZLibStream(srcMs, System.IO.Compression.CompressionMode.Decompress);
		using var destMs = new MemoryStream(dest, true);
		zlibStream.CopyTo(destMs);
		return dest.Length;
	}

	public static int Uncompress_SharpZipLib_Inflater(byte[] dest, byte[] src)
	{
		var inflater = new ICSharpCode.SharpZipLib.Zip.Compression.Inflater();
		inflater.SetInput(src);
		var result = inflater.Inflate(dest);
		return result;
	}

	public static int Uncompress_SharpZipLib_Stream(byte[] dest, byte[] src)
	{
		using var srcMs = new MemoryStream(src, false);
		using var inflaterStream = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(srcMs);
		using var destMs = new MemoryStream(dest, true);
		inflaterStream.CopyTo(destMs);
		return dest.Length;
	}

	public static int Uncompress_DotnetZip_ZlibCodec(byte[] dest, byte[] src)
	{
		var codec = new Ionic.Zlib.ZlibCodec(Ionic.Zlib.CompressionMode.Decompress)
		{
			InputBuffer = src,
			OutputBuffer = dest,
			AvailableBytesIn = src.Length,
			AvailableBytesOut = dest.Length
		};
		var result = codec.Inflate(Ionic.Zlib.FlushType.Full);
		return result;
	}

	public static int Uncompress_DotnetZip_Stream(byte[] dest, byte[] src)
	{
		using var srcMs = new MemoryStream(src, false);
		using var zlibStream = new Ionic.Zlib.ZlibStream(srcMs, Ionic.Zlib.CompressionMode.Decompress);
		var destMs = new MemoryStream(dest, true);
		zlibStream.CopyTo(destMs);
		return dest.Length;
	}
}
