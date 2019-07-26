﻿Namespace kCura.WinEDDS

Public Interface IExportConfig

	ReadOnly Property ExportBatchSize() As Int32

	ReadOnly Property ExportThreadCount() As Int32

	ReadOnly Property UseOldExport() As Boolean

	ReadOnly Property ExportIOErrorWaitTime() As Int32

	ReadOnly Property ExportIOErrorNumberOfRetries() As Int32

	ReadOnly Property ExportErrorNumberOfRetries() As Int32

	ReadOnly Property ExportErrorWaitTime() As Int32
	ReadOnly Property TapiForceHttpClient As Boolean
End Interface

End Namespace
