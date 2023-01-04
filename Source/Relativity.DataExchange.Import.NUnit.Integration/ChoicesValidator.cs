// -----------------------------------------------------------------------------------------------------
// <copyright file="ChoicesValidator.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents positive import job tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;

	public sealed class ChoicesValidator
	{
		private readonly IntegrationTestParameters testParameters;

		public ChoicesValidator(IntegrationTestParameters testParameters)
		{
			this.testParameters = testParameters;
		}

		public async Task ValidateChoiceFieldsValuesWithExpected(
			Tuple<string, IEnumerable<string>> identifierFieldToExpectedValuesMapping,
			Dictionary<string, IEnumerable<string>> fieldsWithPossibleValues,
			char multiValueDelimiter,
			char nestedValueDelimiter,
			int artifactTypeId)
			{
				foreach (KeyValuePair<string, IEnumerable<string>> fieldWithPossibleValues in fieldsWithPossibleValues)
				{
					string fieldName = fieldWithPossibleValues.Key;
					IEnumerable<string> fieldPossibleValues = fieldWithPossibleValues.Value;

					IEnumerable<DocumentChoicesDto> expectedFieldValuesToObjectMapping = identifierFieldToExpectedValuesMapping.Item2
						.Zip(fieldPossibleValues, (id, choices) => new DocumentChoicesDto(id, choices.Split(multiValueDelimiter).SelectMany(x => x.Split(nestedValueDelimiter)).Distinct(CollationStringComparer.SQL_Latin1_General_CP1_CI_AS)));

					await ThenTheChoiceFieldHasExpectedValues(fieldName, identifierFieldToExpectedValuesMapping.Item1, expectedFieldValuesToObjectMapping, artifactTypeId).ConfigureAwait(false);
				}
			}

		public async Task ThenTheChoiceFieldHasExpectedValues(string choiceName, string identifierFieldName, IEnumerable<DocumentChoicesDto> expectedChoiceToDocumentMapping, int artifactTypeId)
		{
			Dictionary<string, List<string>> actualChoiceToDocumentMapping = await FieldHelper.GetFieldValuesAsync(this.testParameters, choiceName, identifierFieldName, artifactTypeId).ConfigureAwait(false);

			foreach (var expectedMapping in expectedChoiceToDocumentMapping)
			{
				Assert.That(actualChoiceToDocumentMapping, Contains.Key(expectedMapping.ControlNumber));
				List<string> actualChoiceValuesForDocument = actualChoiceToDocumentMapping[expectedMapping.ControlNumber];

				// TODO: 'CompareTwoCollections' to remove after solve: https://jira.kcura.com/browse/REL-419420
				if (!CompareTwoCollections(actualChoiceValuesForDocument, expectedMapping.ChoicesNames))
				{
					Assert.That(
						actualChoiceValuesForDocument,
						Is.EquivalentTo(expectedMapping.ChoicesNames.Distinct(CollationStringComparer.SQL_Latin1_General_CP1_CI_AS)).Using(CollationStringComparer.SQL_Latin1_General_CP1_CI_AS),
						$"Field: {choiceName}, ControlNumber: {expectedMapping.ControlNumber}");
				}
			}
		}

		// TODO: to remove after solve: https://jira.kcura.com/browse/REL-419420
		private static bool CompareTwoCollections(List<string> actualChoiceValuesForDocument, List<string> expectedMapping)
		{
			IEnumerable<string> expectedChoices = expectedMapping.Distinct(
				CollationStringComparer.SQL_Latin1_General_CP1_CI_AS);

			foreach (string expectedValue in expectedChoices)
			{
				bool found = actualChoiceValuesForDocument.Any(
					actualValue => CollationStringComparer.SQL_Latin1_General_CP1_CI_AS.Equals(actualValue, expectedValue.TrimStart())); // REL-419430

				if (!found)
				{
					return false;
				}
			}

			if (actualChoiceValuesForDocument.Count != expectedMapping.Count)
			{
				return IsCaseFrom_Rel_419420(actualChoiceValuesForDocument);
			}

			return true;
		}

		private static bool IsCaseFrom_Rel_419420(List<string> actualChoiceValuesForDocument)
		{
			return actualChoiceValuesForDocument.Any(actualValue => actualValue.Contains("ÙJÐ\"Ò2'=") || actualValue.Contains("ÙJð\"ò2'="));
		}
	}
}
