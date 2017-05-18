

namespace kCura.WinEDDS.Core.Import
{
	public interface IImporterSettings
	{
		LoadFile LoadFile { get; }
		string RunId { get; }
	}
}
