// <copyright file="KeplerBulkImportManagerBase.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Integration.Service.BulkImport
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.EDDS.WebAPI.BulkImportManagerBase;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.Services.Objects.DataContracts;

	using ErrorFileKey = Relativity.DataExchange.Service.ErrorFileKey;

	public class KeplerBulkImportManagerBase : KeplerServiceTestBase
	{
		protected const int NumberOfElements = 10;
		protected const string FieldDelimiter = "þþKþþ";
		protected const int DefaultFieldLength = 255;
		protected const int DocumentObjectTypeId = (int)ArtifactType.Document;
		private const string ImageFieldDelimiter = ",";

		public KeplerBulkImportManagerBase(bool useKepler)
			: base(useKepler)
		{
		}

		protected RelativityObject DocumentIdentifier { get; private set; }

		protected string DefaultFileRepository { get; private set; }

		protected string BcpPath { get; private set; }

		[OneTimeSetUp]
		public async Task OneTimeSetupAsync()
		{
			this.DefaultFileRepository = await WorkspaceHelper.GetDefaultFileRepositoryAsync(this.TestParameters).ConfigureAwait(false);
			this.DocumentIdentifier = await FieldHelper.QueryIdentifierRelativityObjectAsync(
				                          this.TestParameters,
				                          WellKnownArtifactTypes.DocumentArtifactTypeName).ConfigureAwait(false);
		}

		[SetUp]
		public async Task SetupAsync()
		{
			await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, DocumentObjectTypeId).ConfigureAwait(false);
		}

		[TearDown]
		public async Task TearDownAsync()
		{
			await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, DocumentObjectTypeId).ConfigureAwait(false);
		}

		[SetUp]
		public new void Setup()
		{
			this.BcpPath = BcpFileHelper.GetBcpPath(
				this.TestParameters,
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc);
		}

		protected static void ThenTheFieldsHaveCorrectValues(DataTable expected, IList<RelativityObject> actualObjects)
		{
			_ = actualObjects ?? throw new ArgumentNullException(nameof(actualObjects));
			_ = expected ?? throw new ArgumentNullException(nameof(expected));

			var rows = expected.Rows.Cast<DataRow>().ToArray();

			foreach (var relativityObject in actualObjects)
			{
				var controlNumber =
					relativityObject.FieldValues.SingleOrDefault(x => x.Field.Name == WellKnownFields.ControlNumber);
				Assert.That(controlNumber, Is.Not.Null);
				var actualDictionary =
					relativityObject.FieldValues.ToDictionary(x => x.Field.Name, y => y.Value.ToString());

				var expectedRow = rows.SingleOrDefault(x => x[RemoveWhitespace(WellKnownFields.ControlNumber)].ToString() == controlNumber.Value.ToString());
				Assert.That(expectedRow, Is.Not.Null);
				ThenTheRowHasCorrectValues(actualDictionary.Keys, expectedRow, actualDictionary);
			}
		}

		protected static string GetLoadFileContent(DataTable fieldValues, bool isNative, int parentFolderId)
		{
			if (fieldValues == null)
			{
				throw new ArgumentNullException(nameof(fieldValues));
			}

			StringBuilder metadataBuilder = new StringBuilder();
			var parentFolderPath = isNative ? FieldDelimiter : string.Empty; // kCura_Import_ParentFolderPath

			for (int i = 0; i < fieldValues.Rows.Count; i++)
			{
				string values = string.Join(FieldDelimiter, fieldValues.Rows[i].ItemArray.Select(item => item.ToString()));
				metadataBuilder
					.Append("0").Append(FieldDelimiter) // kCura_Import_ID
					.Append("0").Append(FieldDelimiter) // kCura_Import_Status
					.Append("0").Append(FieldDelimiter) // kCura_Import_IsNew
					.Append("0").Append(FieldDelimiter) // ArtifactID
					.Append(i).Append(FieldDelimiter) // kCura_Import_OriginalLineNumber
					.Append(FieldDelimiter) // kCura_Import_FileGuid
					.Append(FieldDelimiter) // kCura_Import_Filename
					.Append(FieldDelimiter) // kCura_Import_Location
					.Append(FieldDelimiter) // kCura_Import_OriginalFileLocation
					.Append("0").Append(FieldDelimiter) // kCura_Import_FileSize
					.Append(parentFolderId).Append(FieldDelimiter) // kCura_Import_ParentFolderID - WorkspaceRootFolderId for native, WorkspaceArtifactId for objects
					.Append(values).Append(FieldDelimiter) // ControlNumber, field1, field2, field3 .......
					.Append(parentFolderPath) // kCura_Import_ParentFolderPath - not used in ObjectsLoad
					.Append(FieldDelimiter) // kCura_Import_DataGridException
					.Append(FieldDelimiter) // kCura_Import_ErrorData
					.Append(Environment.NewLine);
			}

			return metadataBuilder.ToString();
		}

		protected static void AssertThereIsNoDocumentProcessedAndNoErrors(
			MassImportResults result,
			bool hasErrors,
			ErrorFileKey errorFileKey)
		{
			_ = result ?? throw new ArgumentNullException(nameof(result));
			_ = errorFileKey ?? throw new ArgumentNullException(nameof(errorFileKey));

			Assert.That(
				result.ExceptionDetail,
				Is.Null,
				$"An error occurred when running import: {result.ExceptionDetail?.ExceptionMessage}");
			Assert.That(result.ArtifactsCreated, Is.EqualTo(0));
			Assert.That(result.ArtifactsUpdated, Is.EqualTo(0));
			Assert.That(result.FilesProcessed, Is.EqualTo(0));

			Assert.That(hasErrors, Is.False);

			Assert.That(errorFileKey, Is.Not.Null);
			Assert.That(errorFileKey.LogKey, Is.EqualTo(string.Empty));
			Assert.That(errorFileKey.OpticonKey, Is.Null);
		}

		protected static string GetImageFileContent(List<Dictionary<string, string>> fields)
		{
			_ = fields ?? throw new ArgumentNullException(nameof(fields));

			StringBuilder metadataBuilder = new StringBuilder();
			for (int i = 0; i < fields.Count; i++)
			{
				var doc = fields[i];
				metadataBuilder
					.Append("1").Append(ImageFieldDelimiter)
					.Append("0").Append(ImageFieldDelimiter)
					.Append("0").Append(ImageFieldDelimiter) // Status
					.Append("0").Append(ImageFieldDelimiter) // IsNew
					.Append(i).Append(ImageFieldDelimiter) // OriginalLineNumber
					.Append(doc["DocumentIdentifier"]).Append(ImageFieldDelimiter) // DocumentIdentifier
					.Append(doc["FileIdentifier"]).Append(ImageFieldDelimiter) // FileIdentifier
					.Append(doc["Guid"]).Append(ImageFieldDelimiter) // Guid
					.Append(doc["Filename"]).Append(ImageFieldDelimiter) // Filename
					.Append("0").Append(ImageFieldDelimiter) // Order
					.Append("0").Append(ImageFieldDelimiter) // Offset
					.Append(doc["Filesize"]).Append(ImageFieldDelimiter) // Filesize
					.Append(doc["Location"]).Append(ImageFieldDelimiter) // Location
					.Append(doc["OriginalFileLocation"]).Append(ImageFieldDelimiter) // OriginalFileLocation
					.Append(ImageFieldDelimiter)
					.Append(FieldDelimiter)
					.Append(Environment.NewLine);
			}

			return metadataBuilder.ToString();
		}

		protected List<Dictionary<string, string>> GetFieldsForImages(int numberOfElements)
		{
			var list = new List<Dictionary<string, string>>(numberOfElements);
			var repository = Path.Combine(
				DefaultFileRepository,
				$"EDDS{this.TestParameters.WorkspaceId}",
				$"RV_{Guid.NewGuid()}");
			for (int i = 1; i <= numberOfElements; i++)
			{
				var docId = $"AZIPPER_000{i}";
				var docGuid = Guid.NewGuid().ToString();
				var documentInfo = new Dictionary<string, string>()
					                   {
						                   { "DocumentIdentifier", docId },
						                   { "FileIdentifier", docId },
						                   { "Guid", docGuid },
						                   { "Filename", $"{docId}.tif" },
						                   { "Filesize", "1234" },
						                   { "Location", Path.Combine(repository, docGuid) },
						                   { "OriginalFileLocation", $@".\VOL001\IMAGES\IMG001\{docId}.tif" },
					                   };

				list.Add(documentInfo);
			}

			return list;
		}

		protected async Task<kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo> GetImageLoadInfoAsync(List<Dictionary<string, string>> fields)
		{
			string loadFileContent = GetImageFileContent(fields);

			return new kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo
			{
					       AuditLevel = kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel.FullAudit,
					       Billable = true,
					       BulkFileName = await BcpFileHelper.CreateAsync(this.TestParameters, loadFileContent, this.BcpPath).ConfigureAwait(false),
					       DataGridFileName = await BcpFileHelper.CreateEmptyAsync(this.TestParameters, BcpPath).ConfigureAwait(false),
					       DestinationFolderArtifactID = WorkspaceRootFolderId,
					       DisableUserSecurityCheck = false,
					       ExecutionSource = kCura.EDDS.WebAPI.BulkImportManagerBase.ExecutionSource.ImportAPI,
					       KeyFieldArtifactID = this.DocumentIdentifier.ArtifactID,
					       Overlay = kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Append,
					       OverlayArtifactID = this.DocumentIdentifier.ArtifactID,
					       Repository = DefaultFileRepository,
					       RunID = Guid.NewGuid().ToString().Replace('-', '_'),
					       UploadFullText = false,
					       UseBulkDataImport = true,
			};
		}

		protected async Task AssertDocuments(int numberOfElements, List<Dictionary<string, string>> fieldsInfo)
		{
			var objects = await RdoHelper.QueryRelativityObjectsAsync(
				              this.TestParameters,
				              DocumentObjectTypeId,
				              new[] { WellKnownFields.ControlNumber }).ConfigureAwait(false);
			Assert.That(objects.Count, Is.EqualTo(numberOfElements));
			var actualControlNumbers = objects.Select(
				x => x.FieldValues.Single(y => y.Field.Name == WellKnownFields.ControlNumber).Value.ToString());
			var expectedControlNumbers = fieldsInfo.Select(x => x["DocumentIdentifier"]);

			Assert.That(expectedControlNumbers, Is.EquivalentTo(actualControlNumbers));
		}

		private static void ThenTheRowHasCorrectValues(IEnumerable<string> columnNames, DataRow expected, Dictionary<string, string> actual)
		{
			foreach (string columnName in columnNames)
			{
				Assert.That(actual.ContainsKey(columnName), $"Actual values dictionary does not contains key: {columnName}. Keys: {string.Join(",", actual.Keys)}");

				string actualValue = actual[columnName];
				string expectedValue = expected[RemoveWhitespace(columnName)].ToString();

				if (bool.TryParse(actualValue, out var boolValue))
				{
					actualValue = boolValue ? "1" : "0";
				}

				Assert.AreEqual(expectedValue, actualValue, $"Incorrect value in {columnName} field");
			}
		}

		private static string RemoveWhitespace(string input)
		{
			return new string(input.ToCharArray()
				.Where(c => !char.IsWhiteSpace(c))
				.ToArray());
		}
	}
}
