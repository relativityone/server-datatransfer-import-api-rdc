
using System;
using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Model;
using kCura.WinEDDS.Core.Model.Export.Process;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.Natives.Name
{
	public class FieldFileNamePartProvider : FileNamePartProvider<FieldDescriptorPart>
	{
		private readonly Dictionary<int, int> _cache = new Dictionary<int, int>();
		
		public override string GetPartName(FieldDescriptorPart descriptorDescriptorPart, ObjectExportInfo exportObject)
		{
			CheckCache(descriptorDescriptorPart, exportObject);
			int index = _cache[descriptorDescriptorPart.Value];
			return exportObject.Metadata[index].ToString();
		}

		private void CheckCache(FieldDescriptorPart descriptorDescriptorPart, ObjectExportInfo exportObject)
		{
			var extExportObject = exportObject as ExtendedObjectExportInfo;
			if (!_cache.ContainsKey(descriptorDescriptorPart.Value))
			{
				int foundIndex = extExportObject.SelectedViewFields.ToList().FindIndex(item => item.AvfId == descriptorDescriptorPart.Value);
				if (foundIndex < 0)
				{
					throw new Exception($"Can not find field id: {descriptorDescriptorPart.Value} in selection list!");
				}
				_cache[descriptorDescriptorPart.Value] = foundIndex;
			}
		}
	}

}
