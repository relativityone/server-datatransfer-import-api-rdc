using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using Moq;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Statistics
{
	public class ImageFilesProgressHandlerTests : ProgressHandlerTests
	{
		protected override ProgressHandler CreateInstance(IDownloadProgressManager downloadProgressManager)
		{
			return new ImageFilesProgressHandler(downloadProgressManager, new NullLogger());
		}

		protected override void VerifyFileMarkedAsDownloaded(Mock<IDownloadProgressManager> downloadProgressManager, string id)
		{
			downloadProgressManager.Verify(x => x.MarkImageAsDownloaded(id), Times.Once);
		}

		protected override void VerifyFileNotMarkedAsDownloaded(Mock<IDownloadProgressManager> downloadProgressManager, string id)
		{
			downloadProgressManager.Verify(x => x.MarkImageAsDownloaded(id), Times.Never);
		}
	}
}