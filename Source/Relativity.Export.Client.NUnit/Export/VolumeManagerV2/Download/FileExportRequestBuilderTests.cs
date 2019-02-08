using System;
using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Download
{
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
			_filePathProvider = new Mock<IFilePathProvider>();
			_fileNameProvider = new Mock<IFileNameProvider>();
			_validator = new Mock<IExportFileValidator>();
			_fileProcessingStatistics = new Mock<IFileProcessingStatistics>();
			Instance = CreateInstance(_filePathProvider.Object, _fileNameProvider.Object, _validator.Object, _fileProcessingStatistics.Object);
		}

		protected abstract ExportRequestBuilder CreateInstance(IFilePathProvider filePathProvider, IFileNameProvider fileNameProvider, IExportFileValidator exportFileValidator,
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

			_fileNameProvider.Setup(x => x.GetName(artifact)).Returns(fileName);
			_filePathProvider.Setup(x => x.GetPathForFile(fileName, It.IsAny<int>())).Returns(exportPath);
			_validator.Setup(x => x.CanExport(exportPath, It.IsAny<string>())).Returns(false);

			//ACT
			IList<ExportRequest> requests = Instance.Create(artifact, CancellationToken.None);

			//ASSERT
			CollectionAssert.IsEmpty(requests);
			_fileProcessingStatistics.Verify(x => x.UpdateStatisticsForFile(exportPath));
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

			_fileNameProvider.Setup(x => x.GetName(artifact)).Returns(fileName);
			_filePathProvider.Setup(x => x.GetPathForFile(fileName, It.IsAny<int>())).Returns(exportPath);
			_validator.Setup(x => x.CanExport(exportPath, It.IsAny<string>())).Returns(true);

			//ACT
			IList<ExportRequest> requests = Instance.Create(artifact, CancellationToken.None);

			//ASSERT
			Assert.That(requests.Count, Is.EqualTo(1));
			Assert.That(requests[0].DestinationLocation, Is.EqualTo(exportPath));
		}
	}
}