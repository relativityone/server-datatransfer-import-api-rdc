using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Export.Natives.Name.Factories
{
	public class FileNameProviderContainerFactory : IFileNameProviderContainerFactory
	{
		private readonly IDictionary<ExportNativeWithFilenameFrom, IFileNameProvider> _fileNameProviders;

		public FileNameProviderContainerFactory(IDictionary<ExportNativeWithFilenameFrom, IFileNameProvider> fileNameProviders)
		{
			_fileNameProviders = fileNameProviders;
		}

		public IFileNameProvider Create(ExportFile settings)
		{
			return new FileNameProviderContainer(settings, _fileNameProviders);
		}
	}
}