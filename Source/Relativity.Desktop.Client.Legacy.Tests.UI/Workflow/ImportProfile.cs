using Relativity.Desktop.Client.Legacy.Tests.UI.Windows.Names;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Workflow
{
	internal abstract class ImportProfile
	{
		protected ImportProfile(ImportWindowName importWindow, ProgressWindowName progressWindow)
		{
			ImportWindow = importWindow;
			ProgressWindow = progressWindow;
		}

		public ImportWindowName ImportWindow { get; }
		public ProgressWindowName ProgressWindow { get; }
	}
}