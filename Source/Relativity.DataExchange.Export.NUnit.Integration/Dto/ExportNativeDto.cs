// ----------------------------------------------------------------------------
// <copyright file="ExportNativeDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration.Dto
{
	using kCura.WinEDDS;

	public class ExportNativeDto
	{
		public ExportNativeDto(ExportFile.ExportedFilePathType exportedFilePathType, string filePrefix)
		{
			this.ExportedFilePathType = exportedFilePathType;
			this.FilePrefix = filePrefix;
		}

		public ExportFile.ExportedFilePathType ExportedFilePathType { get; }

		public string FilePrefix { get; }

		public override string ToString()
		{
			return $"({this.ExportedFilePathType}, {this.FilePrefix ?? "null"})";
		}
	}
}