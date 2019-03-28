using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public class ImageLoadFileRetryableStreamWriter : RetryableStreamWriter
	{
		public ImageLoadFileRetryableStreamWriter(WritersRetryPolicy writersRetryPolicy, StreamFactory streamFactory, ImageLoadFileDestinationPath destinationPath,
			MetadataStatistics processingStatistics, IStatus status, ILog logger) : base(writersRetryPolicy, streamFactory, destinationPath, processingStatistics, status, logger)
		{
		}
	}
}