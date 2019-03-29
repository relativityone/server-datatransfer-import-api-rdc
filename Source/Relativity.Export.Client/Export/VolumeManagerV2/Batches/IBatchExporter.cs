﻿namespace Relativity.Export.VolumeManagerV2.Batches
{
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	public interface IBatchExporter
	{
		void Export(ObjectExportInfo[] artifacts, CancellationToken cancellationToken);
	}
}