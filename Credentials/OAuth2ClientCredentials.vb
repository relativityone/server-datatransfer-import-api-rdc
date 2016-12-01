
Imports System.Net
Imports System.Threading.Tasks

Namespace kCura.WinEDDS.Credentials

	Public Class OAuth2ClientCredentials
		Implements ICredentialsProvider

		Private Const _OAUTH_USERNAME As String = "XxX_BearerTokenCredentials_XxX"
		Private ReadOnly _tokenProvider As Relativity.OAuth2Client.Interfaces.ITokenProvider 

		Public Sub New(tokenProvider As Relativity.OAuth2Client.Interfaces.ITokenProvider)
			_tokenProvider = tokenProvider
		End Sub

		Public Function GetCredentials() As System.Net.NetworkCredential Implements ICredentialsProvider.GetCredentials

			dim creds As NetworkCredential = GetCredentialsAsync().Result
			return creds

		End Function

		Public Async Function GetCredentialsAsync() As Task(Of System.Net.NetworkCredential) Implements ICredentialsProvider.GetCredentialsAsync
			
			dim token As String = Await _tokenProvider.GetAccessTokenAsync().ConfigureAwait(False)
			dim creds As System.Net.NetworkCredential = new NetworkCredential(_OAUTH_USERNAME, token)
			return creds

		End Function

	End Class


End Namespace

