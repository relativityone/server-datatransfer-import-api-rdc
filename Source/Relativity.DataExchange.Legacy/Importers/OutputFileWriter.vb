Imports System.Collections.Generic
Imports System.IO
Imports Relativity.DataExchange.Io
Imports Relativity.Logging

Namespace kCura.WinEDDS
	Public Class OutputFileWriter
		Implements IDisposable
		Private Const _MAXIMUM_NUMBER_OF_ITEMS_IN_QUEUE As Integer = 12

		Private ReadOnly _createdFilesPaths As Queue(Of String) = New Queue(Of String)
		Private ReadOnly _syncRoot As Object = New Object

		Private ReadOnly _logger As ILog
		Private ReadOnly _fileSystem As IFileSystem

		Private _filesCreated As Boolean
		Private _filesOpened As Boolean
		Private _nativeFileWriterRollbackPos As Long
		Private _dataGridFileWriterRollbackPos As Long
		Private _fullTextOnServerLength As Long
		Private _disposed As Boolean

		''' <summary>
		''' Initializes a new instance of the <see cref="OutputFileWriter"/> class.
		''' </summary>
		''' <param name="fileSystem">
		''' The file system instance.
		''' </param>
		Public Sub New(logger As ILog, fileSystem As Global.Relativity.DataExchange.Io.IFileSystem)
			If logger Is Nothing Then
				Throw New ArgumentNullException(NameOf(logger))
			End If

			If fileSystem Is Nothing Then
				Throw New ArgumentNullException(NameOf(fileSystem))
			End If

			Me._disposed = False
			Me._filesCreated = False
			Me._filesOpened = False

			Me._fileSystem = fileSystem
			Me._logger = logger.ForContext(Of OutputFileWriter)
		End Sub

		''' <summary>
		''' Gets the full path for the output native file.
		''' </summary>
		''' <value>
		''' The full path.
		''' </value>
		Public ReadOnly Property OutputNativeFilePath As String

		''' <summary>
		''' Gets the full path for the output data grid file.
		''' </summary>
		''' <value>
		''' The full path.
		''' </value>
		Public ReadOnly Property OutputDataGridFilePath As String

		''' <summary>
		''' Gets the full path for the output code file.
		''' </summary>
		''' <value>
		''' The full path.
		''' </value>
		Public ReadOnly Property OutputCodeFilePath As String

		''' <summary>
		''' Gets the full path for the output object file.
		''' </summary>
		''' <value>
		''' The full path.
		''' </value>
		Public ReadOnly Property OutputObjectFilePath As String

		''' <summary>
		''' Gets the stream writer that writes to the native load file.
		''' </summary>
		''' <value>
		''' The <see cref="System.IO.StreamWriter"/> instance.
		''' </value>
		Public ReadOnly Property OutputNativeFileWriter As Global.Relativity.DataExchange.Io.IStreamWriter

		''' <summary>
		''' Gets the stream writer that writes to the data grid load file.
		''' </summary>
		''' <value>
		''' The <see cref="System.IO.StreamWriter"/> instance.
		''' </value>
		Public ReadOnly Property OutputDataGridFileWriter As Global.Relativity.DataExchange.Io.IStreamWriter

		''' <summary>
		''' Gets the stream writer that writes to the code load file.
		''' </summary>
		''' <value>
		''' The <see cref="System.IO.StreamWriter"/> instance.
		''' </value>
		Public ReadOnly Property OutputCodeFileWriter As Global.Relativity.DataExchange.Io.IStreamWriter

		''' <summary>
		''' Gets the stream writer that writes to the object load file.
		''' </summary>
		''' <value>
		''' The <see cref="System.IO.StreamWriter"/> instance.
		''' </value>
		Public ReadOnly Property OutputObjectFileWriter As Global.Relativity.DataExchange.Io.IStreamWriter

		''' <summary>
		''' Deletes all load files and resets theirs names, so subsequent calls to <see cref="Open"/> will create files with different names.
		''' <see cref="Close"/> should be executed prior to calling this method.
		''' </summary>
		Public Sub DeleteFiles()
			SyncLock _syncRoot
				Me.CheckDispose()

				If Me._filesOpened Then
					_logger.LogWarning("An attempt was made to delete temp files while they were still open. Trying to close.")
					Me.CloseWithoutLocking()
				End If
				Me.DeleteFilesWithoutLocking()
			End SyncLock
		End Sub

		''' <summary>
		''' Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		''' </summary>
		Public Sub Dispose() Implements IDisposable.Dispose
			SyncLock _syncRoot
				Me.Dispose(True)
				GC.SuppressFinalize(Me)
			End SyncLock
		End Sub

		Public Sub Open(Optional ByVal append As Boolean = False)
			SyncLock _syncRoot
				Me.CheckDispose()
				Me.OpenWithoutLocking(append)
			End SyncLock
		End Sub

		Public Sub Close()
			SyncLock _syncRoot
				Me.CheckDispose()
				Me.CloseWithoutLocking()
			End SyncLock
		End Sub

		Public Sub MarkRollbackPosition()
			SyncLock _syncRoot
				Me.CheckDispose()

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

					Return Me.OutputNativeFileWriter.BaseStream.Length + Me.OutputDataGridFileWriter.BaseStream.Length + Me._fullTextOnServerLength
				End SyncLock
			End Get
		End Property

		Public Sub RollbackDocumentLineWrites()
			SyncLock _syncRoot
				Me.CheckDispose()
				Me.CloseWithoutLocking()
				Me.SetStreamLengths()
				Me.OpenWithoutLocking(True)
			End SyncLock
		End Sub

		''' <summary>
		''' Method tries to close and delete all temporary files created by this object.
		''' </summary>
		''' <returns>Number of temporary files which were not deleted</returns>
		Public Function TryCloseAndDeleteAllTempFiles() As Integer
			SyncLock Me._syncRoot
				Me.CheckDispose()
				Return Me.TryCloseAndDeleteAllTempFilesWithoutLocking()
			End SyncLock
		End Function

		Private Sub OpenWithoutLocking(append As Boolean)
			If Me._filesOpened Then
				Return
			End If

			If Not Me._filesCreated Then
				Me.CreateTempFiles()
			End If

			Me._outputNativeFileWriter = _fileSystem.CreateStreamWriter(Me.OutputNativeFilePath, append, System.Text.Encoding.Unicode)
			Me._outputDataGridFileWriter = _fileSystem.CreateStreamWriter(Me.OutputDataGridFilePath, append, System.Text.Encoding.Unicode)
			Me._outputCodeFileWriter = _fileSystem.CreateStreamWriter(Me.OutputCodeFilePath, append, System.Text.Encoding.Unicode)
			Me._outputObjectFileWriter = _fileSystem.CreateStreamWriter(Me.OutputObjectFilePath, append, System.Text.Encoding.Unicode)
			
			Me._fullTextOnServerLength = 0
			Me._filesOpened = True
		End Sub

		Private Sub CloseWithoutLocking()
			If Not Me._filesOpened Then
				Return
			End If

			Me.TryCloseWriter(Me.OutputNativeFileWriter)
			Me._outputNativeFileWriter = Nothing
			Me.TryCloseWriter(Me.OutputDataGridFileWriter)
			Me._outputDataGridFileWriter = Nothing
			Me.TryCloseWriter(Me.OutputCodeFileWriter)
			Me._outputCodeFileWriter = Nothing
			Me.TryCloseWriter(Me.OutputObjectFileWriter)
			Me._outputObjectFileWriter = Nothing

			Me._filesOpened = False
		End Sub

		Private Sub DeleteFilesWithoutLocking()
			Me._outputNativeFilePath = Nothing
			Me._outputDataGridFilePath = Nothing
			Me._outputCodeFilePath = Nothing
			Me._outputObjectFilePath = Nothing
			_filesCreated = False

			Dim numberOfFilesToDelete As Integer = Me._createdFilesPaths.Count
			For index As Integer = 1 To numberOfFilesToDelete
				Dim fileToDelete As String = Me._createdFilesPaths.Dequeue()
				Try
					_fileSystem.File.Delete(fileToDelete)
				Catch ex As IOException When IsExceptionDueToLockedFile(ex)
					Me._logger.LogWarning(ex, "Unable to delete file because it was locked. Adding to queue for retry.")
					_createdFilesPaths.Enqueue(fileToDelete)
				End Try
			Next

			EnsureCreatedFilesQueueSizeLimit()
		End Sub

		Private Sub EnsureCreatedFilesQueueSizeLimit()
			Dim numberOfItemsToRemove As Integer = Math.Max(0, _createdFilesPaths.Count - _MAXIMUM_NUMBER_OF_ITEMS_IN_QUEUE)
			For i As Integer = 1 To numberOfItemsToRemove
				_createdFilesPaths.Dequeue()
			Next
		End Sub

		Private Function TryCloseAndDeleteAllTempFilesWithoutLocking() As Integer
			Me.CloseWithoutLocking()
			Me.DeleteFilesWithoutLocking()

			Return Me._createdFilesPaths.Count
		End Function

		Private Sub SetStreamLengths()
			Using fs As New System.IO.FileStream(Me.OutputNativeFilePath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite)
				fs.SetLength(_nativeFileWriterRollbackPos)
				fs.Close()
			End Using

			Using fs As New System.IO.FileStream(Me.OutputDataGridFilePath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite)
				fs.SetLength(_dataGridFileWriterRollbackPos)
				fs.Close()
			End Using
		End Sub

		
		Public Sub ReportFullTextSizeOnServer(length As Long)
			SyncLock Me._syncRoot
				Me.CheckDispose()
				Me._fullTextOnServerLength += length
			End SyncLock
		End Sub

		Private Sub CreateTempFiles()
			Me._outputNativeFilePath = CreateTempFile(TempFileConstants.NativeLoadFileNameSuffix)
			Me._outputDataGridFilePath = CreateTempFile(TempFileConstants.DataGridLoadFileNameSuffix)
			Me._outputCodeFilePath = CreateTempFile(TempFileConstants.CodeLoadFileNameSuffix)
			Me._outputObjectFilePath = CreateTempFile(TempFileConstants.ObjectLoadFileNameSuffix)

			Me._filesCreated = True
		End Sub

		Private Function CreateTempFile(nameSuffix As String) As String
			Dim createdFilePath As String = Me._fileSystem.Path.GetTempFileName(nameSuffix)
			_createdFilesPaths.Enqueue(createdFilePath)
			Return createdFilePath
		End Function

		Private Sub TryCloseWriter(writer As IStreamWriter)
			If Not writer Is Nothing Then
				Try
					writer.Close()
				Catch ex As Exception
					Me._logger.LogWarning(ex, "Exception was thrown while trying to close stream writer.")
				End Try
			End If
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

			If disposing Then
				Try
					Me.TryCloseAndDeleteAllTempFilesWithoutLocking()
				Finally
					Me._disposed = True
				End Try
			End If
		End Sub

		''' <summary>
		''' Error codes are documented at <see href="https://docs.microsoft.com/en-us/windows/win32/debug/system-error-codes--0-499-"/>
		''' </summary>
		''' <param name="exception"></param>
		''' <returns></returns>
		Private Shared Function IsExceptionDueToLockedFile(exception As IOException) As Boolean
			Const sharingViolationErrorCode As Integer = &H20
			Const lockViolationErrorCode As Integer = &H21

			Dim errorCode As Integer = exception.HResult And &HFFFF
			Return errorCode = sharingViolationErrorCode OrElse errorCode = lockViolationErrorCode
		End Function
	End Class
End Namespace