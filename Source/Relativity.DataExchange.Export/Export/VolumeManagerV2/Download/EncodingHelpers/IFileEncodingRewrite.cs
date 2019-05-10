namespace Relativity.Export.VolumeManagerV2.Download.EncodingHelpers
{
	using System.Text;
	using System.Threading;

	public interface IFileEncodingRewrite
	{
		void RewriteFile(string filePath, string tmpFilePath, Encoding sourceEncoding, Encoding destinationEncoding, CancellationToken cancellationToken);
	}
}