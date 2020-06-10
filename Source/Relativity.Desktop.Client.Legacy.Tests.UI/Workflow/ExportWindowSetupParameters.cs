namespace Relativity.Desktop.Client.Legacy.Tests.UI.Workflow
{
	public class ExportWindowSetupParameters
	{
		public string FieldSourceName { get; set; }
		public string ExportPath { get; set; }
		public string FilesNamedAfter { get; set; }
		public int VolumeInformationDigitPadding { get; set; }
		public bool ExportImages { get; set; }
		public string ImageFileFormat { get; set; }
		public string ImageFileType { get; set; }
		public bool ExportNativeFiles { get; set; }
		public bool ExportFullTextAsFile { get; set; }
		public string MetadataFileFormat { get; set; }
		public string MetadataFileEncoding { get; set; }
		public string TextFileEncoding { get; set; }
		public string TextFieldPrecedence { get; set; }
		public bool ExportRenderedPDFs { get; set; }
		public string PDFPrefix { get; set; }
	}
}