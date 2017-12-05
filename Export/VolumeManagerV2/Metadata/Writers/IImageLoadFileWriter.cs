using System;
using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public interface IImageLoadFileWriter: IDisposable
	{
		void Write(IList<KeyValuePair<string, string>> linesToWrite, ObjectExportInfo[] artifacts, CancellationToken cancellationToken);
	}
}