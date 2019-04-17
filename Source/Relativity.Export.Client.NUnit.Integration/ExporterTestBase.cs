// -----------------------------------------------------------------------------------------------------
// <copyright file="ExporterTestBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a base class for <see cref="Exporter"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Net;
	using System.Text;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Container;
	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.Service.Export;

	using Moq;

	using Relativity.Export.VolumeManagerV2.Container;
	using Relativity.Import.Export;
	using Relativity.Import.Export.Process;
	using Relativity.Import.Export.Services;
	using Relativity.Import.Export.TestFramework;
	using Relativity.Logging;
	using Relativity.Transfer;

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
		private readonly List<string> alerts = new List<string>();
		private readonly List<string> alertCriticalErrors = new List<string>();
		private readonly List<string> alertWarningSkippables = new List<string>();
		private readonly List<string> statusMessages = new List<string>();
		private readonly List<string> fatalErrors = new List<string>();
		private CookieContainer cookieContainer;
		private NetworkCredential credentials;
		private Mock<IUserNotification> userNotification;
		private TempDirectory tempDirectory;
		private int selectedFolderId;
		private ExportFile.ExportType exportType;
		private string identifierColumnName;
		private Encoding encoding;
		private bool searchResult;
		private Mock<Relativity.Logging.ILog> logger;
		private ExtendedExportFile exportFile;

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
		/// The test setup.
		/// </summary>
		[SetUp]
		public void Setup()
		{
			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11
				| SecurityProtocolType.Tls12;
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
			this.encoding = Encoding.Unicode;
			this.logger = new Mock<ILog>();
			this.searchResult = false;
			this.tempDirectory = new TempDirectory { ClearReadOnlyAttributes = true };
			this.tempDirectory.Create();
			this.userNotification = new Mock<IUserNotification>();
			this.userNotification.Setup(x => x.Alert(It.IsAny<string>())).Callback<string>(msg =>
			{
				this.alerts.Add(msg);
				Console.WriteLine($"Alert: {msg}");
			});
			this.userNotification.Setup(x => x.AlertCriticalError(It.IsAny<string>()))
				.Callback<string>(msg =>
				{
					this.alertCriticalErrors.Add(msg);
					Console.WriteLine($"Alert critical error: {msg}");
				});
			this.userNotification.Setup(x => x.AlertWarningSkippable(It.IsAny<string>()))
				.Callback<string>(msg =>
				{
					this.alertWarningSkippables.Add(msg);
					Console.WriteLine($"Alert warning skipped: {msg}");
				}).Returns(true);
			AppSettings.Instance.WebApiServiceUrl = this.TestParameters.RelativityWebApiUrl.ToString();
		}

		/// <summary>
		/// The test teardown.
		/// </summary>
		[TearDown]
		public void Teardown()
		{
			if (this.tempDirectory != null)
			{
				this.tempDirectory?.Dispose();
				this.tempDirectory = null;
			}
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
		protected void GivenTheFilesAreImported(IEnumerable<string> files)
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
		protected void GivenTheFilesAreImported(string folderPath, IEnumerable<string> files)
		{
			if (files == null)
			{
				throw new ArgumentNullException(nameof(files));
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
		/// Given the encoding.
		/// </summary>
		/// <param name="value">
		/// The encoding.
		/// </param>
		protected void GivenTheEncoding(Encoding value)
		{
			this.encoding = value;
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
				FolderCache folderCache = new FolderCache(this.logger.Object, folderManager, caseInfo.RootFolderID, caseInfo.ArtifactID);
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

					// settings for exporting natives
					ExportNative = true,
					TypeOfExportedFilePath = ExportFile.ExportedFilePathType.Absolute,
					IdentifierColumnName = this.identifierColumnName,
					LoadFileEncoding = this.encoding,
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
					ImagePrecedence = new Pair[]
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
		/// When executing the export search.
		/// </summary>
		protected void WhenExecutingTheExportSearch()
		{
			ContainerFactoryProvider.ContainerFactory = new ContainerFactory();
			var mockProcessEventWriter = new Mock<IProcessEventWriter>();
			var mockProcessErrorWriter = new Mock<IProcessErrorWriter>();
			var mockAppSettings = new Mock<IAppSettings>();
			var mockLog = new Mock<ILog>();
			var processContext = new ProcessContext(
				mockProcessEventWriter.Object,
				mockProcessErrorWriter.Object,
				mockAppSettings.Object,
				mockLog.Object);
			var exporter = new Exporter(
				this.exportFile,
				processContext,
				new WebApiServiceFactory(this.exportFile),
				new ExportFileFormatterFactory(),
				new ExportConfig());
			exporter.StatusMessage += args =>
			{
				if (!string.IsNullOrEmpty(args.Message))
				{
					this.statusMessages.Add(args.Message);
					Console.WriteLine($"Status: {args.Message}");
				}
			};

			exporter.FatalErrorEvent += (message, exception) =>
			{
				this.fatalErrors.Add(message);
				Console.WriteLine($"Fatal errors: {message}");
			};

			this.searchResult = exporter.ExportSearch();
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
	}
}