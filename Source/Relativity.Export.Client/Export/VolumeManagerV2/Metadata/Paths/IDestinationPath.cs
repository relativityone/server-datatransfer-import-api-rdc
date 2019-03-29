namespace Relativity.Export.VolumeManagerV2.Metadata.Paths
{
	using System.Text;

	using kCura.WinEDDS.Exceptions;

	public interface IDestinationPath
	{
		string Path { get; }
		Encoding Encoding { get; }
		FileWriteException.DestinationFile DestinationFileType { get; }
	}
}