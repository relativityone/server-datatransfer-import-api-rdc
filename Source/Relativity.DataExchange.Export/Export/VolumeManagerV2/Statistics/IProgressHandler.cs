namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;

	public interface IProgressHandler
	{
		void Attach(ITapiBridge tapiBridge);
		void Detach();
	}
}