// ----------------------------------------------------------------------------
// <copyright file="SelfReferencedObjectsTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Testing.Identification;

	[TestFixture]
	public class SelfReferencedObjectsTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		private const string SingleObjectFieldName = "SingleSelfObject";
		private const string MultiObjectFieldName = "MultiSelfObject";
		private const char Delimiter = SettingsConstants.DefaultMultiValueDelimiter;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "This field is used as ValueSource")]
		private static readonly Dictionary<string, IEnumerable<string>>[] DataSource =
			{
				new Dictionary<string, IEnumerable<string>>
					{
						{ WellKnownFields.RdoIdentifier, new List<string> { "Obj2", "Obj1", "Obj4" } },
						{ SingleObjectFieldName, new List<string> { "Obj3", "Obj2", "Obj4" } },
					},

				new Dictionary<string, IEnumerable<string>>
					{
						{ WellKnownFields.RdoIdentifier, new List<string> { "Obj1", "Obj2", "Obj4" } },
						{ MultiObjectFieldName, new List<string> { $"Obj1{Delimiter}Obj4", "Obj1", $"Obj2{Delimiter}Obj1" } },
					},

				new Dictionary<string, IEnumerable<string>>
					{
						{ WellKnownFields.RdoIdentifier, new List<string> { "Obj3", "Obj2", "Obj1" } },
						{ SingleObjectFieldName, new List<string> { "Obj3", "Obj5", "Obj1" } },
						{ MultiObjectFieldName, new List<string> { "Obj5", "Obj2", $"Obj2{Delimiter}Obj1" } },
					},
			};

		private int objectArtifactTypeId;

		[OneTimeSetUp]
		public async Task OneTimeSetupAsync()
		{
			this.objectArtifactTypeId = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, $"{nameof(SelfReferencedObjectsTests)}-Object")
				.ConfigureAwait(false);
			await FieldHelper.CreateSingleObjectFieldAsync(this.TestParameters, SingleObjectFieldName, this.objectArtifactTypeId, this.objectArtifactTypeId)
				.ConfigureAwait(false);
			await FieldHelper.CreateMultiObjectFieldAsync(this.TestParameters, MultiObjectFieldName, this.objectArtifactTypeId, this.objectArtifactTypeId)
				.ConfigureAwait(false);
		}

		[TearDown]
		public Task TearDown()
		{
			return RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, this.objectArtifactTypeId);
		}

		[IdentifiedTest("b92eb969-5330-443b-a720-0a2bc1db5a57")]
		[IgnoreIfVersionLowerThan(RelativityVersion.LanceleafREL438573)]
		public async Task ShouldNotCreateDuplicatedObjectsWithSelfReferencedFields(
			[Values(OverwriteModeEnum.Append, OverwriteModeEnum.AppendOverlay, OverwriteModeEnum.Overlay)] OverwriteModeEnum overwriteMode,
			[ValueSource(nameof(DataSource))] Dictionary<string, IEnumerable<string>> dataSource)
		{
			// ARRANGE
			if (overwriteMode == OverwriteModeEnum.Overlay)
			{
				this.ImportDataWithOverwriteMode(dataSource, OverwriteModeEnum.Append);
			}

			// ACT
			ImportTestJobResult result = this.ImportDataWithOverwriteMode(dataSource, overwriteMode);

			// ASSERT
			Assert.That(result.ErrorRows, Is.Empty);
			Assert.That(result.NumberOfJobMessages, Is.Positive);
			Assert.That(result.NumberOfCompletedRows, Is.EqualTo(dataSource[WellKnownFields.RdoIdentifier].Count()));

			this.ThenCorrectObjectsWereImported(dataSource);

			if (overwriteMode != OverwriteModeEnum.Append)
			{
				// In append mode we don't update self referenced fields correctly so we can't validate them. This is known issue
				await this.ThenTheFieldsHaveCorrectValuesAsync(dataSource).ConfigureAwait(false);
			}
		}

		private static Dictionary<string, IEnumerable<string>> GetDataSourceWithoutIdentifierField(
			Dictionary<string, IEnumerable<string>> dataSource)
		{
			return dataSource.Where(pair => pair.Key != WellKnownFields.RdoIdentifier).ToDictionary(pair => pair.Key, pair => pair.Value);
		}

		private ImportTestJobResult ImportDataWithOverwriteMode(Dictionary<string, IEnumerable<string>> dataToImport, OverwriteModeEnum overwriteMode)
		{
			Settings settings = NativeImportSettingsProvider.DefaultSettings(
				this.objectArtifactTypeId,
				WellKnownFields.RdoIdentifier);
			settings.OverwriteMode = overwriteMode;
			settings.MultiValueDelimiter = Delimiter;
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			ImportDataSourceBuilder dataSourceBuilder = ImportDataSourceBuilder.New();
			foreach (var fieldNameValues in dataToImport)
			{
				dataSourceBuilder = dataSourceBuilder.AddField(fieldNameValues.Key, fieldNameValues.Value);
			}

			return this.JobExecutionContext.Execute(dataSourceBuilder.Build());
		}

		private void ThenCorrectObjectsWereImported(Dictionary<string, IEnumerable<string>> dataSource)
		{
			var actualObjects = RdoHelper.QueryRelativityObjects(
				this.TestParameters,
				this.objectArtifactTypeId,
				dataSource.Keys);

			IEnumerable<string> actualObjectNames = actualObjects
				.Select(obj => obj.FieldValues
					.Single(fieldValue => fieldValue.Field.Name == WellKnownFields.RdoIdentifier))
				.Select(fieldValue => fieldValue.Value.ToString());

			IEnumerable<string> expectedObjectNames = string.Join(Delimiter.ToString(), dataSource.Values.SelectMany(x => x))
				.Split(Delimiter).Distinct();

			Assert.That(actualObjectNames.OrderBy(x => x).SequenceEqual(expectedObjectNames.OrderBy(x => x)), Is.True);
		}

		private async Task ThenTheFieldsHaveCorrectValuesAsync(Dictionary<string, IEnumerable<string>> dataSource)
		{
			var identifierFieldToValueMapping = new Tuple<string, IEnumerable<string>>(
				WellKnownFields.RdoIdentifier,
				dataSource[WellKnownFields.RdoIdentifier]);

			await new ObjectsValidator(this.TestParameters).ValidateObjectFieldsValuesWithExpectedAsync(
				identifierFieldToValueMapping,
				GetDataSourceWithoutIdentifierField(dataSource),
				Delimiter,
				this.objectArtifactTypeId).ConfigureAwait(false);
		}
	}
}