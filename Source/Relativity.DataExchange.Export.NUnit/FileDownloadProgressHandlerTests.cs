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

		protected override void VerifyFileMarkedAsCompleted(Mock<IDownloadProgressManager> downloadProgressManager, string targetFile, int lineNumber, bool transferResult)
		{
			downloadProgressManager.Verify(x => x.MarkFileAsCompleted(targetFile, lineNumber, transferResult), Times.Once);
		}

		protected override void VerifyFileNotMarkedAsCompleted(Mock<IDownloadProgressManager> downloadProgressManager, string targetFile, int lineNumber, bool transferResult)
		{
			downloadProgressManager.Verify(x => x.MarkFileAsCompleted(targetFile, lineNumber, transferResult), Times.Never);
		}
	}
}