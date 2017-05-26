using System.IO;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ErrorManager : IErrorManager
	{
		private readonly IImportMetadata _importMetadata;
		private readonly IClientErrors _clientErrors;
		private readonly IAllErrors _allErrors;

		private readonly IFileHelper _fileHelper;
		private readonly ErrorFileNames _errorFileNames;

		public ErrorManager(IClientErrors clientErrors, IAllErrors allErrors, IImportMetadata importMetadata, IFileHelper fileHelper, ErrorFileNames errorFileNames)
		{
			_importMetadata = importMetadata;
			_clientErrors = clientErrors;
			_allErrors = allErrors;
			_fileHelper = fileHelper;
			_errorFileNames = errorFileNames;
		}

		public void ExportErrors(string exportLocation, string loadFilePath)
		{
			var errorFileName = _errorFileNames.GetErrorLinesFileName(loadFilePath);
			var errorReportFileName = _errorFileNames.GetErrorReportFileName(loadFilePath);

			ExportErrorFile(Path.Combine(exportLocation, errorFileName));
			ExportErrorReport(Path.Combine(exportLocation, errorReportFileName));
		}

		public void ExportErrorReport(string exportLocation)
		{
			if (_allErrors.HasErrors())
			{
				var tempFile = _allErrors.WriteErrorsToTempFile();
				_fileHelper.Copy(tempFile, exportLocation, true);
			}
			else
			{
				_fileHelper.Create(exportLocation).Close();
			}
		}

		public void ExportErrorFile(string exportLocation)
		{
			if (!_allErrors.HasErrors())
			{
				return;
			}
			var mergedFile = PrepareMergedFile();
			_fileHelper.Copy(mergedFile, exportLocation, true);
		}

		private string PrepareMergedFile()
		{
			var allErrorsTempFile = _allErrors.WriteErrorsToTempFile();
			var clientErrorsTempFile = _clientErrors.WriteErrorsToTempFile();

			return _importMetadata.ArtifactReader.ManageErrorRecords(allErrorsTempFile, clientErrorsTempFile);
		}
	}
}