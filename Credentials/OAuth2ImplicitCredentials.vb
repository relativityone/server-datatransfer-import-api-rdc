﻿Imports System.Diagnostics
Imports System.Net
Imports System.Threading
Imports System.Threading.Tasks
Imports Credentials
Imports kCura.WinEDDS.Exceptions
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
		Private Readonly _loginFormFacotry As Func(Of LoginForm)
		Private ReadOnly _stsUri As Uri
		Private ReadOnly _clientId As String
		Private _loginView As ILoginView
		Private _cancellationTokenSource As CancellationTokenSource
		Private _currentCredentials As NetworkCredential
		
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
			_stsUri = stsUri
			_clientId = clientID
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
			Try
				If _loginView.Browser.IsDisposed
					CreateTokenProvider()
				End If

				If _cancellationTokenSource.IsCancellationRequested
					_cancellationTokenSource = New CancellationTokenSource()
				End If

				Dim token As String = Await _tokenProvider.GetAccessTokenAsync(_cancellationTokenSource.Token)
				LoginComplete()
				Dim creds As System.Net.NetworkCredential = New NetworkCredential(_OAUTH_USERNAME, token)
				Return creds
			Catch ex As TaskCanceledException
				Throw new LoginCanceledException(ex)
			End Try
		End Function

		Private Sub CreateTokenProvider()
			_loginView = New LoginView(_stsUri, new Uri(_REDIRECT_URI), _clientId, LoginForm.browser)
			Dim providerFactory As Relativity.OAuth2Client.Interfaces.ITokenProviderFactory = New ImplicitTokenProviderFactory(_loginView)
			Dim tokenProvider As Relativity.OAuth2Client.Interfaces.ITokenProvider = providerFactory.GetTokenProvider("WebApi", New String() {"UserInfoAccess"})
			_tokenProvider = tokenProvider
			Events = _tokenProvider.Events
			AddHandler Events.TokenRequested, AddressOf On_TokenRequested
			AddHandler LoginForm.Closed, AddressOf On_LoginFormClosing
		End Sub

		Private Sub LoginComplete()
			_cancellationTokenSource = New CancellationTokenSource()
			LoginForm.Close()
		End Sub

		Private Sub On_LoginFormClosing(ByVal sender As Object, ByVal e As EventArgs)
			If(Not _cancellationTokenSource.IsCancellationRequested)
				_cancellationTokenSource.Cancel()
				_cancellationTokenSource = New CancellationTokenSource()
			End If
		 End Sub

		Private Sub On_TokenRequested(source As ITokenProvider, args As ITokenRequestEventArgs)
			LoginForm.TopLevel = True
			LoginForm.TopMost = True
			LoginForm.Show()
		End Sub
		
	End Class


End Namespace


