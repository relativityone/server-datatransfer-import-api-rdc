using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.WinEDDS.Core.Model;

namespace kCura.WinEDDS.Core.Export.Natives.Name.Factories
{
	public interface IFileNamePartProviderContainer
	{
		IFileNamePartProvider GetProvider(DescriptorPart descriptor);

		void RegisterFileName(Type descriptorPartType, IFileNamePartProvider provider);
	}
}
