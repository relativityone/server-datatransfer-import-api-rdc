using System;
using System.Collections.Generic;
using kCura.WinEDDS.Core.Model;

namespace kCura.WinEDDS.Core.Export.Natives.Name
{
	public interface IFileNamePartProvider
	{
		string GetFileName(DescriptorPart descriptorDescriptorPartBase, int artifactId, int artifactType);
	}

	public interface IFileNamePartProvider<in T> : IFileNamePartProvider
	{
		string GetFileName(T descriptorPart, int artifactId, int artifactType);
	}
}
