using Relativity.Desktop.Client.Legacy.Tests.UI.Windows.Names;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Workflow
{
	internal sealed class RdoImportProfile : ImportProfile
	{
		public static readonly RdoImportProfile DocumentLoadFile = new RdoImportProfile(
			ImportWindowName.ImportDocumentLoadFile, ProgressWindowName.ImportLoadFileProgress);

		public static readonly RdoImportProfile ImagingProfileLoadFile = new RdoImportProfile(
			ImportWindowName.ImportImagingProfileLoadFile, ProgressWindowName.ImportLoadFileProgress);

		private RdoImportProfile(ImportWindowName importWindow, ProgressWindowName progressWindow) : base(
			importWindow, progressWindow)
		{
		}
	}
}