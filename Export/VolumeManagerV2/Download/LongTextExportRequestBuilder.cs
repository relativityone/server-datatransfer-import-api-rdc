using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class LongTextExportRequestBuilder
	{
		private readonly LongTextRepository _longTextRepository;
		private readonly ILog _logger;

		public LongTextExportRequestBuilder(LongTextRepository longTextRepository, ILog logger)
		{
			_longTextRepository = longTextRepository;
			_logger = logger;
		}

		public IEnumerable<LongTextExportRequest> Create(ObjectExportInfo artifact)
		{
			_logger.LogVerbose("Creating TextExportRequests based on LongTextRepository.");
			return _longTextRepository.GetLongTexts().Select(x => x.ExportRequest).Where(x => x != null);
		}
	}
}