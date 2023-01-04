// ----------------------------------------------------------------------------
// <copyright file="DocumentWithChoicesImportDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration.Dto
{
	using System.ComponentModel;

	using Relativity.DataExchange.TestFramework;

	public class DocumentWithChoicesImportDto
	{
		public DocumentWithChoicesImportDto(string controlNumber, string singleChoiceField, string multiChoiceField)
		{
			this.ControlNumber = controlNumber;
			this.SingleChoiceField = singleChoiceField;
			this.MultiChoiceField = multiChoiceField;
		}

		[DisplayName(WellKnownFields.ControlNumber)]
		public string ControlNumber { get; }

		[DisplayName("SINGLE_CHOICE_FIELD")]
		public string SingleChoiceField { get; }

		[DisplayName("MULTI_CHOICE_FIELD_1")]
		public string MultiChoiceField { get; }
	}
}
