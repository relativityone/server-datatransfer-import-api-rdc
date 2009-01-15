Namespace kCura.WinEDDS
	Public Class MetaDocument
		Private _fileGuid As String
		Private _identityValue As String
		Private _fieldCollection As DocumentFieldCollection
		Private _indexFileInDB As Boolean
		Private _filename As String
		Private _fullFilePath As String
		Private _uploadFile As Boolean
		Private _lineNumber As Int32
		Private _parentFolderID As Int32
		Private _md5Hash As String
		Private _sourceLine As String()
		Private _fileIdData As OI.FileID.FileIDData
		Private _lineStatus As Int32
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

		Public Property FieldCollection() As DocumentFieldCollection
			Get
				Return _fieldCollection
			End Get
			Set(ByVal value As DocumentFieldCollection)
				_fieldCollection = value
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

		Public Property SourceLine() As String()
			Get
				Return _sourceLine
			End Get
			Set(ByVal value As String())
				_sourceLine = value
			End Set
		End Property

		Public Property Md5Hash() As String
			Get
				Return _md5Hash
			End Get
			Set(ByVal value As String)
				_md5Hash = value
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

		Public Property FileIdData() As OI.FileID.FileIDData
			Get
				Return _fileIdData
			End Get
			Set(ByVal value As OI.FileID.FileIDData)
				_fileIdData = value
			End Set
		End Property

		Public ReadOnly Property Size() As Int64
			Get
				Dim retval As Int64 = 0
				For Each cell As String In SourceLine
					retval += CType(cell.Length, Int64)
				Next
				Return retval * 2
			End Get
		End Property

		Public Property LineStatus() As Int32
			Get
				Return _lineStatus
			End Get
			Set(ByVal value As Int32)
				_lineStatus = value
			End Set
		End Property

		Public Sub New( _
		 ByVal fileGuid As String, _
		 ByVal identityValue As String, _
		 ByVal fieldCollection As DocumentFieldCollection, _
		 ByVal indexFileInDB As Boolean, _
		 ByVal filename As String, _
		 ByVal fullFilePath As String, _
		 ByVal uploadFile As Boolean, _
		 ByVal lineNumber As Int32, _
		 ByVal parentFolderID As Int32, _
		 ByVal md5Hash As String, _
		 ByVal sourceLine As String(), _
		 ByVal oixFileData As OI.FileID.FileIDData, _
		 ByVal lineStatus As Int32, _
		 ByVal destinationVolume As String _
		 )
			_fileGuid = fileGuid
			_identityValue = identityValue
			_fieldCollection = fieldCollection
			_indexFileInDB = indexFileInDB
			_filename = filename
			_fullFilePath = fullFilePath
			_uploadFile = uploadFile
			_lineNumber = lineNumber
			_parentFolderID = parentFolderID
			_sourceLine = sourceLine
			_md5Hash = md5Hash
			_fileIdData = oixFileData
			_lineStatus = lineStatus
			_destinationVolume = destinationVolume
		End Sub
	End Class
End Namespace
