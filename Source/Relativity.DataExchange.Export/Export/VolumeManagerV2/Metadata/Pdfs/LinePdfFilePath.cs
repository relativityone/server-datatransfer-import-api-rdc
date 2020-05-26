// ----------------------------------------------------------------------------
// <copyright file="LinePdfFilePath.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Pdfs
{
	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Logger;
	using Relativity.Logging;

	public class LinePdfFilePath : ILinePdfFilePath
	{
		private readonly ILoadFileCellFormatter _loadFileCellFormatter;
		private readonly ExportFile _exportSettings;
		private readonly IFilePathTransformer _filePathTransformer;
		private readonly ILog _logger;

		public LinePdfFilePath(ILoadFileCellFormatter loadFileCellFormatter, ExportFile exportSettings, IFilePathTransformer filePathTransformer, ILog logger)
		{
			this._loadFileCellFormatter = loadFileCellFormatter;
			this._exportSettings = exportSettings;
			this._filePathTransformer = filePathTransformer;
			this._logger = logger;
		}

		public void AddPdfFilePath(DeferredEntry loadFileEntry, ObjectExportInfo artifact)
		{
			if (_exportSettings.ExportPdf)
			{
				_logger.LogVerbose("Exporting searchable PDF files, so adding path to load file entry.");
				string pdfLocationCell;
				if (_exportSettings.VolumeInfo.CopyPdfFilesFromRepository)
				{
					string pdfLocation = string.IsNullOrWhiteSpace(artifact.PdfDestinationLocation) ? artifact.PdfDestinationLocation : _filePathTransformer.TransformPath(artifact.PdfDestinationLocation);
					pdfLocationCell = _loadFileCellFormatter.CreatePdfCell(pdfLocation, artifact);
					_logger.LogVerbose("Copying searchable PDF files, so path is local {path}.", pdfLocation.Secure());
				}
				else
				{
					_logger.LogVerbose("Not copying searchable PDF files, so path is remote {path}.", artifact.PdfSourceLocation.Secure());
					pdfLocationCell = _loadFileCellFormatter.CreatePdfCell(artifact.PdfSourceLocation, artifact);
				}

				loadFileEntry.AddStringEntry(pdfLocationCell);
			}
		}
	}
}