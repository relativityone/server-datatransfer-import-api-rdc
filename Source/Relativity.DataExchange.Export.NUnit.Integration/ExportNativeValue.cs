// ----------------------------------------------------------------------------
// <copyright file="ExportNativeValue.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using kCura.WinEDDS;

	public class ExportNativeValue
	{
		public ExportNativeValue(ExportFile.ExportedFilePathType exportedFilePathType, string filePrefix)
		{
			this.ExportedFilePathType = exportedFilePathType;
			this.FilePrefix = filePrefix;
		}

		public ExportFile.ExportedFilePathType ExportedFilePathType { get; set; }

		public string FilePrefix { get; set; }

		public override string ToString()
		{
			return $"({this.ExportedFilePathType}, {this.FilePrefix ?? "null"})";
		}
	}
}