// ----------------------------------------------------------------------------
// <copyright file="ExportImageDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration.Dto
{
	using kCura.WinEDDS;

	public class ExportImageDto
	{
		public ExportImageDto(LoadFileType.FileFormat fileFormat, ExportFile.ImageType imageType)
		{
			this.FileFormat = fileFormat;
			this.ImageType = imageType;
		}

		public LoadFileType.FileFormat FileFormat { get; }

		public ExportFile.ImageType ImageType { get; }

		public override string ToString()
		{
			return $"({this.FileFormat}, {this.ImageType})";
		}
	}
}