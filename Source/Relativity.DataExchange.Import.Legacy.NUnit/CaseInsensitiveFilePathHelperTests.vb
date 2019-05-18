' -----------------------------------------------------------------------------------------------------
' <copyright file="CaseInsensitiveFilePathHelperTests.cs" company="Relativity ODA LLC">
'   © Relativity All Rights Reserved.
' </copyright>
' -----------------------------------------------------------------------------------------------------

Imports kCura.WinEDDS.Helpers

Imports Moq

Imports NUnit.Framework

Imports Relativity.DataExchange.Io

Namespace Relativity.DataExchange.Import.NUnit

	<TestFixture>
	Public Class CaseInsensitiveFilePathHelperTests
		Private _filePathHelper As CaseInsensitiveFilePathHelper
		Private _fileMock As Mock(Of IFile)

		Private Const _SAMPLE_PATH As String = "\\dir\somePath.ext"

		<SetUp> Public Sub SetUp()
			_fileMock = New Mock(Of IFile)
			Dim fileSystemMock As Mock(Of IFileSystem) = New Mock(Of IFileSystem)
			fileSystemMock.Setup(Function(x) x.File).Returns(_fileMock.Object)
			_filePathHelper = New CaseInsensitiveFilePathHelper(fileSystemMock.Object)
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
			_fileMock.Setup(Function(x) x.Exists(_SAMPLE_PATH)).Returns(True)
			Dim actual As String = _filePathHelper.GetExistingFilePath(_SAMPLE_PATH)
			Assert.AreEqual(actual, _SAMPLE_PATH)
		End Sub


		<Test>
		Public Sub ShouldReturnNothingWhenOriginalDoesNotExist()
			_fileMock.Setup(Function(x) x.Exists(_SAMPLE_PATH)).Returns(False)
			Dim actual As String = _filePathHelper.GetExistingFilePath(_SAMPLE_PATH)
			Assert.IsNull(actual)
		End Sub
	End Class
End NameSpace