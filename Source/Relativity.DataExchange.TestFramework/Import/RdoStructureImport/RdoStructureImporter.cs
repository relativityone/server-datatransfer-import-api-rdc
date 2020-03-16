// <copyright file="RdoStructureImporter.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.RdoStructureImport
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;

	public class RdoStructureImporter
	{
		private readonly int rdoArtifactTypeId;
		private readonly IntegrationTestParameters testParameters;
		private readonly SingleObjectImporter dataImporter;

		public RdoStructureImporter(IntegrationTestParameters testParameters, SingleObjectImporter dataImporter, int rdoArtifactTypeId)
		{
			this.testParameters = testParameters;
			this.dataImporter = dataImporter;
			this.rdoArtifactTypeId = rdoArtifactTypeId;
		}

		/// <summary>
		/// This method creates structure of fields in a workspace and imports data into that structure.
		/// </summary>
		/// <param name="importDataDefinition">Definition of objects structure to create in a workspace.</param>
		/// <returns>Task to await.</returns>
		public async Task ImportAsync(RdoStructureDefinition importDataDefinition)
		{
			bool areFoldersPresent = importDataDefinition.FoldersValueSource != null;

			ImportDataSourceBuilder importDataSourceBuilder = new ImportDataSourceBuilder();
			importDataSourceBuilder.AddField(importDataDefinition.IdentifierFieldName, importDataDefinition.IdentifierValueSource);
			if (areFoldersPresent)
			{
				importDataSourceBuilder.AddField(WellKnownFields.FolderName, importDataDefinition.FoldersValueSource);
			}

			await CreateFieldsFromDefinitionsAsync(
				"SingleChoice",
				importDataDefinition.SingleChoiceFieldDefinitions,
				this.CreateSingleChoiceFieldAsync,
				importDataSourceBuilder).ConfigureAwait(false);

			await CreateFieldsFromDefinitionsAsync(
				"MultiChoice",
				importDataDefinition.MultiChoiceFieldDefinitions,
				this.CreateMultiChoiceFieldAsync,
				importDataSourceBuilder).ConfigureAwait(false);

			await CreateFieldsFromDefinitionsAsync(
				"LongText",
				importDataDefinition.LongTextFieldDefinitions,
				this.CreateLongTextFieldAsync,
				importDataSourceBuilder).ConfigureAwait(false);

			await CreateFieldsFromDefinitionsAsync(
				"WholeNumber",
				importDataDefinition.WholeNumberFieldDefinitions,
				this.CreateWholeNumberFieldAsync,
				importDataSourceBuilder).ConfigureAwait(false);

			await CreateFieldsFromDefinitionsAsync(
				"SingleObject",
				importDataDefinition.SingleObjectsFieldDefinitions,
				this.CreateSingleObjectFieldAsync,
				importDataSourceBuilder).ConfigureAwait(false);

			await CreateFieldsFromDefinitionsAsync(
				"MultiObject",
				importDataDefinition.MultiObjectsFieldDefinitions,
				this.CreateMultiObjectFieldAsync,
				importDataSourceBuilder).ConfigureAwait(false);
			ImportDataSource<object[]> importDataSource = importDataSourceBuilder.Build(importDataDefinition.NumberOfRecordsToImport);

			var settingsBuilder = new NativeImportSettingsBuilder();
			settingsBuilder = settingsBuilder
				.WithDefaultSettings()
				.WithDestinationType(this.rdoArtifactTypeId)
				.WithIdentifierField(importDataDefinition.IdentifierFieldName);

			if (areFoldersPresent)
			{
				settingsBuilder = settingsBuilder.WithFolderPath(WellKnownFields.FolderName);
			}

			Settings settings = settingsBuilder.Build();
			this.dataImporter.ImportData(importDataSource, settings);
		}

		private static async Task CreateFieldsFromDefinitionsAsync<TFieldDefinition>(
			string fieldNamePrefix,
			IEnumerable<TFieldDefinition> fieldDefinitions,
			Func<string, TFieldDefinition, Task<IFieldValueSource>> createFieldFunction,
			ImportDataSourceBuilder dataSourceBuilder)
			where TFieldDefinition : IFieldDefinition<object>
		{
			foreach ((TFieldDefinition fieldDefinition, int index) in fieldDefinitions.Select((element, index) => (element, index)))
			{
				string newFieldNamePrefix = $"{fieldNamePrefix}-{index}";
				await CreateFieldsFromDefinitionAsync(fieldDefinition, newFieldNamePrefix, createFieldFunction, dataSourceBuilder).ConfigureAwait(false);
			}
		}

		private static async Task CreateFieldsFromDefinitionAsync<TFieldDefinition>(
			TFieldDefinition fieldDefinition,
			string namePrefix,
			Func<string, TFieldDefinition, Task<IFieldValueSource>> createFieldFunction,
			ImportDataSourceBuilder dataSourceBuilder)
			where TFieldDefinition : IFieldDefinition<object>
		{
			for (int j = 0; j < fieldDefinition.NumberOfFieldsToCreate; j++)
			{
				string name = $"{namePrefix}-{j}";
				var valuesSource = await createFieldFunction(name, fieldDefinition).ConfigureAwait(false);

				dataSourceBuilder.AddField(name, valuesSource);
			}
		}

		private async Task<IFieldValueSource> CreateSingleChoiceFieldAsync(
			string fieldName,
			FieldDefinition<ChoicesValueSource> choiceDefinition)
		{
			var fieldId = await FieldHelper.CreateSingleChoiceFieldAsync(this.testParameters, fieldName, this.rdoArtifactTypeId, choiceDefinition.IsOpenToAssociations).ConfigureAwait(false);
			await ChoiceHelper.ImportValuesIntoChoiceAsync(this.testParameters, fieldId, choiceDefinition.ValuesSource).ConfigureAwait(false);
			return choiceDefinition.ValuesSource;
		}

		private async Task<IFieldValueSource> CreateMultiChoiceFieldAsync(
			string fieldName,
			FieldDefinition<ChoicesValueSource> choiceDefinition)
		{
			var fieldId = await FieldHelper.CreateMultiChoiceFieldAsync(this.testParameters, fieldName, this.rdoArtifactTypeId, choiceDefinition.IsOpenToAssociations).ConfigureAwait(false);
			await ChoiceHelper.ImportValuesIntoChoiceAsync(this.testParameters, fieldId, choiceDefinition.ValuesSource).ConfigureAwait(false);
			return choiceDefinition.ValuesSource;
		}

		private async Task<IFieldValueSource> CreateLongTextFieldAsync(
			string fieldName,
			FieldDefinition<TextValueSource> definition)
		{
			await FieldHelper.CreateLongTextFieldAsync(this.testParameters, fieldName, this.rdoArtifactTypeId, definition.IsOpenToAssociations).ConfigureAwait(false);
			return definition.ValuesSource;
		}

		private async Task<IFieldValueSource> CreateWholeNumberFieldAsync(
			string fieldName,
			FieldDefinition<WholeNumberValueSource> definition)
		{
			await FieldHelper.CreateWholeNumberFieldAsync(this.testParameters, fieldName, this.rdoArtifactTypeId, definition.IsOpenToAssociations).ConfigureAwait(false);
			return definition.ValuesSource;
		}

		private async Task<IFieldValueSource> CreateSingleObjectFieldAsync(
			string fieldName,
			ObjectFieldDefinition objectFieldDefinition)
		{
			int objectId = await RdoHelper.CreateObjectTypeAsync(this.testParameters, fieldName).ConfigureAwait(false);
			await FieldHelper.CreateSingleObjectFieldAsync(this.testParameters, fieldName, objectId, this.rdoArtifactTypeId).ConfigureAwait(false);

			var structureCreator = new RdoStructureImporter(this.testParameters, this.dataImporter, objectId);
			await structureCreator.ImportAsync(objectFieldDefinition.ValuesSource).ConfigureAwait(false);

			return ObjectNameValueSource.CreateForSingleObject(objectFieldDefinition.ValuesSource.IdentifierValueSource, objectFieldDefinition.ValuesSource.NumberOfRecordsToImport);
		}

		private async Task<IFieldValueSource> CreateMultiObjectFieldAsync(
			string fieldName,
			ObjectFieldDefinition objectFieldDefinition)
		{
			int objectId = await RdoHelper.CreateObjectTypeAsync(this.testParameters, fieldName).ConfigureAwait(false);
			await FieldHelper.CreateMultiObjectFieldAsync(this.testParameters, fieldName, objectId, this.rdoArtifactTypeId).ConfigureAwait(false); // TODO

			var structureCreator = new RdoStructureImporter(this.testParameters, this.dataImporter, objectId);
			await structureCreator.ImportAsync(objectFieldDefinition.ValuesSource).ConfigureAwait(false);

			var importDataDefinition = objectFieldDefinition.ValuesSource;
			return ObjectNameValueSource.CreateForMultiObjects(importDataDefinition.IdentifierValueSource, importDataDefinition.NumberOfRecordsToImport, objectFieldDefinition.MaxNumberOfMultiValues, objectFieldDefinition.MultiValuesDelimiter);
		}
	}
}
