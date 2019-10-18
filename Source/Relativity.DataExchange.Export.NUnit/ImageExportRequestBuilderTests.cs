// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageExportRequestBuilderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.Logging;

	[TestFixture]
	public class ImageExportRequestBuilderTests
	{
		private ImageExportRequestBuilder _instance;

		private Mock<IFilePathProvider> _filePathProvider;
		private Mock<IExportFileValidator> _validator;
		private Mock<IFileProcessingStatistics> _fileProcessingStatistics;

		[SetUp]
		public void SetUp()
		{
			this._filePathProvider = new Mock<IFilePathProvider>();
			this._validator = new Mock<IExportFileValidator>();
			this._fileProcessingStatistics = new Mock<IFileProcessingStatistics>();

			this._instance = new ImageExportRequestBuilder(this._filePathProvider.Object, this._validator.Object, new NullLogger(), this._fileProcessingStatistics.Object);
		}

		[Test]
		public void ItShouldSkipImageWithEmptyGuid()
		{
			ImageExportInfo image = new ImageExportInfo
			{
				FileGuid = string.Empty
			};
			List<ImageExportInfo> images = new List<ImageExportInfo> { image };

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = images
			};

			// ACT
			IList<ExportRequest> requests = this._instance.Create(artifact, CancellationToken.None);

			// ASSERT
			CollectionAssert.IsEmpty(requests);
		}

		[Test]
		public void ItShouldUpdateStatisticsAndSkipImageWhenCannotExport()
		{
			const string exportPath = "export_path";

			ImageExportInfo image = new ImageExportInfo
			{
				FileGuid = Guid.NewGuid().ToString(),
				FileName = "image_file_name"
			};

			List<ImageExportInfo> images = new List<ImageExportInfo> { image };

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = images
			};

			this._filePathProvider.Setup(x => x.GetPathForFile(image.FileName, It.IsAny<int>())).Returns(exportPath);
			this._validator.Setup(x => x.CanExport(exportPath, It.IsAny<string>())).Returns(false);

			// ACT
			IList<ExportRequest> requests = this._instance.Create(artifact, CancellationToken.None);

			// ASSERT
			CollectionAssert.IsEmpty(requests);
			this._fileProcessingStatistics.Verify(x => x.UpdateStatisticsForFile(exportPath));
		}

		[Test]
		public void ItShouldCreateRequests()
		{
			ImageExportInfo image1 = new ImageExportInfo
			{
				FileGuid = Guid.NewGuid().ToString(),
				FileName = "image_file_name_1"
			};
			ImageExportInfo image2 = new ImageExportInfo
			{
				FileGuid = Guid.NewGuid().ToString(),
				FileName = "image_file_name_2"
			};

			List<ImageExportInfo> images = new List<ImageExportInfo> { image1, image2 };

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = images
			};

			this._filePathProvider.Setup(x => x.GetPathForFile(It.IsAny<string>(), It.IsAny<int>())).Returns((string fileName, int artifactId) => $"{fileName}.img");
			this._validator.Setup(x => x.CanExport(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

			// ACT
			IList<ExportRequest> requests = this._instance.Create(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(requests.Count, Is.EqualTo(2));
			Assert.That(requests[0].DestinationLocation, Is.EqualTo($"{image1.FileName}.img"));
			Assert.That(requests[1].DestinationLocation, Is.EqualTo($"{image2.FileName}.img"));
		}
	}
}