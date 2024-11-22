using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ZlibngDotNet.Tests;

public class Tests : IAsyncLifetime
{
	private string _filePath;
	private Zlibng _zlibng;

	public async Task InitializeAsync()
	{
		_filePath = await DownloadAsync();
		_zlibng = new Zlibng(_filePath);
	}

	[Fact]
	public void CompressAndDecompress()
	{
		var randomString = GetRandomString(8192);
		var randomStringBuffer = Encoding.ASCII.GetBytes(randomString);

		var compressedBufferSize = (int)_zlibng.CompressBound(randomStringBuffer.Length);
		Assert.NotEqual(0, compressedBufferSize);

		var compressedBuffer = new byte[compressedBufferSize];
		var compressionResult = _zlibng.Compress(compressedBuffer, randomStringBuffer, out int compressedSize);
		Assert.Equal(ZlibngCompressionResult.Ok, compressionResult);
		Assert.NotEqual(0, compressedSize);

		var decompressedBuffer = new byte[randomStringBuffer.Length];
		var decompressionResult = _zlibng.Uncompress(decompressedBuffer, compressedBuffer.AsSpan(0, compressedSize), out int decompressedSize);
		Assert.Equal(ZlibngCompressionResult.Ok, decompressionResult);
		Assert.NotEqual(0, decompressedSize);

		Assert.Equal(decompressedSize, randomStringBuffer.Length);
		Assert.True(randomStringBuffer.AsSpan().SequenceEqual(decompressedBuffer));
	}

	public Task DisposeAsync()
	{
		_zlibng.Dispose();
		File.Delete(_filePath);
		return Task.CompletedTask;
	}

	private static string GetRandomString(int length)
	{
		const string pool = "0123456789ABCDEF";
		return RandomNumberGenerator.GetString(pool, length);
	}

	public static async Task<string> DownloadAsync()
	{
		if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux())
		{
			throw new PlatformNotSupportedException("this test is not supported on the current platform");
		}

		const string baseUrl = "https://github.com/NotOfficer/Zlib-ng.NET/releases/download/1.0.0/";
		string url;

		if (OperatingSystem.IsWindows())
		{
			url = baseUrl + "zlib-ng2.dll";
		}
		else if (OperatingSystem.IsLinux())
		{
			url = baseUrl + "libz-ng.so";
		}
		else
		{
			throw new UnreachableException();
		}

		using var client = new HttpClient(new SocketsHttpHandler
		{
			UseProxy = false,
			UseCookies = true,
			AutomaticDecompression = DecompressionMethods.All
		});
		using var response = await client.GetAsync(url);
		response.EnsureSuccessStatusCode();
		var filePath = Path.GetTempFileName();
		await using var fs = File.Create(filePath);
		await response.Content.CopyToAsync(fs);
		return filePath;
	}
}
