// -----------------------------------------------------------------------------------------------------
// <copyright file="FileDownloadProgressHandlerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
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