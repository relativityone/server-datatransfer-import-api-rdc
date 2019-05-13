namespace Relativity.DataExchange.Export.Natives.Name.Factories
{
	using System.Collections.Generic;

	using kCura.WinEDDS;

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