using Relativity.Desktop.Client.Legacy.Tests.UI.Windows.Names;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Workflow
{
	internal sealed class ExportProfile
	{
		public static readonly ExportProfile FoldersAndSubfolders = new ExportProfile(
			ExportWindowName.ExportFolderAndSubfolders, ProgressWindowName.ExportFoldersAndSubfolders,
			"Select View");

		public static readonly ExportProfile ProductionSet = new ExportProfile(
			ExportWindowName.ExportProductionSet, ProgressWindowName.ExportProductionSet, "Select Production");

		public static readonly ExportProfile ExportSavedSearch = new ExportProfile(
			ExportWindowName.ExportSavedSearch, ProgressWindowName.ExportSavedSearch, "Select Saved Search");

		public static readonly ExportProfile ExportImagingProfileObjects = new ExportProfile(
			ExportWindowName.ExportImagingProfileObjects, ProgressWindowName.ExportImagingProfileObjects, "Select View");

		private ExportProfile(ExportWindowName exportWindow, ProgressWindowName progressWindow,
			string fieldsSourceDialogName)
		{
			ExportWindow = exportWindow;
			ProgressWindow = progressWindow;
			FieldsSourceDialogName = fieldsSourceDialogName;
		}

		public ExportWindowName ExportWindow { get; }
		public ProgressWindowName ProgressWindow { get; }
		public string FieldsSourceDialogName { get; }
	}
}