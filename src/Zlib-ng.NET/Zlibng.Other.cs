﻿using System.Numerics;

namespace ZlibngDotNet;

public unsafe partial class Zlibng
{
	/// <summary>
	///   compressBound() returns an upper bound on the compressed size after
	/// compress() or compress2() on sourceLen bytes.  It would be used before a
	/// compress() or compress2() call to allocate the destination buffer.
	/// </summary>
	public delegate* unmanaged<nint, nint> CompressBoundFunctionPointer { get; }

	/// <summary>
	/// Returns the current build version
	/// </summary>
	public delegate* unmanaged<byte*> VersionFunctionPointer { get; }

	/// <inheritdoc cref="VersionFunctionPointer" />
	public string GetVersionString => Util.GetStringFromPtr(VersionFunctionPointer());

	/// <inheritdoc cref="CompressBoundFunctionPointer" />
	public nint CompressBound<T>(T sourceLen) where T : IBinaryInteger<T>
		=> CompressBoundFunctionPointer(nint.CreateChecked(sourceLen));

	public static bool IsError(int result, out ZlibngCompressionResult error)
	{
		var compressionResult = (ZlibngCompressionResult)result;
		if (IsError(compressionResult))
		{
			error = compressionResult;
			return true;
		}
		error = ZlibngCompressionResult.Ok;
		return false;
	}

	// not using this until zstream is implemeted
	//public static bool IsError(ZlibngCompressionResult result) => result <= ZlibngCompressionResult.NeedDict;
	public static bool IsError(ZlibngCompressionResult result) => result <= ZlibngCompressionResult.Ok;
}
