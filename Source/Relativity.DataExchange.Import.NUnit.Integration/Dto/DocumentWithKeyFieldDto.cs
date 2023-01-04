// ----------------------------------------------------------------------------
// <copyright file="DocumentWithKeyFieldDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration.Dto
{
	using System.ComponentModel;

	using Relativity.DataExchange.TestFramework;

	public class DocumentWithKeyFieldDto
	{
		public DocumentWithKeyFieldDto(string controlNumber, string keyField)
		{
			this.ControlNumber = controlNumber;
			this.KeyField = keyField;
		}

		[DisplayName(WellKnownFields.ControlNumber)]
		public string ControlNumber { get; }

		[DisplayName(WellKnownFields.KeyFieldName)]
		public string KeyField { get; }

		public override string ToString()
		{
			return $"{ControlNumber}_{KeyField}";
		}
	}
}
