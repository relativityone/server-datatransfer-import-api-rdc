namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images
{
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	public class ImageLoadFile : IImageLoadFile
	{
		private readonly IImageLoadFileMetadataBuilder _imageLoadFileMetadataBuilder;

		public ImageLoadFile(IImageLoadFileMetadataBuilder imageLoadFileMetadataBuilder)
		{
			_imageLoadFileMetadataBuilder = imageLoadFileMetadataBuilder;
		}

		public void Create(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			_imageLoadFileMetadataBuilder.CreateLoadFileEntries(artifacts, cancellationToken);
		}
	}
}