using System.Runtime.InteropServices;

namespace ZlibngDotNet;

public unsafe partial class Zlibng : IDisposable
{
	private readonly nint _handle;

	public Zlibng(string libraryPath) : this(NativeLibrary.Load(libraryPath)) { }
	public Zlibng(nint handle)
	{
		Util.ThrowIfNull(handle);
		_handle = handle;

		var compressBoundAddress = NativeLibrary.GetExport(_handle, "zng_compressBound");
		var uncompressAddress    = NativeLibrary.GetExport(_handle, "zng_uncompress");
		var uncompress2Address   = NativeLibrary.GetExport(_handle, "zng_uncompress2");
		var compressAddress      = NativeLibrary.GetExport(_handle, "zng_compress");
		var compress2Address     = NativeLibrary.GetExport(_handle, "zng_compress2");
		var versionAddress       = NativeLibrary.GetExport(_handle, "zlibng_version");

		CompressBoundFunctionPointer = (delegate* unmanaged<nint, nint>)compressBoundAddress;
		UncompressFunctionPointer    = (delegate* unmanaged<void*, ref nint, void*, nint, ZlibngCompressionResult>)uncompressAddress;
		Uncompress2FunctionPointer   = (delegate* unmanaged<void*, ref nint, void*, ref nint, ZlibngCompressionResult>)uncompress2Address;
		CompressFunctionPointer      = (delegate* unmanaged<void*, ref nint, void*, nint, ZlibngCompressionResult>)compressAddress;
		Compress2FunctionPointer     = (delegate* unmanaged<void*, ref nint, void*, nint, ZlibngCompressionLevel, ZlibngCompressionResult>)compress2Address;
		VersionFunctionPointer       = (delegate* unmanaged<byte*>)versionAddress;
	}

	private void ReleaseUnmanagedResources()
	{
		NativeLibrary.Free(_handle);
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~Zlibng()
	{
		ReleaseUnmanagedResources();
	}
}
