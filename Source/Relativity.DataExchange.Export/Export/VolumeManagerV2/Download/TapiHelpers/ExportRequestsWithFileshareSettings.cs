namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;
	using System.Collections.Generic;

	public class ExportRequestsWithFileshareSettings
	{
		public IRelativityFileShareSettings FileshareSettings { get; }
		public IEnumerable<ExportRequest> Requests { get; }

		public ExportRequestsWithFileshareSettings(IRelativityFileShareSettings fileshareSettings, IEnumerable<ExportRequest> requests)
		{
			FileshareSettings = fileshareSettings;
			Requests = requests ?? throw new ArgumentNullException($"Argument name {nameof(requests)} cannot be null.");
		}
	}
}