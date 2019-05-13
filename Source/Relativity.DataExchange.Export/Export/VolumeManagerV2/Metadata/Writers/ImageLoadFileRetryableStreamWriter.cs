namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers
{
	using kCura.WinEDDS;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Paths;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.Logging;

	public class ImageLoadFileRetryableStreamWriter : RetryableStreamWriter
	{
		public ImageLoadFileRetryableStreamWriter(WritersRetryPolicy writersRetryPolicy, StreamFactory streamFactory, ImageLoadFileDestinationPath destinationPath,
			MetadataStatistics processingStatistics, IStatus status, ILog logger) : base(writersRetryPolicy, streamFactory, destinationPath, processingStatistics, status, logger)
		{
		}
	}
}