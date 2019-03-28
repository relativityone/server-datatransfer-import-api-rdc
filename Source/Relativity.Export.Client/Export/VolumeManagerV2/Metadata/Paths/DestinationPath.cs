﻿using System.Text;
using kCura.WinEDDS.Exceptions;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths
{
	public abstract class DestinationPath : IDestinationPath
	{
		protected readonly ExportFile ExportSettings;

		protected DestinationPath(ExportFile exportSettings)
		{
			ExportSettings = exportSettings;
		}

		protected string FormatPath(string extension)
		{
			return System.IO.Path.Combine(ExportSettings.FolderPath, $"{ExportSettings.LoadFilesPrefix}_export{extension}");
		}

		public abstract string Path { get; }
		public abstract Encoding Encoding { get; }
		public abstract FileWriteException.DestinationFile DestinationFileType { get; }
	}
}