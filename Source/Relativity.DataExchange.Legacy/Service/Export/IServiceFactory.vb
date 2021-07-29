Namespace kCura.WinEDDS.Service.Export
	Public Interface IServiceFactory
		Function CreateAuditManager(correlationIdFunc As Func(Of String)) As IAuditManager
		Function CreateExportManager(correlationIdFunc As Func(Of String)) As IExportManager
		Function CreateFieldManager(correlationIdFunc As Func(Of String)) As IFieldManager
		Function CreateProductionManager(correlationIdFunc As Func(Of String)) As IProductionManager
		Function CreateSearchManager(correlationIdFunc As Func(Of String)) As ISearchManager
	End Interface
End Namespace