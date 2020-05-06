﻿Namespace kCura.WinEDDS

	Public Interface IExportConfig

		ReadOnly Property ExportBatchSize As Int32

		ReadOnly Property ExportIOErrorWaitTime As Int32

		ReadOnly Property ExportIOErrorNumberOfRetries As Int32

		ReadOnly Property ExportErrorNumberOfRetries As Int32

		ReadOnly Property ExportErrorWaitTime As Int32

		ReadOnly Property ExportLongTextDataGridThreadCount As Int32

		ReadOnly Property ExportLongTextObjectManagerEnabled As Boolean

		ReadOnly Property ExportLongTextSqlThreadCount As Int32

		ReadOnly Property HttpErrorNumberOfRetries As Int32

		ReadOnly Property HttpErrorWaitTimeInSeconds As Int32

		ReadOnly Property TapiForceHttpClient As Boolean
	End Interface

End Namespace
