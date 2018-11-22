Imports NUnit.Framework

Namespace kCura.WinEDDS.NUnit.Helpers

	<TestFixture>
	Public Class LoginHelperTests
		Private _exception  As RelativityVersionMismatchException

		<SetUp>
		Public Sub SetUp()
			_exception = Nothing
			kCura.WinEDDS.Config.ConfigSettings(NameOf(Config.ApplicationName)) = String.Empty
		End Sub

		<Test>
		<TestCase(Nothing, "1.0.1.0", "20.0.1.0")>
		<TestCase("Application", Nothing, "20.0.1.0")>
		<TestCase("Application", "1.0.1.0", Nothing)>
		Public Sub ShouldNotThrowWhenTheExplicitParameterIsNullOrEmpty(testApplicationName As String, testClientVersion As String, testRelativityVersion As String)
			GivenTheTheConfigApplicationName(String.Empty)
			WhenCreatingTheExceptionWithExplicitParameters(testRelativityVersion, testClientVersion, testApplicationName)
			If String.IsNullOrEmpty(testApplicationName) Then
				ThenTheExceptionMessageShouldEqualTheExpectedValue(kCura.WinEDDS.Api.LoginHelper.DefaultApplicationName, testClientVersion, testRelativityVersion)
			Else If String.IsNullOrEmpty(testClientVersion) Then
				ThenTheExceptionMessageShouldEqualTheExpectedValue(testApplicationName, kCura.WinEDDS.Api.LoginHelper.DefaultUnknownVersion, testRelativityVersion)
			Else If String.IsNullOrEmpty(testRelativityVersion) Then
				ThenTheExceptionMessageShouldEqualTheExpectedValue(testApplicationName, testClientVersion, kCura.WinEDDS.Api.LoginHelper.DefaultUnknownVersion)
			End If
		End Sub

		<Test>
		<TestCase("Relativity Desktop Client", "1.0.1.0", "20.0.1.0")>
		<TestCase("Relativity Integration Points", "2.0.1.0", "21.0.1.0")>
		<TestCase("Processing", "3.0.1.0", "22.0.1.0")>
		Public Sub ShouldCreateTheExceptionWithExplicitParameters(testApplicationName As String, testClientVersion As String, testRelativityVersion As String)
			GivenTheTheConfigApplicationName(String.Empty)
			WhenCreatingTheExceptionWithExplicitParameters(testRelativityVersion, testClientVersion, testApplicationName)
			ThenTheExceptionIsNotNull()
			ThenTheExceptionClientVersionIsNotNullOrEmpty()
			ThenTheExceptionRelativityVersionIsNotNullOrEmpty()
			ThenTheExceptionMessageShouldEqualTheExpectedValue(testApplicationName, testClientVersion, testRelativityVersion)
		End Sub

		<Test>
		<TestCase("11.0.1.0")>
		<TestCase("12.0.1.0")>
		<TestCase("13.0.1.0")>
		Public Sub ShouldCreateTheExceptionWithAssumedAssemblyParameters(testRelativityVersion As String)
			GivenTheTheConfigApplicationName(String.Empty)
			WhenCreatingTheExceptionWithAssumedParameters(testRelativityVersion, GetAssumedAssembly())
			ThenTheExceptionIsNotNull()
			ThenTheExceptionClientVersionIsNotNullOrEmpty()
			ThenTheExceptionRelativityVersionIsNotNullOrEmpty()
			ThenTheExceptionMessageShouldEqualTheExpectedValue("Import API", GetAssumedAssemblyVersion(), testRelativityVersion)
		End Sub

		<Test>
		<TestCase("My RDC", "20.0.1.0", True)>
		<TestCase("My RDC", "20.0.1.0", False)>
		<TestCase("My RIP", "21.0.1.0", True)>
		<TestCase("My RIP", "21.0.1.0", False)>
		<TestCase("My Processing", "22.0.1.0", True)>
		<TestCase("My Processing", "22.0.1.0", False)>
		Public Sub ShouldCreateTheExceptionWithAssumedAssemblyAndConfigParameters(testConfigApplicationName As String, testRelativityVersion As String, passAssembly As Boolean)
			GivenTheTheConfigApplicationName(testConfigApplicationName)
			If passAssembly Then
				WhenCreatingTheExceptionWithAssumedParameters(testRelativityVersion, GetAssumedAssembly())
			Else
				WhenCreatingTheExceptionWithAssumedParameters(testRelativityVersion)
			End If
			
			ThenTheExceptionIsNotNull()
			ThenTheExceptionClientVersionIsNotNullOrEmpty()
			ThenTheExceptionRelativityVersionIsNotNullOrEmpty()
			ThenTheExceptionMessageShouldEqualTheExpectedValue(testConfigApplicationName, GetAssumedAssemblyVersion(), testRelativityVersion)
		End Sub

		Private Sub GivenTheTheConfigApplicationName(ByVal name As String)
			kCura.WinEDDS.Config.ConfigSettings(NameOf(Config.ApplicationName)) = name
		End Sub

		Private Sub WhenCreatingTheExceptionWithAssumedParameters(relativityVersion As String)
			_exception = kCura.WinEDDS.Api.LoginHelper.CreateRelativityVersionMismatchException(relativityVersion)
		End Sub

		Private Sub WhenCreatingTheExceptionWithAssumedParameters(relativityVersion As String, ByVal assembly As System.Reflection.Assembly)
			_exception = kCura.WinEDDS.Api.LoginHelper.CreateRelativityVersionMismatchException(relativityVersion, assembly)
		End Sub

		Private Sub WhenCreatingTheExceptionWithExplicitParameters(relativityVersion As String, clientVersion As String, applicationName As String)
			_exception = kCura.WinEDDS.Api.LoginHelper.CreateRelativityVersionMismatchException(relativityVersion, clientVersion, applicationName)
		End Sub

		Private Sub ThenTheExceptionIsNotNull()
			Assert.That(_exception, [Is].Not.Null)
		End Sub

		Private Sub ThenTheExceptionClientVersionIsNotNullOrEmpty()
			Assert.That(_exception.ClientVersion, [Is].Not.Null.Or.Empty)
		End Sub

		Private Sub ThenTheExceptionRelativityVersionIsNotNullOrEmpty()
			Assert.That(_exception.RelativityVersion, [Is].Not.Null.Or.Empty)
		End Sub

		Private Function GetAssumedAssembly() As System.Reflection.Assembly
			' The assumed assembly name is taken from the kCura.WinEDDS assembly.
			Return GetType(kCura.WinEDDS.Api.LoginHelper).Assembly
		End Function

		Private Function GetAssumedAssemblyVersion() As String
			Return GetAssumedAssembly().GetName().Version.ToString()
		End Function

		Private Sub ThenTheExceptionMessageShouldEqualTheExpectedValue(applicationName As String, clientVersion As String, relativityVersion As String)
			Dim expectedMessage As String = kCura.WinEDDS.Api.LoginHelper.CreateRelativityVersionMismatchMessage(relativityVersion, clientVersion, applicationName)
			Assert.That(_exception.Message, [Is].EqualTo(expectedMessage))
		End Sub
	End Class
End NameSpace