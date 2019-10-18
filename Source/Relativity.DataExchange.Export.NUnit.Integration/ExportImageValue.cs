// ----------------------------------------------------------------------------
// <copyright file="ExportImageValue.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using kCura.WinEDDS;

	public class ExportImageValue
	{
		public ExportImageValue(LoadFileType.FileFormat fileFormat, ExportFile.ImageType imageType)
		{
			this.FileFormat = fileFormat;
			this.ImageType = imageType;
		}

		public LoadFileType.FileFormat FileFormat { get; set; }

		public ExportFile.ImageType ImageType { get; set; }

		public override string ToString()
		{
			return $"({this.FileFormat}, {this.ImageType})";
		}
	}
}