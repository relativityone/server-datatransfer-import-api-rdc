Imports kCura.CommandLine
Imports NUnit.Framework

Namespace kCura.EDDS.WinForm.NUnit

<TestFixture()>
Public Class AuthenticationOptionsTests
	Private _commandLine As CommandList	
	Private _authOptions As AuthenticationOptions = New AuthenticationOptions()

		<Test()>
		<TestCase("testClient", "testSecret", "testUser", "testpassword", True)>
		<TestCase(Nothing, "testSecret", "testUser", "testpassword", True)>
		<TestCase(Nothing, "testSecret", Nothing, "testpassword", True)>
		<TestCase(Nothing, "testSecret", "testUser", Nothing, True)>
		<TestCase("testClient", Nothing, Nothing, "testpassword", True)>
		<TestCase("testClient", Nothing, "testUser", Nothing, True)>
		<TestCase("testClient", Nothing, "testUser", "testpassword", True)>
		<TestCase("testClient", "testSecret", "testUser", Nothing, True)>
		<TestCase("testClient", "testSecret", Nothing, "testUser", True)>
		Public Sub Validate_MultipleCredentialExeption(clientId As String, clientSecret As String, userName As String, password As String, expected As Boolean)
		'Arrange
		_commandLine = new CommandList()
		_commandLine.Add(New Command() With {.Directive = "clientID", .Value = clientID })
		_commandLine.Add(New Command() With {.Directive = "clientSecret", .Value = clientSecret })
		_commandLine.Add(New Command() With {.Directive = "u", .Value = userName })
		_commandLine.Add(New Command() With {.Directive = "p", .Value = password })
		' Set
		_authOptions.SetCredentials(_commandLine)
		' Assert
		Assert.Throws (Of Exceptions.MultipleCredentialException)(Sub() _authOptions.CredentialsAreSet())
		
		End Sub

		<Test()>
		<TestCase(Nothing, Nothing, Nothing, Nothing, True)>
		<TestCase(Nothing, Nothing, Nothing, "testpassword", True)>
		Public Sub Validate_UsernameException(clientId As String, clientSecret As String, userName As String, password As String, expected As Boolean)
		'Arrange
		_commandLine = new CommandList()
		_commandLine.Add(New Command() With {.Directive = "clientID", .Value = clientID })
		_commandLine.Add(New Command() With {.Directive = "clientSecret", .Value = clientSecret })
		_commandLine.Add(New Command() With {.Directive = "u", .Value = userName })
		_commandLine.Add(New Command() With {.Directive = "p", .Value = password })
		' Set
		_authOptions.SetCredentials(_commandLine)
		' Assert
		Assert.Throws (Of Exceptions.UsernameException)(Sub() _authOptions.CredentialsAreSet())
		
		End Sub

		<Test()>
		<TestCase(Nothing, Nothing, "testUser", Nothing, True)>
		Public Sub Validate_PasswordException(clientId As String, clientSecret As String, userName As String, password As String, expected As Boolean)
		'Arrange
		_commandLine = new CommandList()
		_commandLine.Add(New Command() With {.Directive = "clientID", .Value = clientID })
		_commandLine.Add(New Command() With {.Directive = "clientSecret", .Value = clientSecret })
		_commandLine.Add(New Command() With {.Directive = "u", .Value = userName })
		_commandLine.Add(New Command() With {.Directive = "p", .Value = password })
		' Set
		_authOptions.SetCredentials(_commandLine)
		' Assert
		Assert.Throws (Of Exceptions.PasswordException)(Sub() _authOptions.CredentialsAreSet())
		
		End Sub

		<Test()>
		<TestCase(Nothing, "testSecret", Nothing, Nothing, True)>
		Public Sub Validate_ClientIDException(clientId As String, clientSecret As String, userName As String, password As String, expected As Boolean)
		'Arrange
		_commandLine = new CommandList()
		_commandLine.Add(New Command() With {.Directive = "clientID", .Value = clientID })
		_commandLine.Add(New Command() With {.Directive = "clientSecret", .Value = clientSecret })
		_commandLine.Add(New Command() With {.Directive = "u", .Value = userName })
		_commandLine.Add(New Command() With {.Directive = "p", .Value = password })
		' Set
		_authOptions.SetCredentials(_commandLine)
		' Assert
		Assert.Throws (Of Exceptions.ClientIDException)(Sub() _authOptions.CredentialsAreSet())
		
		End Sub

		<Test()>
		<TestCase("testClient", Nothing, Nothing, Nothing, True)>
		Public Sub Validate_ClientSecretException(clientId As String, clientSecret As String, userName As String, password As String, expected As Boolean)
		'Arrange
		_commandLine = new CommandList()
		_commandLine.Add(New Command() With {.Directive = "clientID", .Value = clientID })
		_commandLine.Add(New Command() With {.Directive = "clientSecret", .Value = clientSecret })
		_commandLine.Add(New Command() With {.Directive = "u", .Value = userName })
		_commandLine.Add(New Command() With {.Directive = "p", .Value = password })
		' Set
		_authOptions.SetCredentials(_commandLine)
		' Assert
		Assert.Throws (Of Exceptions.ClientSecretException)(Sub() _authOptions.CredentialsAreSet())
		
		End Sub

		<Test()>
		<TestCase("testClient", "testSecret", Nothing, Nothing, True)>
		<TestCase(Nothing, Nothing, "testUser", "testpassword", True)>
		Public Sub Validate_NoExceptionThrown(clientId As String, clientSecret As String, userName As String, password As String, expected As Boolean)
		'Arrange
		_commandLine = new CommandList()
		_commandLine.Add(New Command() With {.Directive = "clientID", .Value = clientID })
		_commandLine.Add(New Command() With {.Directive = "clientSecret", .Value = clientSecret })
		_commandLine.Add(New Command() With {.Directive = "u", .Value = userName })
		_commandLine.Add(New Command() With {.Directive = "p", .Value = password })
		' Set
		_authOptions.SetCredentials(_commandLine)
		' Assert
		Assert.DoesNotThrow((Sub() _authOptions.CredentialsAreSet()))
		
		End Sub
End Class

End Namespace