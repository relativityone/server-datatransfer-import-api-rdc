using Relativity.Desktop.Client.Legacy.Tests.UI.Windows.Names;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Workflow
{
	internal sealed class ImageImportProfile : ImportProfile
	{
		public static readonly ImageImportProfile ProductionLoadFile = new ImageImportProfile(
			ImportWindowName.ImportProductionLoadFile, ProgressWindowName.ImportProductionFile);

		public static readonly ImageImportProfile ImageLoadFile = new ImageImportProfile(
			ImportWindowName.ImportImageLoadFile, ProgressWindowName.ImportImageFile);

		private ImageImportProfile(ImportWindowName importWindow, ProgressWindowName progressWindow) : base(
			importWindow, progressWindow)
		{
		}
	}
}