﻿' -----------------------------------------------------------------------------------------------------
' <copyright file="CaseInsensitiveFilePathHelperTests.cs" company="Relativity ODA LLC">
'   © Relativity All Rights Reserved.
' </copyright>
' -----------------------------------------------------------------------------------------------------

Imports kCura.WinEDDS.Helpers

Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Imports NUnit.Framework
Imports Relativity.Import.Export.Io

Namespace Relativity.Import.Client.NUnit

	<TestFixture>
	Public Class CaseInsensitiveFilePathHelperTests
		Private _filePathHelper As CaseInsensitiveFilePathHelper
		Private _fileMock As IFile

		Private Const _SAMPLE_PATH As String = "\\dir\somePath.ext"

		<SetUp> Public Sub SetUp()
			_fileMock = Substitute.For(Of IFile)()
			Dim fileSystemMock As IFileSystem = Substitute.For(Of IFileSystem)()
			fileSystemMock.File.Returns(_fileMock)
			_filePathHelper = New CaseInsensitiveFilePathHelper(fileSystemMock)
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
			_fileMock.Exists(_SAMPLE_PATH).Returns(True)

			Dim actual As String = _filePathHelper.GetExistingFilePath(_SAMPLE_PATH)

			Assert.AreEqual(actual, _SAMPLE_PATH)
		End Sub


		<Test>
		Public Sub ShouldReturnNothingWhenOriginalDoesNotExist()
			_fileMock.Exists(_SAMPLE_PATH).Returns(False)

			Dim actual As String = _filePathHelper.GetExistingFilePath(_SAMPLE_PATH)

			Assert.IsNull(actual)
		End Sub
	End Class
End NameSpace