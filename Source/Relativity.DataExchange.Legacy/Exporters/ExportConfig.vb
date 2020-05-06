Imports Relativity.DataExchange

Namespace kCura.WinEDDS

	Public Class ExportConfig
			Implements IExportConfig

		Public ReadOnly Property ExportBatchSize As Integer Implements IExportConfig.ExportBatchSize
			Get
				Return AppSettings.Instance.ExportBatchSize
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

		Public ReadOnly Property ExportLongTextDataGridThreadCount As Integer Implements IExportConfig.ExportLongTextDataGridThreadCount
			Get
				Return AppSettings.Instance.ExportLongTextDataGridThreadCount
			End Get
		End Property

		Public ReadOnly Property ExportLongTextObjectManagerEnabled As Boolean Implements IExportConfig.ExportLongTextObjectManagerEnabled
			Get
				Return AppSettings.Instance.ExportLongTextObjectManagerEnabled
			End Get
		End Property

		Public ReadOnly Property ExportLongTextSqlThreadCount As Integer Implements IExportConfig.ExportLongTextSqlThreadCount
			Get
				Return AppSettings.Instance.ExportLongTextSqlThreadCount
			End Get
		End Property

		Public ReadOnly Property HttpErrorNumberOfRetries As Integer Implements IExportConfig.HttpErrorNumberOfRetries
			Get
				Return AppSettings.Instance.HttpErrorNumberOfRetries
			End Get
		End Property

		Public ReadOnly Property HttpErrorWaitTimeInSeconds As Integer Implements IExportConfig.HttpErrorWaitTimeInSeconds
			Get
				Return AppSettings.Instance.HttpErrorWaitTimeInSeconds
			End Get
		End Property

		Public ReadOnly Property TapiForceHttpClient As Boolean Implements IExportConfig.TapiForceHttpClient
			Get
				Return AppSettings.Instance.TapiForceHttpClient
			End Get
		End Property
	End Class
End Namespace