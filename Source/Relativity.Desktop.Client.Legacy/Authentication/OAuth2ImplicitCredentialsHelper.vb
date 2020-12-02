Imports System.Threading
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
			Dim authEndpoint As String = $"{_identityServerLocationProvider()}/{"connect/authorize"}"
			Dim implicitProvider As OAuth2ImplicitCredentials = New OAuth2ImplicitCredentials(New Uri(authEndpoint), "Relativity Desktop Client", _tokenResponseHandler)
			RelativityWebApiCredentialsProvider.Instance().SetProvider(implicitProvider)
		End Sub
		
		Public Function GetCredentialsAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of System.Net.NetworkCredential)
			If Not RelativityWebApiCredentialsProvider.Instance().CredentialsSet() Then
				SetImplicitCredentialProvider()
			End If
			Return RelativityWebApiCredentialsProvider.Instance().GetCredentialsAsync(cancellationToken)
		End Function
	End Class
End Namespace
