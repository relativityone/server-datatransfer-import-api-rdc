Imports System.Net
Imports System.Net.Security
Imports System.Threading
Imports System.Threading.Tasks
Imports Relativity.Constant
Imports Relativity.OAuth2Client
Imports Relativity.OAuth2Client.Implicit
Imports Relativity.OAuth2Client.Implicit.LoginForms
Imports Relativity.OAuth2Client.Implicit.LoginView
Imports Relativity.OAuth2Client.Interfaces.Events

Namespace kCura.WinEDDS.Credentials

	Public Class  OAuth2ImplicitCredentials
		Implements ICredentialsProvider

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

			Dim creds As System.Net.NetworkCredential = New NetworkCredential(kCura.WinEDDS.Credentials.Constants.OAuthWebApiBearerTokenUserName, token)
			Return creds
		End Function

		Public Sub CloseLoginView()
			If(Not _loginView.LoginForm.IsDisposed AndAlso _loginView.LoginForm.Visible)
				_loginView.LoginForm.Close()
			End If
		End Sub

		Private Sub CreateTokenProvider()
			Dim oAuthConfig As OAuth2ClientConfiguration = New OAuth2ClientConfiguration(_clientId) With { .TimeOut = TimeSpan.FromMinutes(15) }
			Dim homeRealmKey As String = Config.GetRegistryKeyValue(RegistryKeys.HomeRealmKey)
			_loginView = New LoginView(_stsUri, New Uri(_REDIRECT_URI), oAuthConfig, Function() new LoginForm(), homeRealmKey)
			Dim providerFactory As Relativity.OAuth2Client.Interfaces.ITokenProviderFactory = New ImplicitTokenProviderFactory(_loginView)
			Dim tokenProvider As Relativity.OAuth2Client.Interfaces.ITokenProvider = providerFactory.GetTokenProvider("WebApi", New String() {"UserInfoAccess"})
			_tokenProvider = tokenProvider
			Events = _tokenProvider.Events
			AddHandler Events.TokenRetrieved, _onTokenHandler
		End Sub
		
	End Class


End Namespace


