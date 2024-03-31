namespace ZlibngDotNet;

public enum ZlibngCompressionLevel
{
	None = 0,
	BestSpeed = 1,
	Optimal = 5,
	Best = 9,
	Default = -1,
}

public enum ZlibngCompressionResult
{
	Ok = 0,
	StreamEnd = 1,
	NeedDict = 2,
	ErrNo = -1,
	StreamError = -2,
	DataError = -3,
	MemError = -4,
	BufError = -5,
	VersionError = -6
}
