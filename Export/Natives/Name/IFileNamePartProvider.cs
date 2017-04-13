using System;
using System.Collections.Generic;
using kCura.WinEDDS.Core.Model;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.Natives.Name
{
	public interface IFileNamePartProvider
	{
		string GetPartName(DescriptorPart descriptorDescriptorPartBase, ObjectExportInfo exportObject);
	}

	public interface IFileNamePartProvider<in T> : IFileNamePartProvider
	{
		string GetPartName(T descriptorPart, ObjectExportInfo exportObject);
	}
}
