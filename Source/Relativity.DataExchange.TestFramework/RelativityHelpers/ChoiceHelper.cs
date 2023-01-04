// <copyright file="ChoiceHelper.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Polly;

	using Relativity.DataExchange.TestFramework.Extensions;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;
	using Relativity.Services.Interfaces.Choice;
	using Relativity.Services.Interfaces.Choice.Models;
	using Relativity.Services.Interfaces.Shared.Models;

	public static class ChoiceHelper
	{
		private const int BatchSizeForChoicesImport = 3000;

		public static async Task ImportValuesIntoChoiceAsync(IntegrationTestParameters parameters, int fieldId, ChoicesValueSource choicesValueSource)
		{
			using (var choiceManager = ServiceHelper.GetServiceProxy<IChoiceManager>(parameters))
			{
				var choiceTemplateData = new MassCreateChoiceModel
				{
					Field = new ObjectIdentifier { ArtifactID = fieldId },
					Notes = string.Empty,
					Keywords = string.Empty,
				};

				var choicesStructure = choicesValueSource.UniqueValues
					.SelectMany(values => values.Split(choicesValueSource.MultiValueDelimiter))
					.Select(path => path.Split(choicesValueSource.NestedValueDelimiter))
					.Select(ConvertChoicesPathToMassCreateChoiceStructure);

				foreach (var batch in choicesStructure.Batch(BatchSizeForChoicesImport))
				{
					var request = new MassCreateChoiceRequest
					{
						ChoiceTemplateData = choiceTemplateData,
						Choices = batch.ToList(),
					};

					await Policy
						.Handle<Exception>()
						.WaitAndRetryAsync(3, retryNumber => TimeSpan.FromSeconds(3 ^ retryNumber))
						.ExecuteAsync(() => choiceManager.CreateAsync(parameters.WorkspaceId, request))
						.ConfigureAwait(false);
				}
			}
		}

		private static MassCreateChoiceStructure ConvertChoicesPathToMassCreateChoiceStructure(string[] values)
		{
			var root = new MassCreateChoiceStructure { Name = values[0] };
			var current = root;
			for (int i = 1; i < values.Length; i++)
			{
				var child = new MassCreateChoiceStructure { Name = values[i] };
				current.Children = new List<MassCreateChoiceStructure> { child };
				current = child;
			}

			return root;
		}
	}
}
