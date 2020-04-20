// ----------------------------------------------------------------------------
// <copyright file="AssociateObjectsTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections;
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
		private int referenceToObjectFieldId;

		[OneTimeSetUp]
		public async Task OneTimeSetupAsync()
		{
			this.objectArtifactTypeId = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, $"{nameof(AssociateObjectsTests)}-Object")
											.ConfigureAwait(false);
			this.referenceToObjectFieldId = await FieldHelper.CreateSingleObjectFieldAsync(this.TestParameters, ReferenceToObjectFieldName, this.objectArtifactTypeId, (int)ArtifactType.Document)
				.ConfigureAwait(false);
			await FieldHelper.CreateSingleObjectFieldAsync(this.TestParameters, ReferenceToDocumentFieldName, (int)ArtifactType.Document, this.objectArtifactTypeId)
				.ConfigureAwait(false);
			this.initialImportBatchSize = AppSettings.Instance.ImportBatchSize;
		}

		[TearDown]
		public void TearDown()
		{
			AppSettings.Instance.ImportBatchSize = this.initialImportBatchSize;
		}

		[IdentifiedTest("1415cfe8-8c4a-4559-b0ec-36ded3925f55")]
		public void ShouldNotCreateAssociatedDocumentThatNotExist()
		{
			// ARRANGE
			const int RowsWithNonExistingDocuments = 5;
			int rowsWithExistingDocuments = TestData.SampleDocFiles.Count();
			int totalRows = rowsWithExistingDocuments + RowsWithNonExistingDocuments;

			Settings settings = NativeImportSettingsProvider.DefaultNativeObjectImportSettings(this.objectArtifactTypeId);
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
			Settings settings = NativeImportSettingsProvider.DefaultNativeDocumentImportSettings;
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
			Settings settings = NativeImportSettingsProvider.DefaultNativeDocumentImportSettings;
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
		public async Task ShouldNotCreateObjectsThatAreChildrenAsync()
		{
			// ARRANGE
			const int RowsWithNonExistingObjects = 5;
			const int RowsWithExistingObjects = 10;
			const int TotalRows = RowsWithExistingObjects + RowsWithNonExistingObjects;

			const string ReferenceToChildObjectFieldName = "ReferenceToChildObject";
			string childObjectTypeName = $"{nameof(AssociateObjectsTests)}-ChildObject";
			string existingChildObjectName = $"{nameof(this.ShouldNotCreateObjectsThatAreChildrenAsync)}-existing";

			int childObjectTypeId = await this.CreateChildObjectTypeAsync(childObjectTypeName, this.objectArtifactTypeId).ConfigureAwait(false);
			await this.CreateChildObjectInstance(existingChildObjectName, childObjectTypeId, this.objectArtifactTypeId).ConfigureAwait(false);
			await FieldHelper.CreateSingleObjectFieldAsync(
				this.TestParameters,
				ReferenceToChildObjectFieldName,
				childObjectTypeId,
				(int)ArtifactType.Document).ConfigureAwait(false);

			IEnumerable<string> controlNumberSource = Enumerable.Range(1, TotalRows)
				.Select(i => $"doc-{nameof(this.ShouldNotCreateObjectsThatAreChildrenAsync)}-{i}");
			IEnumerable<string> referenceToChildObject = Enumerable.Range(1, TotalRows)
				.Select(i => i <= RowsWithExistingObjects ? existingChildObjectName : "NonExistingChildObjectName");

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumberSource)
				.AddField(ReferenceToChildObjectFieldName, referenceToChildObject)
				.Build();
			Settings settings = NativeImportSettingsProvider.DefaultNativeDocumentImportSettings;
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

		private static void ThenTheErrorRowsHaveCorrectMessage(IEnumerable<IDictionary> errorRows, string expectedMessage)
		{
			foreach (var row in errorRows)
			{
				string actualMessage = (string)row["Message"];
				StringAssert.AreEqualIgnoringCase(expectedMessage, actualMessage);
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

		private async Task CreateChildObjectInstance(string name, int artifactTypeId, int parentArtifactTypeId)
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