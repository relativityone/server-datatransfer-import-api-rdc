using System;
using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.Helpers;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives
{
	public class LoadFileData
	{
		private DeferredEntry _loadFileEntry;
		private ObjectExportInfo _artifact;
		private bool _hasWrittenColumnHeaderString;

		private readonly ILoadFileCellFormatter _loadFileCellFormatter;
		private readonly IFilePathProvider _filePathProvider;
		private readonly IFieldService _fieldLookupService;
		private readonly LongTextHelper _longTextHelper;
		private readonly StatisticsWrapper _statistics;

		/// <summary>
		///     TODO remove this
		/// </summary>
		private readonly ExportFile _exportSettings;

		public LoadFileData(ILoadFileCellFormatter loadFileCellFormatter, IFieldService fieldLookupService, StatisticsWrapper statistics, ExportFile exportSettings,
			IFilePathProvider filePathProvider, LongTextHelper longTextHelper)
		{
			_loadFileCellFormatter = loadFileCellFormatter;
			_fieldLookupService = fieldLookupService;
			_statistics = statistics;
			_exportSettings = exportSettings;
			_filePathProvider = filePathProvider;
			_longTextHelper = longTextHelper;

			_hasWrittenColumnHeaderString = false;
		}

		public IDictionary<int, ILoadFileEntry> AddLines(ObjectExportInfo[] artifacts)
		{
			IDictionary<int, ILoadFileEntry> loadFileEntries = new Dictionary<int, ILoadFileEntry>();

			if (!_hasWrittenColumnHeaderString)
			{
				_hasWrittenColumnHeaderString = true;
				ILoadFileEntry header = new CompletedLoadFileEntry(_fieldLookupService.GetColumnHeader());
				loadFileEntries.Add(-1, header);
			}

			foreach (var artifact in artifacts)
			{
				AddLine(artifact);
				loadFileEntries.Add(artifact.ArtifactID, _loadFileEntry);
			}
			return loadFileEntries;
		}

		private void AddLine(ObjectExportInfo artifact)
		{
			_artifact = artifact;
			_loadFileEntry = new DeferredEntry();

			AddPrefix();

			AddFieldsValue(artifact.Metadata);

			AddImageField();

			AddNativeFilePath();

			AddSuffix();

			AddNewLine();
		}

		private void AddPrefix()
		{
			string rowPrefix = _loadFileCellFormatter.RowPrefix;

			if (!string.IsNullOrEmpty(rowPrefix))
			{
				_loadFileEntry.AddStringEntry(rowPrefix);
			}
		}

		private void AddFieldsValue(object[] record)
		{
			List<ViewFieldInfo> fields = _fieldLookupService.GetColumns().ToList();
			for (int i = 0; i < fields.Count; i++)
			{
				ViewFieldInfo field = fields[i];

				object rawFieldValue = record[_fieldLookupService.GetOrdinalIndex(field.AvfColumnName)];

				if (_longTextHelper.IsLongTextField(field))
				{
					HandleTextField(rawFieldValue, field);
				}
				else
				{
					HandleNonTextField(rawFieldValue, field);
				}

				if (i != fields.Count - 1 && !_exportSettings.LoadFileIsHtml)
				{
					_loadFileEntry.AddStringEntry(_exportSettings.RecordDelimiter.ToString());
				}
			}
		}

		private void HandleTextField(object rawFieldValue, ViewFieldInfo field)
		{
			if (_longTextHelper.IsTextTooLong(_artifact, field.AvfColumnName))
			{
				//TODO handle too long text
			}

			//TODO
			//Slong extractedTextByteCount = _longTextData.AddLongText(_loadFileEntry, rawFieldValue, field, downloadedTextTempFile, _artifact, 1 /*currentVolumeNumber*/, 1 /*currentSubdirectoryNumber*/);
			//_statistics.TotalExtractedTextFileLength += extractedTextByteCount;
		}

		private void HandleNonTextField(object rawFieldValue, ViewFieldInfo field)
		{
			string fieldValue = FieldValueHelper.ConvertToString(rawFieldValue, field, _exportSettings.MultiRecordDelimiter);
			_loadFileEntry.AddStringEntry(_loadFileCellFormatter.TransformToCell(fieldValue));
		}

		private void AddImageField()
		{
			string imagesCell = _loadFileCellFormatter.CreateImageCell(_artifact);
			if (!string.IsNullOrEmpty(imagesCell))
			{
				_loadFileEntry.AddStringEntry(imagesCell);
			}
		}

		private void AddNativeFilePath()
		{
			if (_exportSettings.ExportNative)
			{
				string nativeLocationCell;
				if (_exportSettings.VolumeInfo.CopyNativeFilesFromRepository)
				{
					string nativeLocation = _filePathProvider.GetPathForLoadFile(_artifact.NativeTempLocation);
					nativeLocationCell = _loadFileCellFormatter.CreateNativeCell(nativeLocation, _artifact);
				}
				else
				{
					nativeLocationCell = _loadFileCellFormatter.CreateNativeCell(_artifact.NativeSourceLocation, _artifact);
				}
				_loadFileEntry.AddStringEntry(nativeLocationCell);
			}
		}

		private void AddSuffix()
		{
			string rowSuffix = _loadFileCellFormatter.RowSuffix;
			if (!string.IsNullOrEmpty(rowSuffix))
			{
				_loadFileEntry.AddStringEntry(rowSuffix);
			}
		}

		private void AddNewLine()
		{
			_loadFileEntry.AddStringEntry(Environment.NewLine);
		}
	}
}