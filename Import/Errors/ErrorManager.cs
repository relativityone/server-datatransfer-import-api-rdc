using System.IO;
using kCura.WinEDDS.Api;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ErrorManager : IErrorManager
	{
		private readonly IArtifactReader _artifactReader;
		private readonly IClientErrors _clientErrors;
		private readonly IAllErrors _allErrors;

		private readonly IFileHelper _fileHelper;
		private readonly ErrorFileNames _errorFileNames;

		public ErrorManager(IArtifactReader artifactReader, IClientErrors clientErrors, IAllErrors allErrors, IFileHelper fileHelper, ErrorFileNames errorFileNames)
		{
			_artifactReader = artifactReader;
			_clientErrors = clientErrors;
			_allErrors = allErrors;
			_fileHelper = fileHelper;
			_errorFileNames = errorFileNames;
		}

		/// <summary>
		/// Old name: ExportServerErrors - TODO remove this comment after integrating with RDC
		/// </summary>
		/// <param name="exportLocation"></param>
		/// <param name="loadFilePath"></param>
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

			return _artifactReader.ManageErrorRecords(allErrorsTempFile, clientErrorsTempFile);
		}
	}
}