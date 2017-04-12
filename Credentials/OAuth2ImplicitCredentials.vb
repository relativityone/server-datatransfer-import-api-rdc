Imports System.Diagnostics
Imports System.Net
Imports System.Threading
Imports System.Threading.Tasks
Imports Credentials
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
		Private ReadOnly _tokenProvider As Relativity.OAuth2Client.Interfaces.ITokenProvider
		Private _loginFormFacotry As Func(Of LoginForm)
		Private _cancellationTokenSource As CancellationTokenSource
		
		Public WithEvents Events As IOAuth2ClientEvents

		Public ReadOnly Property Browser() As RelativityWebBrowser
			Get
				return LoginForm.Browser
			End Get
		End Property

		Public ReadOnly Property LoginForm() As LoginForm
			Get
				return _loginFormFacotry.Invoke()
			End Get
		End Property

		Public Sub New(stsUri As Uri, clientID As String, formFactory As Func(Of LoginForm))
			_loginFormFacotry = formFactory
			Dim loginView As ILoginView = New LoginView(stsUri, new Uri(_REDIRECT_URI), clientID, LoginForm.browser)
			Dim providerFactory As Relativity.OAuth2Client.Interfaces.ITokenProviderFactory = New ImplicitTokenProviderFactory(loginView)
			Dim tokenProvider As Relativity.OAuth2Client.Interfaces.ITokenProvider = providerFactory.GetTokenProvider("WebApi", New String() {"UserInfoAccess"})
			_tokenProvider = tokenProvider
			Events = _tokenProvider.Events
			AddHandler Events.TokenRequested, AddressOf On_TokenRequested
			_cancellationTokenSource = New CancellationTokenSource()
		End Sub

		Public Function GetCredentials() As System.Net.NetworkCredential Implements ICredentialsProvider.GetCredentials
			Dim creds As NetworkCredential = New NetworkCredential()
			Try
				creds = Task.Run(AddressOf GetCredentialsAsync).Result
			Catch ex As AggregateException
				Throw ex.InnerException
			End Try

			Return creds

		End Function

		Public Async Function GetCredentialsAsync() As Task(Of System.Net.NetworkCredential) Implements ICredentialsProvider.GetCredentialsAsync
			Dim token As String = Await _tokenProvider.GetAccessTokenAsync(_cancellationTokenSource.Token)
			LoginComplete()
			Dim creds As System.Net.NetworkCredential = New NetworkCredential(_OAUTH_USERNAME, token)
			Return creds

		End Function

		Public Sub Cancel() Implements ICredentialsProvider.Cancel
			If(Not _cancellationTokenSource.IsCancellationRequested)
				_cancellationTokenSource.Cancel()
			End If
		End Sub

		Private Sub LoginComplete()
			_cancellationTokenSource = New CancellationTokenSource()
			LoginForm.Close()
		End Sub

		Private Sub On_TokenRequested(source As ITokenProvider, args As ITokenRequestEventArgs)
			LoginForm.Show()
		End Sub
		
	End Class


End Namespace


