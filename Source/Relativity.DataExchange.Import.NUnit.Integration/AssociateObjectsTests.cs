// ----------------------------------------------------------------------------
// <copyright file="AssociateObjectsTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using global::NUnit.Framework;
	using kCura.Relativity.DataReaderClient;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.Services.Interfaces.ObjectType;
	using Relativity.Services.Interfaces.ObjectType.Models;
	using Relativity.Services.Interfaces.Shared;
	using Relativity.Services.Interfaces.Shared.Models;
	using Relativity.Services.Objects;
	using Relativity.Services.Objects.DataContracts;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class AssociateObjectsTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		private const string ReferenceToObjectFieldName = "ReferenceToObject";
		private const string ReferenceToDocumentFieldName = "ReferenceToDocument";

		/// <summary>
		/// Test case can change import batch size, so we have to revert it to the initial value after each test.
		/// </summary>
		private int initialImportBatchSize;

		private int objectArtifactTypeId;
		private int childObjectArtifactTypeId;
		private int referenceToObjectFieldId;

		[OneTimeSetUp]
		public async Task OneTimeSetupAsync()
		{
			this.objectArtifactTypeId = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, $"{nameof(AssociateObjectsTests)}-Object")
				.ConfigureAwait(false);
			this.childObjectArtifactTypeId = await this.CreateChildObjectTypeAsync($"{nameof(AssociateObjectsTests)}-ChildObject", this.objectArtifactTypeId)
				.ConfigureAwait(false);
			this.referenceToObjectFieldId = await FieldHelper.CreateSingleObjectFieldAsync(this.TestParameters, ReferenceToObjectFieldName, this.objectArtifactTypeId, (int)ArtifactType.Document)
				.ConfigureAwait(false);
			await FieldHelper.CreateSingleObjectFieldAsync(this.TestParameters, ReferenceToDocumentFieldName, (int)ArtifactType.Document, this.objectArtifactTypeId)
				.ConfigureAwait(false);
			this.initialImportBatchSize = AppSettings.Instance.ImportBatchSize;

			ImportHelper.ImportDefaultTestData(this.TestParameters);
		}

		[TearDown]
		public void TearDown()
		{
			AppSettings.Instance.ImportBatchSize = this.initialImportBatchSize;
		}

		[OneTimeTearDown]
		public Task OneTimeTearDown()
		{
			return RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document);
		}

		[IdentifiedTest("4ae96850-4ef9-4a1d-95eb-3140a5d7efa5")]
		public async Task ShouldNotAppendOverlayChildObjectsThatNotExist()
		{
			// ARRANGE
			const int RowsWithExistingObject = 5;
			const int RowsWithNonExistingObject = 10;
			const int TotalRows = RowsWithNonExistingObject + RowsWithExistingObject;

			IEnumerable<string> existingRowsNames = Enumerable
				.Range(1, RowsWithExistingObject)
				.Select(i => $"{nameof(this.ShouldNotAppendOverlayChildObjectsThatNotExist)}-existing-{i}")
				.ToList();

			foreach (string name in existingRowsNames)
			{
				await this.CreateChildObjectInstanceAsync(name, this.childObjectArtifactTypeId, this.objectArtifactTypeId).ConfigureAwait(false);
			}

			IEnumerable<string> notExistingRowsNames = Enumerable
				.Range(1, RowsWithNonExistingObject)
				.Select(i => $"{nameof(this.ShouldNotAppendOverlayChildObjectsThatNotExist)}-non-existing-{i}")
				.ToList();

			IEnumerable<string> nameSource = existingRowsNames.Concat(notExistingRowsNames);

			Settings settings = NativeImportSettingsProvider.DefaultSettings(this.childObjectArtifactTypeId, WellKnownFields.RdoIdentifier);
			settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);
			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.RdoIdentifier, nameSource)
				.Build();

			// ACT
			ImportTestJobResult result = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			ThenTheErrorRowsHaveCorrectMessage(result.ErrorRows, " - Your account does not have rights to add a document or object to this case\n - No parent artifact specified for this new object");
			Assert.That(result.JobReportErrorsCount, Is.EqualTo(RowsWithNonExistingObject));
			Assert.That(result.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(result.NumberOfCompletedRows, Is.EqualTo(TotalRows));
		}

		[IdentifiedTest("0d9cd961-0b6a-42d0-99ac-b58ea4ef21b7")]
		public void ShouldNotOverlayDocumentsWithSharedOverlayIdentifier()
		{
			// ARRANGE
			const int RowsWithDuplicatedOverlayIdentifier = 5;
			const int RowsWithUniqueOverlayIdentifier = 10;
			const int TotalRows = RowsWithUniqueOverlayIdentifier + RowsWithDuplicatedOverlayIdentifier;

			List<int> objectIdentifiers = new List<int>();
			for (int i = 1; i <= TotalRows; i++)
			{
				int id = RdoHelper.CreateObjectTypeInstance(
					this.TestParameters,
					this.objectArtifactTypeId,
					new Dictionary<string, object> { { WellKnownFields.RdoIdentifier, $"obj-{nameof(this.ShouldNotOverlayDocumentsWithSharedOverlayIdentifier)}-{i}" } });
				objectIdentifiers.Add(id);
				if (i <= RowsWithDuplicatedOverlayIdentifier)
				{
					objectIdentifiers.Add(id);
				}
			}

			IEnumerable<string> controlNumberSource = Enumerable.Range(1, TotalRows + RowsWithDuplicatedOverlayIdentifier)
				.Select(i => $"doc-{nameof(this.ShouldNotOverlayDocumentsWithSharedOverlayIdentifier)}-{i}");

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumberSource)
				.AddField(ReferenceToObjectFieldName, objectIdentifiers)
				.Build();

			Settings settings = NativeImportSettingsProvider.DefaultSettings();
			settings.ObjectFieldIdListContainsArtifactId = new List<int> { this.referenceToObjectFieldId };
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			this.JobExecutionContext.Execute(importDataSource);

			Settings overlaySettings = NativeImportSettingsProvider.DefaultSettings();
			overlaySettings.SelectedIdentifierFieldName = ReferenceToObjectFieldName;
			overlaySettings.OverwriteMode = OverwriteModeEnum.Overlay;
			overlaySettings.IdentityFieldId = this.referenceToObjectFieldId;
			overlaySettings.ObjectFieldIdListContainsArtifactId = new List<int> { this.referenceToObjectFieldId };
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, overlaySettings);

			ImportDataSource<object[]> overlayImportDataSource = ImportDataSourceBuilder.New()
				.AddField(ReferenceToObjectFieldName, objectIdentifiers.Distinct())
				.Build();

			// ACT
			ImportTestJobResult result = this.JobExecutionContext.Execute(overlayImportDataSource);

			// ASSERT
			ThenTheErrorRowsHaveCorrectMessage(result.ErrorRows, " - This record's Overlay Identifier is shared by multiple documents in the case, and cannot be imported");
			Assert.That(result.JobReportErrorsCount, Is.EqualTo(RowsWithDuplicatedOverlayIdentifier));
			Assert.That(result.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(result.NumberOfCompletedRows, Is.EqualTo(TotalRows));
		}

		[IdentifiedTest("1415cfe8-8c4a-4559-b0ec-36ded3925f55")]
		public void ShouldNotCreateAssociatedDocumentThatNotExist()
		{
			// ARRANGE
			const int RowsWithNonExistingDocuments = 5;
			int rowsWithExistingDocuments = TestData.SampleDocFiles.Count();
			int totalRows = rowsWithExistingDocuments + RowsWithNonExistingDocuments;

			Settings settings = NativeImportSettingsProvider.DefaultSettings(this.objectArtifactTypeId, WellKnownFields.RdoIdentifier);
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			IEnumerable<string> nameFieldSource = Enumerable.Range(1, totalRows).Select(i => $"{nameof(this.ShouldNotCreateAssociatedDocumentThatNotExist)}-{i}");
			IEnumerable<string> referenceToDocumentFieldSource = TestData.SampleDocFiles.Select(Path.GetFileName).Concat(
				Enumerable.Range(1, RowsWithNonExistingDocuments)
					.Select(i => $"{nameof(this.ShouldNotCreateAssociatedDocumentThatNotExist)}-{i}"));

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.RdoIdentifier, nameFieldSource)
				.AddField(ReferenceToDocumentFieldName, referenceToDocumentFieldSource)
				.Build();

			// ACT
			ImportTestJobResult result = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			ThenTheErrorRowsHaveCorrectMessage(result.ErrorRows, " - An object field references a document which does not exist");
			Assert.That(result.JobReportErrorsCount, Is.EqualTo(RowsWithNonExistingDocuments));
			Assert.That(result.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(result.NumberOfCompletedRows, Is.EqualTo(totalRows));
		}

		[IdentifiedTest("0c543911-4d9f-441b-8949-511c84cb1701")]
		public void ShouldNotCreateAssociatedObjectsReferencedByIdThatNotExist()
		{
			// ARRANGE
			const int RowsWithNonExistingObjects = 5;
			const int RowsWithExistingObjects = 10;
			const int TotalRows = RowsWithNonExistingObjects + RowsWithExistingObjects;

			int existingObjectArtifactId = RdoHelper.CreateObjectTypeInstance(
				this.TestParameters,
				this.objectArtifactTypeId,
				new Dictionary<string, object> { { WellKnownFields.RdoIdentifier, $"{nameof(this.ShouldNotCreateAssociatedObjectsReferencedByIdThatNotExist)}" } });

			IEnumerable<string> controlNumberSource = Enumerable.Range(1, TotalRows)
				.Select(i => $"{nameof(this.ShouldNotCreateAssociatedObjectsReferencedByIdThatNotExist)}-{i}");
			IEnumerable<int> referenceToMyObjectSource = Enumerable.Range(1, TotalRows)
				.Select(i => i <= RowsWithExistingObjects ? existingObjectArtifactId : -1);

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumberSource)
				.AddField(ReferenceToObjectFieldName, referenceToMyObjectSource)
				.Build();
			Settings settings = NativeImportSettingsProvider.DefaultSettings();
			settings.ObjectFieldIdListContainsArtifactId = new List<int> { this.referenceToObjectFieldId };
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			ThenTheErrorRowsHaveCorrectMessage(results.ErrorRows, " - An object field references an artifact ID which doesn't exist for the object.");
			Assert.That(results.JobReportErrorsCount, Is.EqualTo(RowsWithNonExistingObjects));
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(TotalRows));
		}

		[IdentifiedTest("96ee72aa-e2d5-45b5-ab55-8c6059fe6637")]
		public void ShouldPreventReferencesToDuplicateAssociateObjects()
		{
			// ARRANGE
			const int RowsReferencingDuplicateObject = 5;
			const int RowsReferencingNonDuplicateObject = 10;
			const int TotalRows = RowsReferencingNonDuplicateObject + RowsReferencingDuplicateObject;

			string duplicateObjectName = $"{nameof(this.ShouldPreventReferencesToDuplicateAssociateObjects)}-duplicate";
			string uniqueObjectName = $"{nameof(this.ShouldPreventReferencesToDuplicateAssociateObjects)}-unique";

			foreach (string nameValue in new List<string> { duplicateObjectName, duplicateObjectName, uniqueObjectName })
			{
				RdoHelper.CreateObjectTypeInstance(
					this.TestParameters,
					this.objectArtifactTypeId,
					new Dictionary<string, object> { { WellKnownFields.RdoIdentifier, nameValue } });
			}

			IEnumerable<string> controlNumberSource = Enumerable.Range(1, TotalRows)
				.Select(i => $"{nameof(this.ShouldPreventReferencesToDuplicateAssociateObjects)}-{i}");
			IEnumerable<string> referenceToMyObjectSource = Enumerable.Range(1, TotalRows)
				.Select(i => i <= RowsReferencingDuplicateObject ? duplicateObjectName : uniqueObjectName);

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumberSource)
				.AddField(ReferenceToObjectFieldName, referenceToMyObjectSource)
				.Build();
			Settings settings = NativeImportSettingsProvider.DefaultSettings();
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			ThenTheErrorRowsHaveCorrectMessage(results.ErrorRows, " - A non unique associated object is specified for this new object");
			Assert.That(results.JobReportErrorsCount, Is.EqualTo(RowsReferencingDuplicateObject));
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(TotalRows));
		}

		[IdentifiedTest("274340c0-e7ae-4c28-8be3-07863271a7a5")]
		public async Task ShouldNotCreateAssociatedObjectsThatAreChildrenAsync()
		{
			// ARRANGE
			const int RowsWithNonExistingObjects = 5;
			const int RowsWithExistingObjects = 10;
			const int TotalRows = RowsWithExistingObjects + RowsWithNonExistingObjects;

			const string ReferenceToChildObjectFieldName = "ReferenceToChildObject";
			string existingChildObjectName = $"{nameof(this.ShouldNotCreateAssociatedObjectsThatAreChildrenAsync)}-existing";
			string nonExistingChildObjectName = $"{nameof(this.ShouldNotCreateAssociatedObjectsThatAreChildrenAsync)}-non-existing";

			await this.CreateChildObjectInstanceAsync(existingChildObjectName, this.childObjectArtifactTypeId, this.objectArtifactTypeId).ConfigureAwait(false);
			await FieldHelper.CreateSingleObjectFieldAsync(
				this.TestParameters,
				ReferenceToChildObjectFieldName,
				this.childObjectArtifactTypeId,
				(int)ArtifactType.Document).ConfigureAwait(false);

			IEnumerable<string> controlNumberSource = Enumerable.Range(1, TotalRows)
				.Select(i => $"doc-{nameof(this.ShouldNotCreateAssociatedObjectsThatAreChildrenAsync)}-{i}");
			IEnumerable<string> referenceToChildObject = Enumerable.Range(1, TotalRows)
				.Select(i => i <= RowsWithExistingObjects ? existingChildObjectName : nonExistingChildObjectName);

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumberSource)
				.AddField(ReferenceToChildObjectFieldName, referenceToChildObject)
				.Build();
			Settings settings = NativeImportSettingsProvider.DefaultSettings();
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			ThenTheErrorRowsHaveCorrectMessage(results.ErrorRows, $" - 20.006. Failed to copy source field into destination field due to missing child object. Review the following destination field(s): '{ReferenceToChildObjectFieldName}'");
			Assert.That(results.JobReportErrorsCount, Is.EqualTo(RowsWithNonExistingObjects));
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(TotalRows));
		}

		[IdentifiedTest("20fe9dfe-7e40-4c7f-85d2-1f37a88f6fcd")]
		[Feature.DataTransfer.ImportApi.BulkInsert.SqlServer]
		public async Task ShouldCreateOnlySingleInstanceOfAssociatedObjectWithGivenNameAsync() // test for https://jira.kcura.com/browse/REL-421458
		{
			// ARRANGE
			const int NumberOfDocuments = 6;
			const int BatchSize = 2;

			AppSettings.Instance.ImportBatchSize = BatchSize;

			var settingsBuilder = NativeImportSettingsBuilder.New().WithDefaultSettings();
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settingsBuilder);

			const string SingleObjectFieldName = "MySingleObject";
			int firstObjectArtifactId = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, "MySingleObject").ConfigureAwait(false);
			await FieldHelper.CreateSingleObjectFieldAsync(this.TestParameters, SingleObjectFieldName, firstObjectArtifactId, (int)ArtifactType.Document).ConfigureAwait(false);

			List<string> singleObjectsSource = Enumerable.Repeat("SingleObjectInstance", NumberOfDocuments).ToList();

			var dataSourceBuilder = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource())
				.AddField(SingleObjectFieldName, singleObjectsSource);

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(dataSourceBuilder.Build(NumberOfDocuments));

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, NumberOfDocuments);
			Assert.That(results.NumberOfJobMessages, Is.Positive, "Wrong number of job messages.");
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(NumberOfDocuments), () => "Wrong number of completed rows.");

			int actualAssociatedObjectCount = RdoHelper.QueryRelativityObjectCount(this.TestParameters, firstObjectArtifactId);
			int expectedAssociatedObjectCount = singleObjectsSource.Distinct(CollationStringComparer.SQL_Latin1_General_CP1_CI_AS).Count();
			Assert.That(actualAssociatedObjectCount, Is.EqualTo(expectedAssociatedObjectCount), () => "Wrong number of associated objects created during import");
		}

		[Test]
		[Category(TestCategories.ImportDoc)]
		[IdentifiedTest("7a83707e-ad9c-47da-b925-39a32784d0d2")]
		public void ShouldNotOverlayDocumentsWhichDoNotExist()
		{
			// ARRANGE
			Settings settings = NativeImportSettingsProvider.DefaultSettings();

			const int NumberOfDocumentsToOverlay = 10;
			const int NumberOfDefaultDocuments = 10;
			const string ControlNumberSuffix = "OverlayTest";

			// Prepare data for import under test
			settings.OverwriteMode = OverwriteModeEnum.Overlay;
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			IEnumerable<string> controlNumber = GetControlNumberEnumerable(
				OverwriteModeEnum.AppendOverlay,
				NumberOfDocumentsToOverlay,
				ControlNumberSuffix);

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumber).Build();

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(NumberOfDocumentsToOverlay + NumberOfDefaultDocuments), () => "Wrong number of job messages.");
			Assert.That(results.ErrorRows.Count, Is.EqualTo(NumberOfDocumentsToOverlay), () => "Wrong number of error messages.");

			ThenTheErrorRowsHaveCorrectMessage(
				results.ErrorRows,
				" - This document identifier does not exist in the workspace - no document to overwrite");
		}

		[Category(TestCategories.ImportDoc)]
		[IdentifiedTest("f6ea6c09-ffb8-4191-95b6-75e8f04c96e3")]
		public void ShouldNotAppendDocumentsWhichAlreadyExist()
		{
			// ARRANGE
			Settings settings = NativeImportSettingsProvider.DefaultSettings();

			const int NumberOfDocumentsToAppend = 10;
			const int NumberOfDefaultDocuments = 10;
			string controlNumberSuffix = "AppendTest";

			// Prepare data for import under test
			settings.OverwriteMode = OverwriteModeEnum.Append;
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			IEnumerable<string> controlNumberStandAlone = GetControlNumberEnumerable(
				OverwriteModeEnum.Append,
				NumberOfDocumentsToAppend,
				controlNumberSuffix);

			IEnumerable<string> controlNumberWithDefaults = GetControlNumberEnumerable(
				OverwriteModeEnum.AppendOverlay,
				NumberOfDocumentsToAppend,
				controlNumberSuffix);

			ImportDataSource<object[]> importDataSourceStandAlone = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumberStandAlone).Build();

			ImportDataSource<object[]> importDataSourceWithDefaults = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumberWithDefaults).Build();

			this.JobExecutionContext.Execute(importDataSourceStandAlone);

			// ACT
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);
			ImportTestJobResult resultsWithDefaults = this.JobExecutionContext.Execute(importDataSourceWithDefaults);

			// ASSERT
			Assert.That(resultsWithDefaults.JobReportTotalRows, Is.EqualTo(NumberOfDocumentsToAppend + NumberOfDefaultDocuments), () => "Wrong number of job messages.");
			Assert.That(resultsWithDefaults.ErrorRows.Count, Is.EqualTo(NumberOfDocumentsToAppend + NumberOfDefaultDocuments), () => "Wrong number of errors.");

			foreach (var elem in resultsWithDefaults.ErrorRows)
			{
				string errorMsg = (string)elem["Message"];
				string controlName = (string)elem["Identifier"];

				Assert.That(errorMsg, Is.EqualTo($" - An item with identifier {controlName} already exists in the workspace"), () => "Unexpected error message.");
			}
		}

		private async Task<int> CreateChildObjectTypeAsync(string name, int parentArtifactTypeId)
		{
			using (var objectManager = ServiceHelper.GetServiceProxy<IObjectTypeManager>(this.TestParameters))
			{
				var request = new ObjectTypeRequest
				{
					Name = name,
					ParentObjectType = new Securable<ObjectTypeIdentifier>(new ObjectTypeIdentifier { ArtifactTypeID = parentArtifactTypeId }),
				};
				int newObjectId = await objectManager.CreateAsync(this.TestParameters.WorkspaceId, request)
					.ConfigureAwait(false);
				ObjectTypeResponse objectTypeResponse = await objectManager
					.ReadAsync(this.TestParameters.WorkspaceId, newObjectId)
					.ConfigureAwait(false);
				return objectTypeResponse.ArtifactTypeID;
			}
		}

		private async Task CreateChildObjectInstanceAsync(string name, int artifactTypeId, int parentArtifactTypeId)
		{
			int parentObjectArtifactId = RdoHelper.CreateObjectTypeInstance(
				this.TestParameters,
				parentArtifactTypeId,
				new Dictionary<string, object> { { WellKnownFields.RdoIdentifier, Guid.NewGuid().ToString() } });

			using (var objectManager = ServiceHelper.GetServiceProxy<IObjectManager>(this.TestParameters))
			{
				var request = new CreateRequest
				{
					ParentObject = new RelativityObjectRef { ArtifactID = parentObjectArtifactId },
					ObjectType = new ObjectTypeRef { ArtifactTypeID = artifactTypeId },
					FieldValues = new List<FieldRefValuePair> { new FieldRefValuePair { Field = new FieldRef { Name = WellKnownFields.RdoIdentifier }, Value = name } },
				};
				await objectManager.CreateAsync(this.TestParameters.WorkspaceId, request).ConfigureAwait(false);
			}
		}
	}
}