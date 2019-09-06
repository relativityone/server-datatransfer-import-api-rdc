// -----------------------------------------------------------------------------------------------------
// <copyright file="ImagesRollupManagerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.ImagesRollup;
	using Relativity.Logging;

	public class ImagesRollupManagerTests
	{
		private ImagesRollupManager _subjectUnderTest;

		private Mock<IImagesRollup> _imagesRollup;

		private Mock<IStatus> _status;

		private Mock<ILog> _logger;

		[SetUp]
		public void Init()
		{
			this._imagesRollup = new Mock<IImagesRollup>();
			this._status = new Mock<IStatus>();
			this._logger = new Mock<ILog>();

			this._subjectUnderTest = new ImagesRollupManager(this._imagesRollup.Object, this._status.Object, this._logger.Object);
		}

		[Test]
		public void ItShouldThrowException()
		{
			// Arrange
			Exception exception = new Exception();
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			ObjectExportInfo objectExportInfo = new ObjectExportInfo();

			this._imagesRollup.Setup(rollup => rollup.RollupImages(It.IsAny<ObjectExportInfo>())).Throws(exception);

			Assert.That(objectExportInfo.DocumentError, Is.False);

			// Act
			this._subjectUnderTest.RollupImagesForArtifacts(new[] { objectExportInfo }, cancellationTokenSource.Token);

			// Assert
			this._logger.Verify(log => log.LogError(exception, It.IsAny<string>(), It.IsAny<object[]>()));
			this._status.Verify(status => status.WriteError(It.IsAny<string>()));
			Assert.That(objectExportInfo.DocumentError, Is.True);
		}
	}
}
