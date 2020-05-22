// ----------------------------------------------------------------------------
// <copyright file="DocumentWithoutIdentifierDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration.Dto
{
	using System.ComponentModel;

	using Relativity.DataExchange.TestFramework;

	public class DocumentWithoutIdentifierDto
	{
		public DocumentWithoutIdentifierDto(string keyField, string textField)
		{
			this.TextField = textField;
			this.KeyField = keyField;
		}

		[DisplayName(WellKnownFields.KeyFieldName)]
		public string KeyField { get; }

		[DisplayName(WellKnownFields.TextFieldName)]
		public string TextField { get; }
	}
}
