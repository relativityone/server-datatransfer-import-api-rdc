using Castle.MicroKernel.Lifestyle;
using Castle.Windsor;
using kCura.WinEDDS.Core.Import.Helpers;

namespace kCura.WinEDDS.Core.Import.Factories
{
	public class ImportBatchJobFactory : IImportBatchJobFactory
	{
		private readonly IWindsorContainer _container;

		public ImportBatchJobFactory(IWindsorContainer container)
		{
			_container = container;
		}

		public IImportBatchJob Create(ImportBatchContext batchContext)
		{
			using (var context = _container.BeginScope())
			{
				_container.Resolve<ImportBatchContextProvider>().ImportBatchContext = batchContext;
				return _container.Resolve<IImportBatchJob>();
			}
		}
	}
}