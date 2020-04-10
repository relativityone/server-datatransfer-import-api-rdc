// ----------------------------------------------------------------------------
// <copyright file="DocumentChoicesDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration.Dto
{
	using System.Collections.Generic;
	using System.Linq;

	public class DocumentChoicesDto
	{
		public DocumentChoicesDto(string controlNumber, IEnumerable<string> choicesNames)
		{
			this.ControlNumber = controlNumber;
			this.ChoicesNames = RemoveWhiteSpaces(RemoveEmptyChoices(choicesNames)).ToList();
		}

		public string ControlNumber { get; }

		public List<string> ChoicesNames { get; }

		public DocumentChoicesDto AddChoicesNames(IEnumerable<string> choicesNames)
		{
			this.ChoicesNames.AddRange(RemoveEmptyChoices(choicesNames));
			return this;
		}

		private static IEnumerable<string> RemoveEmptyChoices(IEnumerable<string> choices) =>
			choices.Where(choice => !string.IsNullOrEmpty(choice));

		private static IEnumerable<string> RemoveWhiteSpaces(IEnumerable<string> choices) =>
			choices.Select(choice => choice.Trim());
	}
}
