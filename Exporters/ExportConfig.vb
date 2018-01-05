Namespace kCura.WinEDDS

Public Class ExportConfig
		Implements IExportConfig

	Public ReadOnly Property ExportBatchSize As Integer Implements IExportConfig.ExportBatchSize
		Get
			return Config.ExportBatchSize
		End Get
	End Property

	Public ReadOnly Property ExportThreadCount As Integer Implements IExportConfig.ExportThreadCount
		Get
			Return Config.ExportThreadCount
		End Get
	End Property

	Public ReadOnly Property ExportIOErrorWaitTime As Integer Implements IExportConfig.ExportIOErrorWaitTime
		Get
			return kCura.Utility.Config.IOErrorWaitTimeInSeconds
		End Get
	End Property

	Public ReadOnly Property ExportIOErrorNumberOfRetries As Integer Implements IExportConfig.ExportIOErrorNumberOfRetries
		Get
			Return kCura.Utility.Config.IOErrorNumberOfRetries
		End Get
	End Property

	Public ReadOnly Property ExportErrorNumberOfRetries As Integer Implements IExportConfig.ExportErrorNumberOfRetries
		Get
			return kCura.Utility.Config.ExportErrorNumberOfRetries
		End Get
	End Property

	Public ReadOnly Property ExportErrorWaitTime As Integer Implements IExportConfig.ExportErrorWaitTime
		Get
			Return kCura.Utility.Config.ExportErrorWaitTimeInSeconds
		End Get
	End Property

End Class

End Namespace
