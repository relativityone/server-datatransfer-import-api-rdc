using System;
using System.Linq;
using System.Net;
using kCura.EDDS.WebAPI.BulkImportManagerBase;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Factories;
using kCura.WinEDDS.Core.Import.Helpers;
using kCura.WinEDDS.Core.Import.Managers;
using BulkImportManager = kCura.WinEDDS.Service.BulkImportManager;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class PushMetadataFilesTask : IPushMetadataFilesTask
	{
		private readonly ITransferConfig _transferConfig;
		private readonly INativeLoadInfoFactory _nativeLoadInfoFactory;
		private readonly IFileUploaderFactory _fileUploaderFactory;
		private readonly IBulkImportManager _bulkImportManager;
		private readonly IServerErrorManager _serverErrorManager;

		public PushMetadataFilesTask(INativeLoadInfoFactory nativeLoadInfoFactory, IFileUploaderFactory fileUploaderFactory, ITransferConfig transferConfig, 
			IBulkImportManager bulkImportManager, IServerErrorManager serverErrorManager)
		{
			_nativeLoadInfoFactory = nativeLoadInfoFactory;
			_fileUploaderFactory = fileUploaderFactory;
			_transferConfig = transferConfig;
			_bulkImportManager = bulkImportManager;
			_serverErrorManager = serverErrorManager;
		}

		public void PushMetadataFiles(ImportBatchContext importBatchContext)
		{
			var fileUploader = _fileUploaderFactory.CreateBcpFileUploader();

			fileUploader.UploadFile(importBatchContext.MetadataFilesInfo.NativeFilePath);
			fileUploader.UploadFile(importBatchContext.MetadataFilesInfo.DataGridFilePath);
			fileUploader.UploadFile(importBatchContext.MetadataFilesInfo.CodeFilePath);
			fileUploader.UploadFile(importBatchContext.MetadataFilesInfo.ObjectFilePath);

			var uploadResult = fileUploader.WaitForUploadToComplete();
			if (uploadResult.Any(x => !x.Value.Success))
			{
				throw new Exception();
			}

			var settings = _nativeLoadInfoFactory.Create(importBatchContext.MetadataFilesInfo, importBatchContext.ImportContext);

			var result = BulkImport(settings, importBatchContext.ImportContext);
			//TODO statistics

			_serverErrorManager.ManageErrors(importBatchContext.ImportContext);
		}

		private MassImportResults BulkImport(NativeLoadInfo settings, ImportContext importContext)
		{
			//TODO BatchSizeHistory

			var tries = _transferConfig.IoErrorNumberOfRetries;
			MassImportResults results = null;

			while (tries > 0)
			{
				try
				{
					results = _bulkImportManager.BulkImport(settings, importContext);
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

			return results;
		}
	}
}