namespace Relativity.DataExchange.Export.Natives.Name.Factories
{
	using kCura.WinEDDS;

	public interface IFileNameProviderContainerFactory
	{
		IFileNameProvider Create(ExportFile settings);
	}
}