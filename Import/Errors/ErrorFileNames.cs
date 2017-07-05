using System.IO;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ErrorFileNames
	{
		private const string _DEFAULT_ERROR_FILE_EXTENSION = ".txt";
		private const string _ERROR_REPORT_FILE_EXTENSION = ".csv";

		private const string _ERROR_LINES_FILE_NAME = "_ErrorLines_";
		private const string _ERROR_REPORT_FILE_NAME = "_ErrorReport_";

		private readonly IDateTimeHelper _dateTimeHelper;

		public ErrorFileNames(IDateTimeHelper dateTimeHelper)
		{
			_dateTimeHelper = dateTimeHelper;
		}

		public string GetErrorLinesFileName(string loadFilePath)
		{
			var fileExtension = GetFileExtension(loadFilePath);

			return GetFileName(loadFilePath, _ERROR_LINES_FILE_NAME, fileExtension);
		}

		private static string GetFileExtension(string loadFilePath)
		{
			var fileExtension = Path.GetExtension(loadFilePath);
			if (string.IsNullOrEmpty(fileExtension))
			{
				fileExtension = _DEFAULT_ERROR_FILE_EXTENSION;
			}
			return fileExtension;
		}

		public string GetErrorReportFileName(string loadFilePath)
		{
			return GetFileName(loadFilePath, _ERROR_REPORT_FILE_NAME, _ERROR_REPORT_FILE_EXTENSION);
		}

		private string GetFileName(string loadFilePath, string fileName, string fileExtension)
		{
			var loadFileNameWithoutExtension = GetFilePrefix(loadFilePath);

			var fileNameSuffix = GetFileSuffix();

			return $"{loadFileNameWithoutExtension}{fileName}{fileNameSuffix}{fileExtension}";
		}

		private static string GetFilePrefix(string loadFilePath)
		{
			return Path.GetFileNameWithoutExtension(loadFilePath);
		}

		private string GetFileSuffix()
		{
			return _dateTimeHelper.Now().Ticks.ToString();
		}
	}
}