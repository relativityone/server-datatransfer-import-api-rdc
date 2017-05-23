using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

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
				_container.Register(Component.For<ImportContext>().UsingFactoryMethod(k => batchContext.ImportContext).LifestyleScoped());
				return _container.Resolve<IImportBatchJob>();
			}
		}
	}
}