namespace kCura.WinEDDS.Core.Export.Natives.Name.Factories
{
	public interface IFileNameProviderContainerFactory
	{
		IFileNameProvider Create(ExportFile settings);
	}
}