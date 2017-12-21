using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Directories
{
	public class LongTextFilePathProviderTests : FilePathProviderTests
	{
		protected override FilePathProvider CreateInstance(IDirectoryHelper directoryHelper, ILabelManager labelManager, ExportFile exportSettings)
		{
			return new LongTextFilePathProvider(labelManager, exportSettings, directoryHelper, new NullLogger());
		}

		protected override string Subdirectory => "text_sub";
	}
}