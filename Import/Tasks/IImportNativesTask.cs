using kCura.WinEDDS.Api;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public interface IImportNativesTask
	{
		void Execute(FileMetadata fileMetadata);
	}
}
