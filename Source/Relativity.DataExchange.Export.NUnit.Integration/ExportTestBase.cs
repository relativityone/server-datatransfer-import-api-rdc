// -----------------------------------------------------------------------------------------------------
// <copyright file="ExportTestBase.cs" company="Relativity ODA LLC">
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
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Reflection;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using Castle.MicroKernel.Registration;
	using Castle.Windsor;

	using global::NUnit.Framework;

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
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.Transfer;

	using ArtifactType = Relativity.DataExchange.Service.ArtifactType;
	using Field = kCura.EDDS.WebAPI.DocumentManagerBase.Field;
	using FieldType = Relativity.DataExchange.Service.FieldType;
	using File = System.IO.File;

	[TestFixture]
	public abstract class ExportTestBase : IDisposable
	{
		private ExportTestJobResult exporterTestJobResult;
		private WindsorContainer testContainer;
		private CookieContainer cookieContainer;
		private NetworkCredential credentials;
		private CancellationTokenSource cancellationTokenSource;
		private DateTime testExecutionStartTime;

		protected ExportTestBase()
		{
			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11
				| SecurityProtocolType.Tls12;
		}

		internal static int ViewId { get; set; } = 1003684;

		internal Mock<TapiObjectService> MockTapiObjectService { get; private set; }

		protected abstract IntegrationTestParameters TestParameters { get; }

		protected ExtendedExportFile ExtendedExportFile { get; set; }

		protected TempDirectory2 TempDirectory { get; set; }

		protected Mock<IExportRequestRetriever> MockExportRequestRetriever { get; private set; }

		protected Mock<IFileShareSettingsService> MockFileShareSettingsService { get; private set; }

		protected TestNullLogger Logger { get; private set; }

		[SetUp]
		public void Setup()
		{
			TapiClientModeAvailabilityChecker.InitializeTapiClient(this.TestParameters);
			this.Logger = new TestNullLogger();

			// This registers all components.
			this.testContainer = new WindsorContainer();
			ContainerFactoryProvider.ContainerFactory = new TestContainerFactory(this.testContainer, this.Logger);

			this.cookieContainer = new CookieContainer();
			this.credentials = new NetworkCredential(TestParameters.RelativityUserName, TestParameters.RelativityPassword);

			this.TempDirectory = new TempDirectory2();
			this.TempDirectory.Create();

			this.ExtendedExportFile = new ExtendedExportFile((int)ArtifactType.Document)
			{
				CookieContainer = this.cookieContainer,
				Credential = this.credentials,

				TypeOfExport = ExportFile.ExportType.ParentSearch,
				FolderPath = this.TempDirectory.Directory,
				TextFileEncoding = Encoding.UTF8,

				// settings for exporting natives
				ExportNative = true,
				TypeOfExportedFilePath = kCura.WinEDDS.ExportFile.ExportedFilePathType.Absolute,

				IdentifierColumnName = WellKnownFields.ControlNumber,
				LoadFileEncoding = Encoding.UTF8,
				LoadFilesPrefix = "Documents",
				LoadFileExtension = "dat",
				MultiRecordDelimiter = ';',
				NestedValueDelimiter = '\\',
				NewlineDelimiter = '@',
				QuoteDelimiter = 'þ',
				RecordDelimiter = '¶',
				ViewID = ViewId,
				SelectedViewFields = new kCura.WinEDDS.ViewFieldInfo[] { },

				// settings for exporting images
				ExportImages = true,
				LogFileFormat = LoadFileType.FileFormat.Opticon,
				TypeOfImage = kCura.WinEDDS.ExportFile.ImageType.Pdf,
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

			this.MockExportRequestRetriever = new Mock<IExportRequestRetriever>();
			this.MockFileShareSettingsService = new Mock<IFileShareSettingsService>();
			this.MockTapiObjectService = MockObjectFactory.CreateMockTapiObjectService();

			this.cancellationTokenSource = new CancellationTokenSource();
			this.exporterTestJobResult = new ExportTestJobResult();

			AppSettingsManager.Default(AppSettings.Instance);
		}

		[TearDown]
		public void Teardown()
		{
			if (this.TempDirectory != null)
			{
				this.TempDirectory.ClearReadOnlyAttributes = true;
				this.TempDirectory.Dispose();
				this.TempDirectory = null;
			}

			if (this.testContainer != null)
			{
				this.testContainer.Dispose();
				this.testContainer = null;
			}

			if (this.cancellationTokenSource != null)
			{
				this.cancellationTokenSource.Dispose();
				this.cancellationTokenSource = null;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
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

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Teardown();
			}
		}

		protected void GivenTheMockTapiObjectServiceIsRegistered()
		{
			this.testContainer.Register(Component
				.For<ITapiObjectService>()
				.Instance(this.MockTapiObjectService.Object)
				.IsDefault());
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
				.Throws(fatal ? new DivideByZeroException() as Exception : new InvalidOperationException());
		}

		protected void GivenTheMockedExportRequestRetrieverReturns(List<ExportRequest> fileExportRequests, List<LongTextExportRequest> longTextExportRequests)
		{
			this.MockExportRequestRetriever.Setup(x => x.RetrieveFileExportRequests()).Returns(fileExportRequests);
			this.MockExportRequestRetriever.Setup(x => x.RetrieveLongTextExportRequests()).Returns(longTextExportRequests);

			this.testContainer.Register(Component
				.For<IExportRequestRetriever>()
				.Instance(this.MockExportRequestRetriever.Object)
				.IsDefault());
		}

		protected void GivenTheMockedSettingsForFileShareIsNull()
		{
			this.MockFileShareSettingsService.Setup(x => x.GetSettingsForFileShare(It.IsAny<int>(), It.IsAny<string>()))
				.Returns((IRelativityFileShareSettings)null);
			this.MockFileShareSettingsService.Setup(x => x.ReadFileSharesAsync(It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			this.testContainer.Register(Component
				.For<IFileShareSettingsService>()
				.Instance(this.MockFileShareSettingsService.Object)
				.IsDefault());
		}

		protected int WhenGettingTheFolderId(CaseInfo caseInfo, string folderPath)
		{
			if (caseInfo == null)
			{
				throw new ArgumentNullException(nameof(caseInfo));
			}

			using (var folderManager = new FolderManager(this.credentials, this.cookieContainer))
			{
				FolderCache folderCache = new FolderCache(this.Logger, folderManager, caseInfo.RootFolderID, caseInfo.ArtifactID);
				int folderArtifactId = folderCache.GetFolderId(folderPath);
				return folderArtifactId;
			}
		}

		protected void WhenCreatingTheExportFile()
		{
			using (var caseManager = new CaseManager(this.credentials, this.cookieContainer))
			{
				CaseInfo caseInfo = caseManager.Read(TestParameters.WorkspaceId);
				this.ExtendedExportFile.CaseInfo = caseInfo;
				this.ExtendedExportFile.ArtifactID = caseInfo.RootFolderID;
			}

			using (var searchManager = new SearchManager(this.credentials, this.cookieContainer))
			using (var productionManager = new ProductionManager(this.credentials, this.cookieContainer))
			{
				this.ExtendedExportFile.ObjectTypeName = this.GetObjectTypeName(this.ExtendedExportFile.ArtifactTypeID);
				switch (this.ExtendedExportFile.TypeOfExport)
				{
					case kCura.WinEDDS.ExportFile.ExportType.Production:
						this.ExtendedExportFile.DataTable = productionManager
							.RetrieveProducedByContextArtifactID(TestParameters.WorkspaceId).Tables[0];
						break;

					default:
						this.ExtendedExportFile.DataTable = this.GetSearchExportDataSource(
							searchManager,
							this.ExtendedExportFile.TypeOfExport == kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch,
							this.ExtendedExportFile.ArtifactTypeID);
						break;
				}

				int[] artifactIds = this.ExtendedExportFile.DataTable.Rows
					.Cast<DataRow>()
					.Select(row => (int)row["ArtifactID"])
					.ToArray();

				this.ExtendedExportFile.FileField = this.GetFields(this.ExtendedExportFile.ArtifactTypeID)
					.FirstOrDefault(p => p.FieldTypeID == (int)FieldType.File);

				if (artifactIds.Length == 0)
				{
					this.ExtendedExportFile.ArtifactAvfLookup = new HybridDictionary();
					this.ExtendedExportFile.AllExportableFields = new kCura.WinEDDS.ViewFieldInfo[] { };
				}
				else
				{
					this.ExtendedExportFile.ArtifactAvfLookup = searchManager.RetrieveDefaultViewFieldsForIdList(
						TestParameters.WorkspaceId,
						this.ExtendedExportFile.ArtifactTypeID,
						artifactIds,
						this.ExtendedExportFile.TypeOfExport == kCura.WinEDDS.ExportFile.ExportType.Production);
					this.ExtendedExportFile.AllExportableFields =
						searchManager.RetrieveAllExportableViewFields(TestParameters.WorkspaceId, this.ExtendedExportFile.ArtifactTypeID);
				}
			}
		}

		protected void WhenExportingTheDocs()
		{
			var exporter = new Exporter(
				this.ExtendedExportFile,
				new ProcessContext(),
				new WebApiServiceFactory(this.ExtendedExportFile),
				new ExportFileFormatterFactory(),
				new ExportConfig(),
				this.Logger,
				this.cancellationTokenSource.Token);
			try
			{
				exporter.InteractionManager = new EventBackedUserNotification(exporter);
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

		protected void ExecuteFolderAndSubfoldersAndVerify()
		{
			this.testExecutionStartTime = DateTime.Now;
			this.WhenCreatingTheExportFile();
			this.WhenExportingTheDocs();
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
			Assert.That(this.exporterTestJobResult.SearchResult, Is.True);
			Assert.That(this.exporterTestJobResult.AlertCriticalErrors, Has.Count.Zero);
			Assert.That(this.exporterTestJobResult.Alerts, Has.Count.Zero);
			Assert.That(this.exporterTestJobResult.AlertWarningSkippables, Has.Count.Zero);
			Assert.That(this.exporterTestJobResult.FatalErrors, Has.Count.Zero);
			Assert.That(this.exporterTestJobResult.StatusMessages, Has.Count.Positive);
			Assert.That(this.exporterTestJobResult.TotalDocumentsProcessed, Is.EqualTo(expectedDocsProcessed));
			Assert.That(this.exporterTestJobResult.TransferModes, Has.All.AnyOf(TapiClient.Direct, TapiClient.Web, TapiClient.Aspera));
		}

		protected void ThenTheExportJobIsNotSuccessful(int expectedDocsProcessed, int expectedErrorsCount)
		{
			Assert.That(this.exporterTestJobResult.TotalDocumentsProcessed, Is.EqualTo(expectedDocsProcessed));
			Assert.That(this.exporterTestJobResult.ErrorMessages, Has.Count.EqualTo(expectedErrorsCount));
			Assert.That(this.exporterTestJobResult.SearchResult, Is.False);
			Assert.That(this.exporterTestJobResult.AlertCriticalErrors, Has.Count.Zero);
			Assert.That(this.exporterTestJobResult.Alerts, Has.Count.Zero);
			Assert.That(this.exporterTestJobResult.AlertWarningSkippables, Has.Count.Zero);
			Assert.That(this.exporterTestJobResult.FatalErrors, Has.Count.Zero);
			Assert.That(this.exporterTestJobResult.StatusMessages, Has.Count.Positive);
			Assert.That(this.exporterTestJobResult.TransferModes, Has.All.AnyOf(TapiClient.Direct, TapiClient.Web));
		}

		protected void ThenTheFilesAreExported(int expectedExportedFilesCount)
		{
			int exportedFilesCount = Directory.GetFiles(ExtendedExportFile.FolderPath, "*.*", SearchOption.AllDirectories).Length;
			Assert.That(exportedFilesCount, Is.EqualTo(expectedExportedFilesCount));
		}

		protected Task ThenTheExportedDocumentLoadFileIsAsExpectedAsync(string pathToExportedFile, string expectedFileContent)
		{
			// Validate that content is the same as in stored template
			return ExportedFilesValidator.ValidateFileStringContentAsync(pathToExportedFile, expectedFileContent, this.ExtendedExportFile.LoadFileExtension, this.ExtendedExportFile.LoadFileEncoding);
		}

		protected Task ThenTheExportedDocumentLoadFileIsAsExpectedAsync()
		{
			if (this.ExtendedExportFile.ExportNative)
			{
				return ThenTheExportedDocumentLoadFileIsAsExpectedAsync(
					GetPathToExportedDocumentLoadFile(),
					GenerateExpectedDocumentLoadFileContent());
			}

			return ThenTheExportedDocumentLoadFileIsEmptyAsync();
		}

		protected async Task ThenTheExportedImageLoadFileIsAsExpectedAsync()
		{
			string fileExtension = this.GetImageLoadFileExtension();
			string pathToExportedFile = GetPathToExportedImageLoadFile();

			if (this.ExtendedExportFile.ExportImages)
			{
				await ExportedFilesValidator.ValidateFileStringContentAsync(pathToExportedFile, GenerateExpectedImageLoadFileContent(), fileExtension).ConfigureAwait(false);
			}
			else
			{
				Assert.That(!File.Exists(pathToExportedFile), $"Image load file was exported while it shouldn't");
			}
		}

		protected void ThenTheStatusMessageWasAdded(string expectedMessage)
		{
			Assert.That(this.exporterTestJobResult.StatusMessages.Where((actualMessage) => actualMessage.Contains(expectedMessage)), Is.Not.Empty);
		}

		/// <summary>
		/// Verifies audit for export operation.
		/// This method might be expensive due to the wait and retry policy in <see cref="AuditHelper"/>.
		/// </summary>
		/// <param name="userId">ID of user who performed the export.</param>
		/// <returns>Task.</returns>
		protected async Task ThenTheAuditIsCorrectAsync(int userId)
		{
			int nrOfLastAuditsToTake = 1;
			var auditsDetails = await AuditHelper.GetLastAuditDetailsForActionAsync(this.TestParameters, AuditHelper.AuditAction.Export, this.testExecutionStartTime, nrOfLastAuditsToTake, userId)
				                    .ConfigureAwait(false);
			var expectedAuditDetails = ToAuditDetails(this.ExtendedExportFile);

			var actualAuditDetails = auditsDetails.First();

			foreach (string key in expectedAuditDetails.Keys)
			{
				Assert.AreEqual(expectedAuditDetails[key], actualAuditDetails[key], $"Audit verification failed for field '{key}'");
			}
		}

		private static Dictionary<string, string> ToAuditDetails(ExtendedExportFile exportFile)
		{
			string ToYesNo(bool value)
			{
				return value ? "Yes" : "No";
			}

			return new Dictionary<string, string>
			{
				{ "Export Images", ToYesNo(exportFile.ExportImages) },
				{ "Export Native Files", ToYesNo(exportFile.ExportNative) },
				{ "Export PDF Files", ToYesNo(exportFile.ExportPdf) },
				{ "Overwrite Files", ToYesNo(exportFile.Overwrite) },
				{ "Append Original Filenames", ToYesNo(exportFile.AppendOriginalFileName) },
				{ "Volume Prefix", exportFile.VolumeInfo.VolumePrefix },
				{ "Volume Start Number", exportFile.VolumeInfo.VolumeStartNumber.ToString() },
				{ "Volume Max Size", exportFile.VolumeInfo.VolumeMaxSize.ToString() },
				{ "Subdirectory Max Files", exportFile.VolumeInfo.SubdirectoryMaxSize.ToString() },
				{ "Subdirectory Start Number", exportFile.VolumeInfo.SubdirectoryStartNumber.ToString() },
				{ "Subdirectory Native Prefix", exportFile.VolumeInfo.get_SubdirectoryNativePrefix(false) },
				{ "Subdirectory Text Prefix", exportFile.VolumeInfo.get_SubdirectoryFullTextPrefix(false) },
				{ "Subdirectory PDF Prefix", exportFile.VolumeInfo.get_SubdirectoryPdfPrefix(false) },
				{ "Subdirectory Image Prefix", exportFile.VolumeInfo.get_SubdirectoryImagePrefix(false) },
			};
		}

		private static string GetPathToTemplatesFolder()
		{
			string parentFolder = new System.IO.FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent.Parent.FullName;
			string folderPath = Path.Combine(parentFolder, "Relativity.DataExchange.TestFramework", "Resources", "TestResultsTemplates");
			return folderPath;
		}

		private static string GetPathToDocumentLoadFileTemplate()
		{
			return Path.Combine(GetPathToTemplatesFolder(), "Documents_export_template.txt");
		}

		private Task ThenTheExportedDocumentLoadFileIsEmptyAsync()
		{
			return ExportedFilesValidator.ValidateFileStringContentAsync(GetPathToExportedDocumentLoadFile(), string.Empty);
		}

		private DataTable GetSearchExportDataSource(SearchManager searchManager, bool isArtifactSearch, int artifactType)
		{
			DataSet dataset = searchManager.RetrieveViewsByContextArtifactID(
				TestParameters.WorkspaceId,
				artifactType,
				isArtifactSearch);
			return dataset.Tables[0];
		}

		private string GetObjectTypeName(int artifactTypeId)
		{
			using (var objectTypeManager = new ObjectTypeManager(this.credentials, this.cookieContainer))
			{
				DataSet dataset = objectTypeManager.RetrieveAllUploadable(TestParameters.WorkspaceId);
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

		private DocumentFieldCollection GetFields(int artifactTypeId)
		{
			var fields = new DocumentFieldCollection();
			using (var fieldQuery = new FieldQuery(this.credentials, this.cookieContainer))
			{
				foreach (Field field in fieldQuery.RetrieveAllAsArray(TestParameters.WorkspaceId, artifactTypeId))
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

					if (args.EventType == EventType2.Error)
					{
						this.exporterTestJobResult.ErrorMessages.Add(args.Message);
					}
				}
			}

			Console.WriteLine(
				$"Status: Message={args.Message}, Total Exported Docs={args.DocumentsExported}, Total Docs={args.TotalDocuments}");
		}

		private string GetPathToExportedDocumentLoadFile()
		{
			return Path.Combine(
				this.ExtendedExportFile.FolderPath,
				this.ExtendedExportFile.LoadFilesPrefix + "_export." + this.ExtendedExportFile.LoadFileExtension);
		}

		private string GetImageLoadFileExtension()
		{
			string fileExtension = string.Empty;
			switch (this.ExtendedExportFile.LogFileFormat)
			{
				case LoadFileType.FileFormat.Opticon:
					fileExtension = "opt";
					break;
				case LoadFileType.FileFormat.IPRO:
					fileExtension = "lfp";
					break;
				case LoadFileType.FileFormat.IPRO_FullText:
					fileExtension = "lfp";
					break;
			}

			return fileExtension;
		}

		private string GetPathToExportedImageLoadFile()
		{
			string fileName = ExtendedExportFile.LogFileFormat == LoadFileType.FileFormat.IPRO_FullText ? "Documents_export_FULLTEXT_" : "Documents_export";
			string exportedFile = Path.Combine(this.ExtendedExportFile.FolderPath, fileName + "." + GetImageLoadFileExtension());
			return exportedFile;
		}

		private string GetPathToImageLoadFileTemplate()
		{
			string fileTemplate = string.Empty;

			switch (this.ExtendedExportFile.LogFileFormat)
			{
				case LoadFileType.FileFormat.Opticon:
					switch (this.ExtendedExportFile.TypeOfImage)
					{
						case ExportFile.ImageType.SinglePage:
							fileTemplate = "Images_export_template_SinglePage.opt";
							break;
						case ExportFile.ImageType.MultiPageTiff:
							fileTemplate = "Images_export_template_MultiPageTiff.opt";
							break;
						case ExportFile.ImageType.Pdf:
							fileTemplate = "Images_export_template_Pdf.opt";
							break;
					}

					break;
				case LoadFileType.FileFormat.IPRO:
					fileTemplate = "Images_export_MultiPageTiff.lfp";
					break;
				case LoadFileType.FileFormat.IPRO_FullText:
					switch (this.ExtendedExportFile.TypeOfImage)
					{
						case ExportFile.ImageType.SinglePage:
							fileTemplate = "Images_export_FULLTEXT_SinglePage.lfp";
							break;
						case ExportFile.ImageType.Pdf:
							fileTemplate = "Images_export_FULLTEXT_pdf.lfp";
							break;
						default:
							throw new NotImplementedException(
								$"No template file for '{this.ExtendedExportFile.TypeOfImage.ToString()}'");
					}

					break;
			}

			return Path.Combine(GetPathToTemplatesFolder(), fileTemplate);
		}

		private string GenerateExpectedDocumentLoadFileContent()
		{
			string fileTemplate = GetPathToDocumentLoadFileTemplate();
			string volumeValue = GetVolumeValue();
			string folderPath = GetFolderPathValue();
			string subDirectory = GetSubDirectoryValue();

			string expectedContent = System.IO.File.ReadAllText(fileTemplate).Replace("<NewLineDelimeter>", string.Empty).Replace("<RecordDelimeter>", this.ExtendedExportFile.RecordDelimiter.ToString()).Replace("<QuoteDelimeter>", this.ExtendedExportFile.QuoteDelimiter.ToString()).Replace("<FolderPath>", folderPath).Replace("<Volume>", volumeValue).Replace("<SubDir>", subDirectory);
			return expectedContent;
		}

		private string GenerateExpectedImageLoadFileContent()
		{
			string fileTemplate = GetPathToImageLoadFileTemplate();
			string volumeValue = GetVolumeValue();
			string folderPath = GetFolderPathValue();
			string subDirectory = GetSubDirectoryValue();
			string imageType = this.ExtendedExportFile.TypeOfImage.ToString();

			string expectedFileContent = System.IO.File.ReadAllText(fileTemplate).Replace("<Volume>", volumeValue).Replace("<FolderPath>", folderPath).Replace("<SubDir>", subDirectory).Replace("<ImageType>", imageType);
			return expectedFileContent;
		}

		private string GetVolumeValue()
		{
			string padVolStart = this.ExtendedExportFile.VolumeInfo.VolumeStartNumber
				.ToString()
				.PadLeft(this.ExtendedExportFile.VolumeDigitPadding, '0');

			return $"{this.ExtendedExportFile.VolumeInfo.VolumePrefix}{padVolStart}";
		}

		private string GetFolderPathValue()
		{
			string folderPath = string.Empty;

			switch (this.ExtendedExportFile.TypeOfExportedFilePath)
			{
				case kCura.WinEDDS.ExportFile.ExportedFilePathType.Relative:
					folderPath = ".";
					break;
				case kCura.WinEDDS.ExportFile.ExportedFilePathType.Absolute:
					folderPath = this.ExtendedExportFile.FolderPath.Substring(
						0,
						this.ExtendedExportFile.FolderPath.Length - 1);
					break;
				case ExportFile.ExportedFilePathType.Prefix:
					folderPath = this.ExtendedExportFile.FilePrefix;
					break;
			}

			return folderPath;
		}

		private string GetSubDirectoryValue()
		{
			string subDirectory = this.ExtendedExportFile.VolumeInfo.SubdirectoryStartNumber
				.ToString()
				.PadLeft(this.ExtendedExportFile.SubdirectoryDigitPadding, '0');
			return subDirectory;
		}
	}
}