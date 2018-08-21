Imports System.Net
Imports kCura.WinEDDS.Service.Export
Imports Relativity.Services.ServiceProxy

Namespace kCura.WinEDDS

	Public Class ServiceFactoryFactory
		Public Shared Function Create(credentials As NetworkCredential) As Relativity.Services.ServiceProxy.IServiceFactory
			Dim relativityCredentials As Relativity.Services.ServiceProxy.Credentials
			If credentials.UserName = "XxX_BearerTokenCredentials_XxX" Then
				relativityCredentials = New BearerTokenCredentials(credentials.Password)
			Else
				relativityCredentials = New UsernamePasswordCredentials(credentials.UserName, credentials.Password)
			End If
			Dim webServiceUri As New Uri(Config.WebServiceURL)
			Dim baseUri As New Uri(webServiceUri.GetLeftPart(UriPartial.Authority))
			Dim settings As New ServiceFactorySettings(New Uri(baseUri, "/Relativity.Services"), New Uri(baseUri, "/Relativity.Rest/api"), relativityCredentials)
			Dim factory As Relativity.Services.ServiceProxy.IServiceFactory = New ServiceFactory(settings)

			Return factory
		End Function
	End Class

End Namespace
