using kCura.WinEDDS.Core.Model;

namespace kCura.WinEDDS.Core.Export.Natives.Name
{
	
	public abstract class FileNamePartProvider<T> : IFileNamePartProvider<T> where T : DescriptorPart
	{
		public abstract string GetPartName(T descriptorPart, int artifactId, int artifactType);

		public virtual string GetPartName(DescriptorPart descriptorDescriptorPartBase, int artifactId, int artifactType)
		{
			return GetPartName(descriptorDescriptorPartBase as T, artifactId, artifactType);
		}
	}
}
