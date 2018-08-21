Imports System.Threading.Tasks
Imports Relativity.OAuth2Client.Interfaces.Events

Namespace kCura.WinEDDS.Credentials
	Public Class OAuth2ImplicitCredentialsHelper

		Private ReadOnly _identityServerLocationProvider As Func(Of String)
		Private ReadOnly _tokenResponseHandler As TokenResponseHandler

		Public Sub New(identityServerLocationProvider As Func(Of String), tokenResponseHandler As TokenResponseHandler)
			_identityServerLocationProvider = identityServerLocationProvider
			_tokenResponseHandler = tokenResponseHandler
		End Sub

		Public Sub SetImplicitCredentialProvider()
			If RelativityWebApiCredentialsProvider.Instance().CredentialsSet() AndAlso RelativityWebApiCredentialsProvider.Instance().ProviderType() = GetType(OAuth2ImplicitCredentials) Then
				Dim tempImplicitProvider As OAuth2ImplicitCredentials = CType(RelativityWebApiCredentialsProvider.Instance().GetProvider(), OAuth2ImplicitCredentials)
				tempImplicitProvider.CloseLoginView()
			End If
			Dim authEndpoint As String = $"{_identityServerLocationProvider()}/{"connect/authorize"}"
			Dim implicitProvider As OAuth2ImplicitCredentials = New OAuth2ImplicitCredentials(New Uri(authEndpoint), "Relativity Desktop Client", _tokenResponseHandler)
			RelativityWebApiCredentialsProvider.Instance().SetProvider(implicitProvider)
		End Sub

		Public Async Function GetCredentialsAsync() As Task(Of System.Net.NetworkCredential)
			If Not RelativityWebApiCredentialsProvider.Instance().CredentialsSet() Then
				SetImplicitCredentialProvider()
			End If
			Return Await RelativityWebApiCredentialsProvider.Instance().GetCredentialsAsync().ConfigureAwait(False)
		End Function
	End Class
End Namespace
