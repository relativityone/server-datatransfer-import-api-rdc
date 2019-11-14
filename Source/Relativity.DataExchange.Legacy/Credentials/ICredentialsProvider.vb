Imports System.Threading
Imports System.Threading.Tasks

Namespace kCura.WinEDDS.Credentials
	Public Interface ICredentialsProvider

		Function GetCredentials() As System.Net.NetworkCredential

		Function GetCredentialsAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of System.Net.NetworkCredential)

	End Interface
End Namespace


