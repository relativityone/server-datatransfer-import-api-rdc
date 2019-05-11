' -----------------------------------------------------------------------------------------------------
' <copyright file="OutputFileWriterTests.cs" company="Relativity ODA LLC">
'   © Relativity All Rights Reserved.
' </copyright>
' -----------------------------------------------------------------------------------------------------

Imports kCura.WinEDDS

Imports NUnit.Framework

Namespace Relativity.DataExchange.Import.NUnit

	<TestFixture>
	Public Class OutputFileWriterTests

		Private _writer As OutputFileWriter

		<SetUp()>
		Public Sub Setup()
			_writer = New OutputFileWriter()
		End Sub

		<TearDown()>
		Public Sub TearDown()
			If (System.IO.File.Exists(_writer.OutputNativeFilePath)) Then
				System.IO.File.Delete(_writer.OutputNativeFilePath)
			End If

			If (System.IO.File.Exists(_writer.OutputDataGridFilePath)) Then
				System.IO.File.Delete(_writer.OutputDataGridFilePath)
			End If

			If (System.IO.File.Exists(_writer.OutputCodeFilePath)) Then
				System.IO.File.Delete(_writer.OutputCodeFilePath)
			End If

			If (System.IO.File.Exists(_writer.OutputObjectFilePath)) Then
				System.IO.File.Delete(_writer.OutputObjectFilePath)
			End If
		End Sub

		<Test()>
		Public Sub ItShouldThrowWhenTheFileSystemIsNull()
			Assert.Throws(Of System.ArgumentNullException)(
				Sub()
					Dim writer As OutputFileWriter = New OutputFileWriter(Nothing)
					Assert.That(writer, [Is].Not.Null)
				End Sub)
		End Sub

		<Test()>
		Public Sub ItShouldThrowWhenDisposed()
			_writer.Dispose()
			Dim exception As System.ObjectDisposedException
			exception = Assert.Throws(Of System.ObjectDisposedException)(Sub() _writer.Close())
			Console.WriteLine(exception.Message)
			exception = Assert.Throws(Of System.ObjectDisposedException)(Sub() _writer.DeleteFiles())
			Console.WriteLine(exception.Message)
			exception = Assert.Throws(Of System.ObjectDisposedException)(Sub() _writer.MarkRollbackPosition())
			Console.WriteLine(exception.Message)
			exception = Assert.Throws(Of System.ObjectDisposedException)(Sub() _writer.Open())
			Console.WriteLine(exception.Message)
			exception = Assert.Throws(Of System.ObjectDisposedException)(Sub() _writer.RollbackDocumentLineWrites())
			Console.WriteLine(exception.Message)
		End Sub

		<Test()>
		Public Sub ItShouldDeleteTheFilesWhenDisposed()
			_writer.Dispose()
			Assert.That(_writer.OutputDataGridFilePath, [Is].Not.Null.Or.Empty)
			Assert.That(_writer.OutputDataGridFilePath, Does.Not.Exist)
			Assert.That(_writer.OutputNativeFilePath, [Is].Not.Null.Or.Empty)
			Assert.That(_writer.OutputNativeFilePath, Does.Not.Exist)
			Assert.That(_writer.OutputCodeFilePath, [Is].Not.Null.Or.Empty)
			Assert.That(_writer.OutputCodeFilePath, Does.Not.Exist)
			Assert.That(_writer.OutputObjectFilePath, [Is].Not.Null.Or.Empty)
			Assert.That(_writer.OutputObjectFilePath, Does.Not.Exist)
		End Sub

		<Test()>
		Public Sub ItShouldCreateTheTempFilesWhenConstructed()
			Assert.That(_writer.OutputDataGridFilePath, [Is].Not.Null.Or.Empty)
			Assert.That(_writer.OutputDataGridFilePath, Does.Exist)
			Assert.That(_writer.OutputNativeFilePath, [Is].Not.Null.Or.Empty)
			Assert.That(_writer.OutputNativeFilePath, Does.Exist)
			Assert.That(_writer.OutputCodeFilePath, [Is].Not.Null.Or.Empty)
			Assert.That(_writer.OutputCodeFilePath, Does.Exist)
			Assert.That(_writer.OutputObjectFilePath, [Is].Not.Null.Or.Empty)
			Assert.That(_writer.OutputObjectFilePath, Does.Exist)
		End Sub

		<Test()>
		Public Sub ItShouldOpenAllFiles()
			_writer.Open()
			Assert.IsNotNull(_writer.OutputNativeFileWriter.BaseStream)
			Assert.IsNotNull(_writer.OutputDataGridFileWriter.BaseStream)
			Assert.IsNotNull(_writer.OutputObjectFileWriter.BaseStream)
			Assert.IsNotNull(_writer.OutputCodeFileWriter.BaseStream)
			_writer.Close()
		End Sub

		<Test()>
		Public Sub ItShouldCloseAllFiles()
			_writer.Open()
			_writer.Close()
			Assert.IsNull(_writer.OutputNativeFileWriter.BaseStream)
			Assert.IsNull(_writer.OutputDataGridFileWriter.BaseStream)
			Assert.IsNull(_writer.OutputObjectFileWriter.BaseStream)
			Assert.IsNull(_writer.OutputCodeFileWriter.BaseStream)
		End Sub

		<Test()>
		Public Sub ItShouldRollbackToTheCorrectPosition()
			_writer.Open()
			_writer.OutputNativeFileWriter.Write("Test 1")
			_writer.OutputDataGridFileWriter.Write("Test 1")
			_writer.MarkRollbackPosition()
			_writer.OutputNativeFileWriter.Write("Test 2")
			_writer.OutputDataGridFileWriter.Write("Test 2")
			_writer.RollbackDocumentLineWrites()
			Assert.AreEqual(14, _writer.OutputNativeFileWriter.BaseStream.Length)
			Assert.AreEqual(14, _writer.OutputDataGridFileWriter.BaseStream.Length)
			_writer.Close()
		End Sub
	End Class
End Namespace