namespace Relativity.Export.VolumeManagerV2.Download
{
	using System.Collections.Generic;
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	public interface IExportRequestBuilder
	{
		IList<ExportRequest> Create(ObjectExportInfo artifact, CancellationToken cancellationToken);
	}
}