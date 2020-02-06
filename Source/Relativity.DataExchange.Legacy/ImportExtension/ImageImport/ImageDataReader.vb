Imports kCura.Relativity.DataReaderClient
Imports kCura.WinEDDS.Api
Imports Relativity.DataExchange

Namespace kCura.WinEDDS.ImportExtension
	Public Class ImageDataReader
		Implements kCura.WinEDDS.Api.IImageReader
		Private _currentRecordNumber As Int32 = 0
		Private ReadOnly _reader As System.Data.IDataReader
		Private ReadOnly _imageSettings As ImageSettings
		Private _batesNumberColumnIndex As Integer
		Private _documentIdentifierColumnIndex As Integer
		Private _fileLocationColumnIndex As Integer
		Private _fileNameColumnIndex As Integer
		Private _currentDocumentIdentifier As String

		Public Sub New(ByVal reader As System.Data.IDataReader, ByVal imageSettings As ImageSettings)
			_reader = reader
			_imageSettings = imageSettings

			_reader.ThrowIfNull(nameof(_reader))
			If _reader.IsClosed = True Then Throw New ArgumentException("The reader is closed")

			AdvanceRecord()
			ReadColumnIndexes()
		End Sub

		Public Sub AdvanceRecord() Implements IImageReader.AdvanceRecord
			Try
				If Not _reader.Read() Then
					_reader.Close()
				End If
			Finally
				_currentRecordNumber += 1
			End Try
		End Sub

		Public Sub Cancel() Implements IImageReader.Cancel

		End Sub

		''' <summary>
		''' Reads all remaining rows and then closes <see cref="IDataReader"/>. This is because the only way to get the total number of records is to read all the rows.
		''' </summary>
		Public Sub Close() Implements IImageReader.Close
			While Not _reader.IsClosed
				AdvanceRecord
			End While
		End Sub

		Public Sub Initialize() Implements IImageReader.Initialize

		End Sub

		''' <summary>
		''' Returns total records count, but only when <see cref="_reader"/> already read all the records, otherwise returns null.
		''' This is because <see cref="_reader"/> is a forward-only record viewer and does not contain information about total number of rows.
		''' To calculate total number of records we need to iterate through all the rows and increment <see cref="_currentRecordNumber"/>
		''' </summary>
		''' <returns>Total records count if <see cref="_reader"/> is closed, null otherwise</returns>
		Public Function CountRecords() As Long? Implements IImageReader.CountRecords
			If _reader.IsClosed Then
				Return _currentRecordNumber
			End If
			Return Nothing
		End Function

		Public ReadOnly Property CurrentRecordNumber As Integer Implements IImageReader.CurrentRecordNumber
			Get
				Return _currentRecordNumber
			End Get
		End Property

		Public Function GetImageRecord() As ImageRecord Implements IImageReader.GetImageRecord
			Try
				Dim imageRecord As New ImageRecord
				imageRecord.BatesNumber = _reader.GetString(Me._batesNumberColumnIndex)
				imageRecord.FileLocation = _reader.GetString(Me._fileLocationColumnIndex)
				If (Not Me._fileNameColumnIndex = -1) Then
					imageRecord.FileName = _reader.GetString(Me._fileNameColumnIndex)
				End If
				Dim documentIdentifier As String = _reader.GetString(Me._documentIdentifierColumnIndex)
				imageRecord.IsNewDoc = Not documentIdentifier = _currentDocumentIdentifier

				_currentDocumentIdentifier = documentIdentifier

				Return imageRecord
			Finally
				AdvanceRecord()
			End Try
		End Function


		Public ReadOnly Property HasMoreRecords As Boolean Implements IImageReader.HasMoreRecords
			Get
				Return Not (_reader.IsClosed)
			End Get
		End Property

		Private Sub ReadColumnIndexes()
			_batesNumberColumnIndex = -1
			_documentIdentifierColumnIndex = -1
			_fileLocationColumnIndex = -1
			_fileNameColumnIndex = -1

			For i As Integer = 0 To _reader.FieldCount - 1
				Select Case _reader.GetName(i)
					Case _imageSettings.BatesNumberField
						_batesNumberColumnIndex = i
					Case _imageSettings.DocumentIdentifierField
						_documentIdentifierColumnIndex = i
					Case _imageSettings.FileLocationField
						_fileLocationColumnIndex = i
					Case _imageSettings.FileNameField
						_fileNameColumnIndex = i
				End Select
			Next

			If (_batesNumberColumnIndex < 0) 
				Throw New ArgumentException($"Reader does not contains field named {_imageSettings.BatesNumberField}")
			End If
			If (_documentIdentifierColumnIndex < 0) 
				Throw New ArgumentException($"Reader does not contains field named {_imageSettings.DocumentIdentifierField}")
			End If
			If (_fileLocationColumnIndex < 0) 
				Throw New ArgumentException($"Reader does not contains field named {_imageSettings.FileLocationField}")
			End If
			'File name column is not required in data source

		End Sub
	End Class
End Namespace