using System;
using System.Collections.Generic;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Download
{
	public class ExportRequestRepositoryStub : IExportRequestRepository
	{
		public IList<ExportRequest> ExportRequests { get; } = new List<ExportRequest>();

		public IList<ExportRequest> GetExportRequests()
		{
			return ExportRequests;
		}
	}
}