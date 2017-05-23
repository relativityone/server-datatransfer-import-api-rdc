using System.Collections.Generic;
using Castle.Windsor;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ErrorContainerFactory
	{
		public static IErrorContainer Create(IWindsorContainer container)
		{
			IList<IErrorContainer> errorContainers = new List<IErrorContainer>
			{
				container.Resolve<ClientErrorLineContainer>(),
				container.Resolve<AllErrorsContainer>(),
				container.Resolve<ErrorReporter>()
			};
			return new ErrorContainerComposite(errorContainers);
		}
	}
}