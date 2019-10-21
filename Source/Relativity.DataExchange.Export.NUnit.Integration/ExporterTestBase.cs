// -----------------------------------------------------------------------------------------------------
// <copyright file="ExporterTestBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a base class for <see cref="Exporter"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Net;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using Castle.MicroKernel.Registration;
	using Castle.Windsor;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Container;
	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.Service.Export;

	using Moq;

	using Relativity.DataExchange;
	using Relativity.DataExchange.Export.VolumeManagerV2;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Process;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.Transfer;

	/// <summary>
	/// Represents a base class for <see cref="Exporter"/> tests.
	/// </summary>
	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public abstract class ExporterTestBase
	{
		/// <summary>
		/// The sample PDF file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleDocPdfFileName = "EDRM-Sample1.pdf";

		/// <summary>
		/// The sample Word doc file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleDocWordFileName = "EDRM-Sample2.doc";

		/// <summary>
		/// The sample Excel file name that's available for testingS within the output directory.
		/// </summary>
		protected const string SampleDocExcelFileName = "EDRM-Sample3.xlsx";

		/// <summary>
		/// The sample MSG file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleDocMsgFileName = "EDRM-Sample4.msg";

		/// <summary>
		/// The sample HTM file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleDocHtmFileName = "EDRM-Sample5.htm";

		/// <summary>
		/// The sample EMF file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleDocEmfFileName = "EDRM-Sample6.emf";

		/// <summary>
		/// The sample PPT file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleDocPptFileName = "EDRM-Sample7.ppt";

		/// <summary>
		/// The sample PNG file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleDocPngFileName = "EDRM-Sample8.png";

		/// <summary>
		/// The sample TXT file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleDocTxtFileName = "EDRM-Sample9.txt";

		/// <summary>
		/// The sample WMF file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleDocWmfFileName = "EDRM-Sample10.wmf";

		/// <summary>
		/// The sample TIFF file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleImage1FileName = "EDRM-Sample1.tif";

		/// <summary>
		/// The sample TIFF file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleImage2FileName = "EDRM-Sample2.tif";

		/// <summary>
		/// The sample TIFF file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleImage3FileName = "EDRM-Sample3.tif";

		/// <summary>
		/// The sample production image file name.
		/// </summary>
		protected const string SampleProductionImage1FileName = "EDRM-Sample-000001.tif";

		/// <summary>
		/// The dummy UNC path. This should never be used for positive tests.
		/// </summary>
		private const string DummyUncPath = @"\\files\T001\Files\EDDS123456\";

		private static readonly List<int> ImportedDatasets = new List<int>();
		private readonly object syncRoot = new object();
		private readonly List<string> alerts = new List<string>();
		private readonly List<string> alertCriticalErrors = new List<string>();
		private readonly List<string> alertWarningSkippables = new List<string>();
		private readonly List<string> statusMessages = new List<string>();
		private readonly List<string> fatalErrors = new List<string>();
		private IWindsorContainer testContainer;
		private CookieContainer cookieContainer;
		private NetworkCredential credentials;
		private TempDirectory2 tempDirectory;
		private int selectedFolderId;
		private ExportFile.ExportType exportType;
		private string identifierColumnName;
		private Encoding loadFileEncoding;
		private Encoding textFileEncoding;
		private bool searchResult;
		private ExtendedExportFile exportFile;
		private ProcessContext processContext;
		private TestContainerFactory testContainerFactory;
		private CancellationTokenSource cancellationTokenSource;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExporterTestBase"/> class.
		/// </summary>
		/// <param name="loadFileEncoding">
		/// Specify the encoding used by the load file.
		/// </param>
		/// <param name="textFileEncoding">
		/// Specify the encoding used to convert extracted text files.
		/// </param>
		protected ExporterTestBase(string loadFileEncoding, string textFileEncoding)
		{
			this.GivenTheLoadFileEncoding(Encoding.GetEncoding(loadFileEncoding));
			this.GivenTheTextFileEncoding(Encoding.GetEncoding(textFileEncoding));
		}

		internal Mock<TapiObjectService> MockTapiObjectService
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the list of all sample document file names available for testing within the output directory.
		/// </summary>
		/// <value>
		/// The file names.
		/// </value>
		protected static IReadOnlyList<string> AllSampleDocFileNames =>
			new List<string>
				{
					SampleDocPdfFileName,
					SampleDocWordFileName,
					SampleDocExcelFileName,
					SampleDocMsgFileName,
					SampleDocHtmFileName,
					SampleDocEmfFileName,
					SampleDocPptFileName,
					SampleDocPngFileName,
					SampleDocTxtFileName,
					SampleDocWmfFileName,
				};

		/// <summary>
		/// Gets the list of all sample image file names available for testing within the output directory.
		/// </summary>
		/// <value>
		/// The file names.
		/// </value>
		protected static IReadOnlyList<string> AllSampleImageFileNames =>
			new List<string>
				{
					SampleImage1FileName,
					SampleImage2FileName,
					SampleImage3FileName,
					SampleProductionImage1FileName,
				};

		/// <summary>
		/// Gets the list of full paths for all sample document and image files for testing within the output directory.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		protected static IReadOnlyList<string> AllSampleFiles =>
			AllSampleDocFileNames.Select(ResourceFileHelper.GetDocsResourceFilePath).Concat(
				AllSampleImageFileNames.Select(ResourceFileHelper.GetImagesResourceFilePath)).ToList();

		protected Mock<IAppSettings> MockAppSettings
		{
			get;
			private set;
		}

		protected Mock<IExportRequestRetriever> MockExportRequestRetriever
		{
			get;
			private set;
		}

		protected Mock<IFileShareSettingsService> MockFileShareSettingsService
		{
			get;
			private set;
		}

		protected Mock<Relativity.Logging.ILog> MockLogger
		{
			get;
			private set;
		}

		protected Mock<IProcessErrorWriter> MockProcessErrorWriter
		{
			get;
			private set;
		}

		protected Mock<IProcessEventWriter> MockProcessEventWriter
		{
			get;
			private set;
		}

		protected Mock<IUserNotification> MockUserNotification
		{
			get;
			private set;
		}

		protected string TempDirectory => this.tempDirectory.Directory;

		/// <summary>
		/// Gets integration test parameters.
		/// </summary>
		/// <value>
		/// The <see cref="IntegrationTestParameters"/> value.
		/// </value>
		protected IntegrationTestParameters TestParameters
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the total number of documents that were processed.
		/// </summary>
		/// <value>
		/// The total number of documents.
		/// </value>
		protected int TotalDocumentsProcessed
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the total number of requested documents to export.
		/// </summary>
		/// <value>
		/// The total number of documents.
		/// </value>
		protected int TotalDocuments
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the list of transfer modes.
		/// </summary>
		/// <value>
		/// The <see cref="TapiClient"/> value.
		/// </value>
		protected IList<TapiClient> TransferModes
		{
			get;
			private set;
		}

		/// <summary>
		/// The test setup.
		/// </summary>
		[SetUp]
		public void Setup()
		{
			this.cancellationTokenSource = new CancellationTokenSource();
			foreach (var item in System.Text.Encoding.GetEncodings())
			{
				System.Console.WriteLine($"Name={item.Name}, Display={item.DisplayName}, Code Page={item.CodePage}");
				System.Diagnostics.Debug.WriteLine($"Name={item.Name}, CodePage={item.DisplayName}, CodePage={item.CodePage}");
			}

			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11
				| SecurityProtocolType.Tls12;
			this.MockLogger = MockObjectFactory.CreateMockLogger();
			this.MockAppSettings = MockObjectFactory.CreateMockAppSettings();
			this.MockExportRequestRetriever = MockObjectFactory.CreateMockExportRequestRetriever();
			this.MockFileShareSettingsService = MockObjectFactory.CreateMockFileShareSettingsService();
			this.MockProcessEventWriter = MockObjectFactory.CreateMockProcessEventWriter();
			this.MockProcessErrorWriter = MockObjectFactory.CreateMockProcessErrorWriter();
			this.MockTapiObjectService = MockObjectFactory.CreateMockTapiObjectService();
			this.testContainer = new WindsorContainer();
			this.testContainerFactory = new TestContainerFactory(this.testContainer, this.MockLogger.Object);
			this.AssignTestSettings();
			Assert.That(
				this.TestParameters.WorkspaceId,
				Is.Positive,
				() => "The test workspace must be created or specified in order to run this integration test.");
			this.alerts.Clear();
			this.alertCriticalErrors.Clear();
			this.alertWarningSkippables.Clear();
			this.exportFile = null;
			this.fatalErrors.Clear();
			this.statusMessages.Clear();
			this.selectedFolderId = 0;
			this.exportType = ExportFile.ExportType.AncestorSearch;
			this.cookieContainer = new CookieContainer();
			this.credentials = new NetworkCredential(this.TestParameters.RelativityUserName, this.TestParameters.RelativityPassword);
			this.identifierColumnName = null;
			this.loadFileEncoding = Encoding.Unicode;
			this.textFileEncoding = Encoding.Unicode;
			this.searchResult = false;
			this.tempDirectory = new TempDirectory2();
			this.tempDirectory.ClearReadOnlyAttributes = true;
			this.tempDirectory.Create();
			this.TotalDocumentsProcessed = 0;
			this.TotalDocuments = 0;
			this.TransferModes = new List<TapiClient>();
			this.MockUserNotification = MockObjectFactory.CreateMockUserNotification();
			this.MockUserNotification.Setup(x => x.Alert(It.IsAny<string>())).Callback<string>(msg =>
			{
				this.alerts.Add(msg);
				Console.WriteLine($"Alert: {msg}");
			});
			this.MockUserNotification.Setup(x => x.AlertCriticalError(It.IsAny<string>()))
				.Callback<string>(msg =>
				{
					this.alertCriticalErrors.Add(msg);
					Console.WriteLine($"Alert critical error: {msg}");
				});
			this.MockUserNotification.Setup(x => x.AlertWarningSkippable(It.IsAny<string>()))
				.Callback<string>(msg =>
				{
					this.alertWarningSkippables.Add(msg);
					Console.WriteLine($"Alert warning skipped: {msg}");
				}).Returns(true);
			AppSettings.Instance.WebApiServiceUrl = this.TestParameters.RelativityWebApiUrl.ToString();
			this.processContext = new ProcessContext(
				this.MockProcessEventWriter.Object,
				this.MockProcessErrorWriter.Object,
				this.MockAppSettings.Object,
				this.MockLogger.Object);

			// Sanity check since these are global settings.
			AppSettings.Instance.TapiForceAsperaClient = AppSettingsConstants.TapiForceAsperaClientDefaultValue;
			AppSettings.Instance.TapiForceBcpHttpClient = AppSettingsConstants.TapiForceBcpHttpClientDefaultValue;
			AppSettings.Instance.TapiForceClientCandidates = AppSettingsConstants.TapiForceClientCandidatesDefaultValue;
			AppSettings.Instance.TapiForceFileShareClient = AppSettingsConstants.TapiForceFileShareClientDefaultValue;
			AppSettings.Instance.TapiForceHttpClient = AppSettingsConstants.TapiForceHttpClientDefaultValue;

			// This registers all components.
			ContainerFactoryProvider.ContainerFactory = this.testContainerFactory;
		}

		/// <summary>
		/// The test teardown.
		/// </summary>
		[TearDown]
		public void Teardown()
		{
			this.tempDirectory?.Dispose();
			this.tempDirectory = null;
			this.testContainer?.Dispose();
			this.testContainer = null;
			this.cancellationTokenSource?.Dispose();
			this.cancellationTokenSource = null;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA2201:DoNotRaiseReservedExceptionTypes",
			Justification = "This is used strictly for testing purposes.")]
		protected static Exception CreateFatalException()
		{
			return new OutOfMemoryException();
		}

		protected PhysicalFileExportRequest CreateTestPhysicalFileExportRequest(
			int artifactId,
			int order,
			bool validNativeFileGuid,
			bool validNativeSourceLocation,
			bool validDestinationLocation)
		{
			string fileName = $"{Guid.NewGuid()}.doc";
			string fileUncPath = $"{DummyUncPath}{fileName}";
			ObjectExportInfo artifact = new ObjectExportInfo
				                            {
					                            ArtifactID = artifactId,
					                            NativeSourceLocation = validNativeSourceLocation ? fileUncPath : null,
					                            NativeFileGuid =
						                            validNativeFileGuid ? System.Guid.NewGuid().ToString() : null,
				                            };
			PhysicalFileExportRequest request = new PhysicalFileExportRequest(
				artifact,
				validDestinationLocation ? System.IO.Path.Combine(this.TempDirectory, fileName) : null);
			request.Order = order;
			return request;
		}

		protected FieldFileExportRequest CreateTestFieldFileExportRequest(
			int artifactId,
			int fileFieldArtifactId,
			int order,
			bool validNativeFileGuid,
			bool validNativeSourceLocation,
			bool validDestinationLocation)
		{
			string fileName = $"{Guid.NewGuid()}.msg";
			string fileUncPath = $"{DummyUncPath}{fileName}";
			ObjectExportInfo artifact = new ObjectExportInfo
				                            {
					                            ArtifactID = artifactId,
					                            NativeSourceLocation = validNativeSourceLocation ? fileUncPath : null,
					                            NativeFileGuid =
						                            validNativeFileGuid ? System.Guid.NewGuid().ToString() : null,
				                            };
			FieldFileExportRequest request = new FieldFileExportRequest(
				artifact,
				fileFieldArtifactId,
				validDestinationLocation ? System.IO.Path.Combine(this.TempDirectory, fileName) : null);
			request.Order = order;
			return request;
		}

		protected LongTextExportRequest CreateTestLongTextExportRequest(
			int artifactId,
			int fieldArtifactId,
			int order,
			bool validNativeSourceLocation,
			bool validDestinationLocation)
		{
			string fileName = $"{Guid.NewGuid()}.txt";
			string fileUncPath = $"{DummyUncPath}{fileName}";
			ObjectExportInfo artifact = new ObjectExportInfo
				                            {
					                            ArtifactID = artifactId,
					                            NativeSourceLocation = validNativeSourceLocation ? fileUncPath : null,
					                            NativeFileGuid = null,
				                            };
			LongTextExportRequest request = LongTextExportRequest.CreateRequestForLongText(
				artifact,
				fieldArtifactId,
				validDestinationLocation ? System.IO.Path.Combine(this.TempDirectory, fileName) : null);
			request.Order = order;
			return request;
		}

		/// <summary>
		/// Assign the test parameters. This should always be called from methods with <see cref="SetUpAttribute"/> or <see cref="OneTimeSetUpAttribute"/>.
		/// </summary>
		protected void AssignTestSettings()
		{
			if (this.TestParameters == null)
			{
				this.TestParameters = AssemblySetup.TestParameters.DeepCopy();
			}
		}

		/// <summary>
		/// Given the list of native files are imported.
		/// </summary>
		/// <param name="files">
		/// The list of files import.
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// The exception thrown when the import fails.
		/// </exception>
		protected void GivenTheFilesAreImported(IReadOnlyList<string> files)
		{
			this.GivenTheFilesAreImported(null, files);
		}

		/// <summary>
		/// Given the list of native files are imported and assigned the specified folder path.
		/// </summary>
		/// <param name="folderPath">
		/// The folder path.
		/// </param>
		/// <param name="files">
		/// The list of files import.
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// The exception thrown when the import fails.
		/// </exception>
		protected void GivenTheFilesAreImported(string folderPath, IReadOnlyList<string> files)
		{
			if (files == null)
			{
				throw new ArgumentNullException(nameof(files));
			}

			int datasetId = 0;
			foreach (string file in files)
			{
				const int HashConstant = 397;
				datasetId = (datasetId * HashConstant) ^ file.ToUpperInvariant().GetHashCode();
			}

			if (ImportedDatasets.Contains(datasetId))
			{
				return;
			}

			kCura.Relativity.ImportAPI.ImportAPI importApi =
				new kCura.Relativity.ImportAPI.ImportAPI(
					this.TestParameters.RelativityUserName,
					this.TestParameters.RelativityPassword,
					this.TestParameters.RelativityWebApiUrl.ToString());
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = importApi.NewNativeDocumentImportJob();
			kCura.Relativity.DataReaderClient.Settings settings = job.Settings;
			settings.ArtifactTypeId = WellKnownArtifactTypes.DocumentArtifactTypeId;
			settings.Billable = false;
			settings.BulkLoadFileFieldDelimiter = ";";
			settings.CaseArtifactId = this.TestParameters.WorkspaceId;
			settings.CopyFilesToDocumentRepository = true;
			settings.DisableControlNumberCompatibilityMode = true;
			settings.DisableExtractedTextFileLocationValidation = false;
			settings.DisableNativeLocationValidation = false;
			settings.DisableNativeValidation = false;
			settings.ExtractedTextEncoding = System.Text.Encoding.Unicode;
			settings.ExtractedTextFieldContainsFilePath = false;
			settings.FileSizeColumn = WellKnownFields.NativeFileSize;
			settings.FileSizeMapped = true;
			settings.FolderPathSourceFieldName = WellKnownFields.FolderName;
			settings.IdentityFieldId = WellKnownFields.ControlNumberId;
			settings.LoadImportedFullTextFromServer = false;
			settings.MaximumErrorCount = int.MaxValue - 1;
			settings.MoveDocumentsInAppendOverlayMode = false;
			settings.NativeFileCopyMode = kCura.Relativity.DataReaderClient.NativeFileCopyModeEnum.CopyFiles;
			settings.NativeFilePathSourceFieldName = WellKnownFields.FilePath;
			settings.OIFileIdColumnName = WellKnownFields.OutsideInFileId;
			settings.OIFileIdMapped = true;
			settings.OIFileTypeColumnName = WellKnownFields.OutsideInFileType;
			settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append;
			settings.SelectedIdentifierFieldName = WellKnownFields.ControlNumber;
			settings.StartRecordNumber = 0;
			using (var dataSource = new System.Data.DataTable("Input Data"))
			{
				dataSource.Locale = CultureInfo.InvariantCulture;
				dataSource.Columns.Add(WellKnownFields.ControlNumber);
				dataSource.Columns.Add(WellKnownFields.FilePath);
				dataSource.Columns.Add(WellKnownFields.FolderName);
				foreach (var file in files)
				{
					System.Data.DataRow dr = dataSource.NewRow();
					dr[WellKnownFields.ControlNumber] = "REL-" + Guid.NewGuid();
					dr[WellKnownFields.FilePath] = file;
					dr[WellKnownFields.FolderName] = folderPath;
					dataSource.Rows.Add(dr);
				}

				job.SourceData.SourceData = dataSource.CreateDataReader();
				job.OnFatalException += report => throw report.FatalException;
				job.OnComplete += report =>
				{
					if (report.FatalException != null)
					{
						throw report.FatalException;
					}

					if (report.ErrorRowCount > 0)
					{
						IEnumerable<string> errors = report.ErrorRows.Select(x => $"{x.Identifier} - {x.Message}");
						throw new InvalidOperationException(string.Join("\n", errors));
					}
				};

				job.Execute();
				ImportedDatasets.Add(datasetId);
			}
		}

		protected void GivenTheSelectedFolderId(int value)
		{
			this.selectedFolderId = value;
		}

		/// <summary>
		/// Given the export type.
		/// </summary>
		/// <param name="value">
		/// The export type.
		/// </param>
		protected void GivenTheExportType(ExportFile.ExportType value)
		{
			this.exportType = value;
		}

		/// <summary>
		/// Given the identifier column name.
		/// </summary>
		/// <param name="value">
		/// The column name.
		/// </param>
		protected void GivenTheIdentifierColumnName(string value)
		{
			this.identifierColumnName = value;
		}

		/// <summary>
		/// Given the load file encoding.
		/// </summary>
		/// <param name="encoding">
		/// The encoding used for load files.
		/// </param>
		protected void GivenTheLoadFileEncoding(Encoding encoding)
		{
			this.loadFileEncoding = encoding;
		}

		/// <summary>
		/// Given the encoding used to convert extracted text files.
		/// </summary>
		/// <param name="encoding">
		/// The encoding used to convert extracted text files.
		/// </param>
		protected void GivenTheTextFileEncoding(Encoding encoding)
		{
			this.textFileEncoding = encoding;
		}

		protected void GivenTheMockTapiObjectServiceIsRegistered()
		{
			this.testContainer.Register(
				Component.For<ITapiObjectService>().UsingFactoryMethod(k => this.MockTapiObjectService.Object)
					.IsDefault());
		}

		protected void GivenTheMockFileShareSettingsServiceIsRegistered()
		{
			this.testContainer.Register(
				Component.For<IFileShareSettingsService>()
					.UsingFactoryMethod(k => this.MockFileShareSettingsService.Object).IsDefault());
		}

		protected void GivenTheMockExportRequestRetrieverIsRegistered()
		{
			this.testContainer.Register(
				Component.For<IExportRequestRetriever>()
					.UsingFactoryMethod(k => this.MockExportRequestRetriever.Object).IsDefault());
		}

		protected void GivenTheMockedSearchResultsAreEmpty(bool cloudInstance)
		{
			Mock<ITapiFileStorageSearchResults> mockFileStorageSearchResults =
				MockObjectFactory.CreateMockEmptyTapiFileStorageSearchResults(cloudInstance);
			this.MockTapiObjectService.Setup(
				x => x.SearchFileStorageAsync(
					It.IsAny<TapiBridgeParameters2>(),
					It.IsAny<Relativity.Logging.ILog>(),
					It.IsAny<CancellationToken>())).Returns(Task.FromResult(mockFileStorageSearchResults.Object));
		}

		protected void GivenTheMockedSearchResultsAreInvalid(bool cloudInstance)
		{
			Mock<ITapiFileStorageSearchResults> mockFileStorageSearchResults =
				MockObjectFactory.CreateMockInvalidTapiFileStorageSearchResults(cloudInstance);
			this.MockTapiObjectService.Setup(
				x => x.SearchFileStorageAsync(
					It.IsAny<TapiBridgeParameters2>(),
					It.IsAny<Relativity.Logging.ILog>(),
					It.IsAny<CancellationToken>())).Returns(Task.FromResult(mockFileStorageSearchResults.Object));
		}

		protected void GivenTheMockedFileStorageSearchThrows(bool fatal)
		{
			this.MockTapiObjectService.Setup(
					x => x.SearchFileStorageAsync(
						It.IsAny<TapiBridgeParameters2>(),
						It.IsAny<Relativity.Logging.ILog>(),
						It.IsAny<CancellationToken>()))
				.Throws(fatal ? CreateFatalException() : new InvalidOperationException());
		}

		protected void GivenTheMockedExportRequestRetrieverReturns(IEnumerable<ExportRequest> fileExportRequests, IEnumerable<LongTextExportRequest> longTextExportRequests)
		{
			this.MockExportRequestRetriever.Setup(x => x.RetrieveFileExportRequests()).Returns(fileExportRequests.ToList());
			this.MockExportRequestRetriever.Setup(x => x.RetrieveLongTextExportRequests()).Returns(longTextExportRequests.ToList());
		}

		protected void GivenTheMockedSettingsForFileShareIsNull()
		{
			this.MockFileShareSettingsService.Setup(x => x.GetSettingsForFileShare(It.IsAny<int>(), It.IsAny<string>()))
				.Returns((IRelativityFileShareSettings)null);
			this.MockFileShareSettingsService.Setup(x => x.ReadFileSharesAsync(It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);
		}

		protected void GivenTheTapiForceClientAppSettings(TapiClient client)
		{
			// Export relies on the global parameters.
			switch (client)
			{
				case TapiClient.Aspera:
					AppSettings.Instance.TapiForceAsperaClient = true;
					this.MockAppSettings.SetupGet(x => x.TapiForceAsperaClient).Returns(true);
					break;

				case TapiClient.Direct:
					AppSettings.Instance.TapiForceFileShareClient = true;
					this.MockAppSettings.SetupGet(x => x.TapiForceFileShareClient).Returns(true);
					break;

				case TapiClient.Web:
					AppSettings.Instance.TapiForceHttpClient = true;
					this.MockAppSettings.SetupGet(x => x.TapiForceHttpClient).Returns(true);
					break;

				case TapiClient.None:
					AppSettings.Instance.TapiForceAsperaClient = false;
					AppSettings.Instance.TapiForceFileShareClient = false;
					AppSettings.Instance.TapiForceHttpClient = false;
					this.MockAppSettings.SetupGet(x => x.TapiForceAsperaClient).Returns(false);
					this.MockAppSettings.SetupGet(x => x.TapiForceFileShareClient).Returns(false);
					this.MockAppSettings.SetupGet(x => x.TapiForceHttpClient).Returns(false);
					break;

				default:
					Assert.Fail($"The Transfer API client {client} isn't supported by this test.");
					break;
			}
		}

		/// <summary>
		/// Queries for the total number of documents.
		/// </summary>
		/// <returns>
		/// The total number of documents.
		/// </returns>
		protected int QueryDocumentCount()
		{
			return this.QueryRelativityObjectCount((int)kCura.Relativity.Client.ArtifactType.Document);
		}

		/// <summary>
		/// Queries for the total number of objects for the specified RDO type.
		/// </summary>
		/// <param name="artifactTypeId">
		/// The RDO artifact type identifier.
		/// </param>
		/// <returns>
		/// The total number of objects.
		/// </returns>
		protected int QueryRelativityObjectCount(int artifactTypeId)
		{
			return RdoHelper.QueryRelativityObjectCount(this.TestParameters, artifactTypeId);
		}

		/// <summary>
		/// When asynchronously getting the workspace information.
		/// </summary>
		/// <returns>
		/// The <see cref="CaseInfo"/> instance.
		/// </returns>
		protected Task<CaseInfo> WhenGettingTheWorkspaceInfoAsync()
		{
			using (var caseManager = new kCura.WinEDDS.Service.CaseManager(this.credentials, this.cookieContainer))
			{
				var caseInfo = caseManager.Read(this.TestParameters.WorkspaceId);
				return Task.FromResult(caseInfo);
			}
		}

		/// <summary>
		/// When asynchronously getting the folder artifact unique identifier for the specified folder path.
		/// </summary>
		/// <param name="caseInfo">
		/// The workspace info.
		/// </param>
		/// <param name="folderPath">
		/// The folder path.
		/// </param>
		/// <returns>
		/// The artifact identifier.
		/// </returns>
		protected Task<int> WhenGettingTheFolderIdAsync(CaseInfo caseInfo, string folderPath)
		{
			if (caseInfo == null)
			{
				throw new ArgumentNullException(nameof(caseInfo));
			}

			using (var folderManager = new kCura.WinEDDS.Service.FolderManager(this.credentials, this.cookieContainer))
			{
				FolderCache folderCache = new FolderCache(this.MockLogger.Object, folderManager, caseInfo.RootFolderID, caseInfo.ArtifactID);
				int folderArtifactId = folderCache.GetFolderId(folderPath);
				return Task.FromResult(folderArtifactId);
			}
		}

		/// <summary>
		/// When asynchronously creating the export file object.
		/// </summary>
		/// <param name="caseInfo">
		/// The workspace info.
		/// </param>
		/// <returns>
		/// The <see cref="Task"/> instance.
		/// </returns>
		protected async Task WhenCreatingTheExportFileAsync(CaseInfo caseInfo)
		{
			using (var searchManager = new kCura.WinEDDS.Service.SearchManager(this.credentials, this.cookieContainer))
			using (var productionManager =
				new kCura.WinEDDS.Service.ProductionManager(this.credentials, this.cookieContainer))
			{
				this.exportFile = new kCura.WinEDDS.ExtendedExportFile((int)ArtifactType.Document)
				{
					// general settings
					ArtifactID = this.selectedFolderId,
					CaseInfo = caseInfo,
					CookieContainer = this.cookieContainer,
					Credential = this.credentials,
					TypeOfExport = this.exportType,
					FolderPath = this.tempDirectory.Directory,
					TextFileEncoding = this.textFileEncoding,

					// settings for exporting natives
					ExportNative = true,
					TypeOfExportedFilePath = ExportFile.ExportedFilePathType.Absolute,
					IdentifierColumnName = this.identifierColumnName,
					LoadFileEncoding = this.loadFileEncoding,
					LoadFilesPrefix = "Documents",
					LoadFileExtension = "dat",
					MultiRecordDelimiter = ';',
					NestedValueDelimiter = '\\',
					NewlineDelimiter = '@',
					QuoteDelimiter = 'þ',
					ViewID = 1003684,
					SelectedViewFields = new kCura.WinEDDS.ViewFieldInfo[] { },

					// settings for exporting images
					ExportImages = true,
					LogFileFormat = LoadFileType.FileFormat.Opticon,
					TypeOfImage = ExportFile.ImageType.Pdf,
					ImagePrecedence = new[]
					{
						new Pair("-1", "Original"),
						new Pair("-1", "Original"),
					},

					// settings for volumes and subdirectories
					SubdirectoryDigitPadding = 3,
					VolumeDigitPadding = 2,
					VolumeInfo = new VolumeInfo
					{
						CopyImageFilesFromRepository = true,
						CopyNativeFilesFromRepository = true,
						SubdirectoryStartNumber = 1,
						SubdirectoryMaxSize = 500,
						VolumeStartNumber = 1,
						VolumeMaxSize = 650,
						VolumePrefix = "VOL",
					},
				};

				this.exportFile.ObjectTypeName =
					await this.GetObjectTypeNameAsync(this.exportFile.ArtifactTypeID).ConfigureAwait(false);
				switch (this.exportType)
				{
					case ExportFile.ExportType.Production:
						this.exportFile.DataTable = productionManager
							.RetrieveProducedByContextArtifactID(this.TestParameters.WorkspaceId).Tables[0];
						break;

					default:
						this.exportFile.DataTable = this.GetSearchExportDataSourceAsync(
							searchManager,
							this.exportType == kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch,
							this.exportFile.ArtifactTypeID);
						break;
				}

				List<int> artifactIds = new List<int>();
				foreach (System.Data.DataRow row in this.exportFile.DataTable.Rows)
				{
					artifactIds.Add((int)row["ArtifactID"]);
				}

				var fields = this.GetFieldsAsync(this.exportFile.ArtifactTypeID);
				foreach (DocumentField field in fields)
				{
					if (field.FieldTypeID == (int)FieldType.File)
					{
						this.exportFile.FileField = field;
						break;
					}
				}

				if (artifactIds.Count == 0)
				{
					this.exportFile.ArtifactAvfLookup = new System.Collections.Specialized.HybridDictionary();
					this.exportFile.AllExportableFields = new kCura.WinEDDS.ViewFieldInfo[] { };
				}
				else
				{
					this.exportFile.ArtifactAvfLookup = searchManager.RetrieveDefaultViewFieldsForIdList(
						this.TestParameters.WorkspaceId,
						this.exportFile.ArtifactTypeID,
						artifactIds.ToArray(),
						this.exportType == kCura.WinEDDS.ExportFile.ExportType.Production);
					this.exportFile.AllExportableFields =
						searchManager.RetrieveAllExportableViewFields(this.TestParameters.WorkspaceId, this.exportFile.ArtifactTypeID);
				}
			}
		}

		/// <summary>
		/// When exporting the documents.
		/// </summary>
		protected void WhenExportingTheDocs()
		{
			Exporter exporter = new Exporter(
				this.exportFile,
				this.processContext,
				new WebApiServiceFactory(this.exportFile),
				new ExportFileFormatterFactory(),
				new ExportConfig(),
				this.MockLogger.Object,
				this.cancellationTokenSource.Token);
			try
			{
				exporter.StatusMessage += this.ExporterOnStatusMessage;
				exporter.FileTransferMultiClientModeChangeEvent += this.ExporterOnFileTransferModeChangeEvent;
				exporter.FatalErrorEvent += this.ExporterOnFatalErrorEvent;
				this.searchResult = exporter.ExportSearch();
			}
			finally
			{
				exporter.StatusMessage -= this.ExporterOnStatusMessage;
				exporter.FileTransferMultiClientModeChangeEvent -= this.ExporterOnFileTransferModeChangeEvent;
				exporter.FatalErrorEvent -= this.ExporterOnFatalErrorEvent;
			}
		}

		/// <summary>
		/// Then the alert critical errors count should equal the specified count.
		/// </summary>
		/// <param name="expected">
		/// The expected count.
		/// </param>
		protected void ThenTheAlertCriticalErrorsCountShouldEqual(int expected)
		{
			Assert.That(this.alertCriticalErrors.Count, Is.EqualTo(expected));
		}

		/// <summary>
		/// Then the alert warning skippables count should equal the specified count.
		/// </summary>
		/// <param name="expected">
		/// The expected count.
		/// </param>
		protected void ThenTheAlertWarningSkippablesCountShouldEqual(int expected)
		{
			Assert.That(this.alertWarningSkippables.Count, Is.EqualTo(expected));
		}

		/// <summary>
		/// Then the alerts count should equal the specified count.
		/// </summary>
		/// <param name="expected">
		/// The expected count.
		/// </param>
		protected void ThenTheAlertsCountShouldEqual(int expected)
		{
			Assert.That(this.alerts.Count, Is.EqualTo(expected));
		}

		/// <summary>
		/// Then the fatal errors count should equal the specified count.
		/// </summary>
		/// <param name="expected">
		/// The expected count.
		/// </param>
		protected void ThenTheFatalErrorsCountShouldEqual(int expected)
		{
			Assert.That(this.fatalErrors.Count, Is.EqualTo(expected));
		}

		/// <summary>
		/// Then the status messages count should be non-zero.
		/// </summary>
		protected void ThenTheStatusMessagesCountShouldBeNonZero()
		{
			Assert.That(this.statusMessages.Count, Is.Positive);
		}

		/// <summary>
		/// Then the search result should equal the specified result.
		/// </summary>
		/// <param name="expected">
		/// The expected result.
		/// </param>
		protected void ThenTheSearchResultShouldEqual(bool expected)
		{
			Assert.That(this.searchResult, Is.EqualTo(expected));
		}

		/// <summary>
		/// Then the total number of documents should equal the specified result.
		/// </summary>
		/// <param name="expected">
		/// The expected number of documents.
		/// </param>
		protected void ThenTheTotalDocumentsShouldEqual(int expected)
		{
			Assert.That(this.TotalDocuments, Is.EqualTo(expected));
		}

		/// <summary>
		/// Then the total number of documents processed should equal the specified result.
		/// </summary>
		/// <param name="expected">
		/// The expected number of processed documents.
		/// </param>
		protected void ThenTheTotalDocumentsProcessedShouldEqual(int expected)
		{
			Assert.That(this.TotalDocumentsProcessed, Is.EqualTo(expected));
		}

		/// <summary>
		/// Then the transfer mode should equal direct or web mode.
		/// </summary>
		protected void ThenTheTransferModeShouldEqualDirectOrWebMode()
		{
			foreach (TapiClient tapiClient in this.TransferModes)
			{
				Assert.That(tapiClient, Is.AnyOf(TapiClient.Direct, TapiClient.Web));
			}
		}

		protected void ThenTheMockSearchFileStorageAsyncIsVerified()
		{
			this.MockTapiObjectService.Verify(
				x => x.SearchFileStorageAsync(
					It.IsAny<TapiBridgeParameters2>(),
					It.IsAny<Relativity.Logging.ILog>(),
					It.IsAny<CancellationToken>()));
		}

		protected void ThenTheMockFileShareSettingsServiceIsVerified()
		{
			this.MockFileShareSettingsService.Verify(
				x => x.GetSettingsForFileShare(It.IsAny<int>(), It.IsAny<string>()));
		}

		/// <summary>
		/// Then the overall export experienced a fatal exception.
		/// </summary>
		/// <param name="expectedSearchResult">
		/// The expected search result.
		/// </param>
		protected void ThenTheExportJobIsFatal(bool expectedSearchResult)
		{
			// Note: at present, the export returns true even when fatal!
			this.ThenTheSearchResultShouldEqual(expectedSearchResult);
			this.ThenTheStatusMessagesCountShouldBeNonZero();
			this.ThenTheFatalErrorsCountShouldEqual(1);
		}

		/// <summary>
		/// Then the overall export job is not successful.
		/// </summary>
		/// <param name="expectedDocsProcessed">
		/// The expected number of processed documents.
		/// </param>
		protected void ThenTheExportJobIsNotSuccessful(int expectedDocsProcessed)
		{
			this.ThenTheSearchResultShouldEqual(false);
			this.ThenTheFatalErrorsCountShouldEqual(0);
			this.ThenTheStatusMessagesCountShouldBeNonZero();
			this.ThenTheTotalDocumentsProcessedShouldEqual(expectedDocsProcessed);
		}

		/// <summary>
		/// Then the overall export job is successful.
		/// </summary>
		/// <param name="expectedDocsProcessed">
		/// The expected number of processed documents.
		/// </param>
		protected void ThenTheExportJobIsSuccessful(int expectedDocsProcessed)
		{
			this.ThenTheSearchResultShouldEqual(true);
			this.ThenTheAlertCriticalErrorsCountShouldEqual(0);
			this.ThenTheAlertsCountShouldEqual(0);
			this.ThenTheAlertWarningSkippablesCountShouldEqual(0);
			this.ThenTheFatalErrorsCountShouldEqual(0);
			this.ThenTheStatusMessagesCountShouldBeNonZero();
			this.ThenTheTotalDocumentsProcessedShouldEqual(expectedDocsProcessed);
		}

		private Task<string> GetObjectTypeNameAsync(int artifactTypeId)
		{
			using (var objectTypeManager =
				new kCura.WinEDDS.Service.ObjectTypeManager(this.credentials, this.cookieContainer))
			{
				var dataset = objectTypeManager.RetrieveAllUploadable(this.TestParameters.WorkspaceId);
				var rows = dataset.Tables[0].Rows;
				foreach (System.Data.DataRow row in rows)
				{
					var item = new kCura.WinEDDS.ObjectTypeListItem(
						(int)row["DescriptorArtifactTypeID"],
						(string)row["Name"],
						(bool)row["HasAddPermission"]);
					if (item.Value == artifactTypeId)
					{
						return Task.FromResult(item.Display);
					}
				}

				return Task.FromResult(string.Empty);
			}
		}

		private System.Data.DataTable GetSearchExportDataSourceAsync(
			kCura.WinEDDS.Service.SearchManager searchManager,
			bool isArtifactSearch,
			int artifactType)
		{
			System.Data.DataSet dataset =
				searchManager.RetrieveViewsByContextArtifactID(
					this.TestParameters.WorkspaceId,
					artifactType,
					isArtifactSearch);
			return dataset.Tables[0];
		}

		private kCura.WinEDDS.DocumentFieldCollection GetFieldsAsync(int artifactTypeId)
		{
			var fields = new DocumentFieldCollection();
			using (var fieldQuery = new kCura.WinEDDS.Service.FieldQuery(this.credentials, this.cookieContainer))
			{
				foreach (var field in fieldQuery.RetrieveAllAsArray(this.TestParameters.WorkspaceId, artifactTypeId))
				{
					fields.Add(new DocumentField(
						field.DisplayName,
						field.ArtifactID,
						field.FieldTypeID,
						field.FieldCategoryID,
						field.CodeTypeID,
						field.MaxLength,
						field.AssociativeArtifactTypeID,
						field.UseUnicodeEncoding,
						field.ImportBehavior,
						field.EnableDataGrid));
				}

				return fields;
			}
		}

		private void ExporterOnFatalErrorEvent(string message, Exception ex)
		{
			lock (this.syncRoot)
			{
				this.fatalErrors.Add(message);
			}

			Console.WriteLine($"Fatal errors: {message}");
		}

		private void ExporterOnFileTransferModeChangeEvent(object sender, TapiMultiClientEventArgs e)
		{
			lock (this.syncRoot)
			{
				this.TransferModes.Clear();
				foreach (TapiClient tapiClient in e.TransferClients)
				{
					this.TransferModes.Add(tapiClient);
				}
			}

			Console.WriteLine("Transfer mode changed: {0}", string.Join(",", this.TransferModes));
		}

		private void ExporterOnStatusMessage(ExportEventArgs args)
		{
			lock (this.syncRoot)
			{
				this.TotalDocuments = args.TotalDocuments;
				this.TotalDocumentsProcessed = args.DocumentsExported;
				if (!string.IsNullOrEmpty(args.Message))
				{
					this.statusMessages.Add(args.Message);
				}
			}

			Console.WriteLine(
				$"Status: Message={args.Message}, Total Exported Docs={args.DocumentsExported}, Total Docs={args.TotalDocuments}");
		}
	}
}