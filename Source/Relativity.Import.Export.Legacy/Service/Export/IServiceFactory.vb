Namespace kCura.WinEDDS.Service.Export
	Public Interface IServiceFactory
		Function CreateAuditManager() As IAuditManager
		Function CreateExportFileDownloader() As IExportFileDownloader
		Function CreateExportManager() As IExportManager
		Function CreateFieldManager() As IFieldManager
		Function CreateProductionManager() As IProductionManager
		Function CreateSearchManager() As ISearchManager

	End Interface
End Namespace