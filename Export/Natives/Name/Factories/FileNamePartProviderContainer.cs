using System;
using System.Collections.Generic;
using kCura.WinEDDS.Core.Model;

namespace kCura.WinEDDS.Core.Export.Natives.Name.Factories
{
	public class FileNamePartProviderContainer : IFileNamePartProviderContainer
	{
		private readonly Dictionary<Type, IFileNamePartProvider> _fileNamePartProviders = new Dictionary<Type, IFileNamePartProvider>();

		public FileNamePartProviderContainer()
		{
			RegisterFileName(typeof(SeparatorDescriptorPart), new SeparatorFileNamePartProvider());
			RegisterFileName(typeof(FieldDescriptorPart), new FieldFileNamePartProvider());
		}

		public IFileNamePartProvider GetProvider(DescriptorPart descriptor)
		{
			Type descriptorType = descriptor.GetType();
			if (!_fileNamePartProviders.ContainsKey(descriptorType))
			{
				throw new Exception($"Can not find file name provider for descriptor: {descriptor.GetType()}");
			}
			return _fileNamePartProviders[descriptorType];
		}

		public void RegisterFileName(Type descriptorPartType, IFileNamePartProvider provider)
		{
			if (!descriptorPartType.IsSubclassOf(typeof(DescriptorPart)))
			{
				throw new Exception($"Registarition of file name part provider failed. Inavlid type arument: {descriptorPartType}");
			}
			_fileNamePartProviders[descriptorPartType] = provider;
		}
	}
}
