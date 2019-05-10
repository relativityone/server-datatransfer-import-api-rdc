Imports Relativity.Import.Export

Namespace kCura.WinEDDS

	Public Class ExportConfig
			Implements IExportConfig

		Public ReadOnly Property ExportBatchSize As Integer Implements IExportConfig.ExportBatchSize
			Get
				return AppSettings.Instance.ExportBatchSize
			End Get
		End Property

		Public ReadOnly Property ExportThreadCount As Integer Implements IExportConfig.ExportThreadCount
			Get
				Return AppSettings.Instance.ExportThreadCount
			End Get
		End Property

		Public ReadOnly Property UseOldExport As Boolean Implements IExportConfig.UseOldExport
			Get
				Return AppSettings.Instance.UseOldExport
			End Get
		End Property

		Public ReadOnly Property ForceParallelismInNewExport As Boolean Implements IExportConfig.ForceParallelismInNewExport
			Get
				Return AppSettings.Instance.ForceParallelismInNewExport
			End Get
		End Property
		
		Public ReadOnly Property ExportIOErrorWaitTime As Integer Implements IExportConfig.ExportIOErrorWaitTime
			Get
				return AppSettings.Instance.IoErrorWaitTimeInSeconds
			End Get
		End Property
		
		Public ReadOnly Property ExportIOErrorNumberOfRetries As Integer Implements IExportConfig.ExportIOErrorNumberOfRetries
			Get
				Return AppSettings.Instance.IoErrorNumberOfRetries
			End Get
		End Property
		
		Public ReadOnly Property ExportErrorNumberOfRetries As Integer Implements IExportConfig.ExportErrorNumberOfRetries
			Get
				return AppSettings.Instance.ExportErrorNumberOfRetries
			End Get
		End Property

		Public ReadOnly Property ExportErrorWaitTime As Integer Implements IExportConfig.ExportErrorWaitTime
			Get
				Return AppSettings.Instance.ExportErrorWaitTimeInSeconds
			End Get
		End Property

		Public ReadOnly Property MaxNumberOfFileExportTasks As Integer Implements IExportConfig.MaxNumberOfFileExportTasks
		    Get
		        Return AppSettings.Instance.MaxNumberOfFileExportTasks
		    End Get
		End Property

		Public ReadOnly Property TapiBridgeExportTransferWaitingTimeInSeconds As Integer Implements IExportConfig.TapiBridgeExportTransferWaitingTimeInSeconds
			Get
				Return AppSettings.Instance.TapiBridgeExportTransferWaitingTimeInSeconds
			End Get
		End Property

		Public ReadOnly Property TapiForceHttpClient As Boolean Implements IExportConfig.TapiForceHttpClient
			Get
				Return AppSettings.Instance.TapiForceHttpClient
			End Get
		End Property

		Public ReadOnly Property MaximumFilesForTapiBridge As Integer Implements IExportConfig.MaximumFilesForTapiBridge
			Get
				Return AppSettings.Instance.MaxFilesForTapiBridge
			End Get
		End Property
	End Class
End Namespace