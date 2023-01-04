namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Represents a class object that relates a collection of export requests with file share settings.
	/// </summary>
	public class ExportRequestsWithFileshareSettings
	{
		public ExportRequestsWithFileshareSettings(IRelativityFileShareSettings settings, IEnumerable<ExportRequest> requests)
		{
			// Note: null file share settings is supported and will eventually use transfer modes that don't depend on file share configurations.
			if (requests == null)
			{
				throw new ArgumentNullException($"Argument name {nameof(requests)} cannot be null.");
			}

			this.FileshareSettings = settings;
			this.Requests = requests;
		}

		public IRelativityFileShareSettings FileshareSettings
		{
			get;
		}

		public IEnumerable<ExportRequest> Requests
		{
			get;
		}
	}
}