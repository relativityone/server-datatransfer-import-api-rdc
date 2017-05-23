using System;
using System.Net;
using kCura.EDDS.WebAPI.BulkImportManagerBase;
using kCura.WinEDDS.Core.Import.Helpers;
using kCura.WinEDDS.Core.Import.Managers;
using BulkImportManager = kCura.WinEDDS.Service.BulkImportManager;

namespace kCura.WinEDDS.Core.Import.Tasks.Helpers
{
	public class MetadataFilesServerExecution : IMetadataFilesServerExecution
	{
		private readonly ImportContext _importContext;
		private readonly ITransferConfig _transferConfig;
		private readonly INativeLoadInfoFactory _nativeLoadInfoFactory;
		private readonly IBulkImportManager _bulkImportManager;

		public MetadataFilesServerExecution(ImportContext importContext, ITransferConfig transferConfig, INativeLoadInfoFactory nativeLoadInfoFactory,
			IBulkImportManager bulkImportManager)
		{
			_importContext = importContext;
			_transferConfig = transferConfig;
			_nativeLoadInfoFactory = nativeLoadInfoFactory;
			_bulkImportManager = bulkImportManager;
		}

		public void Import(MetadataFilesInfo metadataFilesInfo)
		{
			var importSettings = _nativeLoadInfoFactory.Create(metadataFilesInfo, _importContext);

			var result = BulkImport(importSettings);
			//TODO statistics
		}

		private MassImportResults BulkImport(NativeLoadInfo settings)
		{
			//TODO BatchSizeHistory

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
				catch (Exception)
				{
					//TODO check if we should continue (RDC: _continue)
					if (--tries == 0)
					{
						throw;
					}
					//RaiseWarningAndPause(ex, WaitTimeBetweenRetryAttempts)
				}
			}

			return null;
		}
	}
}