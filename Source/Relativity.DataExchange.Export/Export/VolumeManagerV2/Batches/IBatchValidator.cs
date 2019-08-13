﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.Batches
{
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	public interface IBatchValidator
	{
		void ValidateExportedBatch(ObjectExportInfo[] artifacts, CancellationToken cancellationToken);
	}
}