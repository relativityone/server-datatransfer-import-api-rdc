using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public class ImageLoadFileWriterRetryable : WriterRetryable, IImageLoadFileWriter
	{
		private readonly ImageLoadFileWriter _imageLoadFileWriter;

		public ImageLoadFileWriterRetryable(WritersRetryPolicy writersRetryPolicy, StreamFactory streamFactory, ILog logger, IStatus status, ImageLoadFileDestinationPath destinationPath,
			ImageLoadFileWriter imageLoadFileWriter) : base(
			writersRetryPolicy, streamFactory, logger, status, destinationPath)
		{
			_imageLoadFileWriter = imageLoadFileWriter;
		}

		public void Write(IList<KeyValuePair<string, string>> linesToWrite, ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			Execute((enumerator, streamWriter) =>
			{
				_imageLoadFileWriter.Write(streamWriter, linesToWrite, enumerator);
			}, artifacts, cancellationToken);
		}
	}
}