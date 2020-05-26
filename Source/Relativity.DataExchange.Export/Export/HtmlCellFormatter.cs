namespace Relativity.DataExchange.Export
{
	using System.Text;
	using System.Web;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Service;

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
			this._settings = settings;
			this._filePathTransformer = filePathTransformer;
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
			if (!this._settings.ExportImages || !this.IsDocument())
			{
				return string.Empty;
			}

			return $"{COLUMN_PREFIX}{this.GetImagesHtmlString(artifact)}{COLUMN_SUFFIX}";
		}

		public string CreateNativeCell(string location, ObjectExportInfo artifact)
		{
			return $"{COLUMN_PREFIX}{this.GetNativeHtmlString(artifact, location)}{COLUMN_SUFFIX}";
		}

		public string CreatePdfCell(string location, ObjectExportInfo artifact)
		{
			return $"{COLUMN_PREFIX}{this.GetPdfHtmlString(artifact, location)}{COLUMN_SUFFIX}";
		}

		private string GetNativeHtmlString(ObjectExportInfo artifact, string location)
		{
			if (this.IsDocument() && artifact.NativeCount == 0)
			{
				return string.Empty;
			}

			if (!this.IsDocument() && artifact.FileID <= 0)
			{
				return string.Empty;
			}

			string nativeFileName = artifact.NativeFileName(this._settings.AppendOriginalFileName);
			return this.GetLink(location, nativeFileName);
		}

		private string GetPdfHtmlString(ObjectExportInfo artifact, string location)
		{
			if (!this.IsDocument() || !artifact.HasPdf)
			{
				return string.Empty;
			}

			string pdfFileName = artifact.PdfFileName(artifact.IdentifierValue, this._settings.AppendOriginalFileName);
			return this.GetLink(location, pdfFileName);
		}

		private bool IsDocument()
		{
			return this._settings.ArtifactTypeID == (int)ArtifactType.Document;
		}

		private string GetImagesHtmlString(ObjectExportInfo artifact)
		{
			StringBuilder returnValue = new StringBuilder();

			foreach (ImageExportInfo image in artifact.Images)
			{
				string location;
				if (this._settings.VolumeInfo.CopyImageFilesFromRepository)
				{
					location = this._filePathTransformer.TransformPath(image.TempLocation);
				}
				else
				{
					location = image.SourceLocation;
				}

				returnValue.Append(this.GetLink(location, image.FileName));
				if (this._settings.TypeOfImage == ExportFile.ImageType.MultiPageTiff || this._settings.TypeOfImage == ExportFile.ImageType.Pdf)
				{
					break;
				}
			}

			return returnValue.ToString();
		}

		private string GetLink(string href, string text)
		{
			return $"<a style='display:block' href='{href}'>{text}</a>";
		}
	}
}