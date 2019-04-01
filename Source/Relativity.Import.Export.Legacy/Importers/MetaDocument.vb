Imports kCura.WinEDDS.Api
Imports Relativity.Import.Export

Namespace kCura.WinEDDS
	Public Class MetaDocument
		Private _fileGuid As String
		Private _identityValue As String
		Private _indexFileInDB As Boolean
		Private _filename As String
		Private _fullFilePath As String
		Private _uploadFile As Boolean
		Private _lineNumber As Int32
		Private _parentFolderID As Int32

		Private _record As Api.ArtifactFieldCollection
		Private _FileIdInfo As FileIdInfo
		Private _lineStatus As Int64
		Private _destinationVolume As String

		Public Property DestinationVolume() As String
			Get
				Return _destinationVolume
			End Get
			Set(ByVal value As String)
				_destinationVolume = value
			End Set
		End Property

		Public Property FileGuid() As String
			Get
				Return _fileGuid
			End Get
			Set(ByVal value As String)
				_fileGuid = value
			End Set
		End Property

		Public Property IdentityValue() As String
			Get
				Return _identityValue
			End Get
			Set(ByVal value As String)
				_identityValue = value
			End Set
		End Property

		Public Property IndexFileInDB() As Boolean
			Get
				Return _indexFileInDB
			End Get
			Set(ByVal value As Boolean)
				_indexFileInDB = value
			End Set
		End Property

		Public Property Filename() As String
			Get
				Return _filename
			End Get
			Set(ByVal value As String)
				_filename = value
			End Set
		End Property

		Public Property UploadFile() As Boolean
			Get
				Return _uploadFile
			End Get
			Set(ByVal value As Boolean)
				_uploadFile = value
			End Set
		End Property

		Public Property LineNumber() As Int32
			Get
				Return _lineNumber
			End Get
			Set(ByVal value As Int32)
				_lineNumber = value
			End Set
		End Property

		Public Property ParentFolderID() As Int32
			Get
				Return _parentFolderID
			End Get
			Set(ByVal value As Int32)
				_parentFolderID = value
			End Set
		End Property

		Public Property Record() As Api.ArtifactFieldCollection
			Get
				Return _record
			End Get
			Set(ByVal value As Api.ArtifactFieldCollection)
				_record = value
			End Set
		End Property



		Public Property FullFilePath() As String
			Get
				Return _fullFilePath
			End Get
			Set(ByVal value As String)
				_fullFilePath = value
			End Set
		End Property

		Public Property FileIdInfo() As FileIdInfo
			Get
				Return _FileIdInfo
			End Get
			Set(ByVal value As FileIdInfo)
				_FileIdInfo = value
			End Set
		End Property

		Public Property LineStatus() As Int64
			Get
				Return _lineStatus
			End Get
			Set(ByVal value As Int64)
				_lineStatus = value
			End Set
		End Property

		Public Property FolderPath() As String

		''' <summary>
		''' Gets or sets the identifier for the record currently stored in
		''' the data grid for this document. If <c>Nothing</c>, then a record
		''' has not been created in DataGrid.
		''' </summary>
		Public Property DataGridID As String

		Public Sub New( _
		 ByVal fileGuid As String, _
		 ByVal identityValue As String, _
		 ByVal indexFileInDB As Boolean, _
		 ByVal filename As String, _
		 ByVal fullFilePath As String, _
		 ByVal uploadFile As Boolean, _
		 ByVal lineNumber As Int32, _
		 ByVal parentFolderID As Int32, _
		 ByVal record As Api.ArtifactFieldCollection, _
		 ByVal oixFileData As FileIdInfo, _
		 ByVal lineStatus As Int64, _
		 ByVal destinationVolume As String,
		 ByVal folderPath As String,
		 ByVal dataGridID As String
		 )
			_fileGuid = fileGuid
			_identityValue = identityValue
			_indexFileInDB = indexFileInDB
			_filename = filename
			_fullFilePath = fullFilePath
			_uploadFile = uploadFile
			_lineNumber = lineNumber
			_parentFolderID = parentFolderID
			_record = record

			_FileIdInfo = oixFileData
			_lineStatus = lineStatus
			_destinationVolume = destinationVolume
			Me.FolderPath = folderPath
			Me.DataGridID = dataGridID
		End Sub

		Public Function GetFileType() As String
			Dim type As String
			If FileIdInfo Is Nothing Then
				type = "Unknown format"
			Else
				type = FileIdInfo.Description
			End If
			Return type
		End Function
	End Class

	Public Class SizedMetaDocument
		Inherits MetaDocument
		Implements IHasFileSize

		Private _size As Long

		Public Sub New( _
		 ByVal fileGuid As String, _
		 ByVal identityValue As String, _
		 ByVal indexFileInDB As Boolean, _
		 ByVal filename As String, _
		 ByVal fullFilePath As String, _
		 ByVal uploadFile As Boolean, _
		 ByVal lineNumber As Int32, _
		 ByVal parentFolderID As Int32, _
		 ByVal record As Api.ArtifactFieldCollection, _
		 ByVal oixFileData As FileIdInfo, _
		 ByVal lineStatus As Int64, _
		 ByVal destinationVolume As String, _
		 ByVal size As Long,
		 ByVal folderPath As String,
		 ByVal dataGridID As String
		 )
			MyBase.New(fileGuid, identityValue, indexFileInDB, filename, fullFilePath, uploadFile, lineNumber, parentFolderID, record, oixFileData, lineStatus, destinationVolume, folderPath, dataGridID)
			_size = size
		End Sub

		Public Function GetFileSize() As Long Implements IHasFileSize.GetFileSize
			Return _size
		End Function
	End Class
End Namespace
