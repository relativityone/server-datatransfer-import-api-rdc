// <copyright file="ComplexFieldBuilder.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Import.NUnit.LoadTests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;

	public class ComplexFieldBuilder
	{
		private readonly Dictionary<DataSetVolume, int> singleObjectFields = new Dictionary<DataSetVolume, int>();
		private readonly Dictionary<DataSetVolume, int> multiObjectsFields = new Dictionary<DataSetVolume, int>();

		private readonly Dictionary<DataSetVolume, int> singleChoiceFields = new Dictionary<DataSetVolume, int>();
		private readonly Dictionary<DataSetVolume, int> multiChoiceFields = new Dictionary<DataSetVolume, int>();

		private readonly IntegrationTestParameters testParameters;

		private ComplexFieldBuilder(IntegrationTestParameters testParameters)
		{
			this.testParameters = testParameters;
		}

		private enum DataSetVolume
		{
			Small,
			Medium,
			Large,
		}

		public static ComplexFieldBuilder Create(IntegrationTestParameters testParams)
		{
			return new ComplexFieldBuilder(testParams);
		}

		public ComplexFieldBuilder WithSmallSingleObjectField(int count)
		{
			this.singleObjectFields[DataSetVolume.Small] = count;
			return this;
		}

		public ComplexFieldBuilder WithSmallSingleObjectField()
		{
			return WithSmallSingleObjectField(15);
		}

		public ComplexFieldBuilder WithMediumSingleObjectField(int count)
		{
			this.singleObjectFields[DataSetVolume.Medium] = count;
			return this;
		}

		public ComplexFieldBuilder WithMediumSingleObjectField()
		{
			return WithMediumSingleObjectField(5);
		}

		public ComplexFieldBuilder WithLargeSingleObjectField(int count)
		{
			this.singleObjectFields[DataSetVolume.Large] = count;
			return this;
		}

		public ComplexFieldBuilder WithLargeSingleObjectField()
		{
			return WithLargeSingleObjectField(2);
		}

		public ComplexFieldBuilder WithSmallMultiObjectsField(int count)
		{
			this.multiObjectsFields[DataSetVolume.Small] = count;
			return this;
		}

		public ComplexFieldBuilder WithSmallMultiObjectsField()
		{
			return WithSmallMultiObjectsField(15);
		}

		public ComplexFieldBuilder WithMediumMultiObjectsField(int count)
		{
			this.multiObjectsFields[DataSetVolume.Medium] = count;
			return this;
		}

		public ComplexFieldBuilder WithMediumMultiObjectsField()
		{
			return WithMediumMultiObjectsField(5);
		}

		public ComplexFieldBuilder WithLargeMultiObjectsField(int count)
		{
			this.multiObjectsFields[DataSetVolume.Large] = count;
			return this;
		}

		public ComplexFieldBuilder WithLargeMultiObjectsField()
		{
			return WithLargeMultiObjectsField(2);
		}

		public ComplexFieldBuilder WithSmallSingleChoiceField(int count)
		{
			this.singleChoiceFields[DataSetVolume.Small] = count;
			return this;
		}

		public ComplexFieldBuilder WithSmallSingleChoiceField()
		{
			return WithSmallSingleChoiceField(3);
		}

		public ComplexFieldBuilder WithMediumSingleChoiceField(int count)
		{
			this.singleChoiceFields[DataSetVolume.Medium] = count;
			return this;
		}

		public ComplexFieldBuilder WithMediumSingleChoiceField()
		{
			return WithMediumSingleChoiceField(2);
		}

		public ComplexFieldBuilder WithLargeSingleChoiceField(int count)
		{
			this.singleChoiceFields[DataSetVolume.Large] = count;
			return this;
		}

		public ComplexFieldBuilder WithLargeSingleChoiceField()
		{
			return WithLargeSingleChoiceField(1);
		}

		public ComplexFieldBuilder WithSmallMultiChoiceField(int count)
		{
			this.multiChoiceFields[DataSetVolume.Small] = count;
			return this;
		}

		public ComplexFieldBuilder WithSmallMultiChoiceField()
		{
			return WithSmallMultiChoiceField(3);
		}

		public ComplexFieldBuilder WithMediumMultiChoiceField(int count)
		{
			this.multiChoiceFields[DataSetVolume.Medium] = count;
			return this;
		}

		public ComplexFieldBuilder WithMediumMultiChoiceField()
		{
			return WithMediumMultiChoiceField(2);
		}

		public ComplexFieldBuilder WithLargeMultiChoiceField(int count)
		{
			this.multiChoiceFields[DataSetVolume.Large] = count;
			return this;
		}

		public ComplexFieldBuilder WithLargeMultiChoiceField()
		{
			return WithLargeMultiChoiceField(1);
		}

		public async Task<IEnumerable<(string, IFieldValueSource valuesSource)>> Build()
		{
			var singleObjectFieldsToCreate = new List<(int, IFieldValueSource)>();
			var multiObjectsFieldsToCreate = new List<(int, IFieldValueSource)>();

			var singleChoiceFieldsToCreate = new List<(int, IFieldValueSource)>();
			var multiChoiceFieldsToCreate = new List<(int, IFieldValueSource)>();

			// Single object fields preparation step
			foreach (var singleObjectFields in this.singleObjectFields)
			{
				singleObjectFieldsToCreate.Add((singleObjectFields.Value, GetValueSourceBySingleObjectVolumeSet(singleObjectFields.Key)));
			}

			IEnumerable<(string fieldName, IFieldValueSource valuesSource)> singleObjectFieldsToImport = await this.CreateSingleObjectsFieldsAsync(singleObjectFieldsToCreate.ToArray()).ConfigureAwait(false);

			// Multi object fields preparation step
			foreach (var multiObjectsFields in this.multiObjectsFields)
			{
				multiObjectsFieldsToCreate.Add((multiObjectsFields.Value, GetValueSourceByMultiObjectsVolumeSet(multiObjectsFields.Key)));
			}

			IEnumerable<(string fieldName, IFieldValueSource valuesSource)> multiObjectFieldsToImport = await this.CreateMultiObjectsFieldsAsync(multiObjectsFieldsToCreate.ToArray()).ConfigureAwait(false);

			// Single choice fields preparation step
			foreach (var singleChoiceFields in this.singleChoiceFields)
			{
				singleChoiceFieldsToCreate.Add((singleChoiceFields.Value, GetValueSourceBySingleChoiceVolumeSet(singleChoiceFields.Key)));
			}

			IEnumerable<(string fieldName, IFieldValueSource valuesSource)> singleChoiceFieldsToImport = await this.CreateSingleChoiceFieldsAsync(singleChoiceFieldsToCreate.ToArray()).ConfigureAwait(false);

			// Multi choice fields preparation step
			foreach (var multiChoiceFields in this.multiChoiceFields)
			{
				multiChoiceFieldsToCreate.Add((multiChoiceFields.Value, GetValueSourceByMultiChoiceVolumeSet(multiChoiceFields.Key)));
			}

			IEnumerable<(string fieldName, IFieldValueSource valuesSource)> multiChoiceFieldsToImport = await this.CreateMultiChoiceFieldsAsync(multiChoiceFieldsToCreate.ToArray()).ConfigureAwait(false);

			return singleObjectFieldsToImport.Concat(multiObjectFieldsToImport).Concat(singleChoiceFieldsToImport).Concat(multiChoiceFieldsToImport);
		}

		private static ChoicesValueSource GetValueSourceBySingleChoiceVolumeSet(DataSetVolume volumeType)
		{
			switch (volumeType)
			{
				case DataSetVolume.Small:
					return ChoicesValueSource.CreateSingleChoiceSource(10);
				case DataSetVolume.Medium:
					return ChoicesValueSource.CreateSingleChoiceSource(200);
				default:
					return ChoicesValueSource.CreateSingleChoiceSource(5_000);
			}
		}

		private static ChoicesValueSource GetValueSourceByMultiChoiceVolumeSet(DataSetVolume volumeType)
		{
			switch (volumeType)
			{
				case DataSetVolume.Small:
					return ChoicesValueSource.CreateMultiChoiceSource(10, 2);
				case DataSetVolume.Medium:
					return ChoicesValueSource.CreateMultiChoiceSource(100, 10);
				default:
					return ChoicesValueSource.CreateMultiChoiceSource(1_000, 50);
			}
		}

		private static ObjectNameValueSource GetValueSourceBySingleObjectVolumeSet(DataSetVolume volumeType)
		{
			switch (volumeType)
			{
				case DataSetVolume.Small:
					return ObjectNameValueSource.CreateForSingleObject(
						new IdentifierValueSource("SMALL-SINGLE-OBJ"),
						numberOfObjects: 200);
				case DataSetVolume.Medium:
					return ObjectNameValueSource.CreateForSingleObject(
						new IdentifierValueSource("MEDIUM-SINGLE-OBJ"),
						numberOfObjects: 5_000);
				default:
					return ObjectNameValueSource.CreateForSingleObject(
						new IdentifierValueSource("LARGE-SINGLE"),
						numberOfObjects: 100_000);
			}
		}

		private static ObjectNameValueSource GetValueSourceByMultiObjectsVolumeSet(DataSetVolume volumeType)
		{
			switch (volumeType)
			{
				case DataSetVolume.Small:
					return ObjectNameValueSource.CreateForMultiObjects(
						new IdentifierValueSource("SMALL-MULTI-OBJ"),
						numberOfObjects: 200,
						maxNumberOfMultiValues: 3);
				case DataSetVolume.Medium:
					return ObjectNameValueSource.CreateForMultiObjects(
						new IdentifierValueSource("MEDIUM-MULTI-OBJ"),
						numberOfObjects: 5_000,
						maxNumberOfMultiValues: 5);
				default:
					return ObjectNameValueSource.CreateForMultiObjects(
						new IdentifierValueSource("LARGE-MULTI-OBJ"),
						numberOfObjects: 100_000,
						maxNumberOfMultiValues: 10);
			}
		}

		private static async Task<IEnumerable<(string fieldName, IFieldValueSource valuesSource)>> CreateChoicesFieldsAsync(
			string fieldNamePrefix,
			(int numberOfFields, IFieldValueSource valueSource)[] input,
			Func<string, Task> createFieldFunctionAsync)
		{
			var output = new List<(string fieldName, IFieldValueSource valuesSource)>();
			for (int i = 0; i < input.Length; i++)
			{
				(int numberOfFields, IFieldValueSource valueSource) = input[i];

				for (int j = 0; j < numberOfFields; j++)
				{
					string fieldName = $"{fieldNamePrefix}-{i}-{j}";
					await createFieldFunctionAsync(fieldName).ConfigureAwait(false);

					output.Add((fieldName, valueSource));
				}
			}

			return output;
		}

		private Task<IEnumerable<(string fieldName, IFieldValueSource valuesSource)>> CreateSingleObjectsFieldsAsync(
			(int numberOfFields, IFieldValueSource valueSource)[] input)
		{
			return this.CreateObjectsFieldsAsync("Single-Obj", input, CreateFieldAsync);

			Task CreateFieldAsync(string fieldName, int objectTypeArtifactId)
			{
				return FieldHelper.CreateSingleObjectFieldAsync(
					this.testParameters,
					fieldName,
					objectArtifactTypeId: (int)ArtifactType.Document,
					associativeObjectArtifactTypeId: objectTypeArtifactId);
			}
		}

		private Task<IEnumerable<(string fieldName, IFieldValueSource valuesSource)>> CreateMultiObjectsFieldsAsync(
			(int numberOfFields, IFieldValueSource valueSource)[] input)
		{
			return this.CreateObjectsFieldsAsync("Multi-Obj", input, CreateFieldAsync);

			Task CreateFieldAsync(string fieldName, int objectTypeArtifactId)
			{
				return FieldHelper.CreateMultiObjectFieldAsync(
					this.testParameters,
					fieldName,
					objectTypeArtifactId,
					(int)ArtifactType.Document);
			}
		}

		private Task<IEnumerable<(string fieldName, IFieldValueSource valuesSource)>> CreateSingleChoiceFieldsAsync(
			(int numberOfFields, IFieldValueSource valueSource)[] input)
		{
			return CreateChoicesFieldsAsync("Single-Choice", input, CreateFieldAsync);

			Task CreateFieldAsync(string fieldName)
			{
				return FieldHelper.CreateSingleChoiceFieldAsync(
					this.testParameters,
					fieldName,
					(int)ArtifactType.Document,
					false);
			}
		}

		private Task<IEnumerable<(string fieldName, IFieldValueSource valuesSource)>> CreateMultiChoiceFieldsAsync(
			(int numberOfFields, IFieldValueSource valueSource)[] input)
		{
			return CreateChoicesFieldsAsync("Multi-Choice", input, CreateFieldAsync);

			Task CreateFieldAsync(string fieldName)
			{
				return FieldHelper.CreateMultiChoiceFieldAsync(
					this.testParameters,
					fieldName,
					(int)ArtifactType.Document,
					false);
			}
		}

		private async Task<IEnumerable<(string fieldName, IFieldValueSource valuesSource)>> CreateObjectsFieldsAsync(
			string fieldNamePrefix,
			(int numberOfFields, IFieldValueSource valueSource)[] input,
			Func<string, int, Task> createFieldFunctionAsync)
		{
			var output = new List<(string fieldName, IFieldValueSource valuesSource)>();
			for (int i = 0; i < input.Length; i++)
			{
				(int numberOfFields, IFieldValueSource valueSource) = input[i];

				for (int j = 0; j < numberOfFields; j++)
				{
					string fieldName = $"{fieldNamePrefix}-{i}-{j}";
					int objectTypeArtifactId = await RdoHelper.CreateObjectTypeAsync(this.testParameters, fieldName).ConfigureAwait(false);
					await createFieldFunctionAsync(fieldName, objectTypeArtifactId).ConfigureAwait(false);

					output.Add((fieldName, valueSource));
				}
			}

			return output;
		}
	}
}
