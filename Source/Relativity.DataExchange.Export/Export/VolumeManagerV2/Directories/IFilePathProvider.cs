﻿namespace Relativity.Export.VolumeManagerV2.Directories
{
	public interface IFilePathProvider
	{
		string GetPathForFile(string fileName, int objectExportInfoArtifactId);
	}
}