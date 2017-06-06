using System.Linq;
using Castle.MicroKernel.ModelBuilder.Inspectors;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using kCura.WinEDDS.Core.Logging;
using Relativity;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Installer
{
	public class ContainerFactory
	{
		public static IWindsorContainer GetContainer(LoadFile loadFile, ExecutionSource executionSource)
		{
			var container = new WindsorContainer();
			DisablePropertyInjection(container);
			container.Install(FromAssembly.Named("kCura.WinEDDS.Core"), FromAssembly.Named("kCura.WinEDDS.Aspera"));

			container.Register(Component.For<IWindsorContainer>().UsingFactoryMethod(k => container).LifestyleSingleton());
			container.Register(Component.For<LoadFile>().UsingFactoryMethod(k => loadFile).LifestyleSingleton());
			container.Register(Component.For<ILog>().UsingFactoryMethod(k => LoggerFactory.Create(executionSource)).LifestyleSingleton());

			return container;
		}

		private static void DisablePropertyInjection(WindsorContainer container)
		{
			var propInjector = container.Kernel.ComponentModelBuilder.Contributors.OfType<PropertiesDependenciesModelInspector>().Single();
			container.Kernel.ComponentModelBuilder.RemoveContributor(propInjector);
		}
	}
}