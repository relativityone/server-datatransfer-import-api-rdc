namespace Relativity.Export.VolumeManagerV2.Statistics
{
	using Relativity.Export.VolumeManagerV2.Download.TapiHelpers;

	public interface ITransferStatistics
	{
		void Attach(ITapiBridge tapiBridge);
		void Detach();
	}
}