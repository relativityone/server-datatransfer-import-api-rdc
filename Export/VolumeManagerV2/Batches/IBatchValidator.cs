﻿using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public interface IBatchValidator
	{
		void ValidateExportedBatch(ObjectExportInfo[] artifacts);
	}
}