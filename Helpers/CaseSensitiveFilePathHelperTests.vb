Imports kCura.WinEDDS.Helpers
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Imports NUnit.Framework


Namespace kCura.WinEDDS.NUnit.Helpers
	<TestFixture>
	Public Class CaseSensitiveFilePathHelperTests
		Private _filePathHelper As CaseSensitiveFilePathHelper
		Private _systemIoMock As ISystemIoFileWrapper

		Private Const _PATH_WITH_LOWER_CASE_EXTENSION As String = "\\dir\somePath.ext"
		Private Const _PATH_WITH_UPPER_CASE_EXTENSION As String = "\\dir\somePath.EXT"
		Private Const _PATH_WITH_MIXED_CASE_EXTENSION As String = "\\dir\somePath.eXt"
		Private Const _PATH_WITH_NO_EXTENSION As String = "\\dir\somePath"

		<SetUp> Public Sub SetUp()
			_systemIoMock = Substitute.For(Of ISystemIoFileWrapper)()
			_systemIoMock.GetExtension(Arg.Any(Of String)).ReturnsForAnyArgs(function(info) System.IO.Path.GetExtension(info.Arg(Of String)()))
			_systemIoMock.ChangeExtension(Arg.Any(Of String), Arg.Any(Of String)).ReturnsForAnyArgs(function(info) System.IO.Path.ChangeExtension(info.ArgAt(Of String)(0), info.ArgAt(Of String)(1)))

			_filePathHelper = New CaseSensitiveFilePathHelper(_systemIoMock)
		End Sub

		<Test>
		Public Sub ShouldReturnNothingWhenArgumentIsNothing()
			Dim actual As String = _filePathHelper.GetExistingFilePath(Nothing)

			Assert.IsNull(actual)
		End Sub

		<Test>
		Public Sub ShouldReturnNothingWhenArgumentIsEmpty()
			Dim actual As String = _filePathHelper.GetExistingFilePath(String.Empty)

			Assert.IsNull(actual)
		End Sub

		<Test>
		Public Sub ShouldReturnNothingWhenWrapperThrows()
			_systemIoMock.Exists(Arg.Any(Of String)()).Throws(New Exception())

			Dim actual As String = _filePathHelper.GetExistingFilePath(Nothing)

			Assert.IsNull(actual)
		End Sub

		<Test>
		Public Sub ShouldReturnSamePathIfOriginalOneExists()
			_systemIoMock.Exists(_PATH_WITH_LOWER_CASE_EXTENSION).Returns(True)

			Dim actual As String = _filePathHelper.GetExistingFilePath(_PATH_WITH_LOWER_CASE_EXTENSION)

			Assert.AreEqual(actual, _PATH_WITH_LOWER_CASE_EXTENSION)
		End Sub

		<Test>
		Public Sub ShouldNotCheckOtherPathsIfOriginalOneExists()
			_systemIoMock.Exists(_PATH_WITH_LOWER_CASE_EXTENSION).Returns(True)

			_filePathHelper.GetExistingFilePath(_PATH_WITH_LOWER_CASE_EXTENSION)

			_systemIoMock.Received(1).Exists(Arg.Any(Of String)())
		End Sub

		<Test>
		Public Sub ShouldCheckForUpperCaseWhenLowerDoesNotExist()
			Dim pathToCheck As String = _PATH_WITH_LOWER_CASE_EXTENSION
			Dim expected As String = _PATH_WITH_UPPER_CASE_EXTENSION

			_systemIoMock.Exists(pathToCheck).Returns(False)
			_systemIoMock.Exists(expected).Returns(True)

			Dim actual As String = _filePathHelper.GetExistingFilePath(pathToCheck)

			Assert.AreEqual(actual, expected)
		End Sub

		<Test>
		Public Sub ShouldCheckForLowerCaseWheUpperDoesNotExist()
			Dim pathToCheck As String = _PATH_WITH_UPPER_CASE_EXTENSION
			Dim expected As String = _PATH_WITH_LOWER_CASE_EXTENSION

			_systemIoMock.Exists(pathToCheck).Returns(False)
			_systemIoMock.Exists(expected).Returns(True)

			Dim actual As String = _filePathHelper.GetExistingFilePath(pathToCheck)

			Assert.AreEqual(actual, expected)
		End Sub

		<Test>
		Public Sub ShouldReturnUpperWhenLowerDoesNotExistAndMixedIsUsed()
			Dim pathToCheck As String = _PATH_WITH_MIXED_CASE_EXTENSION
			Dim expected As String = _PATH_WITH_UPPER_CASE_EXTENSION

			_systemIoMock.Exists(_PATH_WITH_LOWER_CASE_EXTENSION).Returns(False)
			_systemIoMock.Exists(pathToCheck).Returns(False)
			_systemIoMock.Exists(expected).Returns(True)

			Dim actual As String = _filePathHelper.GetExistingFilePath(pathToCheck)

			Assert.AreEqual(actual, expected)
		End Sub

		<Test>
		Public Sub ShouldReturnLowerWhenUpperDoesNotExistAndMixedIsUsed()
			Dim pathToCheck As String = _PATH_WITH_MIXED_CASE_EXTENSION
			Dim expected As String = _PATH_WITH_LOWER_CASE_EXTENSION

			_systemIoMock.Exists(_PATH_WITH_UPPER_CASE_EXTENSION).Returns(False)
			_systemIoMock.Exists(pathToCheck).Returns(False)
			_systemIoMock.Exists(expected).Returns(True)

			Dim actual As String = _filePathHelper.GetExistingFilePath(pathToCheck)

			Assert.AreEqual(actual, expected)
		End Sub

		<Test>
		Public Sub ShouldTryUpperAndLowerWhenMixedDoesNotExist()
			_systemIoMock.Exists(_PATH_WITH_LOWER_CASE_EXTENSION).Returns(False)
			_systemIoMock.Exists(_PATH_WITH_UPPER_CASE_EXTENSION).Returns(False)
			_systemIoMock.Exists(_PATH_WITH_MIXED_CASE_EXTENSION).Returns(False)
			
			_filePathHelper.GetExistingFilePath(_PATH_WITH_MIXED_CASE_EXTENSION)

			_systemIoMock.Received(1).Exists(_PATH_WITH_MIXED_CASE_EXTENSION)
			_systemIoMock.Received(1).Exists(_PATH_WITH_UPPER_CASE_EXTENSION)
			_systemIoMock.Received(1).Exists(_PATH_WITH_LOWER_CASE_EXTENSION)
		End Sub

		<Test>
		Public Sub ShouldReturnNullWhenNoneExists()
			_systemIoMock.Exists(Arg.Any(Of String)).Returns(False)
			
			Dim actual As String = _filePathHelper.GetExistingFilePath(_PATH_WITH_MIXED_CASE_EXTENSION)

			Assert.IsNull(actual)
		End Sub

		<Test>
		Public Sub ShouldReturnSamePathIfExistsAndThereIsNoExtension()
			Dim pathToCheck As String = _PATH_WITH_NO_EXTENSION
			Dim expected As String = _PATH_WITH_NO_EXTENSION

			_systemIoMock.Exists(expected).Returns(True)

			Dim actual As String = _filePathHelper.GetExistingFilePath(pathToCheck)

			Assert.AreEqual(actual, expected)
		End Sub

		<Test>
		Public Sub ShouldReturnNullWhenPathDoesNotExistAndThereIsNoExtension()
			Dim pathToCheck As String = _PATH_WITH_NO_EXTENSION

			_systemIoMock.Exists(pathToCheck).Returns(False)

			Dim actual As String = _filePathHelper.GetExistingFilePath(pathToCheck)

			Assert.IsNull(actual)
		End Sub

		<Test>
		Public Sub ShouldNotCheckOtherPathsIfThereIsNoExtensionAndOriginalOneDoesNotExist()
			Dim pathToCheck As String = _PATH_WITH_NO_EXTENSION

			_systemIoMock.Exists(pathToCheck).Returns(False)

			_filePathHelper.GetExistingFilePath(pathToCheck)

			_systemIoMock.Received(1).Exists(Arg.Any(Of String)())
		End Sub
	End Class
End Namespace



