namespace Relativity.Export.VolumeManagerV2
{
	using Relativity.Transfer;

	public interface IRelativityFileShareSettings
	{
		AsperaCredential TransferCredential { get; }
		string UncPath { get; }

		bool Equals(object obj);
		int GetHashCode();
		bool IsBaseOf(string path);
	}
}