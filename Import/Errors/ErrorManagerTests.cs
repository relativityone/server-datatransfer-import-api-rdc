using System;
using System.IO;
using kCura.WinEDDS.Api;
using kCura.WinEDDS.Core.Import;
using kCura.WinEDDS.Core.Import.Errors;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Import.Errors
{
	[TestFixture]
	public class ErrorManagerTests
	{
		private const string _EXPORT_LOCATION = "export\\location";
		private const string _FILE_SUFFIX = "123456";

		private ErrorManager _instance;

		private Mock<IArtifactReader> _artifactReader;
		private Mock<IClientErrors> _clientErrors;
		private Mock<IAllErrors> _allErrors;
		private Mock<IFileHelper> _fileHelper;

		[SetUp]
		public void SetUp()
		{
			_artifactReader = new Mock<IArtifactReader>();
			_clientErrors = new Mock<IClientErrors>();
			_allErrors = new Mock<IAllErrors>();
			_fileHelper = new Mock<IFileHelper>();

			var dateTimeHelper = new Mock<IDateTimeHelper>();
			dateTimeHelper.Setup(x => x.Now()).Returns(DateTime.MinValue + TimeSpan.FromTicks(int.Parse(_FILE_SUFFIX)));

			var importMetadata = new Mock<IImportMetadata>();
			importMetadata.Setup(x => x.ArtifactReader).Returns(_artifactReader.Object);

			_instance = new ErrorManager(_clientErrors.Object, _allErrors.Object, importMetadata.Object, _fileHelper.Object, new ErrorFileNames(dateTimeHelper.Object));
		}

		[Test]
		public void ItShouldCreateEmptyFileWhenExportingErrorReportWithoutErrors()
		{
			_allErrors.Setup(x => x.HasErrors()).Returns(false);
			_fileHelper.Setup(x => x.Create(_EXPORT_LOCATION)).Returns(new FileStream(Path.Combine(TestContext.CurrentContext.TestDirectory, "a"), FileMode.Append));

			// ACT
			_instance.ExportErrorReport(_EXPORT_LOCATION);

			// ASSERT
			_fileHelper.Verify(x => x.Create(_EXPORT_LOCATION));
		}

		[Test]
		public void ItShouldWriteAllErrorsWhenExportingErrorReport()
		{
			var allErrorsTempFile = "all_errors";
			_allErrors.Setup(x => x.HasErrors()).Returns(true);
			_allErrors.Setup(x => x.WriteErrorsToTempFile()).Returns(allErrorsTempFile);

			// ACT
			_instance.ExportErrorReport(_EXPORT_LOCATION);

			// ASSERT
			_fileHelper.Verify(x => x.Copy(allErrorsTempFile, _EXPORT_LOCATION, true));
		}

		[Test]
		public void ItShouldSkipWhenExportingErrorFileWithoutErrors()
		{
			_allErrors.Setup(x => x.HasErrors()).Returns(false);

			// ACT
			_instance.ExportErrorFile(_EXPORT_LOCATION);

			// ASSERT
			_fileHelper.Verify(x => x.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
		}

		[Test]
		public void ItShouldWriteMergedErrorsWhenExportingErrorFile()
		{
			var allErrorsTempFile = "all_errors";
			_allErrors.Setup(x => x.HasErrors()).Returns(true);
			_allErrors.Setup(x => x.WriteErrorsToTempFile()).Returns(allErrorsTempFile);

			var clientErrorsTempFile = "client_errors";
			_clientErrors.Setup(x => x.WriteErrorsToTempFile()).Returns(clientErrorsTempFile);

			var mergedTempFile = "merged_errors";
			_artifactReader.Setup(x => x.ManageErrorRecords(allErrorsTempFile, clientErrorsTempFile)).Returns(mergedTempFile);

			// ACT
			_instance.ExportErrorFile(_EXPORT_LOCATION);

			// ASSERT
			_fileHelper.Verify(x => x.Copy(mergedTempFile, _EXPORT_LOCATION, true));
		}

		[Test]
		public void ItShouldExportErrorsToAppropriateFiles()
		{
			var allErrorsFile = "all_errors";
			var mergedFile = "merged_file";

			_allErrors.Setup(x => x.HasErrors()).Returns(true);
			_allErrors.Setup(x => x.WriteErrorsToTempFile()).Returns(allErrorsFile);

			_artifactReader.Setup(x => x.ManageErrorRecords(It.IsAny<string>(), It.IsAny<string>())).Returns(mergedFile);

			var loadFilePath = "c:\\temp\\load_file.dat";

			var expectedErrorLineFile = $"{_EXPORT_LOCATION}\\load_file_ErrorLines_{_FILE_SUFFIX}.dat";
			var expectedErrorReportFile = $"{_EXPORT_LOCATION}\\load_file_ErrorReport_{_FILE_SUFFIX}.csv";

			// ACT
			_instance.ExportErrors(_EXPORT_LOCATION, loadFilePath);

			// ASSERT
			_fileHelper.Verify(x => x.Copy(allErrorsFile, expectedErrorReportFile, true), Times.Once);
			_fileHelper.Verify(x => x.Copy(mergedFile, expectedErrorLineFile, true), Times.Once);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void ItShouldReturnHasErrorsWhenAllErrorsContainsErrors(bool expectedResult)
		{
			_allErrors.Setup(x => x.HasErrors()).Returns(expectedResult);

			// ACT
			var hasErrors = _instance.HasErrors;

			// ASSERT
			Assert.That(hasErrors, Is.EqualTo(expectedResult));
		}
	}
}