// ----------------------------------------------------------------------------
// <copyright file="FolderImportDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration.Dto
{
	using System.ComponentModel;

	using Relativity.DataExchange.TestFramework;

	public class FolderImportDto
	{
		public FolderImportDto(string controlNumber, string folder)
		{
			this.ControlNumber = controlNumber;
			this.Folder = folder;
		}

		[DisplayName(WellKnownFields.ControlNumber)]
		public string ControlNumber { get; set; }

		[DisplayName(WellKnownFields.FolderName)]
		public string Folder { get; set; }
	}
}
