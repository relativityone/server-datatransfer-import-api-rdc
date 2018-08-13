using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Directories
{
	public class NativeFilePathProviderTests : FilePathProviderTests
	{
		protected override FilePathProvider CreateInstance(IDirectoryHelper directoryHelper, ILabelManagerForArtifact labelManager, ExportFile exportSettings)
		{
			return new NativeFilePathProvider(labelManager, exportSettings, directoryHelper, new NullLogger());
		}

		protected override string Subdirectory => "native_sub";
	}
}