using kCura.WinEDDS.Core.Model;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.Natives.Name
{
	
	public abstract class FileNamePartProvider<T> : IFileNamePartProvider<T> where T : DescriptorPart
	{
		public abstract string GetPartName(T descriptorPart, ObjectExportInfo exportObject);

		public virtual string GetPartName(DescriptorPart descriptorDescriptorPartBase, ObjectExportInfo exportObject)
		{
			return GetPartName(descriptorDescriptorPartBase as T, exportObject);
		}
	}
}
