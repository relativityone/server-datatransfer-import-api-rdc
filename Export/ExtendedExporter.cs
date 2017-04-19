
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
			return new ExtendedObjectExportInfo
			{
				SelectedViewFieldsCount = Settings.SelectedViewFields.Length,
				SelectedNativeFileNameViewFields = GetSettings<ExtendedExportFile>().SelectedNativesNameViewFields.ToList()
			};
		}

		protected override List<int> GetAvfIds()
		{
			List<int> avfIds = base.GetAvfIds();

			avfIds.AddRange(GetSettings<ExtendedExportFile>().SelectedNativesNameViewFields.Select(item => item.AvfId));
			return avfIds;
		}

		private T GetSettings<T>() where T : ExportFile
		{
			return (T) Settings;
		}
	}
}
