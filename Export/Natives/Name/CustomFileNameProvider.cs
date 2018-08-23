﻿using System;
using System.Collections.Generic;
using System.Text;
using FileNaming.CustomFileNaming;
using kCura.WinEDDS.Core.Export.Natives.Name.Factories;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.Natives.Name
{
	public class CustomFileNameProvider : IFileNameProvider
	{
		#region Fields

		private readonly List<DescriptorPart> _fileNamePartDescriptors;
		private readonly IFileNamePartProviderContainer _fileNamePartNameContainer;

		#endregion Fields

		#region Constructor

		public CustomFileNameProvider(List<DescriptorPart> fileNamePartDescriptors, IFileNamePartProviderContainer fileNamePartNameContainer)
		{
			_fileNamePartDescriptors = fileNamePartDescriptors;
			_fileNamePartNameContainer = fileNamePartNameContainer;
		}

		#endregion //Constructor

		#region Public Methods

		public string GetName(ObjectExportInfo exportObjectInfo)
		{
			StringBuilder name = CreateFileName(exportObjectInfo);
			return GetNameWithNativeExtension(name, exportObjectInfo);
		}

		private StringBuilder CreateFileName(ObjectExportInfo exportObjectInfo)
		{
			var name = new StringBuilder();
			foreach (DescriptorPart descriptor in _fileNamePartDescriptors)
			{
				IFileNamePartProvider fielNamePartProvider = _fileNamePartNameContainer.GetProvider(descriptor);
				name.Append(fielNamePartProvider.GetPartName(descriptor, exportObjectInfo));
			}

			return name;
		}

		private string GetNameWithNativeExtension(StringBuilder name, ObjectExportInfo exportObjectInfo)
		{
			if (!string.IsNullOrEmpty(exportObjectInfo.NativeExtension))
			{
				name.Append($".{exportObjectInfo.NativeExtension}");
			}
			return name.ToString();
		}

		public string GetTextName(ObjectExportInfo exportedObjectInfo)
		{
			StringBuilder name = CreateFileName(exportedObjectInfo);
			return GetNameWithTextExtension(name);
		}

		private string GetNameWithTextExtension(StringBuilder name)
		{
			name.Append(".txt");
			return name.ToString();
		}

		#endregion //Public Methods

	}
}
