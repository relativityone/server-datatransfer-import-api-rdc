namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images
{
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	public interface IImageLoadFileMetadataBuilder
	{
		void CreateLoadFileEntries(ObjectExportInfo[] artifacts, CancellationToken cancellationToken);
	}
}