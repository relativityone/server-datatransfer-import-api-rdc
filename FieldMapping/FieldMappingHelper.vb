Namespace kCura.WinEDDS.FieldMapping
	Public Class FieldMappingHelper
		Public Function GetAppMappingDataForApplication(ByVal credentials As System.Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer, ByVal workspaceID As Int32, ByVal xml As System.Xml.XmlDocument) As EDDS.WebAPI.TemplateManagerBase.AppMappingData
			Dim templateManager As New Service.TemplateManager(credentials, cookieContainer)
			Dim installParams As New kCura.EDDS.WebAPI.TemplateManagerBase.ApplicationInstallationParameters()
			installParams.CaseId = workspaceID
			Return templateManager.GetAppMappingDataForWorkspace(xml, installParams)
		End Function
	End Class
End Namespace

