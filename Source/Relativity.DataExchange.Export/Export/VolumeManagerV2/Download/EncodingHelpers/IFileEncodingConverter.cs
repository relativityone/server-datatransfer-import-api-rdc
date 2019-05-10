namespace Relativity.Export.VolumeManagerV2.Download.EncodingHelpers
{
	using System.Text;
	using System.Threading;

	public interface IFileEncodingConverter
	{
		void Convert(string filePath, Encoding sourceEncoding, Encoding destinationEncoding, CancellationToken cancellationToken);
	}
}