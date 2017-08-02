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

End Class

End Namespace
