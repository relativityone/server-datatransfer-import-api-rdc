namespace Relativity.Export.VolumeManagerV2.Download
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	public class EmptyExportRequestBuilder : IExportRequestBuilder
	{
		public IList<ExportRequest> Create(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			return Enumerable.Empty<ExportRequest>().ToList();
		}
	}
}