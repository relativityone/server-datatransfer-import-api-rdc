// -----------------------------------------------------------------------------------------------------
// <copyright file="ObjectsValidator.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents positive import job tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.Services.Objects.DataContracts;

	public sealed class ObjectsValidator
	{
		private readonly IntegrationTestParameters testParameters;

		public ObjectsValidator(IntegrationTestParameters testParameters)
		{
			this.testParameters = testParameters;
		}

		public static void ThenObjectsFieldsAreImported(IList<RelativityObject> relativityObjects, string[] fieldsToValidate)
		{
			if (relativityObjects == null)
			{
				return;
			}

			foreach (RelativityObject relativityObject in relativityObjects)
			{
				var values = relativityObject.FieldValues
					.Where(x => fieldsToValidate.Contains(x.Field.Name))
					.Select(x => x.Value);
				Assert.That(values, Has.All.Not.Null, "object field value not imported correctly");
			}
		}

		public static void ThenObjectsFieldsAreNotImported(IList<RelativityObject> relativityObjects, string[] fieldsToValidate)
		{
			if (relativityObjects == null)
			{
				return;
			}

			foreach (RelativityObject relativityObject in relativityObjects)
			{
				var values = relativityObject.FieldValues
					.Where(x => fieldsToValidate.Contains(x.Field.Name))
					.Select(x => x.Value);
				Assert.That(!values.Any(), "object field value not imported correctly");
			}
		}

		public async Task ValidateObjectFieldsValuesWithExpected(
			IEnumerable<string> controlNumber,
			Dictionary<string, IEnumerable<string>> fieldToExpectedValuesMapping,
			char multiValueDelimiter,
			int artifactTypeId)
		{
			foreach (KeyValuePair<string, IEnumerable<string>> fieldWithPossibleValues in fieldToExpectedValuesMapping)
			{
				string fieldName = fieldWithPossibleValues.Key;
				IEnumerable<string> fieldPossibleValues = fieldWithPossibleValues.Value;

				IEnumerable<DocumentObjectsDto> expectedFieldValuesToObjectMapping = controlNumber
					.Zip(fieldPossibleValues, (id, objects) => new DocumentObjectsDto(id, objects.Split(multiValueDelimiter)));

				await ThenTheObjectFieldHasExpectedValues(fieldName, expectedFieldValuesToObjectMapping, artifactTypeId).ConfigureAwait(false);
			}
		}

		// TODO: to remove after solve: https://jira.kcura.com/browse/REL-419430 and https://jira.kcura.com/browse/REL-419440
		private static bool CompareTwoCollections(List<string> actualObjectValue, List<string> expectedMapping)
		{
			if (actualObjectValue.Count != expectedMapping.Count)
			{
				return false;
			}

			// check collections are equivalent
			IEnumerable<string> expectedObjectValues = expectedMapping.Distinct(
				CollationStringComparer.SQL_Latin1_General_CP1_CI_AS);

			foreach (string expectedValue in expectedObjectValues)
			{
				bool found = actualObjectValue.Any(
					actualValue => CollationStringComparer.SQL_Latin1_General_CP1_CI_AS.Equals(actualValue, expectedValue));

				if (!found)
				{
					if (!IsCaseFrom_Rel_419430(expectedValue, actualObjectValue)
					    && !IsCaseFrom_Rel_419440(expectedValue, actualObjectValue))
					{
						return false;
					}
				}
			}

			return true;
		}

		private static bool IsCaseFrom_Rel_419430(string expectedValue, List<string> actualObjectValuesForDocument)
		{
			return actualObjectValuesForDocument.Any(actualValue =>
				CollationStringComparer.SQL_Latin1_General_CP1_CI_AS.Equals(actualValue, expectedValue.TrimStart()));
		}

		private static bool IsCaseFrom_Rel_419440(string expectedValue, List<string> actualObjectValuesForDocument)
		{
			foreach (string actualValue in actualObjectValuesForDocument)
			{
				string actual = actualValue.Replace("ß", "ss").TrimStart();
				string expected = expectedValue.Replace("ß", "ss").TrimStart(); // REL-419430

				if (expected.Length > actual.Length)
				{
					if (CollationStringComparer.SQL_Latin1_General_CP1_CI_AS.Equals(actual, expected.Substring(0, actual.Length)))
					{
						return true;
					}
				}
			}

			return false;
		}

		private async Task ThenTheObjectFieldHasExpectedValues(string objectName, IEnumerable<DocumentObjectsDto> expectedObjectToDocumentMapping, int artifactTypeId)
		{
			Dictionary<string, List<string>> actualObjectToDocumentMapping = await FieldHelper.GetFieldValuesAsync(this.testParameters, objectName, artifactTypeId).ConfigureAwait(false);

			foreach (var expectedMapping in expectedObjectToDocumentMapping)
			{
				Assert.That(actualObjectToDocumentMapping, Contains.Key(expectedMapping.ControlNumber));
				List<string> actualObjectValuesForDocument = actualObjectToDocumentMapping[expectedMapping.ControlNumber];

				// TODO: 'CompareTwoCollections' to remove after solve: https://jira.kcura.com/browse/REL-419430 and https://jira.kcura.com/browse/REL-419440
				if (!CompareTwoCollections(actualObjectValuesForDocument, expectedMapping.ObjectsNames))
				{
					Assert.That(
						actualObjectValuesForDocument,
						Is.EquivalentTo(expectedMapping.ObjectsNames.Select(objectValue => objectValue.Trim()).Distinct(CollationStringComparer.SQL_Latin1_General_CP1_CI_AS)).Using(CollationStringComparer.SQL_Latin1_General_CP1_CI_AS),
						$"Field: {objectName}, ControlNumber: {expectedMapping.ControlNumber}");
				}
			}
		}
	}
}
