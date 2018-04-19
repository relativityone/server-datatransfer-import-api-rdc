using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Download
{
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
			_filePathProvider = new Mock<IFilePathProvider>();
			_validator = new Mock<IExportFileValidator>();
			_fileProcessingStatistics = new Mock<IFileProcessingStatistics>();

			_instance = new ImageExportRequestBuilder(_filePathProvider.Object, _validator.Object, new NullLogger(), _fileProcessingStatistics.Object);
		}

		[Test]
		public void ItShouldSkipImageWithEmptyGuid()
		{
			ImageExportInfo image = new ImageExportInfo
			{
				FileGuid = string.Empty
			};
			ArrayList images = new ArrayList {image};

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = images
			};

			//ACT
			IList<ExportRequest> requests = _instance.Create(artifact, CancellationToken.None);

			//ASSERT
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
			ArrayList images = new ArrayList {image};

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = images
			};

			_filePathProvider.Setup(x => x.GetPathForFile(image.FileName)).Returns(exportPath);
			_validator.Setup(x => x.CanExport(exportPath, It.IsAny<string>())).Returns(false);

			//ACT
			IList<ExportRequest> requests = _instance.Create(artifact, CancellationToken.None);

			//ASSERT
			CollectionAssert.IsEmpty(requests);
			_fileProcessingStatistics.Verify(x => x.UpdateStatisticsForFile(exportPath));
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
			ArrayList images = new ArrayList {image1, image2};

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = images
			};

			_filePathProvider.Setup(x => x.GetPathForFile(It.IsAny<string>())).Returns((string fileName) => $"{fileName}.img");
			_validator.Setup(x => x.CanExport(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

			//ACT
			IList<ExportRequest> requests = _instance.Create(artifact, CancellationToken.None);

			//ASSERT
			Assert.That(requests.Count, Is.EqualTo(2));
			Assert.That(requests[0].DestinationLocation, Is.EqualTo($"{image1.FileName}.img"));
			Assert.That(requests[1].DestinationLocation, Is.EqualTo($"{image2.FileName}.img"));
		}
	}
}