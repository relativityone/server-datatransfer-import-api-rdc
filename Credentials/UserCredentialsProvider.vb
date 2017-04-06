
Imports System.Net
Imports System.Threading.Tasks

Namespace kCura.WinEDDS.Credentials

	Public Class UserCredentialsProvider
		Implements ICredentialsProvider

		Private ReadOnly _creds As System.Net.NetworkCredential

		Public Sub New(creds As System.Net.NetworkCredential)
			_creds = creds
		End Sub

		Public Sub New(userName As String, password As String)
			_creds = new NetworkCredential(userName, password)
		End Sub

		Public Sub Cancel() Implements ICredentialsProvider.Cancel
			
		End Sub

		Public Function GetCredentials() As NetworkCredential Implements ICredentialsProvider.GetCredentials
			return _creds
		End Function

		Public Function GetCredentialsAsync() As Task(Of NetworkCredential) Implements ICredentialsProvider.GetCredentialsAsync
			return Task.FromResult(_creds)
		End Function
	End Class

End Namespace

