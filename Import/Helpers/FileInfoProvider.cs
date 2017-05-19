using System;
using kCura.OI.FileID;
using kCura.Utility;
using Polly;
using Polly.Retry;

namespace kCura.WinEDDS.Core.Import.Helpers
{
	public class FileInfoProvider : IFileInfoProvider
	{
		private readonly ITransferConfig _transferConfig;
		private readonly IFileHelper _fileHelper;

		public event EventHandler<RobustIoReporter.IoWarningEventArgs> IoWarningOccurred;

		public FileInfoProvider(ITransferConfig transferConfig, IFileHelper fileHelper)
		{
			_transferConfig = transferConfig;
			_fileHelper = fileHelper;
		}

		public FileIDData GetFileId(string fileName)
		{
			return Manager.Instance.GetFileIDDataByFilePath(fileName);
		}

		public long GetFileSize(FileMetadata fileMetadata)
		{
			long fileSize = 0;
			RetryPolicy policy = Policy
				.Handle<Exception>().WaitAndRetry(_transferConfig.IoErrorNumberOfRetries, 
					count => TimeSpan.FromSeconds(_transferConfig.IoErrorWaitTimeInSeconds), (exception, sleepDurProvider, retryCount, context) =>
				{
					if (_transferConfig.DisableNativeLocationValidation
						&& exception is ArgumentException
						&& exception.Message.Contains("Illegal characters in path."))
					{
						throw new RobustIoReporter.FileInfoFailedException($"File {fileMetadata.FullFilePath} not found: illegal characters in path.");
					}
					IoWarningOccurred?.Invoke(this, new RobustIoReporter.IoWarningEventArgs(0, exception, fileMetadata.LineNumber));
				});
			policy.Execute(() => fileSize = _fileHelper.GetFileSize(fileMetadata.FullFilePath));
			return fileSize;
		}
	}
}
