namespace Relativity.DataExchange.Export.VolumeManagerV2.Repository
{
	public interface IExportRequestRepository
	{
		bool AnyRequestForLocation(string destinationLocation);
	}
}