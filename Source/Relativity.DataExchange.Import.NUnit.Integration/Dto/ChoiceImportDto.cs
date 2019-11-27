// ----------------------------------------------------------------------------
// <copyright file="ChoiceImportDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration.Dto
{
	using System.ComponentModel;

	using Relativity.DataExchange.TestFramework;

	public class ChoiceImportDto
	{
		public ChoiceImportDto(string controlNumber, string confidentialDesignation)
		{
			this.ControlNumber = controlNumber;
			this.ConfidentialDesignation = confidentialDesignation;
		}

		[DisplayName(WellKnownFields.ControlNumber)]
		public string ControlNumber { get; set; }

		[DisplayName(WellKnownFields.ConfidentialDesignation)]
		public string ConfidentialDesignation { get; set; }
	}
}
