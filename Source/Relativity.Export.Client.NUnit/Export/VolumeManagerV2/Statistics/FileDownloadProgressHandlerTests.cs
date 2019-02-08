// ----------------------------------------------------------------------------
// <copyright file="FileDownloadProgressHandlerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit.Export.VolumeManagerV2.Statistics
{
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;

    using Moq;

    using Relativity.Logging;

    public class FileDownloadProgressHandlerTests : ProgressHandlerTests
	{
		protected override ProgressHandler CreateInstance(IDownloadProgressManager downloadProgressManager)
		{
			return new FileDownloadProgressHandler(downloadProgressManager, new NullLogger());
		}

		protected override void VerifyFileMarkedAsDownloaded(Mock<IDownloadProgressManager> downloadProgressManager, string id, int lineNumber)
		{
			downloadProgressManager.Verify(x => x.MarkFileAsDownloaded(id, lineNumber), Times.Once);
		}

		protected override void VerifyFileNotMarkedAsDownloaded(Mock<IDownloadProgressManager> downloadProgressManager, string id, int lineNumber)
		{
			downloadProgressManager.Verify(x => x.MarkFileAsDownloaded(id, lineNumber), Times.Never);
		}
	}
}