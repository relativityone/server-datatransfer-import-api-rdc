using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class ImageLoadFile : IImageLoadFile
	{
		private readonly IImageLoadFileMetadataBuilder _imageLoadFileMetadataBuilder;
		private readonly IImageLoadFileWriter _imageLoadFileWriter;

		public ImageLoadFile(IImageLoadFileMetadataBuilder imageLoadFileMetadataBuilder, IImageLoadFileWriter imageLoadFileWriter)
		{
			_imageLoadFileMetadataBuilder = imageLoadFileMetadataBuilder;
			_imageLoadFileWriter = imageLoadFileWriter;
		}

		public void Create(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			IList<KeyValuePair<string, string>> entries = _imageLoadFileMetadataBuilder.CreateLoadFileEntries(artifacts, cancellationToken);

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			_imageLoadFileWriter.Write(entries, artifacts, cancellationToken);
		}
	}
}