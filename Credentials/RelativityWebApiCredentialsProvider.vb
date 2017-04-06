Imports System.Threading
Imports System.Threading.Tasks

Namespace kCura.WinEDDS.Credentials

	Public Class RelativityWebApiCredentialsProvider
		Implements ICredentialsProvider

		Private ReadOnly Shared _instance As Lazy(Of RelativityWebApiCredentialsProvider) = new Lazy(Of RelativityWebApiCredentialsProvider)(Function() New RelativityWebApiCredentialsProvider())
		Private _provider As ICredentialsProvider

		Private Sub New ()
		End Sub

		Public Shared Function Instance As RelativityWebApiCredentialsProvider
			return _instance.Value
		End Function

		Public Sub SetProvider(provider As ICredentialsProvider)
			_provider = provider
		End Sub

		Public Function ProviderType() As System.Type
			return _provider.GetType()
		End Function

		Public Function CredentialsSet As Boolean
			return (_provider IsNot Nothing)
		End Function

		Public Function GetCredentials() As System.Net.NetworkCredential Implements ICredentialsProvider.GetCredentials
			If _provider Is Nothing
				Throw new CredentialsNotSetException()
			End If

			return _provider.GetCredentials()
		End Function

		Public Function GetCredentialsAsync() As Task(Of System.Net.NetworkCredential) Implements ICredentialsProvider.GetCredentialsAsync
			If _provider Is Nothing
				Throw new CredentialsNotSetException()
			End If

			return _provider.GetCredentialsAsync()
		End Function

		Public Sub Cancel() Implements ICredentialsProvider.Cancel
			_provider.Cancel()
		End Sub
	End Class

End Namespace


