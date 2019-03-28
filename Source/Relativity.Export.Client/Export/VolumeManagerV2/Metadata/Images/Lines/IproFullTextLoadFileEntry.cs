using System.IO;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines
{
	public abstract class IproFullTextLoadFileEntry : IFullTextLoadFileEntry
	{
		private TextReader _textReader;

		private readonly ILog _logger;

		private readonly IFullTextLineWriter _fullTextLineWriter;
		protected readonly IFieldService FieldService;
		protected readonly LongTextHelper LongTextHelper;

		protected IproFullTextLoadFileEntry(IFieldService fieldService, LongTextHelper longTextHelper, ILog logger, IFullTextLineWriter fullTextLineWriter)
		{
			FieldService = fieldService;
			LongTextHelper = longTextHelper;
			_logger = logger;
			_fullTextLineWriter = fullTextLineWriter;
		}

		public void WriteFullTextLine(ObjectExportInfo artifact, string batesNumber, int pageNumber, long pageOffset, IRetryableStreamWriter writer, CancellationToken token)
		{
			_logger.LogVerbose("Attempting to create Full text entry for image {batesNumber}.", batesNumber);
			if (pageNumber == 0)
			{
				_logger.LogVerbose("Processing first page - disposing old Text Reader and creating new one.");
				_textReader?.Dispose();
				_textReader = GetTextStream(artifact);
			}

			_fullTextLineWriter.WriteLine(batesNumber, pageOffset, writer, _textReader, token);

			_logger.LogVerbose("Successfully create Full text entry for image.");
		}

		private TextReader GetTextStream(ObjectExportInfo artifact)
		{
			if (LongTextHelper.IsTextTooLong(artifact, GetTextColumnName()))
			{
				string fileLocation = LongTextHelper.GetLongTextFileLocation(artifact, GetTextSourceFieldId(artifact));
				return new StreamReader(fileLocation);
			}

			string text = LongTextHelper.GetTextFromField(artifact, GetTextColumnName());
			return new StringReader(text);
		}

		protected abstract int GetTextSourceFieldId(ObjectExportInfo artifact);

		protected abstract string GetTextColumnName();

		public void Dispose()
		{
			_textReader?.Dispose();
		}
	}
}