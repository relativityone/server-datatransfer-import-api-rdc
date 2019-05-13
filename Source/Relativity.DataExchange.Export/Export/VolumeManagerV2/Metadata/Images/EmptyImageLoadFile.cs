namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images
{
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	public class EmptyImageLoadFile : IImageLoadFile
	{
		public void Create(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
		}
	}
}