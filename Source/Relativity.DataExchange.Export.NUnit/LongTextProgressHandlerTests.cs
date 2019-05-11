// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextProgressHandlerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
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