Imports System.Net
Imports System.Threading
Imports System.Windows.Forms
Imports kCura.WinEDDS.Service
Imports Relativity.OAuth2Client.Interfaces
Imports Relativity.OAuth2Client.Interfaces.Events

Namespace kCura.WinEDDS.Credentials

	Public Class IntegratedAuthenticationOAuthCredentialsProvider
		Private Const _TOKEN_TIMEOUT_IN_SECOND As Integer = 20

		Private ReadOnly _lock As Object = New Object()
		Private _newCred As NetworkCredential
		Private ReadOnly _relativityManager As RelativityManager

		Public Sub New(relativityManager As RelativityManager)
			_relativityManager = relativityManager
		End Sub

		Public Function LoginWindowsAuthTapi() As NetworkCredential
			SyncLock _lock
				Dim t As Thread = New Thread(AddressOf LoadOAuthToken)
				t.SetApartmentState(ApartmentState.STA)
				t.Start()
				t.Join()
				Return _newCred
			End SyncLock
		End Function

		Private Sub LoadOAuthToken()
			Dim oAuth2ImplicitCredentialsHelper As OAuth2ImplicitCredentialsHelper = New OAuth2ImplicitCredentialsHelper(AddressOf GetIdentityServerLocation, AddressOf On_TokenRetrieved)

			Dim cancellationTokenSource As CancellationTokenSource= New CancellationTokenSource(TimeSpan.FromSeconds(_TOKEN_TIMEOUT_IN_SECOND))
			Dim awaiter As Runtime.CompilerServices.TaskAwaiter(Of NetworkCredential) = oAuth2ImplicitCredentialsHelper.GetCredentialsAsync().GetAwaiter()
			While Not awaiter.IsCompleted
				If cancellationTokenSource.IsCancellationRequested Then
					Throw new TimeoutException("Token retrieval operation has timed-out.")
				End If
				Application.DoEvents()
				Thread.Sleep(TimeSpan.FromMilliseconds(50))
			End While
			_newCred = awaiter.GetResult()
		End Sub

		Private Function GetIdentityServerLocation() As String
			Dim urlString As String = $"{_relativityManager.GetRelativityUrl()}/{"Identity"}"
			Return urlString
		End Function

		Private Sub On_TokenRetrieved(source As ITokenProvider, args As ITokenResponseEventArgs)
		End Sub
	End Class
End NameSpace