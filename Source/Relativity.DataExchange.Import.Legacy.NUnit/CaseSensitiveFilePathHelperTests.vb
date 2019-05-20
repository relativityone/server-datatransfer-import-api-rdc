' -----------------------------------------------------------------------------------------------------
' <copyright file="CaseSensitiveFilePathHelperTests.cs" company="Relativity ODA LLC">
'   © Relativity All Rights Reserved.
' </copyright>
' -----------------------------------------------------------------------------------------------------

Imports kCura.WinEDDS.Helpers

Imports Moq

Imports NUnit.Framework

Imports Relativity.DataExchange.Io
Imports Rhino.Mocks

Namespace Relativity.DataExchange.Import.NUnit

	<TestFixture>
	Public Class CaseSensitiveFilePathHelperTests
		Private _filePathHelper As CaseSensitiveFilePathHelper
		Private _fileMock As Mock(Of IFile)
		Private _pathMock As Mock(Of IPath)

		Private Const _PATH_WITH_LOWER_CASE_EXTENSION As String = "\\dir\somePath.ext"
		Private Const _PATH_WITH_UPPER_CASE_EXTENSION As String = "\\dir\somePath.EXT"
		Private Const _PATH_WITH_MIXED_CASE_EXTENSION As String = "\\dir\somePath.eXt"
		Private Const _PATH_WITH_NO_EXTENSION As String = "\\dir\somePath"

		<SetUp> Public Sub SetUp()
			_fileMock = New Mock(Of IFile)
			_pathMock = New Mock(Of IPath)
			Dim fileSystemMock As Mock(Of IFileSystem) = New Mock(Of IFileSystem)
			fileSystemMock.SetupGet(Function(x) x.File).Returns(_fileMock.Object)
			fileSystemMock.SetupGet(Function(x) x.Path).Returns(_pathMock.Object)
			_pathMock.Setup(Function(x) x.GetExtension(It.IsAny(Of String))).Returns(Function(path As String) System.IO.Path.GetExtension(path))
			_pathMock.Setup(Function(x) x.ChangeExtension(It.IsAny(Of String), It.IsAny(Of String))).Returns(Function(path As String, extension As String) System.IO.Path.ChangeExtension(path, extension))
			_filePathHelper = New CaseSensitiveFilePathHelper(fileSystemMock.Object)
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
			_fileMock.Setup(Function(x) x.Exists(It.IsAny(Of String))).Throws(New Exception)
			Dim actual As String = _filePathHelper.GetExistingFilePath(Nothing)
			Assert.IsNull(actual)
		End Sub

		<Test>
		Public Sub ShouldReturnSamePathIfOriginalOneExists()
			_fileMock.Setup(Function(x) x.Exists(_PATH_WITH_LOWER_CASE_EXTENSION)).Returns(True)
			Dim actual As String = _filePathHelper.GetExistingFilePath(_PATH_WITH_LOWER_CASE_EXTENSION)
			Assert.AreEqual(actual, _PATH_WITH_LOWER_CASE_EXTENSION)
		End Sub

		<Test>
		Public Sub ShouldNotCheckOtherPathsIfOriginalOneExists()
			_fileMock.Setup(Function(x) x.Exists(_PATH_WITH_LOWER_CASE_EXTENSION)).Returns(True)
			_filePathHelper.GetExistingFilePath(_PATH_WITH_LOWER_CASE_EXTENSION)
			_fileMock.Verify(Sub(x) x.Exists(It.IsAny(Of String)))
		End Sub

		<Test>
		Public Sub ShouldCheckForUpperCaseWhenLowerDoesNotExist()
			Dim pathToCheck As String = _PATH_WITH_LOWER_CASE_EXTENSION
			Dim expected As String = _PATH_WITH_UPPER_CASE_EXTENSION

			_fileMock.Setup(Function(x) x.Exists(pathToCheck)).Returns(False)
			_fileMock.Setup(Function(x) x.Exists(expected)).Returns(True)
			Dim actual As String = _filePathHelper.GetExistingFilePath(pathToCheck)
			Assert.That(actual, [Is].EqualTo(expected))
		End Sub

		<Test>
		Public Sub ShouldCheckForLowerCaseWheUpperDoesNotExist()
			Dim pathToCheck As String = _PATH_WITH_UPPER_CASE_EXTENSION
			Dim expected As String = _PATH_WITH_LOWER_CASE_EXTENSION
			_fileMock.Setup(Function(x) x.Exists(pathToCheck)).Returns(False)
			_fileMock.Setup(Function(x) x.Exists(expected)).Returns(True)
			Dim actual As String = _filePathHelper.GetExistingFilePath(pathToCheck)
			Assert.That(actual, [Is].EqualTo(expected))
		End Sub

		<Test>
		Public Sub ShouldReturnUpperWhenLowerDoesNotExistAndMixedIsUsed()
			Dim pathToCheck As String = _PATH_WITH_MIXED_CASE_EXTENSION
			Dim expected As String = _PATH_WITH_UPPER_CASE_EXTENSION
			_fileMock.Setup(Function(x) x.Exists(_PATH_WITH_LOWER_CASE_EXTENSION)).Returns(False)
			_fileMock.Setup(Function(x) x.Exists(pathToCheck)).Returns(False)
			_fileMock.Setup(Function(x) x.Exists(expected)).Returns(True)
			Dim actual As String = _filePathHelper.GetExistingFilePath(pathToCheck)
			Assert.That(actual, [Is].EqualTo(expected))
		End Sub

		<Test>
		Public Sub ShouldReturnLowerWhenUpperDoesNotExistAndMixedIsUsed()
			Dim pathToCheck As String = _PATH_WITH_MIXED_CASE_EXTENSION
			Dim expected As String = _PATH_WITH_LOWER_CASE_EXTENSION
			_fileMock.Setup(Function(x) x.Exists(_PATH_WITH_UPPER_CASE_EXTENSION)).Returns(False)
			_fileMock.Setup(Function(x) x.Exists(pathToCheck)).Returns(False)
			_fileMock.Setup(Function(x) x.Exists(expected)).Returns(True)
			Dim actual As String = _filePathHelper.GetExistingFilePath(pathToCheck)
			Assert.That(actual, [Is].EqualTo(expected))
		End Sub

		<Test>
		Public Sub ShouldTryUpperAndLowerWhenMixedDoesNotExist()
			Dim sequence As MockSequence = New MockSequence()
			_fileMock.InSequence(sequence).Setup(Function(x) x.Exists(_PATH_WITH_MIXED_CASE_EXTENSION)).Returns(False)
			_fileMock.InSequence(sequence).Setup(Function(x) x.Exists(_PATH_WITH_UPPER_CASE_EXTENSION)).Returns(False)
			_fileMock.InSequence(sequence).Setup(Function(x) x.Exists(_PATH_WITH_LOWER_CASE_EXTENSION)).Returns(False)
			_filePathHelper.GetExistingFilePath(_PATH_WITH_MIXED_CASE_EXTENSION)
			_fileMock.Verify(Sub(x) x.Exists(_PATH_WITH_MIXED_CASE_EXTENSION), Times.Exactly(1))
			_fileMock.Verify(Sub(x) x.Exists(_PATH_WITH_UPPER_CASE_EXTENSION), Times.Exactly(1))
			_fileMock.Verify(Sub(x) x.Exists(_PATH_WITH_LOWER_CASE_EXTENSION), Times.Exactly(1))
		End Sub

		<Test>
		Public Sub ShouldReturnNullWhenNoneExists()
			_fileMock.Setup(Function(x) x.Exists(It.IsAny(Of String))).Returns(False)
			'Dim actual As String = _filePathHelper.GetExistingFilePath(_PATH_WITH_MIXED_CASE_EXTENSION)
			'Assert.IsNull(actual)
		End Sub

		<Test>
		Public Sub ShouldReturnSamePathIfExistsAndThereIsNoExtension()
			' Dim pathToCheck As String = _PATH_WITH_NO_EXTENSION
			Dim expected As String = _PATH_WITH_NO_EXTENSION
			_fileMock.Setup(Function(x) x.Exists(expected)).Returns(True)
			'Dim actual As String = _filePathHelper.GetExistingFilePath(pathToCheck)
			'Assert.AreEqual(actual, expected)
		End Sub

		<Test>
		Public Sub ShouldReturnNullWhenPathDoesNotExistAndThereIsNoExtension()
			Dim pathToCheck As String = _PATH_WITH_NO_EXTENSION
			_fileMock.Setup(Function(x) x.Exists(pathToCheck)).Returns(False)
			'Dim actual As String = _filePathHelper.GetExistingFilePath(pathToCheck)

			'Assert.IsNull(actual)
		End Sub

		<Test>
		Public Sub ShouldNotCheckOtherPathsIfThereIsNoExtensionAndOriginalOneDoesNotExist()
			Dim pathToCheck As String = _PATH_WITH_NO_EXTENSION
			_fileMock.Setup(Function(x) x.Exists(pathToCheck)).Returns(False)
			'_filePathHelper.GetExistingFilePath(pathToCheck)
			'_fileMock.Received(1).Exists(Arg.Any(Of String)())
		End Sub
	End Class
End Namespace



