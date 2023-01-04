// ----------------------------------------------------------------------------
// <copyright file="ImageImportDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration.Dto
{
	using System.ComponentModel;

	public class ImageImportDto
	{
		public ImageImportDto(string batesNumber, string documentIdentifier, string fileLocation)
		{
			this.BatesNumber = batesNumber;
			this.DocumentIdentifier = documentIdentifier;
			this.FileLocation = fileLocation;
		}

		[DisplayName(DefaultImageFieldNames.BatesNumber)]
		public string BatesNumber { get; }

		[DisplayName(DefaultImageFieldNames.DocumentIdentifier)]
		public string DocumentIdentifier { get; }

		[DisplayName(DefaultImageFieldNames.FileLocation)]
		public string FileLocation { get; }
	}
}
