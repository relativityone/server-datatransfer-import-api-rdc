Imports NUnit.Framework

Namespace kCura.WinEDDS.NUnit.Helpers

	<TestFixture>
	Public Class LoginHelperTests
		Private _applicationName As String
		Private _clientVersion As String
		Private _relativityVersion As String
		Private _exception  As RelativityVersionMismatchException

		<SetUp>
		Public Sub SetUp()
			_applicationName = String.Empty
			_clientVersion = String.Empty
			_relativityVersion = String.Empty
			_exception = Nothing
			kCura.WinEDDS.Config.ConfigSettings(NameOf(Config.ApplicationName)) = String.Empty
		End Sub

		<Test>
		<TestCase(Nothing, "1.0.1.0", "20.0.1.0")>
		<TestCase("Application", Nothing, "20.0.1.0")>
		<TestCase("Application", "1.0.1.0", Nothing)>
		Public Sub ShouldNotThrowWhenTheExplicitParameterIsNullOrEmpty(testApplicationName As String, testClientVersion As String, testRelativityVersion As String)
			GivenTheApplicationName(testApplicationName)
			GivenTheClientVersion(testClientVersion)
			GivenTheRelativityVersion(testRelativityVersion)
			GivenTheTheConfigApplicationName(String.Empty)
			WhenCreatingTheExceptionWithExplicitParameters()
		End Sub

		<Test>
		<TestCase("Relativity Desktop Client", "1.0.1.0", "20.0.1.0")>
		<TestCase("Relativity Integration Points", "2.0.1.0", "21.0.1.0")>
		<TestCase("Processing", "3.0.1.0", "22.0.1.0")>
		Public Sub ShouldCreateTheExceptionWithExplicitParameters(testApplicationName As String, testClientVersion As String, testRelativityVersion As String)
			GivenTheApplicationName(testApplicationName)
			GivenTheClientVersion(testClientVersion)
			GivenTheRelativityVersion(testRelativityVersion)
			GivenTheTheConfigApplicationName(String.Empty)
			WhenCreatingTheExceptionWithExplicitParameters()
			ThenTheExceptionIsNotNull()
			ThenTheExceptionClientVersionIsNotNullOrEmpty()
			ThenTheExceptionRelativityVersionIsNotNullOrEmpty()
			ThenTheExceptionMessageContains(_applicationName)
			ThenTheExceptionMessageContains(_clientVersion)
			ThenTheExceptionMessageContains(_relativityVersion)
		End Sub

		<Test>
		<TestCase("11.0.1.0")>
		<TestCase("12.0.1.0")>
		<TestCase("13.0.1.0")>
		Public Sub ShouldCreateTheExceptionWithAssumedAssemblyParameters(testRelativityVersion As String)
			GivenTheApplicationName(String.Empty)
			GivenTheClientVersion(String.Empty)
			GivenTheRelativityVersion(testRelativityVersion)
			GivenTheTheConfigApplicationName(String.Empty)
			WhenCreatingTheExceptionWithAssumedParameters(System.Reflection.Assembly.GetExecutingAssembly())
			ThenTheExceptionIsNotNull()
			ThenTheExceptionClientVersionIsNotNullOrEmpty()
			ThenTheExceptionRelativityVersionIsNotNullOrEmpty()
			ThenTheExceptionMessageContains(testRelativityVersion)
			ThenTheExceptionMessageContainsTheAssemblyVersion(System.Reflection.Assembly.GetExecutingAssembly())
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
			GivenTheApplicationName(String.Empty)
			GivenTheClientVersion(String.Empty)
			GivenTheRelativityVersion(testRelativityVersion)
			If passAssembly Then
				WhenCreatingTheExceptionWithAssumedParameters(System.Reflection.Assembly.GetExecutingAssembly())
			Else
				WhenCreatingTheExceptionWithAssumedParameters()
			End If
			
			ThenTheExceptionIsNotNull()
			ThenTheExceptionClientVersionIsNotNullOrEmpty()
			ThenTheExceptionRelativityVersionIsNotNullOrEmpty()
			ThenTheExceptionMessageContains(testRelativityVersion)
			If passAssembly Then
				ThenTheExceptionMessageContainsTheAssemblyVersion(System.Reflection.Assembly.GetExecutingAssembly())
			End If
		End Sub

		Private Sub GivenTheTheConfigApplicationName(ByVal name As String)
			kCura.WinEDDS.Config.ConfigSettings(NameOf(Config.ApplicationName)) = name
		End Sub

		Private Sub GivenTheApplicationName(ByVal value As String) 
			_applicationName = value
		End Sub

		Private Sub GivenTheClientVersion(ByVal value As String) 
			_clientVersion = value
		End Sub

		Private Sub GivenTheRelativityVersion(ByVal value As String) 
			_relativityVersion = value
		End Sub

		Private Sub WhenCreatingTheExceptionWithAssumedParameters()
			_exception = kCura.WinEDDS.Api.LoginHelper.CreateRelativityVersionMismatchException(_relativityVersion)
		End Sub

		Private Sub WhenCreatingTheExceptionWithAssumedParameters(ByVal assembly As System.Reflection.Assembly)
			_exception = kCura.WinEDDS.Api.LoginHelper.CreateRelativityVersionMismatchException(_relativityVersion, assembly)
		End Sub

		Private Sub WhenCreatingTheExceptionWithExplicitParameters()
			_exception = kCura.WinEDDS.Api.LoginHelper.CreateRelativityVersionMismatchException(_relativityVersion, _clientVersion, _applicationName)
		End Sub

		Private Sub ThenTheExceptionIsNotNull()
			Assert.That(_exception, [Is].Not.Null)
		End Sub

		Private Sub ThenTheExceptionMessageContains(ByVal expected As String)
			StringAssert.Contains(expected, _exception.Message)
		End Sub

		Private Sub ThenTheExceptionMessageContainsTheAssemblyVersion(ByVal assembly As System.Reflection.Assembly)
			ThenTheExceptionMessageContains(assembly.GetName.Version.ToString())
		End Sub

		Private Sub ThenTheExceptionClientVersionIsNotNullOrEmpty()
			Assert.That(_exception.ClientVersion, [Is].Not.Null.Or.Empty)
		End Sub

		Private Sub ThenTheExceptionRelativityVersionIsNotNullOrEmpty()
			Assert.That(_exception.RelativityVersion, [Is].Not.Null.Or.Empty)
		End Sub
	End Class
End NameSpace