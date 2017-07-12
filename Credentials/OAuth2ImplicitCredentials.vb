Imports System.Diagnostics
Imports System.Net
Imports System.Net.Security
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Security.Cryptography.X509Certificates
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
		Private _onTokenHandler As TokenResponseHandler
		Private _logInLock As New SemaphoreSlim(1)
		
		Public WithEvents Events As IOAuth2ClientEvents

		
		Public Sub New(stsUri As Uri, clientID As String, onTokenHandler As TokenResponseHandler)
			_stsUri = stsUri
			_clientId = clientID
			_onTokenHandler = onTokenHandler
			CreateTokenProvider()
			_cancellationTokenSource = New CancellationTokenSource()
		End Sub

		Public Function GetCredentials() As System.Net.NetworkCredential Implements ICredentialsProvider.GetCredentials
			Throw new InvalidOperationException("ImplicitCredentials Provider does not support synchronous requests.")
		End Function

		Public Async Function GetCredentialsAsync() As Task(Of System.Net.NetworkCredential) Implements ICredentialsProvider.GetCredentialsAsync
			Dim token As String = String.Empty
			Await _logInLock.WaitAsync() 
			Dim oldCheck As RemoteCertificateValidationCallback = ServicePointManager.ServerCertificateValidationCallback
			Try
				ServicePointManager.ServerCertificateValidationCallback = Nothing
				token = Await _tokenProvider.GetAccessTokenAsync(_cancellationTokenSource.Token)
			Finally
				ServicePointManager.ServerCertificateValidationCallback = oldCheck
				_logInLock.Release()
			End Try
			
			Dim creds As System.Net.NetworkCredential = New NetworkCredential(_OAUTH_USERNAME, token)
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
		
	End Class


End Namespace


