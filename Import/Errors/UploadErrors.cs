using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class UploadErrors : IUploadErrors
	{
		private readonly IErrorContainer _errorContainer;

		public UploadErrors(IErrorContainer errorContainer)
		{
			_errorContainer = errorContainer;
		}

		public void HandleUploadErrors(IDictionary<FileMetadata, UploadResult> uploadResults)
		{
			foreach (var fileMetadata in uploadResults.Keys)
			{
				if (!uploadResults[fileMetadata].Success)
				{
					LineError lineError = new LineError
					{
						LineNumber = fileMetadata.LineNumber,
						Message = uploadResults[fileMetadata].ErrorMessage,
						ErrorType = ErrorType.client
					};
					_errorContainer.WriteError(lineError);
				}
			}
		}
	}
}