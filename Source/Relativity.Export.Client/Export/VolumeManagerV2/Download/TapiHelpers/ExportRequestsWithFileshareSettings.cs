namespace Relativity.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;
	using System.Collections.Generic;

	public class ExportRequestsWithFileshareSettings
	{
		public IRelativityFileShareSettings FileshareSettings { get; }
		public IEnumerable<ExportRequest> Requests { get; }

		public ExportRequestsWithFileshareSettings(IRelativityFileShareSettings fileshareSettings, IEnumerable<ExportRequest> requests)
		{
			if (requests == null)
			{
				throw new ArgumentNullException($"Argument name {nameof(requests)} cannot be null.");
			}

			FileshareSettings = fileshareSettings;
			Requests = requests;
		}
	}
}