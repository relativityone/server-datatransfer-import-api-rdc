Imports NUnit.Framework

Namespace kCura.WinEDDS.NUnit
	<TestFixture()>
	Public Class OutputFileWriterTests

		<Test()>
		Public Sub OutputFileWriter_Open_OpensFile()
			Dim writer As kCura.WinEDDS.OutputFileWriter = New OutputFileWriter()
			writer.Open()
			Assert.IsNotNull(writer.OutputNativeFileWriter.BaseStream)
			Assert.IsNotNull(writer.OutputDataGridFileWriter.BaseStream)
			writer.Close()
			System.IO.File.Delete(writer.OutputNativeFilePath)
			System.IO.File.Delete(writer.OutputDataGridFilePath)
		End Sub

		<Test()>
		Public Sub OutputFileWriter_Close_ClosesFile()
			Dim writer As kCura.WinEDDS.OutputFileWriter = New OutputFileWriter()
			writer.Open()
			writer.Close()
			Assert.IsNull(writer.OutputNativeFileWriter.BaseStream)
			Assert.IsNull(writer.OutputDataGridFileWriter.BaseStream)
			System.IO.File.Delete(writer.OutputNativeFilePath)
			System.IO.File.Delete(writer.OutputDataGridFilePath)
		End Sub

		<Test()>
		Public Sub OutputFileWriter_RollbackDocumentLinWrites_RollsbackToCorrectPosition()
			Dim writer As kCura.WinEDDS.OutputFileWriter = New OutputFileWriter()
			writer.Open()
			writer.OutputNativeFileWriter.Write("Test 1")
			writer.OutputDataGridFileWriter.Write("Test 1")
			writer.MarkRollbackPosition()
			writer.OutputNativeFileWriter.Write("Test 2")
			writer.OutputDataGridFileWriter.Write("Test 2")
			writer.RollbackDocumentLineWrites()
			Assert.AreEqual(14, writer.OutputNativeFileWriter.BaseStream.Length)
			Assert.AreEqual(14, writer.OutputDataGridFileWriter.BaseStream.Length)
			writer.Close()
			System.IO.File.Delete(writer.OutputNativeFilePath)
			System.IO.File.Delete(writer.OutputDataGridFilePath)
		End Sub

	End Class
End Namespace