﻿// ----------------------------------------------------------------------------
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
	using Relativity.Services.Objects.DataContracts;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportRDOs]
	[TestType.EdgeCase]
	public class SelfReferencedObjectsTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		private const string SingleObjectFieldName = "SingleSelfObject";
		private const string MultiObjectFieldName = "MultiSelfObject";
		private const char Delimiter = SettingsConstants.DefaultMultiValueDelimiter;
		private const string OverlayIdentifierField = "OverlayIdentifierField";

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

		private int overlayField;

		[OneTimeSetUp]
		public async Task OneTimeSetupAsync()
		{
			this.objectArtifactTypeId = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, $"{nameof(SelfReferencedObjectsTests)}-Object")
											.ConfigureAwait(false);
			await FieldHelper.CreateSingleObjectFieldAsync(this.TestParameters, SingleObjectFieldName, objectArtifactTypeId: this.objectArtifactTypeId, associativeObjectArtifactTypeId: this.objectArtifactTypeId)
				.ConfigureAwait(false);
			await FieldHelper.CreateMultiObjectFieldAsync(this.TestParameters, MultiObjectFieldName, objectArtifactTypeId: this.objectArtifactTypeId, associativeObjectArtifactTypeId: this.objectArtifactTypeId)
				.ConfigureAwait(false);

			overlayField = await FieldHelper.CreateFixedLengthTextFieldAsync(
								   this.TestParameters,
								   this.objectArtifactTypeId,
								   OverlayIdentifierField,
								   false,
								   255).ConfigureAwait(false);
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
			this.ValidateFatalExceptionsNotExist(result);
			Assert.That(result.ErrorRows, Is.Empty, $"Expected zero error rows, but {result.ErrorRows.Count} were found.");
			Assert.That(result.NumberOfJobMessages, Is.Positive, $"Expected more than zero job messages, but {result.NumberOfJobMessages} was found.");
			Assert.That(result.NumberOfCompletedRows, Is.EqualTo(dataSource[WellKnownFields.RdoIdentifier].Count()), $"Expected {dataSource[WellKnownFields.RdoIdentifier].Count()} completed rows, but {result.NumberOfCompletedRows} were found.");

			this.ThenCorrectObjectsWereImported(dataSource);

			if (overwriteMode != OverwriteModeEnum.Append)
			{
				// In append mode we don't update self referenced fields correctly so we can't validate them. This is known issue
				await this.ThenTheFieldsHaveCorrectValuesAsync(dataSource).ConfigureAwait(false);
			}
		}

		[IdentifiedTest("03F19CF6-6C44-4DA4-BFCB-A193A90D35E6")]
		[IgnoreIfVersionLowerThan(RelativityVersion.PrairieSmoke)]
		public void ShouldProperlyOverlayObjectsWithNonDefaultOverlayIdentifier()
		{
			// ARRANGE
			var data = new Dictionary<string, IEnumerable<string>>
						   {
							   { WellKnownFields.RdoIdentifier, new List<string> { "RDO1", "RDO2", "RDO3" } },
							   { SingleObjectFieldName, new List<string> { "Obj1", "Obj2", "Obj4" } },
							   { OverlayIdentifierField, new List<string> { "OverlayField1", "OverlayField2", "OverlayField3" } },
						   };

			var overlayData = new Dictionary<string, IEnumerable<string>>
						   {
							   { SingleObjectFieldName, new List<string> { "OverlaidObj1", "OverlaidObj2", "OverlaidObj3" } },
							   { OverlayIdentifierField, new List<string> { "OverlayField1", "OverlayField2", "OverlayField3" } },
						   };

			var expectedIdentifierToSingleSelfObjectMapping = new Dictionary<string, string>
			{
				["RDO1"] = "OverlaidObj1",
				["RDO2"] = "OverlaidObj2",
				["RDO3"] = "OverlaidObj3",
			};

			this.ImportDataWithOverwriteMode(data, OverwriteModeEnum.Append);

			// ACT
			ImportTestJobResult result = this.ImportDataWithOverwriteMode(overlayData, OverwriteModeEnum.Overlay, overlayField);

			// ASSERT
			this.ValidateFatalExceptionsNotExist(result);
			Assert.That(result.ErrorRows, Is.Empty, $"Expected zero error rows, but {result.ErrorRows.Count} were found.");
			Assert.That(result.NumberOfJobMessages, Is.Positive, $"Expected more than zero job messages, but {result.NumberOfJobMessages} was found.");
			Assert.That(result.NumberOfCompletedRows, Is.EqualTo(data[OverlayIdentifierField].Count()), $"Expected {data[OverlayIdentifierField].Count()} completed rows, but {result.NumberOfCompletedRows} were found.");

			this.ThenOverlaidObjectsWereLinkedCorrectly(expectedIdentifierToSingleSelfObjectMapping);
		}

		private static Dictionary<string, IEnumerable<string>> GetDataSourceWithoutIdentifierField(
			Dictionary<string, IEnumerable<string>> dataSource)
		{
			return dataSource.Where(pair => pair.Key != WellKnownFields.RdoIdentifier).ToDictionary(pair => pair.Key, pair => pair.Value);
		}

		private ImportTestJobResult ImportDataWithOverwriteMode(Dictionary<string, IEnumerable<string>> dataToImport, OverwriteModeEnum overwriteMode, int? overlayIdentifierArtifactId = null)
		{
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings(
				this.objectArtifactTypeId,
				WellKnownFields.RdoIdentifier);
			settings.OverwriteMode = overwriteMode;
			settings.MultiValueDelimiter = Delimiter;

			if (overlayIdentifierArtifactId.HasValue)
			{
				settings.IdentityFieldId = overlayIdentifierArtifactId.Value;
			}

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			ImportDataSourceBuilder dataSourceBuilder = ImportDataSourceBuilder.New();
			foreach (var fieldNameValues in dataToImport)
			{
				dataSourceBuilder = dataSourceBuilder.AddField(fieldNameValues.Key, fieldNameValues.Value);
			}

			return this.JobExecutionContext.Execute(dataSourceBuilder.Build());
		}

		private void ThenOverlaidObjectsWereLinkedCorrectly(Dictionary<string, string> expectedIdentifierToSingleSelfObjectMapping)
		{
			const string Name = "Name";
			const string ArtifactID = "ArtifactID";
			var fields = new List<string>() { ArtifactID, Name, SingleObjectFieldName, OverlayIdentifierField };

			IList<RelativityObject> allObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, this.objectArtifactTypeId, fields);

			// Parse data to a dictionary
			List<Dictionary<string, object>> allObjectsList =
				allObjects.Select(x => x.FieldValues.ToDictionary(f => f.Field.Name, f => f.Value)).ToList();

			IEnumerable<string> importedObjectsIdentifiers = expectedIdentifierToSingleSelfObjectMapping.Keys;

			var actualIdentifierToSingleSelfObjectMapping = allObjectsList
				.Where(x => importedObjectsIdentifiers.Contains((string)x[Name]))
				.ToDictionary(
					x => (string)x[Name],
					y => ((RelativityObjectValue)y[SingleObjectFieldName]).Name);

			Assert.That(actualIdentifierToSingleSelfObjectMapping, Is.EquivalentTo(expectedIdentifierToSingleSelfObjectMapping), "Linked objects were not updated properly.");
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