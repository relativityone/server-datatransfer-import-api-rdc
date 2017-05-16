

using System;
using kCura.OI.FileID;
using kCura.WinEDDS.Api;
using kCura.WinEDDS.Core.Import;
using kCura.WinEDDS.Core.Import.Helpers;
using kCura.WinEDDS.Core.Import.Tasks;
using Moq;
using NUnit.Framework;
using Relativity;
using FileIDData = kCura.OI.FileID.FileIDData;

namespace kCura.WinEDDS.Core.NUnit.Import
{
	public class ImportNativesAnalyzerTests
	{
		private ImportNativesAnalyzer _subjectUnderTests;

		private Mock<ITransferConfig> _transferConfigMock;
		private Mock<IFileHelper> _fileHelper;
		private Mock<IFileInfoProvider> _fileIdDataProvider;

		private const string _FILE_NAME = "FileName.doc";
		private const int _ARTIFACT_ID = 1234;
		private const int _FILE_ID = 1020;

		private ArtifactFieldCollection _artifactFieldCollection;


		[SetUp]
		public void SetUp()
		{
			_transferConfigMock = new Mock<ITransferConfig>();
			_fileHelper = new Mock<IFileHelper>();
			_fileIdDataProvider = new Mock<IFileInfoProvider>();

			_artifactFieldCollection = new ArtifactFieldCollection
			{
				new ArtifactField("File", _ARTIFACT_ID, FieldTypeHelper.FieldType.File, FieldCategory.FileInfo, 1, 1, null, false)
			};
			_artifactFieldCollection[_ARTIFACT_ID].Value = _FILE_NAME;
			_subjectUnderTests = new ImportNativesAnalyzer(_fileIdDataProvider.Object,_transferConfigMock.Object, _fileHelper.Object);
		}

		[Test]
		[TestCase(_FILE_NAME, _FILE_NAME)]
		[TestCase("\\" + _FILE_NAME, ".\\" + _FILE_NAME)]
		[TestCase("\\\\" + _FILE_NAME, "\\\\" + _FILE_NAME)]
		public void ItShouldReturnDefaultFileMetadata(string inputFileName, string expectedFileName)
		{
			// Arrange
			_transferConfigMock.SetupGet(config => config.DisableNativeLocationValidation).Returns(true);
			_artifactFieldCollection[_ARTIFACT_ID].Value = inputFileName;

			//Act
			FileMetadata fileMetadata = _subjectUnderTests.Process(_artifactFieldCollection);


			// Assert
			_fileHelper.Verify(item => item.Exists(expectedFileName), Times.Never);

			Assert.That(fileMetadata.FileName, Is.EqualTo(expectedFileName));
			Assert.That(fileMetadata.FileExists);
		}

		[Test]
		[TestCase(_FILE_NAME, _FILE_NAME)]
		[TestCase("\\" + _FILE_NAME, ".\\" + _FILE_NAME)]
		[TestCase("\\\\" + _FILE_NAME, "\\\\" + _FILE_NAME)]
		public void ItShouldValidateFileNotExists(string inputFileName, string expectedFileName)
		{
			// Arrange
			_transferConfigMock.SetupGet(config => config.DisableNativeLocationValidation).Returns(false);
			_fileHelper.Setup(item => item.Exists(expectedFileName)).Returns(false);
			_artifactFieldCollection[_ARTIFACT_ID].Value = inputFileName;


			// Act
			FileMetadata fileMetadata = _subjectUnderTests.Process(_artifactFieldCollection);

			// Assert
			_fileHelper.Verify(item => item.Exists(expectedFileName), Times.Once);

			Assert.That(fileMetadata.FileName, Is.EqualTo(expectedFileName));
			Assert.That(!fileMetadata.FileExists);
		}

		[Test]
		[TestCase(true, true, true, _FILE_NAME)] // This is a case when file exists and file size is empty and CreateErrorForEmptyNativeFile flag is set
		[TestCase(false, true, true, _FILE_NAME)] // This is a case when file exists and file size is not empty and CreateErrorForEmptyNativeFile flag is set
		[TestCase(true, false, false, "")] // This is a case when file exists and file size is  empty and CreateErrorForEmptyNativeFile flag is not set
		[TestCase(false, false, true, _FILE_NAME)] // This is a case when file exists and file size is not empty and CreateErrorForEmptyNativeFile flag is not set
		public void ItShouldValidateFileContent(bool fileIsEmpty, bool createErrorForEmptyNativeFile, bool expectedResult, string expectedFileName)
		{
			// Arrange
			_transferConfigMock.SetupGet(config => config.DisableNativeLocationValidation).Returns(false);
			_transferConfigMock.SetupGet(config => config.CreateErrorForEmptyNativeFile).Returns(createErrorForEmptyNativeFile);

			_fileHelper.Setup(item => item.Exists(_FILE_NAME)).Returns(true);
			_fileHelper.Setup(item => item.GetFileSize(_FILE_NAME)).Returns(fileIsEmpty ? 0 : 1);

			_artifactFieldCollection[_ARTIFACT_ID].Value = _FILE_NAME;

			// Act
			FileMetadata fileMetadata = _subjectUnderTests.Process(_artifactFieldCollection);

			// Assert
			_fileHelper.Verify(item => item.Exists(_FILE_NAME), Times.Once());

			Assert.That(fileMetadata.FileName, Is.EqualTo(expectedFileName));
			Assert.That(fileMetadata.FileExists, Is.EqualTo(expectedResult));
			Assert.That(fileMetadata.FileIdData, Is.Null);
		}

		[Test]
		public void ItShouldValidateFileIdData()
		{
			// Arrange
			_transferConfigMock.SetupGet(config => config.DisableNativeLocationValidation).Returns(false);
			
			_fileHelper.Setup(item => item.Exists(_FILE_NAME)).Returns(true);
			_fileHelper.Setup(item => item.GetFileSize(_FILE_NAME)).Returns(1);

			_fileIdDataProvider.Setup(item => item.GetFileId(_FILE_NAME)).Returns(new FileIDData(_FILE_ID, ".doc"));

			_artifactFieldCollection[_ARTIFACT_ID].Value = _FILE_NAME;

			// Act
			FileMetadata fileMetadata = _subjectUnderTests.Process(_artifactFieldCollection);

			// Assert
			_fileHelper.Verify(item => item.Exists(_FILE_NAME), Times.Once());

			Assert.That(fileMetadata.FileName, Is.EqualTo(_FILE_NAME));
			Assert.That(fileMetadata.FileExists);
			Assert.That(fileMetadata.FileIdData, Is.Not.Null);
			Assert.That(fileMetadata.FileIdData.FileID, Is.EqualTo(_FILE_ID));
		}
	}
}
