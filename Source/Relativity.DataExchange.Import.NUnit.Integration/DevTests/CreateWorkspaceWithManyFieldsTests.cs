// <copyright file="CreateWorkspaceWithManyFieldsTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Import.NUnit.Integration.DevTests
{
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.RdoStructureImport;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;
	using Relativity.DataExchange.Transfer;

	[TestFixture]
	[Explicit]
	public class CreateWorkspaceWithManyFieldsTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		[TearDown]
		public Task TearDown()
		{
			return this.ResetContextAsync();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA1801: Review unused parameters",
			Justification = "We are using TestExecutionContext.CurrentContext.CurrentTest.Arguments to retrieve value of client parameter.")]
		[Test]
		public Task CreateWorkspace([Values(TapiClient.Direct)] TapiClient client)
		{
			// arrange
			const int NumberOfDocuments = 10 * 1000;

			string identifierFieldName = WellKnownFields.ControlNumber;
			var identifierSource = new IdentifierValueSource();

			var foldersSource = new FoldersValueSource(
				numberOfDifferentPaths: 10,
				maximumPathDepth: 2,
				numberOfDifferentElements: 100,
				maximumElementLength: 100);

			var singleChoiceFields = new[]
			{
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 45, ChoicesValueSource.CreateSingleChoiceSource(10)),
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 10, ChoicesValueSource.CreateSingleChoiceSource(2500)),
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 4, ChoicesValueSource.CreateSingleChoiceSource(15000)),
			};

			var multiChoiceFields = new[]
			{
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 15, ChoicesValueSource.CreateMultiChoiceSource(8, 1)),
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 5, ChoicesValueSource.CreateMultiChoiceSource(650, 1)),
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 4, ChoicesValueSource.CreateMultiChoiceSource(2500, 1)),
			};

			var longTextFields = new[]
			{
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 33, new TextValueSource(textLength: 40)),
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 50, new TextValueSource(textLength: 200)),
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 5, new TextValueSource(textLength: 2000)),
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 1, new TextValueSource(textLength: 500 * 1024)),
			};

			var wholeNumberFields = new[]
			{
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 166, new WholeNumberValueSource()),
			};

			var simpleObjectDefinition = CreateSimpleObjectDefinition();
			var averageObjectDefinition = CreateAverageObjectDefinition();
			var averageWithChoicesObjectDefinition = CreateAverageWithChoicesObjectDefinition();
			var complexObjectDefinition = CreateComplexObjectDefinition();
			var singleObjects = new[]
			{
				new ObjectFieldDefinition(numberOfFieldsToCreate: 8, simpleObjectDefinition),
				new ObjectFieldDefinition(numberOfFieldsToCreate: 2, averageWithChoicesObjectDefinition),
				new ObjectFieldDefinition(numberOfFieldsToCreate: 1, complexObjectDefinition),
			};
			var multiObjects = new[]
			{
				new ObjectFieldDefinition(numberOfFieldsToCreate: 9, simpleObjectDefinition),
				new ObjectFieldDefinition(numberOfFieldsToCreate: 20, averageObjectDefinition, maxNumberOfMultiValues: 3),
			};

			var workspaceDataDefinition = new RdoStructureDefinition(
				NumberOfDocuments,
				identifierFieldName,
				identifierSource,
				foldersSource,
				singleChoiceFields,
				multiChoiceFields,
				longTextFields,
				wholeNumberFields,
				singleObjects,
				multiObjects);

			return this.CreateWorkspaceAndImportDataAsync(workspaceDataDefinition);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA1801: Review unused parameters",
			Justification = "We are using TestExecutionContext.CurrentContext.CurrentTest.Arguments to retrieve value of client parameter.")]
		[Test]
		public Task CreateWorkspaceWithSmallLongTextAndComplexChoices([Values(TapiClient.Direct)] TapiClient client)
		{
			// arrange
			const int NumberOfDocuments = 10 * 1000;

			string identifierFieldName = WellKnownFields.ControlNumber;
			var identifierSource = new IdentifierValueSource();

			var foldersSource = new FoldersValueSource(
				numberOfDifferentPaths: 10,
				maximumPathDepth: 2,
				numberOfDifferentElements: 100,
				maximumElementLength: 100);

			var singleChoiceFields = new[]
			{
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 15, ChoicesValueSource.CreateSingleChoiceSource(50)),
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 30, ChoicesValueSource.CreateSingleChoiceSource(250)),
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 22, ChoicesValueSource.CreateSingleChoiceSource(2500)),
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 6, ChoicesValueSource.CreateSingleChoiceSource(15000)),
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 1, ChoicesValueSource.CreateSingleChoiceSource(25000)),
			};

			var multiChoiceFields = new[]
			{
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 15, ChoicesValueSource.CreateMultiChoiceSource(50, 1)),
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 15, ChoicesValueSource.CreateMultiChoiceSource(1500, 1)),
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 5, ChoicesValueSource.CreateMultiChoiceSource(5000, 1)),
			};

			var longTextFields = new[]
			{
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 33, new TextValueSource(textLength: 5)),
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 55, new TextValueSource(textLength: 20)),
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 1, new TextValueSource(textLength: 200)),
			};

			var wholeNumberFields = new[]
			{
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 166, new WholeNumberValueSource()),
			};

			var simpleObjectDefinition = CreateSimpleObjectDefinition();
			var averageObjectDefinition = CreateAverageObjectDefinition();
			var averageWithChoicesObjectDefinition = CreateAverageWithChoicesObjectDefinition();
			var complexObjectDefinition = CreateComplexObjectDefinition();
			var singleObjects = new[]
			{
				new ObjectFieldDefinition(numberOfFieldsToCreate: 8, simpleObjectDefinition),
				new ObjectFieldDefinition(numberOfFieldsToCreate: 2, averageWithChoicesObjectDefinition),
				new ObjectFieldDefinition(numberOfFieldsToCreate: 1, complexObjectDefinition),
			};
			var multiObjects = new[]
			{
				new ObjectFieldDefinition(numberOfFieldsToCreate: 9, simpleObjectDefinition),
				new ObjectFieldDefinition(numberOfFieldsToCreate: 20, averageObjectDefinition, maxNumberOfMultiValues: 3),
			};

			var workspaceDataDefinition = new RdoStructureDefinition(
				NumberOfDocuments,
				identifierFieldName,
				identifierSource,
				foldersSource,
				singleChoiceFields,
				multiChoiceFields,
				longTextFields,
				wholeNumberFields,
				singleObjects,
				multiObjects);

			return this.CreateWorkspaceAndImportDataAsync(workspaceDataDefinition);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA1801: Review unused parameters",
			Justification = "We are using TestExecutionContext.CurrentContext.CurrentTest.Arguments to retrieve value of client parameter.")]
		[Test]
		public Task CreateWorkspaceWithComplexObjects([Values(TapiClient.Direct)] TapiClient client)
		{
			// arrange
			const int NumberOfDocuments = 10 * 1000;

			string identifierFieldName = WellKnownFields.ControlNumber;
			var identifierSource = new IdentifierValueSource();

			var foldersSource = new FoldersValueSource(
				numberOfDifferentPaths: 10,
				maximumPathDepth: 2,
				numberOfDifferentElements: 100,
				maximumElementLength: 100);

			var singleChoiceFields = new FieldDefinition<ChoicesValueSource>[0];

			var multiChoiceFields = new FieldDefinition<ChoicesValueSource>[0];

			var longTextFields = new FieldDefinition<TextValueSource>[0];

			var wholeNumberFields = new[]
			{
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 400, new WholeNumberValueSource()),
			};

			var averageObjectDefinition2 = CreateAverageObjectDefinition2();
			var averageObjectDefinition3 = CreateAverageObjectDefinition3();
			var singleObjects = new ObjectFieldDefinition[0];
			var multiObjects = new[]
			{
				new ObjectFieldDefinition(numberOfFieldsToCreate: 18, averageObjectDefinition2, 50),
				new ObjectFieldDefinition(numberOfFieldsToCreate: 2, averageObjectDefinition3, 2000),
			};

			var workspaceDataDefinition = new RdoStructureDefinition(
				NumberOfDocuments,
				identifierFieldName,
				identifierSource,
				foldersSource,
				singleChoiceFields,
				multiChoiceFields,
				longTextFields,
				wholeNumberFields,
				singleObjects,
				multiObjects);

			return this.CreateWorkspaceAndImportDataAsync(workspaceDataDefinition);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA1801: Review unused parameters",
			Justification = "We are using TestExecutionContext.CurrentContext.CurrentTest.Arguments to retrieve value of client parameter.")]
		[TestCase(540, 5, TapiClient.Direct)]
		[TestCase(540, 54, TapiClient.Direct)]
		[TestCase(540, 540, TapiClient.Direct)]
		public Task CreateWorkspaceWithChoices(int numberOfChoices, int numberOfRelations, TapiClient client)
		{
			// arrange
			const int numberOfDocuments = 10 * 1000;

			string identifierFieldName = WellKnownFields.ControlNumber;
			var identifierSource = new IdentifierValueSource();

			var foldersSource = new FoldersValueSource(
				numberOfDifferentPaths: 10,
				maximumPathDepth: 2,
				numberOfDifferentElements: 100,
				maximumElementLength: 100);

			var singleChoiceFields = new FieldDefinition<ChoicesValueSource>[0];

			var multiChoiceFields = new[]
			{
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 1, ChoicesValueSource.CreateMultiChoiceSource(numberOfChoices, numberOfRelations)),
			};

			var longTextFields = new FieldDefinition<TextValueSource>[0];

			var wholeNumberFields = new FieldDefinition<WholeNumberValueSource>[0];

			var singleObjects = new ObjectFieldDefinition[0];
			var multiObjects = new ObjectFieldDefinition[0];

			var workspaceDataDefinition = new RdoStructureDefinition(
				numberOfDocuments,
				identifierFieldName,
				identifierSource,
				foldersSource,
				singleChoiceFields,
				multiChoiceFields,
				longTextFields,
				wholeNumberFields,
				singleObjects,
				multiObjects);

			return this.CreateWorkspaceAndImportDataAsync(workspaceDataDefinition);
		}

		private static RdoStructureDefinition CreateSimpleObjectDefinition()
		{
			var wholeNumberFields = new[]
			{
				FieldDefinition.CreateAssociative(numberOfFieldsToCreate: 1, new WholeNumberValueSource()),
			};

			return new RdoStructureDefinition(
				numberOfRecordsToImport: 20,
				WellKnownFields.RdoIdentifier,
				new IdentifierValueSource("Obj"),
				foldersValueSource: null,
				new FieldDefinition<ChoicesValueSource>[0],
				new FieldDefinition<ChoicesValueSource>[0],
				new FieldDefinition<TextValueSource>[0],
				wholeNumberFields,
				new ObjectFieldDefinition[0],
				new ObjectFieldDefinition[0]);
		}

		private static RdoStructureDefinition CreateAverageObjectDefinition()
		{
			var longTextFields = new[]
			{
				FieldDefinition.CreateAssociative(numberOfFieldsToCreate: 6, new TextValueSource(textLength: 15)),
			};

			return new RdoStructureDefinition(
				numberOfRecordsToImport: 240,
				WellKnownFields.RdoIdentifier,
				new IdentifierValueSource("Obj"),
				foldersValueSource: null,
				new FieldDefinition<ChoicesValueSource>[0],
				new FieldDefinition<ChoicesValueSource>[0],
				longTextFields,
				new FieldDefinition<WholeNumberValueSource>[0],
				new ObjectFieldDefinition[0],
				new ObjectFieldDefinition[0]);
		}

		private static RdoStructureDefinition CreateAverageObjectDefinition2()
		{
			var wholeNumberFields = new[]
			{
				FieldDefinition.CreateAssociative(numberOfFieldsToCreate: 2, new WholeNumberValueSource()),
			};

			return new RdoStructureDefinition(
				numberOfRecordsToImport: 2 * 1000,
				WellKnownFields.RdoIdentifier,
				new IdentifierValueSource("Obj"),
				foldersValueSource: null,
				new FieldDefinition<ChoicesValueSource>[0],
				new FieldDefinition<ChoicesValueSource>[0],
				new FieldDefinition<TextValueSource>[0],
				wholeNumberFields,
				new ObjectFieldDefinition[0],
				new ObjectFieldDefinition[0]);
		}

		private static RdoStructureDefinition CreateAverageObjectDefinition3()
		{
			var wholeNumberFields = new[]
			{
				FieldDefinition.CreateAssociative(numberOfFieldsToCreate: 2, new WholeNumberValueSource()),
			};

			return new RdoStructureDefinition(
				numberOfRecordsToImport: 100 * 1000,
				WellKnownFields.RdoIdentifier,
				new IdentifierValueSource("Obj"),
				foldersValueSource: null,
				new FieldDefinition<ChoicesValueSource>[0],
				new FieldDefinition<ChoicesValueSource>[0],
				new FieldDefinition<TextValueSource>[0],
				wholeNumberFields,
				new ObjectFieldDefinition[0],
				new ObjectFieldDefinition[0]);
		}

		private static RdoStructureDefinition CreateAverageWithChoicesObjectDefinition()
		{
			var singleChoiceFieldForSingleObject = new[]
			{
				FieldDefinition.CreateAssociative(numberOfFieldsToCreate: 3, ChoicesValueSource.CreateSingleChoiceSource(numberOfDifferentChoices: 250)),
			};

			var longTextFields = new[]
			{
				FieldDefinition.CreateAssociative(numberOfFieldsToCreate: 3, new TextValueSource(textLength: 15)),
			};

			return new RdoStructureDefinition(
				numberOfRecordsToImport: 50,
				WellKnownFields.RdoIdentifier,
				new IdentifierValueSource("Obj"),
				foldersValueSource: null,
				singleChoiceFieldForSingleObject,
				new FieldDefinition<ChoicesValueSource>[0],
				longTextFields,
				new FieldDefinition<WholeNumberValueSource>[0],
				new ObjectFieldDefinition[0],
				new ObjectFieldDefinition[0]);
		}

		private static RdoStructureDefinition CreateComplexObjectDefinition()
		{
			var singleChoiceFieldForSingleObject = new[]
			{
				FieldDefinition.CreateAssociative(numberOfFieldsToCreate: 4, ChoicesValueSource.CreateSingleChoiceSource(numberOfDifferentChoices: 50)),
			};

			var longTextFields = new[]
			{
				FieldDefinition.CreateAssociative(18, new TextValueSource(25)),
			};

			return new RdoStructureDefinition(
				numberOfRecordsToImport: 20,
				WellKnownFields.RdoIdentifier,
				new IdentifierValueSource("Obj"),
				foldersValueSource: null,
				singleChoiceFieldForSingleObject,
				new FieldDefinition<ChoicesValueSource>[0],
				longTextFields,
				new FieldDefinition<WholeNumberValueSource>[0],
				new ObjectFieldDefinition[0],
				new ObjectFieldDefinition[0]);
		}

		private Task CreateWorkspaceAndImportDataAsync(RdoStructureDefinition dataDefinition)
		{
			var dataImporter = new SingleObjectImporter(this.TestParameters, this.JobExecutionContext);
			var structureImporter = new RdoStructureImporter(this.TestParameters, dataImporter, (int)ArtifactType.Document);

			return structureImporter.ImportAsync(dataDefinition);
		}
	}
}
