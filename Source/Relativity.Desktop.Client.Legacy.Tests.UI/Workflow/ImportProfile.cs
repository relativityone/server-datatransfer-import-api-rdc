using Relativity.Desktop.Client.Legacy.Tests.UI.Windows.Names;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Workflow
{
	internal sealed class ImportProfile
	{
		public static readonly ImportProfile ProductionSet = new ImportProfile(
			ImportWindowName.ImportProductionLoadFile, ProgressWindowName.ImportProductionFile);

		public static readonly ImportProfile ImageLoadFile = new ImportProfile(
			ImportWindowName.ImportImageLoadFile, ProgressWindowName.ImportImageFile);

		private ImportProfile(ImportWindowName importWindow, ProgressWindowName progressWindow)
		{
			ImportWindow = importWindow;
			ProgressWindow = progressWindow;
		}

		public ImportWindowName ImportWindow { get; }
		public ProgressWindowName ProgressWindow { get; }
	}
}