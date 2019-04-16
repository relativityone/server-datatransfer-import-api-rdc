﻿namespace Relativity.Export.VolumeManagerV2.Metadata.Natives
{
	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	using Relativity.Logging;

	public class LoadFileLine : ILoadFileLine
	{
		private readonly LinePrefix _prefix;
		private readonly ILineFieldsValue _fieldsValue;
		private readonly LineImageField _imageField;
		private readonly ILineNativeFilePath _nativeFilePath;
		private readonly LineSuffix _suffix;
		private readonly LineNewLine _newLine;
		private readonly ILog _logger;

		public LoadFileLine(LinePrefix prefix, ILineFieldsValue fieldsValue, LineImageField imageField, ILineNativeFilePath nativeFilePath, LineSuffix suffix, LineNewLine newLine,
			ILog logger)
		{
			_prefix = prefix;
			_fieldsValue = fieldsValue;
			_imageField = imageField;
			_nativeFilePath = nativeFilePath;
			_suffix = suffix;
			_newLine = newLine;
			_logger = logger;
		}

		public ILoadFileEntry CreateLine(ObjectExportInfo artifact)
		{
			_logger.LogVerbose("Creating load file entry for artifact {artifactId}.", artifact.ArtifactID);

			DeferredEntry loadFileEntry = new DeferredEntry();

			_prefix.AddPrefix(loadFileEntry);

			_fieldsValue.AddFieldsValue(loadFileEntry, artifact);

			_imageField.AddImageField(loadFileEntry, artifact);

			_nativeFilePath.AddNativeFilePath(loadFileEntry, artifact);

			_suffix.AddSuffix(loadFileEntry);

			_newLine.AddNewLine(loadFileEntry);

			_logger.LogVerbose("Load file entry created.");
			return loadFileEntry;
		}
	}
}