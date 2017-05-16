

using kCura.WinEDDS.Api;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public interface IImportNativesAnalyzer
	{
		FileMetadata Process(ArtifactFieldCollection artifactFieldCollection);
	}
}
