// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextPrecedenceBuilderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;

	using ViewFieldInfo = kCura.WinEDDS.ViewFieldInfo;

    [TestFixture]
	public class LongTextPrecedenceBuilderTests
	{
		private LongTextPrecedenceBuilder _instance;

		private ViewFieldInfo _textPrecedence;
		private ExportFile _exportSettings;

		private Mock<IFilePathProvider> _filePathProvider;
		private Mock<IFieldService> _fieldService;
		private Mock<IFileNameProvider> _fileNameProvider;
		private Mock<IExportFileValidator> _exportFileValidator;
		private Mock<IMetadataProcessingStatistics> _metadataProcessingStatistics;

		private string _fileToDelete;

		[SetUp]
		public void SetUp()
		{
			this._textPrecedence = new QueryFieldFactory().GetArtifactIdField();
			this._exportSettings = new ExportFile(1)
				                  {
					                  SelectedTextFields = new[] { this._textPrecedence },
					                  TextFileEncoding = Encoding.Default
				                  };

			this._filePathProvider = new Mock<IFilePathProvider>();
			this._fieldService = new Mock<IFieldService>();
			this._fileNameProvider = new Mock<IFileNameProvider>();
			this._exportFileValidator = new Mock<IExportFileValidator>();
			this._metadataProcessingStatistics = new Mock<IMetadataProcessingStatistics>();

			this._fileToDelete = Path.GetTempFileName();

			this._fieldService.Setup(x => x.GetOrdinalIndex(ServiceConstants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME)).Returns(0);
			this._fieldService.Setup(x => x.GetOrdinalIndex(ServiceConstants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME)).Returns(1);

			LongTextHelper longTextHelper = new LongTextHelper(this._exportSettings, this._fieldService.Object, new LongTextRepository(null, new TestNullLogger()));

			this._instance = new LongTextPrecedenceBuilder(
				this._exportSettings,
				this._filePathProvider.Object,
				this._fieldService.Object,
				longTextHelper,
				this._fileNameProvider.Object,
				new TestNullLogger(),
				this._exportFileValidator.Object,
				this._metadataProcessingStatistics.Object);
		}

		[TearDown]
		public void TearDown()
		{
			if (!string.IsNullOrWhiteSpace(this._fileToDelete) && File.Exists(this._fileToDelete))
			{
				File.Delete(this._fileToDelete);
			}
		}

		[Test]
		public void ItShouldHandleLongTextToLoadFile()
		{
			const string notTooLongText = "not too long text";

			this._exportSettings.ExportFullTextAsFile = false;

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[]
				{
					this._textPrecedence.FieldArtifactId,
					notTooLongText
				}
			};

			// ACT
			IList<LongText> actualResult = this._instance.CreateLongText(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(actualResult.Count, Is.EqualTo(1));
			Assert.That(actualResult[0].ExportRequest, Is.Null);
			Assert.That(actualResult[0].RequireDeletion, Is.False);
			Assert.That(actualResult[0].GetLongText().ReadToEnd(), Is.EqualTo(notTooLongText));
		}

		[Test]
		public void ItShouldHandleLongTextToFile()
		{
			const string notTooLongText = "not too long text";

			this._exportSettings.ExportFullTextAsFile = true;

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[]
				{
					this._textPrecedence.FieldArtifactId,
					notTooLongText
				}
			};

			this._exportFileValidator.Setup(x => x.CanExport(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			this._filePathProvider.Setup(x => x.GetPathForFile(It.IsAny<string>(), It.IsAny<int>())).Returns(this._fileToDelete);

			// ACT
			IList<LongText> actualResult = this._instance.CreateLongText(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(actualResult.Count, Is.EqualTo(1));
			Assert.That(actualResult[0].ExportRequest, Is.Null);
			Assert.That(actualResult[0].RequireDeletion, Is.False);
			using (var reader = actualResult[0].GetLongText())
			{
				Assert.That(reader.ReadToEnd(), Is.EqualTo(notTooLongText));
			}
		}

		[Test]
		public void ItShouldHandleTooLongTextToLoadFile()
		{
			const string tooLongText = ServiceConstants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN;

			this._exportSettings.ExportFullTextAsFile = false;

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[]
				{
					this._textPrecedence.FieldArtifactId,
					tooLongText
				}
			};

			// ACT
			IList<LongText> actualResult = this._instance.CreateLongText(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(actualResult.Count, Is.EqualTo(1));
			Assert.That(actualResult[0].ExportRequest, Is.Not.Null);
			Assert.That(actualResult[0].RequireDeletion, Is.True);
		}

		[Test]
		public void ItShouldHandleTooLongTextToFile()
		{
			const string tooLongText = ServiceConstants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN;

			this._exportSettings.ExportFullTextAsFile = true;

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[]
				{
					this._textPrecedence.FieldArtifactId,
					tooLongText
				}
			};

			this._exportFileValidator.Setup(x => x.CanExport(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			this._filePathProvider.Setup(x => x.GetPathForFile(It.IsAny<string>(), It.IsAny<int>())).Returns(this._fileToDelete);

			// ACT
			IList<LongText> actualResult = this._instance.CreateLongText(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(actualResult.Count, Is.EqualTo(1));
			Assert.That(actualResult[0].ExportRequest, Is.Not.Null);
			Assert.That(actualResult[0].RequireDeletion, Is.False);
		}

		[Test]
		public void ItShouldHandleTooLongTextToFile_FileAlreadyExists()
		{
			const string tooLongText = ServiceConstants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN;

			this._exportSettings.ExportFullTextAsFile = true;

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[]
				{
					this._textPrecedence.FieldArtifactId,
					tooLongText
				}
			};

			this._exportFileValidator.Setup(x => x.CanExport(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
			this._filePathProvider.Setup(x => x.GetPathForFile(It.IsAny<string>(), It.IsAny<int>())).Returns(this._fileToDelete);

			// ACT
			IList<LongText> actualResult = this._instance.CreateLongText(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(actualResult.Count, Is.EqualTo(1));
			Assert.That(actualResult[0].ExportRequest, Is.Null);
			Assert.That(actualResult[0].RequireDeletion, Is.False);
		}
	}
}