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
	using System.Collections.Specialized;
	using System.Data;
	using System.Data.SqlTypes;
	using System.Globalization;
	using System.Linq;
	using System.Net;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using Castle.MicroKernel.Registration;
	using Castle.Windsor;

	using global::NUnit.Framework;

	using kCura.EDDS.WebAPI.DocumentManagerBase;
	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;
	using kCura.WinEDDS;
	using kCura.WinEDDS.Container;
	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Export;

	using Moq;

	using Relativity.DataExchange;
	using Relativity.DataExchange.Export.VolumeManagerV2;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Process;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	using FieldType = Relativity.DataExchange.Service.FieldType;
	using Settings = kCura.Relativity.DataReaderClient.Settings;

	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public abstract class ExporterTestBase
	{
		/// <summary>
		/// The dummy UNC path. This should never be used for positive tests.
		/// </summary>
		private const string DummyUncPath = @"\\files\T001\Files\EDDS123456\";

		private static readonly List<int> ImportedDatasets = new List<int>();

		private ExporterTestJobResult exporterTestJobResult;
		private IWindsorContainer testContainer;
		private CookieContainer cookieContainer;
		private NetworkCredential credentials;
		private TempDirectory2 tempDirectory;
		private int selectedFolderId;
		private ExportFile.ExportType exportType;
		private string identifierColumnName;
		private Encoding loadFileEncoding;
		private Encoding textFileEncoding;
		private ExtendedExportFile exportFile;
		private ProcessContext processContext;
		private TestContainerFactory testContainerFactory;

		protected ExporterTestBase(string loadFileEncoding, string textFileEncoding)
		{
			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11
				| SecurityProtocolType.Tls12;

			this.loadFileEncoding = Encoding.GetEncoding(loadFileEncoding);
			this.textFileEncoding = Encoding.GetEncoding(textFileEncoding);
		}

		internal Mock<TapiObjectService> MockTapiObjectService
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

		protected IntegrationTestParameters TestParameters
		{
			get;
			private set;
		}

		[SetUp]
		public void Setup()
		{
			this.testContainer = new WindsorContainer();
			this.testContainerFactory = new TestContainerFactory(this.testContainer);
			this.AssignTestSettings();

			Assume.That(this.TestParameters.WorkspaceId, Is.Positive, "The test workspace must be created or specified in order to run this integration test.");

			this.exporterTestJobResult = new ExporterTestJobResult();
			this.exportFile = null;
			this.selectedFolderId = 0;
			this.exportType = ExportFile.ExportType.AncestorSearch;
			this.cookieContainer = new CookieContainer();
			this.credentials = new NetworkCredential(this.TestParameters.RelativityUserName, this.TestParameters.RelativityPassword);
			this.identifierColumnName = null;
			this.MockExportRequestRetriever = new Mock<IExportRequestRetriever>();
			this.MockFileShareSettingsService = new Mock<IFileShareSettingsService>();
			this.MockProcessEventWriter = new Mock<IProcessEventWriter>();
			this.MockProcessErrorWriter = new Mock<IProcessErrorWriter>();
			this.MockTapiObjectService = MockObjectFactory.CreateMockTapiObjectService();
			this.tempDirectory = new TempDirectory2();
			this.tempDirectory.ClearReadOnlyAttributes = true;
			this.tempDirectory.Create();

			AppSettings.Instance.WebApiServiceUrl = this.TestParameters.RelativityWebApiUrl.ToString();

			// Sanity check since these are global settings.
			AppSettings.Instance.TapiForceAsperaClient = AppSettingsConstants.TapiForceAsperaClientDefaultValue;
			AppSettings.Instance.TapiForceBcpHttpClient = AppSettingsConstants.TapiForceBcpHttpClientDefaultValue;
			AppSettings.Instance.TapiForceClientCandidates = AppSettingsConstants.TapiForceClientCandidatesDefaultValue;
			AppSettings.Instance.TapiForceFileShareClient = AppSettingsConstants.TapiForceFileShareClientDefaultValue;
			AppSettings.Instance.TapiForceHttpClient = AppSettingsConstants.TapiForceHttpClientDefaultValue;

			// This registers all components.
			ContainerFactoryProvider.ContainerFactory = this.testContainerFactory;

			this.processContext = new ProcessContext(
				this.MockProcessEventWriter.Object,
				this.MockProcessErrorWriter.Object,
				AppSettings.Instance,
				new NullLogger());
		}

		[TearDown]
		public void Teardown()
		{
			this.tempDirectory.Dispose();
			this.testContainer.Dispose();
		}

		protected static void GivenTheTapiForceClientAppSettings(TapiClient client)
		{
			// Export relies on the global parameters.
			switch (client)
			{
				case TapiClient.Aspera:
					AppSettings.Instance.TapiForceAsperaClient = true;
					break;

				case TapiClient.Direct:
					AppSettings.Instance.TapiForceFileShareClient = true;
					break;

				case TapiClient.Web:
					AppSettings.Instance.TapiForceHttpClient = true;
					break;

				case TapiClient.None:
					AppSettings.Instance.TapiForceAsperaClient = false;
					AppSettings.Instance.TapiForceFileShareClient = false;
					AppSettings.Instance.TapiForceHttpClient = false;
					break;

				default:
					Assert.Fail($"The Transfer API client {client} isn't supported by this test.");
					break;
			}
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
			var artifact = new ObjectExportInfo
			{
				ArtifactID = artifactId,
				NativeSourceLocation = validNativeSourceLocation ? fileUncPath : null,
				NativeFileGuid =
					validNativeFileGuid ? System.Guid.NewGuid().ToString() : null,
			};
			var request = new PhysicalFileExportRequest(
				artifact,
				validDestinationLocation ? System.IO.Path.Combine(this.tempDirectory.Directory, fileName) : null);
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
			var artifact = new ObjectExportInfo
			{
				ArtifactID = artifactId,
				NativeSourceLocation = validNativeSourceLocation ? fileUncPath : null,
				NativeFileGuid =
					validNativeFileGuid ? System.Guid.NewGuid().ToString() : null,
			};
			var request = new FieldFileExportRequest(
				artifact,
				fileFieldArtifactId,
				validDestinationLocation ? System.IO.Path.Combine(this.tempDirectory.Directory, fileName) : null);
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
			var artifact = new ObjectExportInfo
			{
				ArtifactID = artifactId,
				NativeSourceLocation = validNativeSourceLocation ? fileUncPath : null,
				NativeFileGuid = null,
			};
			LongTextExportRequest request = LongTextExportRequest.CreateRequestForLongText(
				artifact,
				fieldArtifactId,
				validDestinationLocation ? System.IO.Path.Combine(this.tempDirectory.Directory, fileName) : null);
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

		protected void GivenTheFilesAreImported(string folderPath, IEnumerable<string> files)
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

			var importApi = new ImportAPI(
					this.TestParameters.RelativityUserName,
					this.TestParameters.RelativityPassword,
					this.TestParameters.RelativityWebApiUrl.ToString());
			ImportBulkArtifactJob job = importApi.NewNativeDocumentImportJob();
			Settings settings = job.Settings;
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
			using (var dataSource = new DataTable("Input Data"))
			{
				dataSource.Locale = CultureInfo.InvariantCulture;
				dataSource.Columns.Add(WellKnownFields.ControlNumber);
				dataSource.Columns.Add(WellKnownFields.FilePath);
				dataSource.Columns.Add(WellKnownFields.FolderName);
				foreach (var file in files)
				{
					DataRow dr = dataSource.NewRow();
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

		protected void GivenTheExportType(ExportFile.ExportType value)
		{
			this.exportType = value;
		}

		protected void GivenTheIdentifierColumnName(string value)
		{
			this.identifierColumnName = value;
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

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA2201:DoNotRaiseReservedExceptionTypes",
			Justification = "This is used strictly for testing purposes.")]
		protected void GivenTheMockedFileStorageSearchThrows(bool fatal)
		{
			this.MockTapiObjectService.Setup(
					x => x.SearchFileStorageAsync(
						It.IsAny<TapiBridgeParameters2>(),
						It.IsAny<Relativity.Logging.ILog>(),
						It.IsAny<CancellationToken>()))
				.Throws(fatal ? new OutOfMemoryException() as Exception : new InvalidOperationException());
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

		protected CaseInfo WhenGettingTheWorkspaceInfo()
		{
			using (var caseManager = new CaseManager(this.credentials, this.cookieContainer))
			{
				CaseInfo caseInfo = caseManager.Read(this.TestParameters.WorkspaceId);
				return caseInfo;
			}
		}

		protected int WhenGettingTheFolderId(CaseInfo caseInfo, string folderPath)
		{
			if (caseInfo == null)
			{
				throw new ArgumentNullException(nameof(caseInfo));
			}

			using (var folderManager = new FolderManager(this.credentials, this.cookieContainer))
			{
				FolderCache folderCache = new FolderCache(new NullLogger(), folderManager, caseInfo.RootFolderID, caseInfo.ArtifactID);
				int folderArtifactId = folderCache.GetFolderId(folderPath);
				return folderArtifactId;
			}
		}

		protected void WhenCreatingTheExportFile(CaseInfo caseInfo)
		{
			using (var searchManager = new SearchManager(this.credentials, this.cookieContainer))
			using (var productionManager = new ProductionManager(this.credentials, this.cookieContainer))
			{
				this.exportFile = new ExtendedExportFile((int)ArtifactType.Document)
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

				this.exportFile.ObjectTypeName = this.GetObjectTypeName(this.exportFile.ArtifactTypeID);
				switch (this.exportType)
				{
					case ExportFile.ExportType.Production:
						this.exportFile.DataTable = productionManager
							.RetrieveProducedByContextArtifactID(this.TestParameters.WorkspaceId).Tables[0];
						break;

					default:
						this.exportFile.DataTable = this.GetSearchExportDataSource(
							searchManager,
							this.exportType == kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch,
							this.exportFile.ArtifactTypeID);
						break;
				}

				var artifactIds = new List<int>();
				foreach (DataRow row in this.exportFile.DataTable.Rows)
				{
					artifactIds.Add((int)row["ArtifactID"]);
				}

				DocumentFieldCollection fields = this.GetFields(this.exportFile.ArtifactTypeID);
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
					this.exportFile.ArtifactAvfLookup = new HybridDictionary();
					this.exportFile.AllExportableFields = new kCura.WinEDDS.ViewFieldInfo[] { };
				}
				else
				{
					this.exportFile.ArtifactAvfLookup = searchManager.RetrieveDefaultViewFieldsForIdList(
						this.TestParameters.WorkspaceId,
						this.exportFile.ArtifactTypeID,
						artifactIds.ToArray(),
						this.exportType == ExportFile.ExportType.Production);
					this.exportFile.AllExportableFields =
						searchManager.RetrieveAllExportableViewFields(this.TestParameters.WorkspaceId, this.exportFile.ArtifactTypeID);
				}
			}
		}

		protected void WhenExportingTheDocs()
		{
			var exporter = new Exporter(
				this.exportFile,
				this.processContext,
				new WebApiServiceFactory(this.exportFile),
				new ExportFileFormatterFactory(),
				new ExportConfig());
			try
			{
				exporter.StatusMessage += this.ExporterOnStatusMessage;
				exporter.FileTransferMultiClientModeChangeEvent += this.ExporterOnFileTransferModeChangeEvent;
				exporter.FatalErrorEvent += this.ExporterOnFatalErrorEvent;
				this.exporterTestJobResult.SearchResult = exporter.ExportSearch();
			}
			finally
			{
				exporter.StatusMessage -= this.ExporterOnStatusMessage;
				exporter.FileTransferMultiClientModeChangeEvent -= this.ExporterOnFileTransferModeChangeEvent;
				exporter.FatalErrorEvent -= this.ExporterOnFatalErrorEvent;
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

		protected void ThenTheExportJobIsFatal(bool expectedSearchResult)
		{
			// Note: at present, the export returns true even when fatal!
			Assert.That(this.exporterTestJobResult.SearchResult, Is.EqualTo(expectedSearchResult));
			Assert.That(this.exporterTestJobResult.StatusMessages, Has.Count.Positive);
			Assert.That(this.exporterTestJobResult.FatalErrors, Has.Count.EqualTo(1));
		}

		protected void ThenTheExportJobIsNotSuccessful(int expectedDocsProcessed)
		{
			Assert.That(this.exporterTestJobResult.SearchResult, Is.False);
			Assert.That(this.exporterTestJobResult.FatalErrors, Has.Count.Zero);
			Assert.That(this.exporterTestJobResult.StatusMessages, Has.Count.Positive);
			Assert.That(this.exporterTestJobResult.TotalDocumentsProcessed, Is.EqualTo(expectedDocsProcessed));
		}

		protected void ThenTheExportJobIsSuccessful(int expectedDocsProcessed)
		{
			// Assert.That(this.exporterTestJobResult, ExportTestJobConstraint.IsSuccessful(expectedDocsProcessed));
			Assert.That(this.exporterTestJobResult.SearchResult, Is.True);
			Assert.That(this.exporterTestJobResult.AlertCriticalErrors, Has.Count.Zero);
			Assert.That(this.exporterTestJobResult.Alerts, Has.Count.Zero);
			Assert.That(this.exporterTestJobResult.AlertWarningSkippables, Has.Count.Zero);
			Assert.That(this.exporterTestJobResult.FatalErrors, Has.Count.Zero);
			Assert.That(this.exporterTestJobResult.StatusMessages, Has.Count.Positive);
			Assert.That(this.exporterTestJobResult.TotalDocumentsProcessed, Is.EqualTo(expectedDocsProcessed));
			Assert.That(this.exporterTestJobResult.TransferModes, Has.All.AnyOf(TapiClient.Direct, TapiClient.Web));
		}

		private string GetObjectTypeName(int artifactTypeId)
		{
			using (var objectTypeManager = new ObjectTypeManager(this.credentials, this.cookieContainer))
			{
				DataSet dataset = objectTypeManager.RetrieveAllUploadable(this.TestParameters.WorkspaceId);
				DataRowCollection rows = dataset.Tables[0].Rows;
				foreach (DataRow row in rows)
				{
					var item = new ObjectTypeListItem(
						(int)row["DescriptorArtifactTypeID"],
						(string)row["Name"],
						(bool)row["HasAddPermission"]);
					if (item.Value == artifactTypeId)
					{
						return item.Display;
					}
				}

				return string.Empty;
			}
		}

		private DataTable GetSearchExportDataSource(SearchManager searchManager, bool isArtifactSearch, int artifactType)
		{
			DataSet dataset = searchManager.RetrieveViewsByContextArtifactID(
				this.TestParameters.WorkspaceId,
				artifactType,
				isArtifactSearch);
			return dataset.Tables[0];
		}

		private DocumentFieldCollection GetFields(int artifactTypeId)
		{
			var fields = new DocumentFieldCollection();
			using (var fieldQuery = new FieldQuery(this.credentials, this.cookieContainer))
			{
				foreach (Field field in fieldQuery.RetrieveAllAsArray(this.TestParameters.WorkspaceId, artifactTypeId))
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
			lock (this.exporterTestJobResult)
			{
				this.exporterTestJobResult.FatalErrors.Add(message);
			}

			Console.WriteLine($"Fatal errors: {message}");
		}

		private void ExporterOnFileTransferModeChangeEvent(object sender, TapiMultiClientEventArgs e)
		{
			lock (this.exporterTestJobResult)
			{
				this.exporterTestJobResult.TransferModes.Clear();
				this.exporterTestJobResult.TransferModes.AddRange(e.TransferClients);
			}

			Console.WriteLine("Transfer mode changed: {0}", string.Join(",", this.exporterTestJobResult.TransferModes));
		}

		private void ExporterOnStatusMessage(ExportEventArgs args)
		{
			lock (this.exporterTestJobResult)
			{
				this.exporterTestJobResult.TotalDocuments = args.TotalDocuments;
				this.exporterTestJobResult.TotalDocumentsProcessed = args.DocumentsExported;
				if (!string.IsNullOrEmpty(args.Message))
				{
					this.exporterTestJobResult.StatusMessages.Add(args.Message);
				}
			}

			Console.WriteLine(
				$"Status: Message={args.Message}, Total Exported Docs={args.DocumentsExported}, Total Docs={args.TotalDocuments}");
		}
	}
}