using System;
using FileNaming.CustomFileNaming;

namespace kCura.WinEDDS.Core.Export.Natives.Name.Factories
{
	public interface IFileNamePartProviderContainer
	{
		IFileNamePartProvider GetProvider(DescriptorPart descriptor);

		void Register(Type descriptorPartType, IFileNamePartProvider provider);
	}
}
