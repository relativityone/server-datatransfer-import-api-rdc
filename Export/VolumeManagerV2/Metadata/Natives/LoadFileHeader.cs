using System.Collections.Generic;
using kCura.WinEDDS.LoadFileEntry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives
{
	public class LoadFileHeader
	{
		private bool _hasWrittenColumnHeaderString;

		private readonly IFieldService _fieldLookupService;
		private readonly ILog _logger;

		public LoadFileHeader(IFieldService fieldLookupService, ILog logger)
		{
			_fieldLookupService = fieldLookupService;
			_logger = logger;

			_hasWrittenColumnHeaderString = false;
		}

		public void AddHeader(IDictionary<int, ILoadFileEntry> loadFileEntries)
		{
			if (!_hasWrittenColumnHeaderString)
			{
				_logger.LogVerbose("Adding header to load file.");
				ILoadFileEntry header = CreateHeader();
				loadFileEntries.Add(-1, header);
			}
		}

		private ILoadFileEntry CreateHeader()
		{
			_hasWrittenColumnHeaderString = true;
			return new CompletedLoadFileEntry(_fieldLookupService.GetColumnHeader());
		}
	}
}