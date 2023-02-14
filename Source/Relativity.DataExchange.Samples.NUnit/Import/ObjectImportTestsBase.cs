// ----------------------------------------------------------------------------
// <copyright file="ObjectImportTestsBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Samples.NUnit.Import
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;

	/// <summary>
	/// Represents an abstract test class object that imports objects and validates the results.
	/// </summary>
	public abstract class ObjectImportTestsBase : ImportTestsBase
	{
		// The custom object type under test.
		protected const string TransferArtifactTypeName = "Transfer";
		protected const string TransferFieldDescription = "Description";
		protected const string TransferFieldName = "Name";
		protected const string TransferFieldDetailId = "TransferDetailId";
		protected const string TransferFieldDataSourceId = "TransferDataSourceId";
		protected const string TransferFieldRequestBytes = "RequestBytes";
		protected const string TransferFieldRequestFiles = "RequestFiles";
		protected const string TransferFieldRequestDate = "RequestDate";

		// To verify importing a single-object field.
		protected const string TransferDetailArtifactTypeName = "TransferDetail";
		protected const string TransferDetailFieldName = "Name";
		protected const string TransferDetailFieldTransferredBytes = "TransferredBytes";
		protected const string TransferDetailFieldTransferredFiles = "TransferredFiles";
		protected const string TransferDetailFieldStartDate = "StartDate";
		protected const string TransferDetailFieldEndDate = "EndDate";

		// To verify importing a multi-object field.
		protected const string TransferDataSourceArtifactTypeName = "TransferDataSource";
		protected const string TransferDataSourceFieldName = "Name";
		protected const string TransferDataSourceFieldNumber = "Number";
		protected const string TransferDataSourceFieldConnectionString = "ConnectionString";

		private readonly List<int> dataSourceArtifacts = new List<int>();
		private readonly List<int> detailArtifacts = new List<int>();

		protected ObjectImportTestsBase()
			: base(IntegrationTestHelper.Logger)
		{
			// Assume that AssemblySetup has already setup the singleton.
		}

		protected ObjectImportTestsBase(Relativity.Logging.ILog log)
			: base(log)
		{
		}

		/// <summary>
		/// Gets the list of transfer RDO custom field names.
		/// </summary>
		/// <value>
		/// The field names.
		/// </value>
		protected static IReadOnlyList<string> TransferFields =>
			new List<string>
				{
					TransferFieldName,
					TransferFieldDescription,
					TransferFieldRequestBytes,
					TransferFieldRequestFiles,
					TransferFieldRequestDate,
					TransferFieldDetailId,
					TransferFieldDataSourceId,
				};

		/// <summary>
		/// Gets the list of transfer detail RDO custom field names.
		/// </summary>
		/// <value>
		/// The field names.
		/// </value>
		protected static IReadOnlyList<string> TransferDetailFields =>
			new List<string>
				{
					TransferDetailFieldName,
					TransferDetailFieldTransferredBytes,
					TransferDetailFieldTransferredFiles,
					TransferDetailFieldStartDate,
					TransferDetailFieldEndDate,
				};

		/// <summary>
		/// Gets the list of transfer data source RDO custom field names.
		/// </summary>
		/// <value>
		/// The field names.
		/// </value>
		protected static IReadOnlyList<string> TransferDataSourceFields =>
			new List<string>
				{
					TransferDataSourceFieldName,
					TransferDataSourceFieldNumber,
					TransferDataSourceFieldConnectionString,
				};

		protected static int ObjectTypeUniqueSuffix { get; set; }

		protected int TransferArtifactTypeId
		{
			get;
			set;
		}

		protected int TransferDataSourceArtifactTypeId
		{
			get;
			set;
		}

		protected int TransferDataSourceWorkspaceObjectTypeId
		{
			get;
			set;
		}

		protected int TransferDetailArtifactTypeId
		{
			get;
			set;
		}

		protected int TransferDetailWorkspaceObjectTypeId
		{
			get;
			set;
		}

		protected int TransferIdentifierFieldId
		{
			get;
			set;
		}

		protected int TransferWorkspaceObjectTypeId
		{
			get;
			set;
		}

		[OneTimeSetUp]
		public async Task OneTimeSetUpAsync()
		{
			// Create the object types in reverse order.
			this.AssignTestSettings();
			ObjectTypeUniqueSuffix++;
			await this.CreateTransferDetailObjectTypeAsync().ConfigureAwait(false);
			await this.CreateTransferDataSourceObjectTypeAsync().ConfigureAwait(false);
			await this.CreateTransferObjectTypeAsync().ConfigureAwait(false);
			this.OnOneTimeSetup();
		}

		protected void ConfigureJobSettings(
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job,
			int artifactTypeId,
			int identityFieldId,
			string selectedIdentifierFieldName)
		{
			kCura.Relativity.DataReaderClient.Settings settings = job.Settings;
			settings.ArtifactTypeId = artifactTypeId;
			settings.Billable = false;
			settings.BulkLoadFileFieldDelimiter = ";";
			settings.CaseArtifactId = this.TestParameters.WorkspaceId;
			settings.CopyFilesToDocumentRepository = false;
			settings.DisableControlNumberCompatibilityMode = true;
			settings.DisableExtractedTextEncodingCheck = true;
			settings.DisableExtractedTextFileLocationValidation = true;
			settings.DisableNativeLocationValidation = false;
			settings.DisableNativeValidation = false;
			settings.DisableUserSecurityCheck = true;
			settings.ExtractedTextFieldContainsFilePath = false;
			settings.IdentityFieldId = identityFieldId;
			settings.LoadImportedFullTextFromServer = false;
			settings.MaximumErrorCount = int.MaxValue - 1;
			settings.MoveDocumentsInAppendOverlayMode = false;
			settings.MultiValueDelimiter = ';';
			settings.NativeFileCopyMode = kCura.Relativity.DataReaderClient.NativeFileCopyModeEnum.DoNotImportNativeFiles;
			settings.OverwriteMode = kCura.Relativity.DataReaderClient.OverwriteModeEnum.Append;
			settings.SelectedIdentifierFieldName = selectedIdentifierFieldName;
			settings.StartRecordNumber = 0;
		}

		protected void ConfigureJobEvents(kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job)
		{
			job.OnComplete += report =>
			{
				this.PublishedJobReport = report;
				Console.WriteLine("[Job Complete]");
			};

			job.OnError += row =>
			{
				this.PublishedErrors.Add(row);
			};

			job.OnFatalException += report =>
			{
				this.PublishedFatalException = report.FatalException;
				Console.WriteLine("[Job Fatal Exception]: " + report.FatalException);
			};

			job.OnMessage += status =>
			{
				this.PublishedMessages.Add(status.Message);
				Console.WriteLine("[Job Message]: " + status.Message);
			};

			job.OnProcessProgress += status =>
			{
				this.PublishedProcessProgress.Add(status);
			};

			job.OnProgress += row =>
			{
				this.PublishedProgressRows.Add(row);
			};
		}

		protected int CreateAssociatedDetailInstance(string name)
		{
			Dictionary<string, object> transferDetailFieldValues = new Dictionary<string, object>
			{
				{ TransferDetailFieldName, name },
				{ TransferDetailFieldTransferredBytes, RandomHelper.NextDecimal(100000, 1000000) },
				{ TransferDetailFieldTransferredFiles, RandomHelper.NextDecimal(1000, 500000) },
				{ TransferDetailFieldStartDate, DateTime.Now },
				{ TransferDetailFieldEndDate, DateTime.Now.AddDays(3) },
			};

			int artifactId = this.CreateObjectTypeInstance(
				this.TransferDetailArtifactTypeId,
				transferDetailFieldValues);
			this.detailArtifacts.Add(artifactId);
			return artifactId;
		}

		protected int CreateAssociatedDataSourceInstance(string name)
		{
			Dictionary<string, object> transferDataSourceFieldValues = new Dictionary<string, object>
			{
				{ TransferDataSourceFieldName, name },
				{ TransferDataSourceFieldNumber, RandomHelper.NextDecimal(1, 100) },
				{ TransferDataSourceFieldConnectionString, RandomHelper.NextString(50, 450) },
			};

			int artifactId = this.CreateObjectTypeInstance(
				this.TransferDataSourceArtifactTypeId,
				transferDataSourceFieldValues);
			this.dataSourceArtifacts.Add(artifactId);
			return artifactId;
		}

		protected kCura.Relativity.DataReaderClient.ImportBulkArtifactJob CreateImportBulkArtifactJob()
		{
			kCura.Relativity.ImportAPI.ImportAPI importApi = this.CreateImportApiObject();
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job =
				importApi.NewObjectImportJob(this.TransferArtifactTypeId);
			this.ConfigureJobEvents(job);
			this.ConfigureJobSettings(
				job,
				this.TransferArtifactTypeId,
				this.TransferIdentifierFieldId,
				TransferFieldName);
			this.DataSource.Columns.AddRange(new[]
			{
				new DataColumn(TransferFieldName, typeof(string)),
				new DataColumn(TransferFieldDescription, typeof(string)),
				new DataColumn(TransferFieldRequestBytes, typeof(decimal)),
				new DataColumn(TransferFieldRequestFiles, typeof(decimal)),
				new DataColumn(TransferFieldRequestDate, typeof(DateTime)),
				new DataColumn(TransferFieldDetailId, typeof(string)),
				new DataColumn(TransferFieldDataSourceId, typeof(string)),
			});

			return job;
		}

		protected async Task CreateTransferDetailObjectTypeAsync()
		{
			// This is a 1-to-1 relationship.
			string objectType = $"{TransferDetailArtifactTypeName}-{ObjectTypeUniqueSuffix}";
			this.TransferDetailWorkspaceObjectTypeId = await this.CreateObjectTypeAsync(objectType).ConfigureAwait(false);
			await FieldHelper.CreateDecimalFieldAsync(this.TestParameters, this.TransferDetailWorkspaceObjectTypeId, TransferDetailFieldTransferredBytes).ConfigureAwait(false);
			await FieldHelper.CreateDecimalFieldAsync(this.TestParameters, this.TransferDetailWorkspaceObjectTypeId, TransferDetailFieldTransferredFiles).ConfigureAwait(false);
			await FieldHelper.CreateDateFieldAsync(this.TestParameters, this.TransferDetailWorkspaceObjectTypeId, TransferDetailFieldStartDate).ConfigureAwait(false);
			await FieldHelper.CreateDateFieldAsync(this.TestParameters, this.TransferDetailWorkspaceObjectTypeId, TransferDetailFieldEndDate).ConfigureAwait(false);
			this.TransferDetailArtifactTypeId = this.QueryArtifactTypeId(objectType);
		}

		protected async Task CreateTransferDataSourceObjectTypeAsync()
		{
			// This is a many-to-many relationship.
			string objectType = $"{TransferDataSourceArtifactTypeName}-{ObjectTypeUniqueSuffix}";
			this.TransferDataSourceWorkspaceObjectTypeId = await this.CreateObjectTypeAsync(objectType).ConfigureAwait(false);
			await FieldHelper.CreateDecimalFieldAsync(this.TestParameters, this.TransferDataSourceWorkspaceObjectTypeId, TransferDataSourceFieldNumber).ConfigureAwait(false);
			await this.CreateFixedLengthTextFieldAsync(
				this.TransferDataSourceWorkspaceObjectTypeId,
				TransferDataSourceFieldConnectionString,
				450).ConfigureAwait(false);
			this.TransferDataSourceArtifactTypeId = this.QueryArtifactTypeId(objectType);
		}

		protected virtual void OnOneTimeSetup()
		{
		}

		protected override async Task OnTearDownAsync()
		{
			await base.OnTearDownAsync().ConfigureAwait(false);

			// Note: this removes the artifacts from the supplied list.
			await this.DeleteObjectsAsync(this.dataSourceArtifacts).ConfigureAwait(false);
			await this.DeleteObjectsAsync(this.detailArtifacts).ConfigureAwait(false);
		}

		private async Task CreateTransferObjectTypeAsync()
		{
			var objectType = $"{TransferArtifactTypeName}-{ObjectTypeUniqueSuffix}";
			this.TransferWorkspaceObjectTypeId = this.CreateObjectTypeAsync(objectType).Result;
			await this.CreateFixedLengthTextFieldAsync(this.TransferWorkspaceObjectTypeId, TransferFieldDescription, 450).ConfigureAwait(false);
			await FieldHelper.CreateSingleObjectFieldAsync(
				this.TestParameters,
				TransferFieldDetailId,
				objectArtifactTypeId: this.TransferWorkspaceObjectTypeId,
				associativeObjectArtifactTypeId: this.TransferDetailWorkspaceObjectTypeId).ConfigureAwait(false);
			await FieldHelper.CreateDateFieldAsync(
				this.TestParameters,
				this.TransferDetailWorkspaceObjectTypeId,
				TransferFieldDetailId).ConfigureAwait(false);
			await FieldHelper.CreateMultiObjectFieldAsync(
				this.TestParameters,
				TransferFieldDataSourceId,
				objectArtifactTypeId: this.TransferWorkspaceObjectTypeId,
				associativeObjectArtifactTypeId: this.TransferDataSourceWorkspaceObjectTypeId).ConfigureAwait(false);
			await FieldHelper.CreateDecimalFieldAsync(this.TestParameters, this.TransferWorkspaceObjectTypeId, TransferFieldRequestBytes).ConfigureAwait(false);
			await FieldHelper.CreateDecimalFieldAsync(this.TestParameters, this.TransferWorkspaceObjectTypeId, TransferFieldRequestFiles).ConfigureAwait(false);
			await FieldHelper.CreateDateFieldAsync(
				this.TestParameters,
				this.TransferWorkspaceObjectTypeId,
				TransferFieldRequestDate).ConfigureAwait(false);
			this.TransferArtifactTypeId = this.QueryArtifactTypeId(objectType);
			this.TransferIdentifierFieldId = this.QueryIdentifierFieldId(objectType);
		}
	}
}