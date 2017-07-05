using System;
using System.Net;
using System.Threading;
using kCura.EDDS.WebAPI.BulkImportManagerBase;
using kCura.WinEDDS.Core.Import.Helpers;
using kCura.WinEDDS.Core.Import.Managers;
using kCura.WinEDDS.Core.Import.Statistics;
using kCura.WinEDDS.Core.Import.Status;
using Relativity.Logging;
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
		private readonly ILog _log;
		private readonly IMetadataStatisticsHandler _metadataStatisticsHandler;

		public MetadataFilesServerExecution(ImportContext importContext, ITransferConfig transferConfig, INativeLoadInfoFactory nativeLoadInfoFactory,
			IBulkImportManager bulkImportManager, IBulkImportStatisticsHandler statisticsHandler, IMetadataStatisticsHandler metadataStatisticsHandler, 
			ICancellationProvider cancellationProvider, ILog log)
		{
			_importContext = importContext;
			_transferConfig = transferConfig;
			_nativeLoadInfoFactory = nativeLoadInfoFactory;
			_bulkImportManager = bulkImportManager;
			_statisticsHandler = statisticsHandler;
			_cancellationProvider = cancellationProvider;
			_log = log;
			_metadataStatisticsHandler = metadataStatisticsHandler;
		}

		public void Import(MetadataFilesInfo metadataFilesInfo)
		{
			_log.LogInformation("Start metadara bulk import");
			_metadataStatisticsHandler.RaiseBulkImportMetadataStartedEvent();
			var importSettings = _nativeLoadInfoFactory.Create(metadataFilesInfo, _importContext);

			var ticks = DateTime.Now.Ticks;
			MassImportResults result = BulkImport(importSettings);
			var endTicks = DateTime.Now.Ticks - ticks;
			_statisticsHandler.RaiseBulkImportCompleted(endTicks, result);
			_log.LogInformation($"Metadata bulk import completed. Duration: {ticks / TimeSpan.TicksPerMillisecond } miliseconds");
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
					_log.LogDebug($"Bulk import failed. Retry bulk import again (Retry number: {_transferConfig.IoErrorNumberOfRetries - tries})");
					_statisticsHandler.RaiseIoWarning(ex);
					Thread.Sleep(_transferConfig.IoErrorWaitTimeInSeconds * 1000);
				}
			}

			return null;
		}
	}
}