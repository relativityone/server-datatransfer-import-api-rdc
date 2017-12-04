using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class LongTextToLoadFile : ILongTextHandler
	{
		private readonly LongTextHelper _longTextHelper;
		private readonly TooLongTextToLoadFile _tooLongTextToLoadFile;
		private readonly NotTooLongTextToLoadFile _notTooLongTextToLoadFile;
		private readonly ILog _logger;

		public LongTextToLoadFile(LongTextHelper longTextHelper, TooLongTextToLoadFile tooLongTextToLoadFile, NotTooLongTextToLoadFile notTooLongTextToLoadFile, ILog logger)
		{
			_longTextHelper = longTextHelper;
			_tooLongTextToLoadFile = tooLongTextToLoadFile;
			_notTooLongTextToLoadFile = notTooLongTextToLoadFile;
			_logger = logger;
		}


		public void HandleLongText(ObjectExportInfo artifact, ViewFieldInfo field, DeferredEntry lineEntry)
		{
			if (_longTextHelper.IsTextTooLong(artifact, field.AvfColumnName))
			{
				_logger.LogVerbose("LongText too long - passing it to {type}.", nameof(TooLongTextToLoadFile));
				_tooLongTextToLoadFile.HandleLongText(artifact, field, lineEntry);
			}
			else
			{
				_logger.LogVerbose("LongText in memory - passing it to {type}.", nameof(NotTooLongTextToLoadFile));
				_notTooLongTextToLoadFile.HandleLongText(artifact, field, lineEntry);
			}
		}
	}
}