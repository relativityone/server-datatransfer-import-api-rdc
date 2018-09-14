Namespace kCura.WinEDDS

Public Interface IExportConfig

	ReadOnly Property ExportBatchSize() As Int32

	ReadOnly Property ExportThreadCount() As Int32

	ReadOnly Property UseOldExport() As Boolean

	ReadOnly Property ForceParallelismInNewExport() As Boolean

	ReadOnly Property ExportIOErrorWaitTime() As Int32

	ReadOnly Property ExportIOErrorNumberOfRetries() As Int32

	ReadOnly Property ExportErrorNumberOfRetries() As Int32

	ReadOnly Property ExportErrorWaitTime() As Int32

	ReadOnly Property MaxNumberOfFileExportTasks() As Int32

	ReadOnly Property TapiBridgeExportTransferWaitingTimeInSeconds As Int32

End Interface

End Namespace
