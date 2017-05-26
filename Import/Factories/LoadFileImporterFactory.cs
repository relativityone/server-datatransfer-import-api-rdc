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
		private readonly IImportStatusManager _importStatusManager;
		private readonly ICancellationProvider _cancellationProvider;
		private readonly IErrorManagerFactory _errorManagerFactory;

		public LoadFileImporterFactory(IImportJobFactory jobFactory, IImportStatusManager importStatusManager, ICancellationProvider cancellationProvider, IErrorManagerFactory errorManagerFactory)
		{
			_jobFactory = jobFactory;
			_importStatusManager = importStatusManager;
			_cancellationProvider = cancellationProvider;
			_errorManagerFactory = errorManagerFactory;
		}

		public LoadFileImporter Create(LoadFile args, Controller processController, Guid processId, int timezoneoffset, bool autoDetect, bool initializeUploaders, bool doRetryLogic,
			string bulkLoadFileFieldDelimiter, bool isCloudInstance,
			ExecutionSource executionSource)
		{
			return new LoadFileImporter(_jobFactory, _importStatusManager, _cancellationProvider, _errorManagerFactory, args, processController, processId, timezoneoffset, autoDetect, 
				initializeUploaders, doRetryLogic, bulkLoadFileFieldDelimiter, isCloudInstance, executionSource);
		}
	}
}