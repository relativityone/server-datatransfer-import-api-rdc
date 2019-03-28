using System.Text;
using System.Web;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;
using Relativity;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public class HtmlCellFormatter : ILoadFileCellFormatter
	{
		private const string ROW_PREFIX = "<tr>";
		private const string ROW_SUFFIX = "</tr>";
		private const string COLUMN_PREFIX = "<td>";
		private const string COLUMN_SUFFIX = "</td>";

		private readonly IFilePathTransformer _filePathTransformer;
		private readonly ExportFile _settings;

		public HtmlCellFormatter(ExportFile settings, IFilePathTransformer filePathTransformer)
		{
			_settings = settings;
			_filePathTransformer = filePathTransformer;
		}

		public string TransformToCell(string contents)
		{
			contents = HttpUtility.HtmlEncode(contents);
			return $"{COLUMN_PREFIX}{contents}{COLUMN_SUFFIX}";
		}

		public string RowPrefix => ROW_PREFIX;

		public string RowSuffix => ROW_SUFFIX;

		public string CreateImageCell(ObjectExportInfo artifact)
		{
			if (!_settings.ExportImages || !IsDocument())
			{
				return string.Empty;
			}

			return $"{COLUMN_PREFIX}{GetImagesHtmlString(artifact)}{COLUMN_SUFFIX}";
		}

		public string CreateNativeCell(string location, ObjectExportInfo artifact)
		{
			return $"{COLUMN_PREFIX}{GetNativeHtmlString(artifact, location)}{COLUMN_SUFFIX}";
		}

		private string GetNativeHtmlString(ObjectExportInfo artifact, string location)
		{
			if (IsDocument() && artifact.NativeCount == 0)
			{
				return string.Empty;
			}

			if (!IsDocument() && artifact.FileID <= 0)
			{
				return string.Empty;
			}

			string nativeFileName = artifact.NativeFileName(_settings.AppendOriginalFileName);
			return GetLink(location, nativeFileName);
		}

		private bool IsDocument()
		{
			return _settings.ArtifactTypeID == (int)ArtifactType.Document;
		}

		private string GetImagesHtmlString(ObjectExportInfo artifact)
		{
			StringBuilder retval = new StringBuilder();

			foreach (ImageExportInfo image in artifact.Images)
			{
				string location;
				if (_settings.VolumeInfo.CopyImageFilesFromRepository)
				{
					location = _filePathTransformer.TransformPath(image.TempLocation);
				}
				else
				{
					location = image.SourceLocation;
				}

				retval.Append(GetLink(location, image.FileName));
				if (_settings.TypeOfImage == ExportFile.ImageType.MultiPageTiff || _settings.TypeOfImage == ExportFile.ImageType.Pdf)
				{
					break;
				}
			}

			return retval.ToString();
		}

		private string GetLink(string href, string text)
		{
			return $"<a style='display:block' href='{href}'>{text}</a>";
		}
	}
}