namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows.Names
{
	internal sealed class ProgressWindowName : RdcWindowName
	{
		public static readonly ProgressWindowName
			ImportLoadFileProgress = new ProgressWindowName("Import Load File Progress ...");

		public static readonly ProgressWindowName ExportFoldersAndSubfolders =
			new ProgressWindowName("Export Folders and Subfolders Progress ...");

		public static readonly ProgressWindowName ExportProductionSet =
			new ProgressWindowName("Export Production Set Progress ...");

		public static readonly ProgressWindowName ExportSavedSearch =
			new ProgressWindowName("Export Saved Search Progress ...");

		public static readonly ProgressWindowName ExportImagingProfileObjects = ExportFoldersAndSubfolders;

		private ProgressWindowName(string value) : base(value)
		{
		}
	}
}