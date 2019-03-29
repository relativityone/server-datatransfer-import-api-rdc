namespace Relativity.Export.VolumeManagerV2.Metadata.Writers
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	using Relativity.Export.VolumeManagerV2.Batches;

	public interface ILoadFileWriter : IStateful, IDisposable
	{
		void Write(IDictionary<int, ILoadFileEntry> linesToWrite, ObjectExportInfo[] artifacts, CancellationToken cancellationToken);
	}
}