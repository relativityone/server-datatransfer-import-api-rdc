using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public interface IRelativityFileShareSettings
	{
		AsperaCredential TransferCredential { get; }
		string UncPath { get; }

		bool Equals(object obj);
		int GetHashCode();
		bool IsBaseOf(string path);
	}
}