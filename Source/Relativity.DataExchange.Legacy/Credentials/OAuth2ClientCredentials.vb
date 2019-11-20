Imports System.Net
Imports System.Threading
Imports System.Threading.Tasks
Imports Relativity.OAuth2Client.TokenProviders.ProviderFactories

Namespace kCura.WinEDDS.Credentials

	Public Class OAuth2ClientCredentials
		Implements ICredentialsProvider

		Private ReadOnly _tokenProvider As Global.Relativity.OAuth2Client.Interfaces.ITokenProvider

		Public Sub New(stsUri As Uri, clientID As String, clientSecret As String)
			Dim providerFactory As Global.Relativity.OAuth2Client.Interfaces.IClientTokenProviderFactory = New ClientTokenProviderFactory(stsUri, clientId, clientSecret)
			Dim tokenProvider As Global.Relativity.OAuth2Client.Interfaces.ITokenProvider = providerFactory.GetTokenProvider("WebApi", New String() { "SystemUserInfo" })
			_tokenProvider = tokenProvider
		End Sub

		Public Sub New(tokenProvider As Global.Relativity.OAuth2Client.Interfaces.ITokenProvider)
			_tokenProvider = tokenProvider
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

		Public Async Function GetCredentialsAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of System.Net.NetworkCredential) Implements ICredentialsProvider.GetCredentialsAsync
			
			dim token As String = Await _tokenProvider.GetAccessTokenAsync(cancellationToken).ConfigureAwait(False)
			Dim creds As System.Net.NetworkCredential = New NetworkCredential(kCura.WinEDDS.Credentials.Constants.OAuthWebApiBearerTokenUserName, token)
			Return creds

		End Function

	End Class


End Namespace

