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

            Dim baseUri As New Uri(Config.WebServiceURL)
			Dim settings As New ServiceFactorySettings(New Uri($"https://{baseUri.Host}/Relativity.Services"), New Uri($"https://{baseUri.Host}/Relativity.Rest/api"), relativityCredentials)
			Dim factory As Relativity.Services.ServiceProxy.IServiceFactory = New ServiceFactory(settings)

			Return factory
		End Function
	End Class

End Namespace
