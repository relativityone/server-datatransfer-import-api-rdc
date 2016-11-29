Imports System.Diagnostics
Imports System.Net
Imports System.Threading.Tasks
Imports Relativity.OAuth2Client.TokenProviders

Public Class CredentialsProvider

	Public Class CredentialsNotSetException
		Inherits Exception
	End Class

	Private _populateCredentials As Func(Of Task(Of System.Net.NetworkCredential))
	Private Const _OAUTH_USERNAME As String = "XxX_BearerTokenCredentials_XxX"
	Private Shared readonly _instance As Lazy(Of CredentialsProvider) = New Lazy(Of CredentialsProvider)(Function()
																											return New CredentialsProvider()
																										End Function)
	Private Sub New()

		_populateCredentials = Function()
								   Throw new CredentialsNotSetException
		                       End Function

	End Sub

	Public Shared ReadOnly Property Instance As CredentialsProvider
		Get
			Return _instance.Value
		End Get
	End Property

	Public Sub SetCredentials(userName As string, password As string)

		Dim creds As System.Net.NetworkCredential = new System.Net.NetworkCredential(userName, password)

		_populateCredentials = Function()
								   return Task.FromResult(creds)
		                       End Function

	End Sub

	Public Sub SetCredentials(tokenProvider As Relativity.OAuth2Client.Interfaces.ITokenProvider)

		_populateCredentials = Async Function()
								   dim token As String = Await tokenProvider.GetAccessTokenAsync()
								   dim creds As System.Net.NetworkCredential = new NetworkCredential(_OAUTH_USERNAME, token)
								   return creds
		                       End Function

	End sub

	Public Async Function GetCredentials() As Task(Of System.Net.NetworkCredential)

		Dim creds As System.Net.NetworkCredential = Await _populateCredentials.Invoke().ConfigureAwait(False)
		return creds

	End Function

End Class
