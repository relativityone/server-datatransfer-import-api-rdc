// -----------------------------------------------------------------------------------------------------
// <copyright file="ExportRequestBuilderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;

	[TestFixture]
	public abstract class ExportRequestBuilderTests
	{
		private Mock<IFilePathProvider> _filePathProvider;
		private Mock<IFileNameProvider> _fileNameProvider;
		private Mock<IExportFileValidator> _validator;
		private Mock<IFileProcessingStatistics> _fileProcessingStatistics;

		protected ExportRequestBuilder Instance { get; private set; }

		[SetUp]
		public void SetUp()
		{
			this._filePathProvider = new Mock<IFilePathProvider>();
			this._fileNameProvider = new Mock<IFileNameProvider>();
			this._validator = new Mock<IExportFileValidator>();
			this._fileProcessingStatistics = new Mock<IFileProcessingStatistics>();
			this.Instance = this.CreateInstance(this._filePathProvider.Object, this._fileNameProvider.Object, this._validator.Object, this._fileProcessingStatistics.Object);
		}

		protected abstract ExportRequestBuilder CreateInstance(
			IFilePathProvider filePathProvider,
			IFileNameProvider fileNameProvider,
			IExportFileValidator exportFileValidator,
			IFileProcessingStatistics fileProcessingStatistics);

		[Test]
		public void ItShouldUpdateStatisticsAndSkipImageWhenCannotExport()
		{
			const string fileName = "file_name";
			const string exportPath = "export_path";

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				NativeFileGuid = Guid.NewGuid().ToString(),
				FileID = 1,
				NativeSourceLocation = "source_location"
			};

			this._fileNameProvider.Setup(x => x.GetName(artifact)).Returns(fileName);
			this._filePathProvider.Setup(x => x.GetPathForFile(fileName, It.IsAny<int>())).Returns(exportPath);
			this._validator.Setup(x => x.CanExport(exportPath, It.IsAny<string>())).Returns(false);

			// ACT
			IList<ExportRequest> requests = this.Instance.Create(artifact, CancellationToken.None);

			// ASSERT
			CollectionAssert.IsEmpty(requests);
			this._fileProcessingStatistics.Verify(x => x.UpdateStatisticsForFile(exportPath));
		}

		[Test]
		public void ItShouldCreateRequests()
		{
			const string fileName = "file_name";
			const string exportPath = "export_path";

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				NativeFileGuid = Guid.NewGuid().ToString(),
				FileID = 1,
				NativeSourceLocation = "source_location"
			};

			this._fileNameProvider.Setup(x => x.GetName(artifact)).Returns(fileName);
			this._filePathProvider.Setup(x => x.GetPathForFile(fileName, It.IsAny<int>())).Returns(exportPath);
			this._validator.Setup(x => x.CanExport(exportPath, It.IsAny<string>())).Returns(true);

			// ACT
			IList<ExportRequest> requests = this.Instance.Create(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(requests.Count, Is.EqualTo(1));
			Assert.That(requests[0].DestinationLocation, Is.EqualTo(exportPath));
		}
	}
}