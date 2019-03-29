namespace Relativity.Export.VolumeManagerV2.Statistics
{
	using Relativity.Export.VolumeManagerV2.Download.TapiHelpers;

	public interface IProgressHandler
	{
		void Attach(ITapiBridge tapiBridge);
		void Detach();
	}
}