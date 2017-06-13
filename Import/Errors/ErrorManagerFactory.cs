namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ErrorManagerFactory : IErrorManagerFactory
	{
		private readonly IClientErrors _clientErrors;
		private readonly IAllErrors _allErrors;
		private readonly IFileHelper _fileHelper;
		private readonly ErrorFileNames _errorFileNames;

		public ErrorManagerFactory(IClientErrors clientErrors, IAllErrors allErrors, IFileHelper fileHelper, ErrorFileNames errorFileNames)
		{
			_clientErrors = clientErrors;
			_allErrors = allErrors;
			_fileHelper = fileHelper;
			_errorFileNames = errorFileNames;
		}

		public IErrorManager Create(IImportMetadata importMetadata)
		{
			return new ErrorManager(_clientErrors, _allErrors, importMetadata, _fileHelper, _errorFileNames);
		}
	}
}