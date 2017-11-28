using System;
using kCura.Windows.Process;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public interface IStatus
	{
		void WriteWarning(string warning);
		void WriteError(string error);
		void WriteImgProgressError(ObjectExportInfo artifact, int imageIndex, Exception ex, string notes);
		void WriteStatusLine(EventType type, string line, bool isEssential);
	}
}