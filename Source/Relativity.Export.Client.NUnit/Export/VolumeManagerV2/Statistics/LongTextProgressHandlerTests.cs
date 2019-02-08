using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using Moq;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Statistics
{
	public class LongTextProgressHandlerTests : ProgressHandlerTests
	{
		protected override ProgressHandler CreateInstance(IDownloadProgressManager downloadProgressManager)
		{
			return new LongTextProgressHandler(downloadProgressManager, new NullLogger());
		}

		protected override void VerifyFileMarkedAsDownloaded(Mock<IDownloadProgressManager> downloadProgressManager, string id, int lineNumber)
		{
			downloadProgressManager.Verify(x => x.MarkLongTextAsDownloaded(id, lineNumber), Times.Once);
		}

		protected override void VerifyFileNotMarkedAsDownloaded(Mock<IDownloadProgressManager> downloadProgressManager, string id, int lineNumber)
		{
			downloadProgressManager.Verify(x => x.MarkLongTextAsDownloaded(id, lineNumber), Times.Never);
		}
	}
}