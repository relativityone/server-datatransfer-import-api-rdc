using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
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