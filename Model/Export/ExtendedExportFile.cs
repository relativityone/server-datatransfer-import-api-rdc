
using System.Collections.Generic;
using ZetaLongPaths.Native.FileOperations.Interop;

namespace kCura.WinEDDS.Core.Model.Export
{
	public class ExtendedExportFile : ExportFile
	{
		public ExtendedExportFile(int artifactTypeID) : base(artifactTypeID)
		{
		}

		public List<ViewFieldInfo> SelectedNativesNameViewFields;

		
	}
}
