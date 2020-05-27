namespace Relativity.DataExchange.Export.VolumeManagerV2.Repository
{
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;

	public class ImageRequest : FileRequest<ImageExportInfo>
	{
		public ImageRequest(ImageExportInfo artifact) : base(artifact)
		{
		}
	}
}