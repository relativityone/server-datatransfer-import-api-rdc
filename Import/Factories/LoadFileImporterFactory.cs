using System;
using kCura.Windows.Process;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Status;
using Relativity;

namespace kCura.WinEDDS.Core.Import.Factories
{
	public class LoadFileImporterFactory : ILoadFileImporterFactory
	{
		private readonly IImportJobFactory _jobFactory;
		private readonly ITransferConfig _config;
		private readonly IErrorContainer _errorContainer;
		private readonly IImportStatusManager _importStatusManager;

		public LoadFileImporterFactory(IImportJobFactory jobFactory, ITransferConfig config, IErrorContainer errorContainer, IImportStatusManager importStatusManager)
		{
			_jobFactory = jobFactory;
			_config = config;
			_errorContainer = errorContainer;
			_importStatusManager = importStatusManager;
		}

		public LoadFileImporter Create(LoadFile args, Controller processController, Guid processId, int timezoneoffset, bool autoDetect, bool initializeUploaders, bool doRetryLogic,
			string bulkLoadFileFieldDelimiter, bool isCloudInstance,
			ExecutionSource executionSource)
		{
			return new LoadFileImporter(_jobFactory, _config, _errorContainer, _importStatusManager, args, processController, processId, timezoneoffset, autoDetect, initializeUploaders,
				doRetryLogic, bulkLoadFileFieldDelimiter, isCloudInstance, executionSource);
		}
	}
}