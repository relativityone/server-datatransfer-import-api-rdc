using Castle.Core;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Delimiter;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class LongTextHandler : ILongTextHandler
	{
		private readonly ILongTextHandler _textPrecedenceHandler;
		private readonly ILongTextHandler _textToLoadFile;
		private readonly IDelimiter _delimiter;
		private readonly ILog _logger;

		public LongTextHandler(ILongTextHandler textPrecedenceHandler, LongTextToLoadFile textToLoadFile, IDelimiter delimiter, ILog logger)
			: this(textPrecedenceHandler, (ILongTextHandler) textToLoadFile, delimiter, logger)
		{
		}

		/// <summary>
		///     Used for testing
		/// </summary>
		/// <param name="textPrecedenceHandler"></param>
		/// <param name="textToLoadFile"></param>
		/// <param name="delimiter"></param>
		/// <param name="logger"></param>
		[DoNotSelect]
		public LongTextHandler(ILongTextHandler textPrecedenceHandler, ILongTextHandler textToLoadFile, IDelimiter delimiter, ILog logger)
		{
			_textPrecedenceHandler = textPrecedenceHandler;
			_textToLoadFile = textToLoadFile;
			_delimiter = delimiter;
			_logger = logger;
		}

		public void HandleLongText(ObjectExportInfo artifact, ViewFieldInfo field, DeferredEntry lineEntry)
		{
			lineEntry.AddStringEntry(_delimiter.Start);

			if (field is CoalescedTextViewField)
			{
				_logger.LogVerbose("Handling LongText using TextPrecedence handler for field {fieldName}.", field.AvfColumnName);
				_textPrecedenceHandler.HandleLongText(artifact, field, lineEntry);
			}
			else
			{
				_logger.LogVerbose("Writing LongText to load file for field {fieldName}.", field.AvfColumnName);
				_textToLoadFile.HandleLongText(artifact, field, lineEntry);
			}

			lineEntry.AddStringEntry(_delimiter.End);
		}
	}
}