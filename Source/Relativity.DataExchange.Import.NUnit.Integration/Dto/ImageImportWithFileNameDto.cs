// ----------------------------------------------------------------------------
// <copyright file="ImageImportWithFileNameDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration.Dto
{
	using System.ComponentModel;

	using Relativity.DataExchange.TestFramework;

	public class ImageImportWithFileNameDto : ImageImportDto
	{
		public ImageImportWithFileNameDto(
			string batesNumber,
			string documentIdentifier,
			string fileLocation,
			string fileName)
			: base(batesNumber, documentIdentifier, fileLocation)
		{
			this.FileName = fileName;
		}

		[DisplayName(DefaultImageFieldNames.FileName)]
		public string FileName { get; }
	}
}
