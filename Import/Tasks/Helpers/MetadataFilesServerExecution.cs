using System;
using System.Net;
using System.Threading;
using kCura.EDDS.WebAPI.BulkImportManagerBase;
using kCura.WinEDDS.Core.Import.Helpers;
using kCura.WinEDDS.Core.Import.Managers;
using kCura.WinEDDS.Core.Import.Statistics;
using kCura.WinEDDS.Core.Import.Status;
using BulkImportManager = kCura.WinEDDS.Service.BulkImportManager;

namespace kCura.WinEDDS.Core.Import.Tasks.Helpers
{
	public class MetadataFilesServerExecution : IMetadataFilesServerExecution
	{
		private readonly ImportContext _importContext;
		private readonly ITransferConfig _transferConfig;
		private readonly INativeLoadInfoFactory _nativeLoadInfoFactory;
		private readonly IBulkImportManager _bulkImportManager;
		private readonly IBulkImportStatisticsHandler _statisticsHandler;
		private readonly ICancellationProvider _cancellationProvider;

		public MetadataFilesServerExecution(ImportContext importContext, ITransferConfig transferConfig, INativeLoadInfoFactory nativeLoadInfoFactory,
			IBulkImportManager bulkImportManager, IBulkImportStatisticsHandler statisticsHandler, ICancellationProvider cancellationProvider)
		{
			_importContext = importContext;
			_transferConfig = transferConfig;
			_nativeLoadInfoFactory = nativeLoadInfoFactory;
			_bulkImportManager = bulkImportManager;
			_statisticsHandler = statisticsHandler;
			_cancellationProvider = cancellationProvider;
		}

		public void Import(MetadataFilesInfo metadataFilesInfo)
		{
			var importSettings = _nativeLoadInfoFactory.Create(metadataFilesInfo, _importContext);

			var ticks = DateTime.Now.Ticks;
			MassImportResults result = BulkImport(importSettings);
			_statisticsHandler.RaiseBulkImportCompleted(DateTime.Now.Ticks - ticks, result);
		}

		private MassImportResults BulkImport(NativeLoadInfo settings)
		{
			var tries = _transferConfig.IoErrorNumberOfRetries;

			while (tries > 0)
			{
				try
				{
					return _bulkImportManager.BulkImport(settings, _importContext);
				}
				catch (BulkImportManager.BulkImportSqlTimeoutException)
				{
					throw;
				}
				catch (WebException e) when (e.Message.Contains("timed out"))
				{
					throw;
				}
				catch (BulkImportManager.BulkImportSqlException)
				{
					throw;
				}
				catch (BulkImportManager.InsufficientPermissionsForImportException)
				{
					throw;
				}
				catch (Exception ex)
				{
					if (--tries == 0 || _cancellationProvider.GetToken().IsCancellationRequested)
					{
						throw;
					}
					_statisticsHandler.RaiseIoWarning(ex);
					Thread.Sleep(_transferConfig.IoErrorWaitTimeInSeconds * 1000);
				}
			}

			return null;
		}
	}
}