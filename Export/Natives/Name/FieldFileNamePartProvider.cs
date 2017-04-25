using System;
using System.Collections.Concurrent;
using System.Linq;
using kCura.WinEDDS.Core.Exceptions;
using kCura.WinEDDS.Core.Model;
using kCura.WinEDDS.Core.Model.Export.Process;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.Helpers;

namespace kCura.WinEDDS.Core.Export.Natives.Name
{
	public class FieldFileNamePartProvider : FileNamePartProvider<FieldDescriptorPart>
	{
		private readonly ConcurrentDictionary<int, ViewFieldInfo> _cache = new ConcurrentDictionary<int, ViewFieldInfo>();
		
		public override string GetPartName(FieldDescriptorPart descriptorDescriptorPart, ObjectExportInfo exportObject)
		{
			var extExportObject = exportObject as ExtendedObjectExportInfo;

			ViewFieldInfo viewFieldInfo = GetViewField(descriptorDescriptorPart, extExportObject);

			string fieldValue = FieldValueHelper.ConvertToString(extExportObject.GetFieldValue(viewFieldInfo.AvfColumnName), viewFieldInfo, ' ');

			return kCura.Utility.File.Instance.ConvertIllegalCharactersInFilename(fieldValue);
		}

		private ViewFieldInfo GetViewField(FieldDescriptorPart descriptorDescriptorPart, ExtendedObjectExportInfo exportObject)
		{
			if (!_cache.ContainsKey(descriptorDescriptorPart.Value))
			{
				ViewFieldInfo foundViewField = exportObject.SelectedNativeFileNameViewFields.ToList()
					.Find(item => item.AvfId == descriptorDescriptorPart.Value);

				if (foundViewField == null)
				{
					throw new ObjectNotFoundException($"Can not find field id: {descriptorDescriptorPart.Value} in selection list!");
				}
				_cache[descriptorDescriptorPart.Value] = foundViewField;
			}
			return _cache[descriptorDescriptorPart.Value];
		}
	}

}
