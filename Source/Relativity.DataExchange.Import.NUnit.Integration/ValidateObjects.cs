// -----------------------------------------------------------------------------------------------------
// <copyright file="ValidateObjects.cs" company="Relativity ODA LLC">
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

	using Castle.Core.Internal;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.Services.Objects.DataContracts;

	public sealed class ValidateObjects
	{
		private ValidateObjects()
		{
		}

		public static void ValidateImportedObjectsCountAndNotEmptyFieldsValues(int importedObjectsCount, bool validateFieldsWithObjectsAreNotEmpty, string[] fieldsToValidate, int objectArtifactTypeId, IntegrationTestParameters testParameters)
		{
			var objectList = RdoHelper.QueryRelativityObjects(testParameters, objectArtifactTypeId, fieldsToValidate);
			Assert.That(objectList.Count, Is.EqualTo(importedObjectsCount));

			if (validateFieldsWithObjectsAreNotEmpty)
			{
				objectList.ForEach(item => ValidateFieldsWithObjectsAreNotEmpty(item));
			}
		}

		public static async Task ValidateObjectFieldValuesWithExpected(
			IEnumerable<string> controlNumber,
			Dictionary<string, IEnumerable<string>> fieldsWithPossibleValues,
			char multiValueDelimiter,
			int artifactTypeId,
			IntegrationTestParameters testParameters)
		{
			foreach (KeyValuePair<string, IEnumerable<string>> fieldWithPossibleValues in fieldsWithPossibleValues)
			{
				string fieldName = fieldWithPossibleValues.Key;
				IEnumerable<string> fieldPossibleValues = fieldWithPossibleValues.Value;

				IEnumerable<DocumentObjectsDto> expectedFieldValuesToObjectMapping = controlNumber
					.Zip(fieldPossibleValues, (id, objects) => new DocumentObjectsDto(id, objects.Split(multiValueDelimiter)));

				await ThenTheObjectFieldHasExpectedValues(fieldName, expectedFieldValuesToObjectMapping, artifactTypeId, testParameters).ConfigureAwait(false);
			}
		}

		private static async Task ThenTheObjectFieldHasExpectedValues(string objectName, IEnumerable<DocumentObjectsDto> expectedObjectToDocumentMapping, int artifactTypeId, IntegrationTestParameters testParameters)
		{
			Dictionary<string, List<string>> actualObjectToDocumentMapping = await FieldHelper.GetFieldValuesAsync(objectName, artifactTypeId, testParameters).ConfigureAwait(false);

			foreach (var expectedMapping in expectedObjectToDocumentMapping)
			{
				Assert.That(actualObjectToDocumentMapping, Contains.Key(expectedMapping.ControlNumber));
				List<string> actualObjectValuesForDocument = actualObjectToDocumentMapping[expectedMapping.ControlNumber];

				// TODO: 'CompareTwoCollections' to remove after solve: https://jira.kcura.com/browse/REL-419430 and https://jira.kcura.com/browse/REL-419440
				if (!CompareTwoCollections(actualObjectValuesForDocument, expectedMapping))
				{
					Assert.That(
						actualObjectValuesForDocument,
						Is.EquivalentTo(
								expectedMapping.ObjectsNames.Select(objectValue => objectValue.Trim()).Distinct(
									CollationStringComparer.SQL_Latin1_General_CP1_CI_AS))
							.Using(CollationStringComparer.SQL_Latin1_General_CP1_CI_AS), $"Field: {objectName}, ControlNumber: {expectedMapping.ControlNumber}");
				}
			}
		}

		private static void ValidateFieldsWithObjectsAreNotEmpty(RelativityObject fieldRow)
		{
			fieldRow.FieldValues.ForEach(column => Assert.That(column.Value, !Is.Null, $"{column.Value} value is null - object field value not imported correctly"));
		}

		// TODO: to remove after solve: https://jira.kcura.com/browse/REL-419430 and https://jira.kcura.com/browse/REL-419440
		private static bool CompareTwoCollections(List<string> actualObjectValue, DocumentObjectsDto expectedMapping)
		{
			bool result = false;

			if (actualObjectValue.Count == expectedMapping.ObjectsNames.Count)
			{
				result = true;

				IEnumerable<string> expectedObjectValues = expectedMapping.ObjectsNames.Distinct(
					CollationStringComparer.SQL_Latin1_General_CP1_CI_AS);

				foreach (string expectedValue in expectedObjectValues)
				{
					bool found = false;

					foreach (string actualValue in actualObjectValue)
					{
						if (CollationStringComparer.Compare(actualValue, expectedValue))
						{
							found = true;
							break;
						}
					}

					if (!found)
					{
						if (IsCaseFrom_Rel_419430(expectedValue, actualObjectValue) || IsCaseFrom_Rel_419440(expectedValue, actualObjectValue))
						{
							result = true;
						}
						else
						{
							result = false;
							break;
						}
					}
				}
			}

			return result;
		}

		private static bool IsCaseFrom_Rel_419430(string expectedValue, List<string> actualObjectValuesForDocument)
		{
			foreach (string actualValue in actualObjectValuesForDocument)
			{
				if (CollationStringComparer.Compare(actualValue, expectedValue.TrimStart()))
				{
					return true;
				}
			}

			return false;
		}

		private static bool IsCaseFrom_Rel_419440(string expectedValue, List<string> actualObjectValuesForDocument)
		{
			foreach (string actualValue in actualObjectValuesForDocument)
			{
				string actual = actualValue.Replace("ß", "ss").TrimStart();
				string expected = expectedValue.Replace("ß", "ss").TrimStart(); // REL-419430

				if (expected.Length > actual.Length)
				{
					if (CollationStringComparer.Compare(actual, expected.Substring(0, actual.Length)))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
