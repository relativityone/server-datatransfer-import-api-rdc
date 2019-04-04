Imports kCura.CommandLine
Imports kCura.EDDS.WinForm

Public Class AuthenticationOptions
	Private _userName As String
	Private _password As String

	Sub New()
	End Sub

	Public Property UserName As String
		Get
			return _userName
		End Get
		Private Set(value As String)
			_userName = value
		End Set
	End Property

	Public Property Password As String
		Get
			return _password
		End Get
		Private Set(value As String)
			_password = value
		End Set
	End Property

	Public Property ClientId As String
	Public Property ClientSecret As String

	Public Sub CredentialsAreSet()

		Dim usernameExists As Boolean = Not String.IsNullOrEmpty(Username)
		Dim passwordExists As Boolean = Not String.IsNullOrEmpty(Password)
		Dim clientIDExists As Boolean = Not String.IsNullOrEmpty(ClientId)
		Dim clientSecretExists As Boolean = Not String.IsNullOrEmpty(ClientSecret)

		If (usernameExists Or passwordExists) AndAlso (clientIDExists Or clientSecretExists) Then
			Throw New Exceptions.MultipleCredentialException
		End If

		If Not clientIDExists AndAlso Not clientSecretExists Then
			If Not usernameExists Then Throw New Exceptions.UsernameException
			If Not passwordExists Then Throw New Exceptions.PasswordException
		Else
			If Not clientIDExists Then Throw New Exceptions.ClientIDException
			If Not clientSecretExists Then Throw New Exceptions.ClientSecretException
		End If
	End Sub

	Public Sub SetCredentials(commandLine As CommandList)

		ClientId = ImportOptions.GetValueFromCommandListByFlag(commandLine, "clientID")
		ClientSecret = ImportOptions.GetValueFromCommandListByFlag(commandLine, "clientSecret")

		UserName = ImportOptions.GetValueFromCommandListByFlag(commandLine, "u")
		Password = ImportOptions.GetValueFromCommandListByFlag(commandLine, "p")
	End Sub
End Class