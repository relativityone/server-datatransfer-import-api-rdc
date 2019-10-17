Imports System.Net
Imports System.Net.Security
Imports System.Threading
Imports System.Threading.Tasks
Imports Relativity.DataExchange
Imports Relativity.OAuth2Client
Imports Relativity.OAuth2Client.Implicit
Imports Relativity.OAuth2Client.Implicit.LoginForms
Imports Relativity.OAuth2Client.Implicit.LoginView
Imports Relativity.OAuth2Client.Interfaces.Events

Namespace kCura.WinEDDS.Credentials
	Public Class  OAuth2ImplicitCredentials
		Implements ICredentialsProvider

		Private _loginView As ILoginView
		Private _tokenProvider As Global.Relativity.OAuth2Client.Interfaces.ITokenProvider

		Private ReadOnly _stsUri As Uri
		Private ReadOnly _clientId As String
		Private ReadOnly _onTokenHandler As TokenResponseHandler
		Private ReadOnly _logInLock As New SemaphoreSlim(1)

		Public WithEvents Events As IOAuth2ClientEvents

		Public Sub New(stsUri As Uri, clientID As String, onTokenHandler As TokenResponseHandler)
			_stsUri = stsUri
			_clientId = clientID
			_onTokenHandler = onTokenHandler
			CreateTokenProvider()
		End Sub

		Public Function GetCredentials() As System.Net.NetworkCredential Implements ICredentialsProvider.GetCredentials
			Throw new InvalidOperationException("ImplicitCredentials Provider does not support synchronous requests.")
		End Function

		Public Async Function GetCredentialsAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of System.Net.NetworkCredential) Implements ICredentialsProvider.GetCredentialsAsync
			Dim token As String = String.Empty
			Await _logInLock.WaitAsync()
			Dim oldCheck As RemoteCertificateValidationCallback = ServicePointManager.ServerCertificateValidationCallback
			Try
				If _tokenProvider Is Nothing
					CreateTokenProvider()
				End If
				ServicePointManager.ServerCertificateValidationCallback = Nothing
				token = Await _tokenProvider.GetAccessTokenAsync(cancellationToken)
			Catch
				DestroyTokenProvider()
			Finally
				ServicePointManager.ServerCertificateValidationCallback = oldCheck
				TryCloseLoginView()
				_logInLock.Release()
			End Try

			Dim creds As System.Net.NetworkCredential = New NetworkCredential(kCura.WinEDDS.Credentials.Constants.OAuthWebApiBearerTokenUserName, token)
			Return creds
		End Function

		Private Sub CreateTokenProvider()
			Dim oAuthConfig As OAuth2ClientConfiguration = New OAuth2ClientConfiguration(_clientId) With { .TimeOut = TimeSpan.FromMinutes(15) }
			Dim openIdLoginHint As String = AppSettings.Instance.OpenIdConnectHomeRealmDiscoveryHint
			Dim redirectUrl As String = AppSettings.Instance.OAuth2ImplicitCredentialRedirectUrl
			_loginView = New LoginView(_stsUri, New Uri(redirectUrl), oAuthConfig, Function() new LoginForm(), openIdLoginHint)
			Dim providerFactory As Global.Relativity.OAuth2Client.Interfaces.ITokenProviderFactory = New ImplicitTokenProviderFactory(_loginView)
			Dim tokenProvider As Global.Relativity.OAuth2Client.Interfaces.ITokenProvider = providerFactory.GetTokenProvider("WebApi", New String() {"UserInfoAccess"})
			_tokenProvider = tokenProvider
			Events = _tokenProvider.Events
			AddHandler Events.TokenRetrieved, _onTokenHandler
		End Sub

		Private Sub DestroyTokenProvider()
			_loginView = Nothing
			_tokenProvider = Nothing
		End Sub
		
		Private Sub TryCloseLoginView()
			If _loginView Is Nothing OrElse _loginView.LoginForm Is Nothing
				Return
			End If

			If Not _loginView.LoginForm.IsDisposed AndAlso _loginView.LoginForm.Visible
				Try
					_loginView.LoginForm.Close()
				Catch
					' We do not want to throw exception in a finally block
				End Try
			End If
		End Sub

	End Class


End Namespace


