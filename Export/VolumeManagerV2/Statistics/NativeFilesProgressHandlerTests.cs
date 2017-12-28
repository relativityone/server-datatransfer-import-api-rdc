using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using Moq;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Statistics
{
	public class NativeFilesProgressHandlerTests : ProgressHandlerTests
	{
		protected override ProgressHandler CreateInstance(IDownloadProgressManager downloadProgressManager)
		{
			return new NativeFilesProgressHandler(downloadProgressManager, new NullLogger());
		}

		protected override void VerifyFileMarkedAsDownloaded(Mock<IDownloadProgressManager> downloadProgressManager, string id)
		{
			downloadProgressManager.Verify(x => x.MarkNativeAsDownloaded(id), Times.Once);
		}

		protected override void VerifyFileNotMarkedAsDownloaded(Mock<IDownloadProgressManager> downloadProgressManager, string id)
		{
			downloadProgressManager.Verify(x => x.MarkNativeAsDownloaded(id), Times.Never);
		}
	}
}