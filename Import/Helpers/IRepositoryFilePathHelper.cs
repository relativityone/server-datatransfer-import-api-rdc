
namespace kCura.WinEDDS.Core.Import.Helpers
{
	public interface IRepositoryFilePathHelper
	{
		string GetNextDestinationDirectory();

		string CurrentDestinationDirectory { get; }
	}
}
