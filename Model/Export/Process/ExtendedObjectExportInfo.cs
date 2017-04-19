using System.Collections.Generic;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Model.Export.Process
{
	public class ExtendedObjectExportInfo : ObjectExportInfo
	{
		public List<ViewFieldInfo> SelectedNativeFileNameViewFields { get; set; }
	}
}
