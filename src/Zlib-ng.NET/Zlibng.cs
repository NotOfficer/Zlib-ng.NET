using System.Runtime.InteropServices;

namespace ZlibngDotNet;

/// <summary>
/// Wrapper class for the native zlib-ng library
/// </summary>
public unsafe partial class Zlibng : IDisposable
{
    /// <summary>
    /// Library handle for the current zlib-ng instance
    /// </summary>
    public nint Handle { get; }

    /// <summary>
    /// Initializes via a native zlib-ng library path
    /// </summary>
    /// <returns/>
    /// <param name="libraryPath">The path of the native zlib-ng library to be loaded</param>
    /// <inheritdoc cref="NativeLibrary.Load(string)"/>
    public Zlibng(string libraryPath) : this(NativeLibrary.Load(libraryPath)) { }

    /// <summary>
    /// Initializes via a native zlib-ng library handle
    /// </summary>
    /// <param name="handle">The handle of the native zlib-ng library to be loaded</param>
    /// <exception cref="ArgumentNullException">If the handle is invalid</exception>
    /// <exception cref="EntryPointNotFoundException">If the handle/library is invalid or not supported</exception>
    public Zlibng(nint handle)
    {
        Util.ThrowIfNull(handle);
        Handle = handle;

        var compressBoundAddress = NativeLibrary.GetExport(Handle, "zng_compressBound");
        var uncompressAddress    = NativeLibrary.GetExport(Handle, "zng_uncompress");
        var uncompress2Address   = NativeLibrary.GetExport(Handle, "zng_uncompress2");
        var compressAddress      = NativeLibrary.GetExport(Handle, "zng_compress");
        var compress2Address     = NativeLibrary.GetExport(Handle, "zng_compress2");
        var versionAddress       = NativeLibrary.GetExport(Handle, "zlibng_version");

        CompressBoundFunctionPointer = (delegate* unmanaged<nint, nint>)compressBoundAddress;
        UncompressFunctionPointer    = (delegate* unmanaged<void*, ref nint, void*, nint, ZlibngCompressionResult>)uncompressAddress;
        Uncompress2FunctionPointer   = (delegate* unmanaged<void*, ref nint, void*, ref nint, ZlibngCompressionResult>)uncompress2Address;
        CompressFunctionPointer      = (delegate* unmanaged<void*, ref nint, void*, nint, ZlibngCompressionResult>)compressAddress;
        Compress2FunctionPointer     = (delegate* unmanaged<void*, ref nint, void*, nint, ZlibngCompressionLevel, ZlibngCompressionResult>)compress2Address;
        VersionFunctionPointer       = (delegate* unmanaged<byte*>)versionAddress;
    }

    private void ReleaseUnmanagedResources()
    {
        if (Handle != nint.Zero)
            NativeLibrary.Free(Handle);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    ~Zlibng()
    {
        ReleaseUnmanagedResources();
    }
}
