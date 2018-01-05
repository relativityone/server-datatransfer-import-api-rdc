using Castle.Core;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class LongTextToLoadFile : ILongTextHandler
	{
		private readonly LongTextHelper _longTextHelper;
		private readonly ILongTextHandler _tooLongTextToLoadFile;
		private readonly ILongTextHandler _notTooLongTextToLoadFile;
		private readonly ILog _logger;

		public LongTextToLoadFile(LongTextHelper longTextHelper, TooLongTextToLoadFile tooLongTextToLoadFile, NotTooLongTextToLoadFile notTooLongTextToLoadFile, ILog logger)
			: this(longTextHelper, tooLongTextToLoadFile, (ILongTextHandler) notTooLongTextToLoadFile, logger)
		{
		}

		/// <summary>
		///     Used for testing
		/// </summary>
		/// <param name="longTextHelper"></param>
		/// <param name="tooLongTextToLoadFile"></param>
		/// <param name="notTooLongTextToLoadFile"></param>
		/// <param name="logger"></param>
		[DoNotSelect]
		public LongTextToLoadFile(LongTextHelper longTextHelper, ILongTextHandler tooLongTextToLoadFile, ILongTextHandler notTooLongTextToLoadFile, ILog logger)
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