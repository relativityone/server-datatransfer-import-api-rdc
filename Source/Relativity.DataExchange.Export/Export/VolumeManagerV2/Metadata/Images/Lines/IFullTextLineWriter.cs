namespace Relativity.Export.VolumeManagerV2.Metadata.Images.Lines
{
	using System.IO;
	using System.Threading;

	using Relativity.Export.VolumeManagerV2.Metadata.Writers;

	public interface IFullTextLineWriter
	{
		void WriteLine(string batesNumber, long pageOffset, IRetryableStreamWriter writer, TextReader textReader, CancellationToken token);
	}
}