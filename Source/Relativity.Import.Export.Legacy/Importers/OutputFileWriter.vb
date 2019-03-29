Namespace kCura.WinEDDS
	Public Class OutputFileWriter
		Implements IDisposable

		Private ReadOnly _syncRoot As Object = New Object
		Private ReadOnly _fileSystem As Global.Relativity.Import.Export.Io.IFileSystem
		Private _nativeFileWriterRollbackPos As Long
		Private _dataGridFileWriterRollbackPos As Long
		Private _disposed As Boolean

		''' <summary>
		''' Initializes a new instance of the <see cref="OutputFileWriter"/> class.
		''' </summary>
		Public Sub New()
			Me.New(Global.Relativity.Import.Export.Io.FileSystem.Instance)
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="OutputFileWriter"/> class.
		''' </summary>
		''' <param name="fileSystem">
		''' The file system instance.
		''' </param>
		Public Sub New(fileSystem As Global.Relativity.Import.Export.Io.IFileSystem)
			If (fileSystem Is Nothing) Then
				Throw New ArgumentNullException(NameOf(fileSystem))
			End If

			Me._disposed = False
			Me._fileSystem = fileSystem
			Me.OutputNativeFilePath = fileSystem.Path.GetTempFileName(TempFileConstants.NativeLoadFileNameSuffix)
			Me.OutputDataGridFilePath = fileSystem.Path.GetTempFileName(TempFileConstants.DataGridLoadFileNameSuffix)
			Me.OutputCodeFilePath = fileSystem.Path.GetTempFileName(TempFileConstants.CodeLoadFileNameSuffix)
			Me.OutputObjectFilePath = fileSystem.Path.GetTempFileName(TempFileConstants.ObjectLoadFileNameSuffix)
		End Sub

		''' <summary>
		''' Gets or sets the full path for the output native file.
		''' </summary>
		''' <value>
		''' The full path.
		''' </value>
		Public Property OutputNativeFilePath As String

		''' <summary>
		''' Gets or sets the full path for the output data grid file.
		''' </summary>
		''' <value>
		''' The full path.
		''' </value>
		Public Property OutputDataGridFilePath As String

		''' <summary>
		''' Gets or sets the full path for the output code file.
		''' </summary>
		''' <value>
		''' The full path.
		''' </value>
		Public Property OutputCodeFilePath As String

		''' <summary>
		''' Gets or sets the full path for the output object file.
		''' </summary>
		''' <value>
		''' The full path.
		''' </value>
		Public Property OutputObjectFilePath As String

		''' <summary>
		''' Gets or sets the stream writer that writes to the native load file.
		''' </summary>
		''' <value>
		''' The <see cref="System.IO.StreamWriter"/> instance.
		''' </value>
		Public Property OutputNativeFileWriter As Global.Relativity.Import.Export.Io.IStreamWriter

		''' <summary>
		''' Gets or sets the stream writer that writes to the data grid load file.
		''' </summary>
		''' <value>
		''' The <see cref="System.IO.StreamWriter"/> instance.
		''' </value>
		Public Property OutputDataGridFileWriter As Global.Relativity.Import.Export.Io.IStreamWriter

		''' <summary>
		''' Gets or sets the stream writer that writes to the code load file.
		''' </summary>
		''' <value>
		''' The <see cref="System.IO.StreamWriter"/> instance.
		''' </value>
		Public Property OutputCodeFileWriter As Global.Relativity.Import.Export.Io.IStreamWriter

		''' <summary>
		''' Gets or sets the stream writer that writes to the object load file.
		''' </summary>
		''' <value>
		''' The <see cref="System.IO.StreamWriter"/> instance.
		''' </value>
		Public Property OutputObjectFileWriter As Global.Relativity.Import.Export.Io.IStreamWriter

		''' <summary>
		''' Deletes all load files and retains the original file names.
		''' </summary>
		Public Sub DeleteFiles()
			Me.CheckDispose()
			If Not String.IsNullOrEmpty(Me.OutputNativeFilePath) Then
				_fileSystem.File.Delete(Me.OutputNativeFilePath)
			End If

			If Not String.IsNullOrEmpty(Me.OutputDataGridFilePath) Then
				_fileSystem.File.Delete(Me.OutputDataGridFilePath)
			End If

			If Not String.IsNullOrEmpty(Me.OutputCodeFilePath) Then
				_fileSystem.File.Delete(Me.OutputCodeFilePath)
			End If

			If Not String.IsNullOrEmpty(Me.OutputObjectFilePath) Then
				_fileSystem.File.Delete(Me.OutputObjectFilePath)
			End If
		End Sub

		''' <summary>
		''' Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		''' </summary>
		Public Sub Dispose() Implements IDisposable.Dispose
			Me.Dispose(True)
			GC.SuppressFinalize(Me)
		End Sub

		Public Sub Open(Optional ByVal append As Boolean = False)
			Me.CheckDispose()
			Me.OutputNativeFileWriter = _fileSystem.CreateStreamWriter(Me.OutputNativeFilePath, append, System.Text.Encoding.Unicode)
			Me.OutputDataGridFileWriter = _fileSystem.CreateStreamWriter(Me.OutputDataGridFilePath, append, System.Text.Encoding.Unicode)
			Me.OutputCodeFileWriter = _fileSystem.CreateStreamWriter(Me.OutputCodeFilePath, append, System.Text.Encoding.Unicode)
			Me.OutputObjectFileWriter = _fileSystem.CreateStreamWriter(Me.OutputObjectFilePath, append, System.Text.Encoding.Unicode)
		End Sub

		Public Sub Close()
			Me.CheckDispose()
			SyncLock _syncRoot
				If (Not Me.OutputNativeFileWriter Is Nothing) Then
					Me.OutputNativeFileWriter.Close()
				End If

				If (Not Me.OutputDataGridFileWriter Is Nothing) Then
					Me.OutputDataGridFileWriter.Close()
				End If

				If (Not Me.OutputCodeFileWriter Is Nothing) Then
					Me.OutputCodeFileWriter.Close()
				End If

				If (Not Me.OutputObjectFileWriter Is Nothing) Then
					Me.OutputObjectFileWriter.Close()
				End If
			End SyncLock
		End Sub

		Public Sub MarkRollbackPosition()
			Me.CheckDispose()
			SyncLock _syncRoot
				If Not Me.OutputNativeFileWriter Is Nothing AndAlso Not Me.OutputNativeFileWriter.BaseStream Is Nothing Then
					Me.OutputNativeFileWriter.Flush()
					_nativeFileWriterRollbackPos = Me.OutputNativeFileWriter.BaseStream.Length
				End If

				If Not Me.OutputDataGridFileWriter Is Nothing AndAlso Not Me.OutputDataGridFileWriter.BaseStream Is Nothing Then
					Me.OutputDataGridFileWriter.Flush()
					_dataGridFileWriterRollbackPos = Me.OutputDataGridFileWriter.BaseStream.Length
				End If
			End SyncLock
		End Sub

		Public ReadOnly Property CombinedStreamLength As Long
			Get
				' Never throw exceptions from a property.
				SyncLock _syncRoot
					If Me.OutputNativeFileWriter Is Nothing OrElse Me.OutputDataGridFileWriter Is Nothing Then
						Return 0
					End If

					If Me.OutputNativeFileWriter.BaseStream Is Nothing OrElse Me.OutputDataGridFileWriter.BaseStream Is Nothing Then
						Return 0
					End If

					Return Me.OutputNativeFileWriter.BaseStream.Length + Me.OutputDataGridFileWriter.BaseStream.Length
				End SyncLock
			End Get
		End Property

		Public Sub RollbackDocumentLineWrites()
			Me.CheckDispose()
			Me.Close()
			Me.SetStreamLengths()
			Me.Open(True)
		End Sub

		Private Sub SetStreamLengths()
			SyncLock _syncRoot
				Dim fs As New System.IO.FileStream(Me.OutputNativeFilePath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite)
				fs.SetLength(_nativeFileWriterRollbackPos)
				fs.Close()
				fs = New System.IO.FileStream(Me.OutputDataGridFilePath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite)
				fs.SetLength(_dataGridFileWriterRollbackPos)
				fs.Close()
			End SyncLock
		End Sub

		''' <summary>
		''' Checks to see whether this instance has been disposed.
		''' </summary>
		''' <exception cref="ObjectDisposedException">
		''' Thrown when this instance has been disposed.
		''' </exception>
		Private Sub CheckDispose()
			If (Not _disposed) Then
				Return
			End If

			Throw New ObjectDisposedException("This load file operation cannot be performed because the load file writer has been disposed.")
		End Sub

		''' <summary>
		''' Releases unmanaged and - optionally - managed resources.
		''' </summary>
		''' <param name="disposing">
		''' <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
		''' </param>
		Private Sub Dispose(disposing As Boolean)
			If (Me._disposed) Then
				Return
			End If

			If (disposing) Then				
				Try
					Me.Close()
				Finally
					Me.DeleteFiles()
					Me._disposed = True
				End Try
			End If
        End Sub
	End Class
End Namespace