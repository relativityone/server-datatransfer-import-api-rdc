using System;
using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public interface ILoadFileWriter : IStateful, IDisposable
	{
		void Write(IDictionary<int, ILoadFileEntry> linesToWrite, ObjectExportInfo[] artifacts, CancellationToken cancellationToken);
	}
}