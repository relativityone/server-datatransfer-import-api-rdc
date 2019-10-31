// ----------------------------------------------------------------------------
// <copyright file="ExportTypeDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration.Dto
{
	using kCura.WinEDDS;

	public class ExportTypeDto
	{
		public ExportTypeDto(ExportFile.ExportType exportType)
		{
			this.ExportType = exportType;
		}

		public ExportFile.ExportType ExportType { get; }

		public override string ToString()
		{
			return $"({this.ExportType})";
		}
	}
}
