using System.IO;
using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class NotTooLongTextToLoadFile : ILongTextHandler
	{
		private readonly ExportFile _exportSettings;
		private readonly LongTextHelper _longTextHelper;
		private readonly FromFieldToLoadFileWriter _fileWriter;
		private readonly ILog _logger;

		public NotTooLongTextToLoadFile(LongTextHelper longTextHelper, FromFieldToLoadFileWriter fileWriter, ILog logger, ExportFile exportSettings)
		{
			_longTextHelper = longTextHelper;
			_fileWriter = fileWriter;
			_logger = logger;
			_exportSettings = exportSettings;
		}

		public void HandleLongText(ObjectExportInfo artifact, ViewFieldInfo field, DeferredEntry lineEntry)
		{
			_logger.LogVerbose("Writing text from memory to memory stream for formatting. Passing value to line entry. Field {fieldName}.", field.AvfColumnName);
			using (var memoryStream = new MemoryStream())
			{
				using (var streamWriter = new StreamWriter(memoryStream, _exportSettings.LoadFileEncoding))
				{
					string unformattedString = _longTextHelper.GetTextFromField(artifact, field.AvfColumnName);

					_fileWriter.WriteLongTextFileToDatFile(streamWriter, unformattedString, Encoding.Default);

					streamWriter.Flush();
					memoryStream.Position = 0;
					using (var streamReader = new StreamReader(memoryStream, _exportSettings.LoadFileEncoding))
					{
						string text = streamReader.ReadToEnd();
						lineEntry.AddStringEntry(text);
					}
				}
			}
		}
	}
}