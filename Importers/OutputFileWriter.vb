﻿Namespace kCura.WinEDDS
	Public Class OutputFileWriter

#Region "Members"
		Private _nativeFileWriterRollbackPos As Long
		Private _dataGridFileWriterRollbackPos As Long
#End Region

#Region "Accessors"
		Public Property OutputNativeFilePath As String
		Public Property OutputDataGridFilePath As String

		Public Property OutputNativeFileWriter As System.IO.StreamWriter
		Public Property OutputDataGridFileWriter As System.IO.StreamWriter
#End Region

		Public Sub New()
			OutputNativeFilePath = TempFileBuilder.GetTempFile(TempFileBuilder.NativeLoadFileNamePrefix)
			OutputDataGridFilePath = TempFileBuilder.GetTempFile(TempFileBuilder.DatagridLoadFileNamePrefix)
		End Sub

		Public Sub Open(Optional ByVal appendMode As Boolean = False)
			OutputNativeFileWriter = New System.IO.StreamWriter(OutputNativeFilePath, appendMode, System.Text.Encoding.Unicode)
			OutputDataGridFileWriter = New System.IO.StreamWriter(OutputDataGridFilePath, appendMode, System.Text.Encoding.Unicode)
		End Sub

		Public Sub Close()
			OutputNativeFileWriter.Close()
			OutputDataGridFileWriter.Close()
		End Sub

		Public Sub MarkRollbackPosition()
			OutputNativeFileWriter.Flush()
			OutputDataGridFileWriter.Flush()

			_nativeFileWriterRollbackPos = OutputNativeFileWriter.BaseStream.Length
			_dataGridFileWriterRollbackPos = OutputDataGridFileWriter.BaseStream.Length
		End Sub

		Public ReadOnly Property CombinedStreamLength As Long
			Get
				Return OutputNativeFileWriter.BaseStream.Length + OutputDataGridFileWriter.BaseStream.Length
			End Get
		End Property

		Public Sub RollbackDocumentLineWrites()
			Close()

			SetStreamLengths()

			Open(True)
		End Sub

		Private Sub SetStreamLengths()
			Dim fs As New System.IO.FileStream(OutputNativeFilePath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite)
			fs.SetLength(_nativeFileWriterRollbackPos)
			fs.Close()

			fs = New System.IO.FileStream(OutputDataGridFilePath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite)
			fs.SetLength(_dataGridFileWriterRollbackPos)
			fs.Close()
		End Sub
	End Class
End Namespace