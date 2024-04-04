namespace ZlibngDotNet;

/// <summary>
/// Compression levels
/// </summary>
public enum ZlibngCompressionLevel
{
	/// <summary/>
	None = 0,
	/// <summary/>
	BestSpeed = 1,
	/// <summary/>
	Optimal = 5,
	/// <summary/>
	Best = 9,
	/// <summary/>
	Default = -1
}

/// <summary>
/// Return codes for the compression/decompression functions. Negative values<br/>
/// are errors, positive values are used for special but normal events.
/// </summary>
public enum ZlibngCompressionResult
{
	/// <summary/>
	Ok = 0,
	/// <summary/>
	StreamEnd = 1,
	/// <summary/>
	NeedDict = 2,
	/// <summary/>
	ErrNo = -1,
	/// <summary/>
	StreamError = -2,
	/// <summary/>
	DataError = -3,
	/// <summary/>
	MemError = -4,
	/// <summary/>
	BufError = -5,
	/// <summary/>
	VersionError = -6
}
