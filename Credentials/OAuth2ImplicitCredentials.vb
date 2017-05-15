Imports System.Diagnostics
Imports System.Net
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Threading.Tasks
Imports Credentials
Imports kCura.WinEDDS.Exceptions
Imports Relativity.OAuth2Client.Events
Imports Relativity.OAuth2Client.Implicit
Imports Relativity.OAuth2Client.Implicit.LoginView
Imports Relativity.OAuth2Client.Implicit.RelativityWebBrowser
Imports Relativity.OAuth2Client.Interfaces
Imports Relativity.OAuth2Client.Interfaces.Events
Imports Relativity.OAuth2Client.TokenProviders.ProviderFactories

Namespace kCura.WinEDDS.Credentials

	Public Class  OAuth2ImplicitCredentials
		Implements ICredentialsProvider

		Private Const _OAUTH_USERNAME As String = "XxX_BearerTokenCredentials_XxX"
		Private Const _REDIRECT_URI As String = "http://relativityimplicit/"
		Private _tokenProvider As Relativity.OAuth2Client.Interfaces.ITokenProvider
		Private ReadOnly _stsUri As Uri
		Private ReadOnly _clientId As String
		Private _loginView As ILoginView
		Private _cancellationTokenSource As CancellationTokenSource
		Private _currentCredentials As NetworkCredential
		Private _onTokenHandler As TokenResponseHandler
		
		Public WithEvents Events As IOAuth2ClientEvents

		
		Public Sub New(stsUri As Uri, clientID As String, onTokenHandler As TokenResponseHandler)
			_stsUri = stsUri
			_clientId = clientID
			_onTokenHandler = onTokenHandler
			CreateTokenProvider()
			_cancellationTokenSource = New CancellationTokenSource()
		End Sub

		Public Function GetCredentials() As System.Net.NetworkCredential Implements ICredentialsProvider.GetCredentials
			If _currentCredentials Is Nothing
				throw new CredentialsNotSetException()
			End If
			
			Return _currentCredentials

		End Function

		Public Async Function GetCredentialsAsync() As Task(Of System.Net.NetworkCredential) Implements ICredentialsProvider.GetCredentialsAsync
				Dim token As String = Await _tokenProvider.GetAccessTokenAsync(_cancellationTokenSource.Token)

				Dim creds As System.Net.NetworkCredential = New NetworkCredential(_OAUTH_USERNAME, token)
				_currentCredentials = creds
				Return creds
		End Function

		Private Sub CreateTokenProvider()
			_loginView = New LoginView(_stsUri, new Uri(_REDIRECT_URI), _clientId)
			Dim providerFactory As Relativity.OAuth2Client.Interfaces.ITokenProviderFactory = New ImplicitTokenProviderFactory(_loginView)
			Dim tokenProvider As Relativity.OAuth2Client.Interfaces.ITokenProvider = providerFactory.GetTokenProvider("WebApi", New String() {"UserInfoAccess"})
			_tokenProvider = tokenProvider
			Events = _tokenProvider.Events
			AddHandler Events.TokenRetrieved, _onTokenHandler
		End Sub


		Private Sub On_LoginFormClosing(ByVal sender As Object, ByVal e As EventArgs)
			If(Not _cancellationTokenSource.IsCancellationRequested)
				_cancellationTokenSource.Cancel()
				_cancellationTokenSource = New CancellationTokenSource()
			End If
		 End Sub
		
	End Class


End Namespace


