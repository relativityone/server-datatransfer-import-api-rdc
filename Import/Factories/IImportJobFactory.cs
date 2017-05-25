using kCura.WinEDDS.Core.Import.Status;

namespace kCura.WinEDDS.Core.Import.Factories
{
	public interface IImportJobFactory
	{
		IImportJob Create(IImportMetadata importMetadata, IImporterSettings importerSettings, IImporterManagers importerManagers, 
			ICancellationProvider cancellationProvider);
	}
}