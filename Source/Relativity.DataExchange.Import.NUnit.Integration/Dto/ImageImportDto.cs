// ----------------------------------------------------------------------------
// <copyright file="ImageImportDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration.Dto
{
	using System.ComponentModel;

	using Relativity.DataExchange.TestFramework;

	public class ImageImportDto
	{
		public ImageImportDto(string controlNumber, string filePath, string documentIdentifier)
		{
			this.ControlNumber = controlNumber;
			this.FilePath = filePath;
			this.DocumentIdentifier = documentIdentifier;
		}

		[DisplayName(WellKnownFields.ControlNumber)]
		public string ControlNumber { get; }

		[DisplayName(WellKnownFields.FilePath)]
		public string FilePath { get; }

		[DisplayName("Document Identifier")]
		public string DocumentIdentifier { get; }
	}
}
