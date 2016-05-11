Namespace kCura.WinEDDS.Service.Export
	Public Class WebApiServiceFactory
		Implements IServiceFactory

		Friend Property ExportSettings As ExportFile
		Public Sub New(settings As ExportFile)
			ExportSettings = settings
		End Sub

		Public Function CreateAuditManager() As IAuditManager Implements IServiceFactory.CreateAuditManager
			Return New AuditManager(ExportSettings.Credential, ExportSettings.CookieContainer)
		End Function

		Public Function CreateExportFileDownloader() As IExportFileDownloader Implements IServiceFactory.CreateExportFileDownloader
			Return New FileDownloader(ExportSettings.Credential, ExportSettings.CaseInfo.DocumentPath & "\EDDS" & ExportSettings.CaseInfo.ArtifactID, ExportSettings.CaseInfo.DownloadHandlerURL, ExportSettings.CookieContainer, Settings.AuthenticationToken)
			FileDownloader.TotalWebTime = 0
		End Function

		Public Function CreateExportManager() As IExportManager Implements IServiceFactory.CreateExportManager
			Return New ExportManager(ExportSettings.Credential, ExportSettings.CookieContainer)
		End Function

		Public Function CreateFieldManager() As IFieldManager Implements IServiceFactory.CreateFieldManager
			Return New FieldManager(ExportSettings.Credential, ExportSettings.CookieContainer)
		End Function

		Public Function CreateProductionManager() As IProductionManager Implements IServiceFactory.CreateProductionManager
			Return New ProductionManager(ExportSettings.Credential, ExportSettings.CookieContainer)
		End Function

		Public Function CreateSearchManager() As ISearchManager Implements IServiceFactory.CreateSearchManager
			Return New SearchManager(ExportSettings.Credential, ExportSettings.CookieContainer)
		End Function
	End Class
End Namespace