namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows.Names
{
	internal sealed class ExportWindowName : RdcWindowName
	{
		public static readonly ExportWindowName ExportFolderAndSubfolders =
			new ExportWindowName("Relativity Desktop Client | Export Folder and Subfolders");

		public static readonly ExportWindowName ExportProductionSet =
			new ExportWindowName("Relativity Desktop Client | Export Production Set");

		public static readonly ExportWindowName ExportSavedSearch =
			new ExportWindowName("Relativity Desktop Client | Export Saved Search");

		public static readonly ExportWindowName ExportImagingProfileObjects =
			new ExportWindowName("Relativity Desktop Client | Export Imaging Profile Objects");

		private ExportWindowName(string value) : base(value)
		{
		}
	}
}