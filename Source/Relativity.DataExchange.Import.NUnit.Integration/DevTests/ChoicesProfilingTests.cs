// <copyright file="ChoicesProfilingTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Import.NUnit.Integration.DevTests
{
	using System.Linq;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.RdoStructureImport;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;

	[Explicit]
	[TestFixture]
	public class ChoicesProfilingTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		[SetUp]
		public Task SetUp()
		{
			return this.ResetContextAsync();
		}

		[CollectDeadlocks]
		[CollectWebApiExecutionPlans]
		[CollectWebApiSql]
		[Test]
		public Task SingleChoices()
		{
			const int NumberOfDocuments = 5_1000;

			string identifierFieldName = WellKnownFields.ControlNumber;
			var identifierSource = new IdentifierValueSource();

			var singleChoiceFields = new[]
			{
				 FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 50, ChoicesValueSource.CreateSingleChoiceSource(numberOfDifferentChoices: 5)),
				 FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 5, ChoicesValueSource.CreateSingleChoiceSource(numberOfDifferentChoices: 100)),
				 FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 1, ChoicesValueSource.CreateSingleChoiceSource(numberOfDifferentChoices: 1000)),
			};

			var multiChoiceFields = Enumerable.Empty<FieldDefinition<ChoicesValueSource>>();

			var workspaceDataDefinition = RdoStructureDefinition.CreateForChoices(
				NumberOfDocuments,
				identifierFieldName,
				identifierSource,
				singleChoiceFields,
				multiChoiceFields);

			return this.CreateWorkspaceAndImportDataAsync(workspaceDataDefinition);
		}

		[CollectDeadlocks]
		[CollectWebApiExecutionPlans]
		[CollectWebApiSql]
		[Test]
		public Task ComplexSingleChoices()
		{
			const int NumberOfDocuments = 5_1000;

			string identifierFieldName = WellKnownFields.ControlNumber;
			var identifierSource = new IdentifierValueSource();

			var singleChoiceFields = new[]
			{
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 30, ChoicesValueSource.CreateSingleChoiceSource(numberOfDifferentChoices: 50)),
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 5, ChoicesValueSource.CreateSingleChoiceSource(numberOfDifferentChoices: 1000)),
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 5, ChoicesValueSource.CreateSingleChoiceSource(numberOfDifferentChoices: 10000)),
				FieldDefinition.CreateNonAssociative(numberOfFieldsToCreate: 1, ChoicesValueSource.CreateSingleChoiceSource(numberOfDifferentChoices: 30000)),
			};

			var multiChoiceFields = Enumerable.Empty<FieldDefinition<ChoicesValueSource>>();

			var workspaceDataDefinition = RdoStructureDefinition.CreateForChoices(
				NumberOfDocuments,
				identifierFieldName,
				identifierSource,
				singleChoiceFields,
				multiChoiceFields);

			return this.CreateWorkspaceAndImportDataAsync(workspaceDataDefinition);
		}

		private Task CreateWorkspaceAndImportDataAsync(RdoStructureDefinition dataDefinition)
		{
			var dataImporter = new SingleObjectImporter(this.TestParameters, this.JobExecutionContext);
			var structureImporter = new RdoStructureImporter(this.TestParameters, dataImporter, (int)ArtifactType.Document);

			return structureImporter.ImportAsync(dataDefinition);
		}
	}
}
