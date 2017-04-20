
using System.Collections.Generic;
using System.Linq;
using kCura.Windows.Process;
using kCura.WinEDDS.Core.Model.Export;
using kCura.WinEDDS.Core.Model.Export.Process;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.Service.Export;

namespace kCura.WinEDDS.Core.Export
{
	public class ExtendedExporter : Exporter
	{
		public ExtendedExporter(ExtendedExportFile exportFile, Controller processController, ILoadFileHeaderFormatterFactory loadFileFormatterFactory) : base(exportFile, processController, loadFileFormatterFactory)
		{
		}

		public ExtendedExporter(ExtendedExportFile exportFile, Controller processController, IServiceFactory serviceFactory, ILoadFileHeaderFormatterFactory loadFileFormatterFactory) : base(exportFile, processController, serviceFactory, loadFileFormatterFactory)
		{
		}

		protected override ObjectExportInfo CreateObjectExportInfo()
		{
			return new ExtendedObjectExportInfo(_volumeManager)
			{
				SelectedNativeFileNameViewFields = GetSettings<ExtendedExportFile>().SelectedNativesNameViewFields.ToList()
			};
		}

		protected override List<int> GetAvfIds()
		{
			var selectedAvfIds = new HashSet<int>(base.GetAvfIds());
			// Add Native File Name part field id
			selectedAvfIds.UnionWith(GetSettings<ExtendedExportFile>().SelectedNativesNameViewFields.Select(item => item.AvfId));
			return selectedAvfIds.ToList();
		}

		private T GetSettings<T>() where T : ExportFile
		{
			return (T) Settings;
		}

	}
}
