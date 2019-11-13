// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextProgressHandlerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.Logging;

	public class LongTextProgressHandlerTests : ProgressHandlerTests
	{
		protected override ProgressHandler CreateInstance(IDownloadProgressManager downloadProgressManager)
		{
			return new LongTextProgressHandler(downloadProgressManager, new NullLogger());
		}

		protected override void VerifyFileMarkedAsCompleted(Mock<IDownloadProgressManager> downloadProgressManager, string targetFile, int lineNumber, bool transferResult)
		{
			downloadProgressManager.Verify(x => x.MarkLongTextAsCompleted(targetFile, lineNumber, transferResult), Times.Once);
		}

		protected override void VerifyFileNotMarkedAsCompleted(Mock<IDownloadProgressManager> downloadProgressManager, string targetFile, int lineNumber, bool transferResult)
		{
			downloadProgressManager.Verify(x => x.MarkLongTextAsCompleted(targetFile, lineNumber, transferResult), Times.Never);
		}
	}
}