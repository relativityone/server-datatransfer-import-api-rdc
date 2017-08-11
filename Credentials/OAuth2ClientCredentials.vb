
Imports System.Net
Imports System.Threading
Imports System.Threading.Tasks
Imports Relativity.OAuth2Client.TokenProviders.ProviderFactories

Namespace kCura.WinEDDS.Credentials

	Public Class OAuth2ClientCredentials
		Implements ICredentialsProvider

		Private Const _OAUTH_USERNAME As String = "XxX_BearerTokenCredentials_XxX"
		Private ReadOnly _tokenProvider As Relativity.OAuth2Client.Interfaces.ITokenProvider
		Private ReadOnly _tokenSource As CancellationTokenSource 

		Public Sub New(stsUri As Uri, clientID As String, clientSecret As String)
			Dim providerFactory As Relativity.OAuth2Client.Interfaces.IClientTokenProviderFactory = New ClientTokenProviderFactory(stsUri, clientId, clientSecret)
			Dim tokenProvider As Relativity.OAuth2Client.Interfaces.ITokenProvider = providerFactory.GetTokenProvider("WebApi", New String() { "SystemUserInfo" })
			_tokenProvider = tokenProvider
			_tokenSource = new CancellationTokenSource()
		End Sub

		Public Sub New(tokenProvider As Relativity.OAuth2Client.Interfaces.ITokenProvider)
			_tokenProvider = tokenProvider
			_tokenSource = new CancellationTokenSource()
		End Sub

		Public Function GetCredentials() As System.Net.NetworkCredential Implements ICredentialsProvider.GetCredentials
			dim creds As NetworkCredential = New NetworkCredential()
			Try
				creds = GetCredentialsAsync().Result
			Catch ex As AggregateException
				Throw ex.InnerException
			End Try
			
			return creds

		End Function

		Public Async Function GetCredentialsAsync() As Task(Of System.Net.NetworkCredential) Implements ICredentialsProvider.GetCredentialsAsync
			
			dim token As String = Await _tokenProvider.GetAccessTokenAsync(_tokenSource.Token).ConfigureAwait(False)
			dim creds As System.Net.NetworkCredential = new NetworkCredential(_OAUTH_USERNAME, token)
			return creds

		End Function

	End Class


End Namespace

