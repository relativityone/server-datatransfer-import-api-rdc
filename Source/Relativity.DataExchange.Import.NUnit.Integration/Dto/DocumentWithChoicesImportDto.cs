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
		public DocumentWithChoicesImportDto(string controlNumber, string confidentialDesignation, string privilegeDesignation)
		{
			this.ControlNumber = controlNumber;
			this.ConfidentialDesignation = confidentialDesignation;
			this.PrivilegeDesignation = privilegeDesignation;
		}

		[DisplayName(WellKnownFields.ControlNumber)]
		public string ControlNumber { get; set; }

		[DisplayName(WellKnownFields.ConfidentialDesignation)]
		public string ConfidentialDesignation { get; set; }

		[DisplayName(WellKnownFields.PrivilegeDesignation)]
		public string PrivilegeDesignation { get; set; }
	}
}
