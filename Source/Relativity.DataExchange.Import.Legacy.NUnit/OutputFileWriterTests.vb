' -----------------------------------------------------------------------------------------------------
' <copyright file="OutputFileWriterTests.cs" company="Relativity ODA LLC">
'   © Relativity All Rights Reserved.
' </copyright>
' -----------------------------------------------------------------------------------------------------

Imports System.IO
Imports kCura.WinEDDS
Imports Moq

Imports NUnit.Framework
Imports Relativity.Logging

Namespace Relativity.DataExchange.Import.NUnit

	<TestFixture>
	Public Class OutputFileWriterTests
		Private _loggerMock As Mock(Of ILog)
		Private _sut As OutputFileWriter

		<SetUp()>
		Public Sub Setup()
			_loggerMock = New Mock(Of ILog)() With {.DefaultValue = DefaultValue.Mock}
			_loggerMock.Setup(Function(logger) logger.ForContext(Of OutputFileWriter)()).Returns(_loggerMock.Object)
			_sut = New OutputFileWriter(_loggerMock.Object, Io.FileSystem.Instance)
		End Sub

		<TearDown()>
		Public Sub TearDown()
			_sut.Dispose()
		End Sub

		<Test()>
		Public Shared Sub ConstructorShouldThrowWhenTheFileSystemIsNull()
			Assert.Throws(Of System.ArgumentNullException)(
				Sub()
					Using writer As New OutputFileWriter(New NullLogger(), Nothing)
						Assert.That(writer, [Is].Not.Null)
					End Using
				End Sub)
		End Sub

		<Test()>
		Public Shared Sub ConstructorShouldThrowWhenTheLoggerIsNull()
			Assert.Throws(Of System.ArgumentNullException)(
				Sub()
					Using writer As New OutputFileWriter(Nothing, Io.FileSystem.Instance)
						Assert.That(writer, [Is].Not.Null)
					End Using
				End Sub)
		End Sub

		<Test()>
		Public Sub AllPublicMethodsShouldThrowWhenDisposed()
			' Arrange
			_sut.Dispose()

			' Act & Assert
			Assert.That(Sub() _sut.Close(), Throws.TypeOf(Of ObjectDisposedException))
			Assert.That(Sub() _sut.DeleteFiles(), Throws.TypeOf(Of ObjectDisposedException))
			Assert.That(Sub() _sut.TryCloseAndDeleteAllTempFiles(), Throws.TypeOf(Of ObjectDisposedException))
			Assert.That(Sub() _sut.MarkRollbackPosition(), Throws.TypeOf(Of ObjectDisposedException))
			Assert.That(Sub() _sut.Open(), Throws.TypeOf(Of ObjectDisposedException))
			Assert.That(Sub() _sut.RollbackDocumentLineWrites(), Throws.TypeOf(Of ObjectDisposedException))
		End Sub

		<Test()>
		Public Sub DisposeShouldDeleteTheFiles()
			' Arrange
			_sut.Open()
			Dim outputDataGridFilePath = _sut.OutputDataGridFilePath
			Dim outputNativeFilePath = _sut.OutputNativeFilePath
			Dim outputCodeFilePath = _sut.OutputCodeFilePath
			Dim outputObjectFilePath = _sut.OutputObjectFilePath

			' Act
			_sut.Dispose()

			' Assert
			Assert.That(_sut.OutputDataGridFilePath, [Is].Null)
			Assert.That(outputDataGridFilePath, Does.Not.Exist)

			Assert.That(_sut.OutputNativeFilePath, [Is].Null)
			Assert.That(outputNativeFilePath, Does.Not.Exist)

			Assert.That(_sut.OutputCodeFilePath, [Is].Null)
			Assert.That(outputCodeFilePath, Does.Not.Exist)

			Assert.That(_sut.OutputObjectFilePath, [Is].Null)
			Assert.That(outputObjectFilePath, Does.Not.Exist)
		End Sub

		<Test()>
		Public Sub DisposeShouldNotThrowWhenCalledTwice()
			' Arrange
			_sut.Dispose()

			' Act & Assert
			Assert.DoesNotThrow(Sub() _sut.Dispose(), "because it should be safe to call dispose more than once")
		End Sub
		
		<Test()>
		Public Sub CloseShouldCloseAllWriters()
			' Arrange
			_sut.Open()

			' Act
			_sut.Close()

			' Assert
			Assert.That(_sut.OutputNativeFileWriter, [Is].Null)
			Assert.That(_sut.OutputNativeFilePath, Does.Exist)

			Assert.That(_sut.OutputDataGridFileWriter, [Is].Null)
			Assert.That(_sut.OutputDataGridFilePath, Does.Exist)

			Assert.That(_sut.OutputObjectFileWriter, [Is].Null)
			Assert.That(_sut.OutputObjectFilePath, Does.Exist)

			Assert.That(_sut.OutputCodeFileWriter, [Is].Null)
			Assert.That(_sut.OutputCodeFilePath, Does.Exist)
		End Sub

		<Test()>
		Public Sub RollbackDocumentLineWritesShouldRollbackToTheCorrectPosition()
			' Arrange
			_sut.Open()
			_sut.OutputNativeFileWriter.Write("Test 1")
			_sut.OutputDataGridFileWriter.Write("Test 1")
			_sut.MarkRollbackPosition()
			_sut.OutputNativeFileWriter.Write("Test 2")
			_sut.OutputDataGridFileWriter.Write("Test 2")

			' Act
			_sut.RollbackDocumentLineWrites()

			' Assert
			Assert.AreEqual(14, _sut.OutputNativeFileWriter.BaseStream.Length)
			Assert.AreEqual(14, _sut.OutputDataGridFileWriter.BaseStream.Length)
		End Sub

		<Test()>
		Public Sub OpenShouldCreateTheTempFiles()
			' Act
			_sut.Open()

			' Arrange
			Assert.That(_sut.OutputDataGridFilePath, [Is].Not.Null.Or.Empty)
			Assert.That(_sut.OutputDataGridFilePath, Does.Exist)
			Assert.That(_sut.OutputNativeFilePath, [Is].Not.Null.Or.Empty)
			Assert.That(_sut.OutputNativeFilePath, Does.Exist)
			Assert.That(_sut.OutputCodeFilePath, [Is].Not.Null.Or.Empty)
			Assert.That(_sut.OutputCodeFilePath, Does.Exist)
			Assert.That(_sut.OutputObjectFilePath, [Is].Not.Null.Or.Empty)
			Assert.That(_sut.OutputObjectFilePath, Does.Exist)
		End Sub

		<Test()>
		Public Sub OpenShouldOpenStreamWriters()
			_sut.Open()
			Assert.IsNotNull(_sut.OutputNativeFileWriter.BaseStream)
			Assert.IsNotNull(_sut.OutputDataGridFileWriter.BaseStream)
			Assert.IsNotNull(_sut.OutputObjectFileWriter.BaseStream)
			Assert.IsNotNull(_sut.OutputCodeFileWriter.BaseStream)
			_sut.Close()
		End Sub

		<Test()>
		Public Sub OpenShouldNotReuseOldTempFilesNamesWhenAllPreviousFilesDeletedSuccessfully()
			' Arrange
			_sut.Open()
			Dim firstOutputDataGridFilePath = _sut.OutputDataGridFilePath
			Dim firstOutputNativeFilePath = _sut.OutputNativeFilePath
			Dim firstOutputCodeFilePath = _sut.OutputCodeFilePath
			Dim firstOutputObjectFilePath = _sut.OutputObjectFilePath

			_sut.TryCloseAndDeleteAllTempFiles()

			' Act
			_sut.Open()

			' Assert
			Assert.That(_sut.OutputDataGridFilePath, [Is].Not.EqualTo(firstOutputDataGridFilePath))
			Assert.That(_sut.OutputNativeFilePath, [Is].Not.EqualTo(firstOutputNativeFilePath))
			Assert.That(_sut.OutputCodeFilePath, [Is].Not.EqualTo(firstOutputCodeFilePath))
			Assert.That(_sut.OutputObjectFilePath, [Is].Not.EqualTo(firstOutputObjectFilePath))
		End Sub

		<Test()>
		Public Sub OpenShouldNotReuseOldTempFilesNamesWhenOldFilesWereLockedWhileDeleting()
			' Arrange
			_sut.Open()
			Dim firstOutputDataGridFilePath = _sut.OutputDataGridFilePath
			Dim firstOutputNativeFilePath = _sut.OutputNativeFilePath
			Dim firstOutputCodeFilePath = _sut.OutputCodeFilePath
			Dim firstOutputObjectFilePath = _sut.OutputObjectFilePath

			_sut.Close()
			Using New FileStream(firstOutputDataGridFilePath, FileMode.Open)
				_sut.DeleteFiles()

				' Act
				_sut.Open()

				' Assert
				Assert.That(_sut.OutputDataGridFilePath, [Is].Not.EqualTo(firstOutputDataGridFilePath))
				Assert.That(_sut.OutputNativeFilePath, [Is].Not.EqualTo(firstOutputNativeFilePath))
				Assert.That(_sut.OutputCodeFilePath, [Is].Not.EqualTo(firstOutputCodeFilePath))
				Assert.That(_sut.OutputObjectFilePath, [Is].Not.EqualTo(firstOutputObjectFilePath))
			End Using
		End Sub

		<Test()>
		Public Sub OpenShouldReuseClosedFilesWhenTheyWereNotDeleted()
			' Arrange
			_sut.Open()
			Dim firstOutputCodeFilePath = _sut.OutputCodeFilePath
			Dim firstOutputDataGridFilePath = _sut.OutputDataGridFilePath
			Dim firstOutputNativeFilePath = _sut.OutputNativeFilePath
			Dim firstOutputObjectFilePath = _sut.OutputObjectFilePath

			_sut.Close()

			' Act
			_sut.Open()

			' Assert
			Assert.That(_sut.OutputCodeFilePath, [Is].EqualTo(firstOutputCodeFilePath))
			Assert.That(_sut.OutputDataGridFilePath, [Is].EqualTo(firstOutputDataGridFilePath))
			Assert.That(_sut.OutputNativeFilePath, [Is].EqualTo(firstOutputNativeFilePath))
			Assert.That(_sut.OutputObjectFilePath, [Is].EqualTo(firstOutputObjectFilePath))
		End Sub

		<Test()>
		Public Sub DeleteFilesShouldLogWarningAndCloseFilesWhenTheyWereNotClosed()
			' Arrange
			Const expectedWarning = "An attempt was made to delete temp files while they were still open. Trying to close."
			_sut.Open()
			Dim firstOutputCodeFilePath = _sut.OutputCodeFilePath
			Dim firstOutputDataGridFilePath = _sut.OutputDataGridFilePath
			Dim firstOutputNativeFilePath = _sut.OutputNativeFilePath
			Dim firstOutputObjectFilePath = _sut.OutputObjectFilePath

			' Act
			_sut.DeleteFiles()

			' Assert
			_loggerMock.Verify(Sub(logger) logger.LogWarning(expectedWarning))
			Assert.That(firstOutputCodeFilePath, Does.Not.Exist)
			Assert.That(firstOutputDataGridFilePath, Does.Not.Exist)
			Assert.That(firstOutputNativeFilePath, Does.Not.Exist)
			Assert.That(firstOutputObjectFilePath, Does.Not.Exist)
		End Sub

		<Test()>
		Public Sub DeleteFilesShouldDeletePreviouslyLockedFilesWhenTheyGetUnlocked()
			' Arrange
			_sut.Open()
			_sut.Close()

			Dim firstLockedFilePath = _sut.OutputCodeFilePath
			Dim secondLockedFilePath As String
			Using New FileStream(firstLockedFilePath, FileMode.Open)
				_sut.DeleteFiles()
				_sut.Open()
				_sut.Close()

				secondLockedFilePath = _sut.OutputDataGridFilePath
				Using New FileStream(secondLockedFilePath, FileMode.Open)
					_sut.DeleteFiles()
				End Using
			End Using

			' Act
			_sut.DeleteFiles()

			' Assert
			Assert.That(firstLockedFilePath, Does.Not.Exist)
			Assert.That(secondLockedFilePath, Does.Not.Exist)
		End Sub

		<Test()>
		Public Sub DeleteFilesShouldLogWarningWhenFileWasLocked()
			' Arrange
			Const expectedWarning = "Unable to delete file because it was locked. Adding to queue for retry."
			_sut.Open()
			_sut.Close()

			Dim firstLockedFilePath = _sut.OutputCodeFilePath
			Using New FileStream(firstLockedFilePath, FileMode.Open)
				' Act
				_sut.DeleteFiles()
			End Using

			' Assert
			_loggerMock.Verify(Sub(logger) logger.LogWarning(It.IsAny(Of IOException), expectedWarning))
		End Sub

		<Test()>
		Public Sub TryCloseAndDeleteAllTempFilesShouldReturnNumberOfTempFilesNotDeleted()
			' Arrange
			_sut.Open()
			_sut.Close()

			Using New FileStream(_sut.OutputCodeFilePath, FileMode.Open)
				_sut.DeleteFiles()
				_sut.Open()
				_sut.Close()

				Using New FileStream(_sut.OutputDataGridFilePath, FileMode.Open)
					' Act
					Dim numberOfNotDeletedTempFiles = _sut.TryCloseAndDeleteAllTempFiles()

					' Assert
					Assert.That(numberOfNotDeletedTempFiles, [Is].EqualTo(2))
				End Using
			End Using
		End Sub
	End Class
End Namespace