using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives
{
	public class LoadFileLine
	{
		private readonly LinePrefix _prefix;
		private readonly LineFieldsValue _fieldsValue;
		private readonly LineImageField _imageField;
		private readonly LineNativeFilePath _nativeFilePath;
		private readonly LineSuffix _suffix;
		private readonly LineNewLine _newLine;

		public LoadFileLine(LinePrefix prefix, LineFieldsValue fieldsValue, LineImageField imageField, LineNativeFilePath nativeFilePath, LineSuffix suffix, LineNewLine newLine)
		{
			_prefix = prefix;
			_fieldsValue = fieldsValue;
			_imageField = imageField;
			_nativeFilePath = nativeFilePath;
			_suffix = suffix;
			_newLine = newLine;
		}

		public ILoadFileEntry CreateLine(ObjectExportInfo artifact)
		{
			DeferredEntry loadFileEntry = new DeferredEntry();

			_prefix.AddPrefix(loadFileEntry);

			_fieldsValue.AddFieldsValue(loadFileEntry, artifact);

			_imageField.AddImageField(loadFileEntry, artifact);

			_nativeFilePath.AddNativeFilePath(loadFileEntry, artifact);

			_suffix.AddSuffix(loadFileEntry);

			_newLine.AddNewLine(loadFileEntry);

			return loadFileEntry;
		}
	}
}