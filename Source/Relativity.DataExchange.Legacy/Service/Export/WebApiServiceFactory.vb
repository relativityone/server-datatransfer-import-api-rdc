Namespace kCura.WinEDDS.Service.Export
	Public Class WebApiServiceFactory
		Implements IServiceFactory

		Friend Property ExportSettings As ExportFile
		Public Sub New(settings As ExportFile)
			ExportSettings = settings
		End Sub

		Public Function CreateAuditManager(correlationIdFunc As Func(Of String)) As IAuditManager Implements IServiceFactory.CreateAuditManager
			Return ManagerFactory.CreateAuditManager(ExportSettings.Credential, ExportSettings.CookieContainer, correlationIdFunc)
		End Function

		Public Function CreateExportManager(correlationIdFunc As Func(Of String)) As IExportManager Implements IServiceFactory.CreateExportManager
			Return ManagerFactory.CreateExportManager(ExportSettings.Credential, ExportSettings.CookieContainer, correlationIdFunc)
		End Function

		Public Function CreateFieldManager(correlationIdFunc As Func(Of String)) As IFieldManager Implements IServiceFactory.CreateFieldManager
			Return ManagerFactory.CreateFieldManager(ExportSettings.Credential, ExportSettings.CookieContainer, correlationIdFunc)
		End Function

		Public Function CreateProductionManager(correlationIdFunc As Func(Of String)) As IProductionManager Implements IServiceFactory.CreateProductionManager
			Return ManagerFactory.CreateProductionManager(ExportSettings.Credential, ExportSettings.CookieContainer, correlationIdFunc)
		End Function

		Public Function CreateSearchManager(correlationIdFunc As Func(Of String)) As ISearchManager Implements IServiceFactory.CreateSearchManager
			Return ManagerFactory.CreateSearchManager(ExportSettings.Credential, ExportSettings.CookieContainer, correlationIdFunc)
		End Function
	End Class
End Namespace