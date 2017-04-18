
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Model;
using kCura.WinEDDS.Core.Model.Export.Process;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.Helpers;

namespace kCura.WinEDDS.Core.Export.Natives.Name
{
	public class FieldFileNamePartProvider : FileNamePartProvider<FieldDescriptorPart>
	{
		private readonly ConcurrentDictionary<int, int> _cache = new ConcurrentDictionary<int, int>();
		
		public override string GetPartName(FieldDescriptorPart descriptorDescriptorPart, ObjectExportInfo exportObject)
		{
			var extExportObject = exportObject as ExtendedObjectExportInfo;

			UpdateCache(descriptorDescriptorPart, extExportObject);

			int fieldIndex = _cache[descriptorDescriptorPart.Value];
			ViewFieldInfo fieldInfo = extExportObject.SelectedViewFields[fieldIndex];

			string fieldValue = FieldValueHelper.ConvertToString(exportObject.Metadata[fieldIndex], fieldInfo, ' ');

			return kCura.Utility.File.Instance.ConvertIllegalCharactersInFilename(fieldValue);
		}

		private void UpdateCache(FieldDescriptorPart descriptorDescriptorPart, ExtendedObjectExportInfo exportObject)
		{
			// persist info about field index in SelectedViewFields
			if (!_cache.ContainsKey(descriptorDescriptorPart.Value))
			{
				int foundIndex = exportObject.SelectedViewFields.ToList().FindIndex(item => item.AvfId == descriptorDescriptorPart.Value);
				if (foundIndex < 0)
				{
					throw new Exception($"Can not find field id: {descriptorDescriptorPart.Value} in selection list!");
				}
				_cache[descriptorDescriptorPart.Value] = foundIndex;
			}
		}
	}

}
