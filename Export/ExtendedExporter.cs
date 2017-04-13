
using System.Linq;
using kCura.Windows.Process;
using kCura.WinEDDS.Core.Model.Export.Process;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.Service.Export;

namespace kCura.WinEDDS.Core.Export
{
	public class ExtendedExporter : Exporter
	{
		public ExtendedExporter(ExportFile exportFile, Controller processController, ILoadFileHeaderFormatterFactory loadFileFormatterFactory) : base(exportFile, processController, loadFileFormatterFactory)
		{
		}

		public ExtendedExporter(ExportFile exportFile, Controller processController, IServiceFactory serviceFactory, ILoadFileHeaderFormatterFactory loadFileFormatterFactory) : base(exportFile, processController, serviceFactory, loadFileFormatterFactory)
		{
		}

		protected override ObjectExportInfo CreateObjectExportInfo()
		{
			return new ExtendedObjectExportInfo
			{
				SelectedViewFields = Settings.SelectedViewFields.ToList()
			};
		}
	}
}
