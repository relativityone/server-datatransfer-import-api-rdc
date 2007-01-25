Namespace kCura.WinEDDS
	Public Class MetaDocument
		Private _fileGuid As String
		Private _identityValue As String
		Private _fieldCollection As DocumentFieldCollection
		Private _indexFileInDB As Boolean
		Private _filename As String
		Private _uploadFile As Boolean
		Private _lineNumber As Int32
		Private _parentFolderID As Int32
		Private _md5Hash As String
		Private _sourceLine As String()

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

		Public Sub New( _
		 ByVal fileGuid As String, _
		 ByVal identityValue As String, _
		 ByVal fieldCollection As DocumentFieldCollection, _
		 ByVal indexFileInDB As Boolean, _
		 ByVal filename As String, _
		 ByVal uploadFile As Boolean, _
		 ByVal lineNumber As Int32, _
		 ByVal parentFolderID As Int32, _
		 ByVal md5Hash As String, _
		 ByVal sourceLine As String() _
		 )
			_fileGuid = fileGuid
			_identityValue = identityValue
			_fieldCollection = fieldCollection
			_indexFileInDB = indexFileInDB
			_filename = filename
			_uploadFile = uploadFile
			_lineNumber = lineNumber
			_parentFolderID = parentFolderID
			_sourceLine = sourceLine
			_md5Hash = md5Hash
		End Sub
	End Class
End Namespace
