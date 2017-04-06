
Imports System.Threading.Tasks

Namespace kCura.WinEDDS.Credentials
	Public Interface ICredentialsProvider

		Function GetCredentials() As System.Net.NetworkCredential

		Function GetCredentialsAsync() As Task(Of System.Net.NetworkCredential)

		Sub Cancel()

	End Interface
End Namespace


