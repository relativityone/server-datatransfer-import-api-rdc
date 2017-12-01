using System.IO;
using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	/// <summary>
	///     TODO NotTooLong... heh
	/// </summary>
	public class NotTooLongTextToLoadFile : ILongTextHandler
	{
		private readonly LongTextHelper _longTextHelper;
		private readonly FromFieldToLoadFileWriter _fileWriter;

		public NotTooLongTextToLoadFile(LongTextHelper longTextHelper, FromFieldToLoadFileWriter fileWriter)
		{
			_longTextHelper = longTextHelper;
			_fileWriter = fileWriter;
		}

		public void HandleLongText(ObjectExportInfo artifact, ViewFieldInfo field, DeferredEntry lineEntry)
		{
			using (var memoryStream = new MemoryStream())
			{
				using (var streamWriter = new StreamWriter(memoryStream, Encoding.Default))
				{
					string unformattedString = _longTextHelper.GetTextFromField(artifact, field.AvfColumnName);

					_fileWriter.WriteLongTextFileToDatFile(streamWriter, unformattedString, Encoding.Default);

					streamWriter.Flush();
					string text = Encoding.Default.GetString(memoryStream.ToArray());
					lineEntry.AddStringEntry(text);
				}
			}
		}
	}
}