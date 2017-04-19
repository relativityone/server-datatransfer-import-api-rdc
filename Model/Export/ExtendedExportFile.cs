
using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Model.Export
{
	public class ExtendedExportFile : ExportFile
	{
		public ExtendedExportFile(int artifactTypeID) : base(artifactTypeID)
		{
		}

		/// <summary>
		/// This property will hold information about fields that values will be used to format native file names
		/// </summary>
		public List<ViewFieldInfo> SelectedNativesNameViewFields;

	}
}
