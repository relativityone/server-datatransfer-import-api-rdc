Imports kCura.WinEDDS.Helpers
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Imports NUnit.Framework


Namespace kCura.WinEDDS.NUnit.Helpers
	<TestFixture>
	Public Class CaseSensitiveFilePathHelperTests
		Private _filePathHelper As CaseSensitiveFilePathHelper
		Private _fileMock As kCura.WinEDDS.TApi.IFile
		Private _pathMock As kCura.WinEDDS.TApi.IPath

		Private Const _PATH_WITH_LOWER_CASE_EXTENSION As String = "\\dir\somePath.ext"
		Private Const _PATH_WITH_UPPER_CASE_EXTENSION As String = "\\dir\somePath.EXT"
		Private Const _PATH_WITH_MIXED_CASE_EXTENSION As String = "\\dir\somePath.eXt"
		Private Const _PATH_WITH_NO_EXTENSION As String = "\\dir\somePath"

		<SetUp> Public Sub SetUp()
			_fileMock = Substitute.For(Of kCura.WinEDDS.TApi.IFile)()
			_pathMock = Substitute.For(Of kCura.WinEDDS.TApi.IPath)()
			_pathMock.GetExtension(Arg.Any(Of String)).ReturnsForAnyArgs(function(info) System.IO.Path.GetExtension(info.Arg(Of String)()))
			_pathMock.ChangeExtension(Arg.Any(Of String), Arg.Any(Of String)).ReturnsForAnyArgs(function(info) System.IO.Path.ChangeExtension(info.ArgAt(Of String)(0), info.ArgAt(Of String)(1)))
			Dim fileSystemMock As kCura.WinEDDS.TApi.IFileSystem = Substitute.For(Of kCura.WinEDDS.TApi.IFileSystem)()
			fileSystemMock.File.Returns(_fileMock)
			fileSystemMock.Path.Returns(_pathMock)
			_filePathHelper = New CaseSensitiveFilePathHelper(fileSystemMock)
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
			_fileMock.Exists(Arg.Any(Of String)()).Throws(New Exception())

			Dim actual As String = _filePathHelper.GetExistingFilePath(Nothing)

			Assert.IsNull(actual)
		End Sub

		<Test>
		Public Sub ShouldReturnSamePathIfOriginalOneExists()
			_fileMock.Exists(_PATH_WITH_LOWER_CASE_EXTENSION).Returns(True)

			Dim actual As String = _filePathHelper.GetExistingFilePath(_PATH_WITH_LOWER_CASE_EXTENSION)

			Assert.AreEqual(actual, _PATH_WITH_LOWER_CASE_EXTENSION)
		End Sub

		<Test>
		Public Sub ShouldNotCheckOtherPathsIfOriginalOneExists()
			_fileMock.Exists(_PATH_WITH_LOWER_CASE_EXTENSION).Returns(True)

			_filePathHelper.GetExistingFilePath(_PATH_WITH_LOWER_CASE_EXTENSION)

			_fileMock.Received(1).Exists(Arg.Any(Of String)())
		End Sub

		<Test>
		Public Sub ShouldCheckForUpperCaseWhenLowerDoesNotExist()
			Dim pathToCheck As String = _PATH_WITH_LOWER_CASE_EXTENSION
			Dim expected As String = _PATH_WITH_UPPER_CASE_EXTENSION

			_fileMock.Exists(pathToCheck).Returns(False)
			_fileMock.Exists(expected).Returns(True)

			Dim actual As String = _filePathHelper.GetExistingFilePath(pathToCheck)

			Assert.AreEqual(actual, expected)
		End Sub

		<Test>
		Public Sub ShouldCheckForLowerCaseWheUpperDoesNotExist()
			Dim pathToCheck As String = _PATH_WITH_UPPER_CASE_EXTENSION
			Dim expected As String = _PATH_WITH_LOWER_CASE_EXTENSION

			_fileMock.Exists(pathToCheck).Returns(False)
			_fileMock.Exists(expected).Returns(True)

			Dim actual As String = _filePathHelper.GetExistingFilePath(pathToCheck)

			Assert.AreEqual(actual, expected)
		End Sub

		<Test>
		Public Sub ShouldReturnUpperWhenLowerDoesNotExistAndMixedIsUsed()
			Dim pathToCheck As String = _PATH_WITH_MIXED_CASE_EXTENSION
			Dim expected As String = _PATH_WITH_UPPER_CASE_EXTENSION

			_fileMock.Exists(_PATH_WITH_LOWER_CASE_EXTENSION).Returns(False)
			_fileMock.Exists(pathToCheck).Returns(False)
			_fileMock.Exists(expected).Returns(True)

			Dim actual As String = _filePathHelper.GetExistingFilePath(pathToCheck)

			Assert.AreEqual(actual, expected)
		End Sub

		<Test>
		Public Sub ShouldReturnLowerWhenUpperDoesNotExistAndMixedIsUsed()
			Dim pathToCheck As String = _PATH_WITH_MIXED_CASE_EXTENSION
			Dim expected As String = _PATH_WITH_LOWER_CASE_EXTENSION

			_fileMock.Exists(_PATH_WITH_UPPER_CASE_EXTENSION).Returns(False)
			_fileMock.Exists(pathToCheck).Returns(False)
			_fileMock.Exists(expected).Returns(True)

			Dim actual As String = _filePathHelper.GetExistingFilePath(pathToCheck)

			Assert.AreEqual(actual, expected)
		End Sub

		<Test>
		Public Sub ShouldTryUpperAndLowerWhenMixedDoesNotExist()
			_fileMock.Exists(_PATH_WITH_LOWER_CASE_EXTENSION).Returns(False)
			_fileMock.Exists(_PATH_WITH_UPPER_CASE_EXTENSION).Returns(False)
			_fileMock.Exists(_PATH_WITH_MIXED_CASE_EXTENSION).Returns(False)
			
			_filePathHelper.GetExistingFilePath(_PATH_WITH_MIXED_CASE_EXTENSION)

			_fileMock.Received(1).Exists(_PATH_WITH_MIXED_CASE_EXTENSION)
			_fileMock.Received(1).Exists(_PATH_WITH_UPPER_CASE_EXTENSION)
			_fileMock.Received(1).Exists(_PATH_WITH_LOWER_CASE_EXTENSION)
		End Sub

		<Test>
		Public Sub ShouldReturnNullWhenNoneExists()
			_fileMock.Exists(Arg.Any(Of String)).Returns(False)
			
			Dim actual As String = _filePathHelper.GetExistingFilePath(_PATH_WITH_MIXED_CASE_EXTENSION)

			Assert.IsNull(actual)
		End Sub

		<Test>
		Public Sub ShouldReturnSamePathIfExistsAndThereIsNoExtension()
			Dim pathToCheck As String = _PATH_WITH_NO_EXTENSION
			Dim expected As String = _PATH_WITH_NO_EXTENSION

			_fileMock.Exists(expected).Returns(True)

			Dim actual As String = _filePathHelper.GetExistingFilePath(pathToCheck)

			Assert.AreEqual(actual, expected)
		End Sub

		<Test>
		Public Sub ShouldReturnNullWhenPathDoesNotExistAndThereIsNoExtension()
			Dim pathToCheck As String = _PATH_WITH_NO_EXTENSION

			_fileMock.Exists(pathToCheck).Returns(False)

			Dim actual As String = _filePathHelper.GetExistingFilePath(pathToCheck)

			Assert.IsNull(actual)
		End Sub

		<Test>
		Public Sub ShouldNotCheckOtherPathsIfThereIsNoExtensionAndOriginalOneDoesNotExist()
			Dim pathToCheck As String = _PATH_WITH_NO_EXTENSION

			_fileMock.Exists(pathToCheck).Returns(False)

			_filePathHelper.GetExistingFilePath(pathToCheck)

			_fileMock.Received(1).Exists(Arg.Any(Of String)())
		End Sub
	End Class
End Namespace



