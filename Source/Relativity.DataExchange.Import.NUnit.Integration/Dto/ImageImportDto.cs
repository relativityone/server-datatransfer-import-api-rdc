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
		public ImageImportDto(string controlNumber, string documentIdentifier, string fileName, string filePath)
		{
			this.ControlNumber = controlNumber;
			this.DocumentIdentifier = documentIdentifier;
			this.FileName = fileName;
			this.FilePath = filePath;
		}

		[DisplayName(WellKnownFields.ControlNumber)]
		public string ControlNumber { get; }

		[DisplayName(WellKnownFields.DocumentIdentifier)]
		public string DocumentIdentifier { get; }

		[DisplayName(WellKnownFields.FilePath)]
		public string FilePath { get; }

		[DisplayName(WellKnownFields.FileName)]
		public string FileName { get; }
	}
}
