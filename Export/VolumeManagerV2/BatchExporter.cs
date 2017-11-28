using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Requests;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public class BatchExporter : IBatchExporter
	{
		private readonly NativeExportRequestBuilder _nativeExportRequestBuilder;
		private readonly ImageExportRequestBuilder _imageExportRequestBuilder;

		public BatchExporter(NativeExportRequestBuilder nativeExportRequestBuilder, ImageExportRequestBuilder imageExportRequestBuilder)
		{
			_nativeExportRequestBuilder = nativeExportRequestBuilder;
			_imageExportRequestBuilder = imageExportRequestBuilder;
		}

		public void Export(ObjectExportInfo[] artifacts, object[] records, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			List<ExportRequest> exportRequests = new List<ExportRequest>();

			foreach (var artifact in artifacts)
			{
				ExportRequest nativeExportRequest = _nativeExportRequestBuilder.Create(artifact);
				exportRequests.Add(nativeExportRequest);

				foreach (var image in artifact.Images.Cast<ImageExportInfo>())
				{
					ExportRequest imageExportRequest = _imageExportRequestBuilder.Create(image);
					exportRequests.Add(imageExportRequest);
				}
			}

			exportRequests.RemoveAll(x => x == null);

			//TODO
		}
	}
}