namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows.SetupParameters
{
	public class ExportWindowSetupParameters
	{
		public string View { get; set; }
		public string ExportPath { get; set; }
		public string FilesNamedAfter { get; set; }
		public int VolumeInformationDigitPadding { get; set; }
		public bool ExportImages { get; set; }
		public bool ExportNativeFiles { get; set; }
		public bool ExportFullTextAsFile { get; set; }
		public string NativeFileFormat { get; set; }
		public string DataFileEncoding { get; set; }
		public string TextFileEncoding { get; set; }
		public string TextFieldPrecedence { get; set; }
	}
}