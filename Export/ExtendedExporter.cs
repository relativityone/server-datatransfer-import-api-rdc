using System.Collections.Generic;
using System.Linq;
using kCura.Windows.Process;
using kCura.WinEDDS.Core.Model.Export;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.FileNaming.CustomFileNaming;
using kCura.WinEDDS.Service.Export;

namespace kCura.WinEDDS.Core.Export
{
	public class ExtendedExporter : Exporter
	{
		#region Members

		private ExtendedExportFile ExportSettings => (ExtendedExportFile)base.Settings;

		#endregion Members

		#region Constructors

		public ExtendedExporter(ExtendedExportFile exportFile, Controller processController,ILoadFileHeaderFormatterFactory loadFileFormatterFactory) :
			base(exportFile, processController, loadFileFormatterFactory)
		{
		}

		public ExtendedExporter(ExtendedExportFile exportFile, Controller processController, IServiceFactory serviceFactory, ILoadFileHeaderFormatterFactory loadFileFormatterFactory,
			IExportConfig exportConfig) : base(exportFile, processController, serviceFactory, loadFileFormatterFactory, exportConfig)
		{
		}

		#endregion Constructors

		protected override ObjectExportInfo CreateObjectExportInfo()
		{
			return new ExtendedObjectExportInfo(FieldLookupService)
			{
				SelectedNativeFileNameViewFields = ExportSettings.SelectedNativesNameViewFields.ToList()
			};
		}

		protected override List<int> GetAvfIds()
		{
			var selectedAvfIds = new HashSet<int>(base.GetAvfIds());
			// Add Native File Name part field id
			selectedAvfIds.UnionWith(ExportSettings.SelectedNativesNameViewFields.Select(item => item.AvfId));
			return selectedAvfIds.ToList();
		}
		
	}
}
