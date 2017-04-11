using System;
using System.Collections.Generic;
using kCura.WinEDDS.Core.Model;

namespace kCura.WinEDDS.Core.Export.Natives.Name
{
	public class CustomFileNameProvider
	{
		#region Fields

		private readonly List<DescriptorPart> _fileNamePartDescriptors;
		private readonly Dictionary<Type, IFileNamePartProvider> _fileNamePartProviders = new Dictionary<Type, IFileNamePartProvider>();

		#endregion Fields

		#region Constructor

		public CustomFileNameProvider(List<DescriptorPart> fileNamePartDescriptors)
		{
			_fileNamePartDescriptors = fileNamePartDescriptors;

			RegisterFileNamePartProviders();
		}

		#endregion //Constructor

		#region Public Methods

		public string GetFileName()
		{
			foreach (DescriptorPart descriptor in _fileNamePartDescriptors)
			{
				IFileNamePartProvider fielNamePartProvider = GetProvider(descriptor);
			}
			throw new NotImplementedException();
		}

		#endregion //Public Methods

		#region Methods

		private IFileNamePartProvider GetProvider(DescriptorPart descriptor)
		{
			throw new NotImplementedException();
		}

		private void RegisterFileNamePartProviders()
		{
			RegisterFileName(typeof(SeparatorDescriptorPart), new SeparatorFileNamePartProvider());
			RegisterFileName(typeof(FieldDescriptorPart), new FieldFileNamePartProvider());
		}

		private void RegisterFileName(Type descriptorPartType, IFileNamePartProvider provider)
		{
			if (!descriptorPartType.IsSubclassOf(typeof(DescriptorPart)))
			{
				throw new Exception($"Registarition of file name part provider failed. Inavlid type arument: {descriptorPartType}");
			}
			_fileNamePartProviders[descriptorPartType] = provider;
		}

		#endregion Methods
	}
}
