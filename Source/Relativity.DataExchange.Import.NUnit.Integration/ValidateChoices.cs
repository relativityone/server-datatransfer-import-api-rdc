// -----------------------------------------------------------------------------------------------------
// <copyright file="ValidateChoices.cs" company="Relativity ODA LLC">
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

	public sealed class ValidateChoices
	{
		private ValidateChoices()
		{
		}

		public static async Task ValidateChoiceFieldValuesWithExpected(
			IEnumerable<string> controlNumber,
			Dictionary<string, IEnumerable<string>> fieldsWithPossibleValues,
			char multiValueDelimiter,
			char nestedValueDelimiter,
			int artifactTypeId,
			IntegrationTestParameters testParameters)
			{
				foreach (KeyValuePair<string, IEnumerable<string>> fieldWithPossibleValues in fieldsWithPossibleValues)
				{
					string fieldName = fieldWithPossibleValues.Key;
					IEnumerable<string> fieldPossibleValues = fieldWithPossibleValues.Value;

					IEnumerable<DocumentChoicesDto> expectedFieldValuesToObjectMapping = controlNumber
						.Zip(fieldPossibleValues, (id, choices) => new DocumentChoicesDto(id, choices.Split(multiValueDelimiter).SelectMany(x => x.Split(nestedValueDelimiter)).Distinct(CollationStringComparer.SQL_Latin1_General_CP1_CI_AS)));

					await ThenTheChoiceFieldHasExpectedValues(fieldName, expectedFieldValuesToObjectMapping, artifactTypeId, testParameters).ConfigureAwait(false);
				}
			}

		public static async Task ThenTheChoiceFieldHasExpectedValues(string choiceName, IEnumerable<DocumentChoicesDto> expectedChoiceToDocumentMapping, int artifactTypeId, IntegrationTestParameters testParameters)
			{
				Dictionary<string, List<string>> actualChoiceToDocumentMapping = await FieldHelper.GetFieldValuesAsync(choiceName, artifactTypeId, testParameters).ConfigureAwait(false);

				foreach (var expectedMapping in expectedChoiceToDocumentMapping)
				{
					Assert.That(actualChoiceToDocumentMapping, Contains.Key(expectedMapping.ControlNumber));
					List<string> actualChoiceValuesForDocument = actualChoiceToDocumentMapping[expectedMapping.ControlNumber];

					// TODO: 'CompareTwoCollections' to remove after solve: https://jira.kcura.com/browse/REL-419420
					if (!CompareTwoCollections(actualChoiceValuesForDocument, expectedMapping))
					{
						Assert.That(
							actualChoiceValuesForDocument,
							Is.EquivalentTo(
									expectedMapping.ChoicesNames.Distinct(
										CollationStringComparer.SQL_Latin1_General_CP1_CI_AS))
								.Using(CollationStringComparer.SQL_Latin1_General_CP1_CI_AS), $"Field: {choiceName}, ControlNumber: {expectedMapping.ControlNumber}");
					}
				}
			}

		// TODO: to remove after solve: https://jira.kcura.com/browse/REL-419420
		private static bool CompareTwoCollections(List<string> actualChoiceValuesForDocument, DocumentChoicesDto expectedMapping)
		{
			bool result = true;
			bool elementCountTheSame = actualChoiceValuesForDocument.Count == expectedMapping.ChoicesNames.Count;

			IEnumerable<string> expectedChoices = expectedMapping.ChoicesNames.Distinct(
				CollationStringComparer.SQL_Latin1_General_CP1_CI_AS);

			foreach (string expectedValue in expectedChoices)
			{
				bool found = false;
				foreach (string actualValue in actualChoiceValuesForDocument)
				{
					if (CollationStringComparer.Compare(actualValue, expectedValue))
					{
						found = true;
						break;
					}
				}

				if (!found)
				{
					result = false;
					break;
				}
			}

			if (result && !elementCountTheSame)
			{
				result = IsCaseFrom_Rel_419420(actualChoiceValuesForDocument);
			}

			return result;
		}

		private static bool IsCaseFrom_Rel_419420(List<string> actualChoiceValuesForDocument)
		{
			foreach (string actualValue in actualChoiceValuesForDocument)
			{
				if (actualValue.Contains("ÙJÐ\"Ò2'=") || actualValue.Contains("ÙJð\"ò2'="))
				{
					return true;
				}
			}

			return false;
		}
	}
}
