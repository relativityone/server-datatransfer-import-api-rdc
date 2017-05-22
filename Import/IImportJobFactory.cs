using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Status;

namespace kCura.WinEDDS.Core.Import
{
	public interface IImportJobFactory
	{
		ImportJob Create(ITransferConfig transferConfig, IImportStatusManager importStatusManager,
			IImportMetadata importMetadata, IImporterSettings importerSettings, IErrorContainer errorContainer);
	}
}
