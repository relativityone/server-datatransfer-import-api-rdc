Namespace kCura.WinEDDS
	Public Class ExportFile
		'Protected _identity As kCura.EDDS.EDDSIdentity
		Protected _caseInfo As kCura.EDDS.Types.CaseInfo
		Protected _dataTable As System.Data.DataTable
		Protected _typeOfExport As ExportType
		Protected _folderPath As String
		Protected _artifactID As Int32
		Protected _viewID As Int32
		Protected _overwrite As Boolean
		Protected _recordDelimiter As Char
		Protected _quoteDelimiter As Char
		Protected _newlineDelimiter As Char
		Protected _multiRecordDelimiter As Char
		Protected _credential As Net.NetworkCredential
		Protected _cookieContainer As System.Net.CookieContainer
		Protected _exportFullText As Boolean
		Protected _exportNative As Boolean
		Protected _logFileFormat As kCura.WinEDDS.LoadFileType.FileFormat
		Protected _useAbsolutePaths As Boolean
		Protected _renameFilesToIdentifier As Boolean
		Protected _identifierColumnName As String
		Protected _volumeInfo As kCura.WinEDDS.Exporters.VolumeInfo
		Protected _exportImages As Boolean
		Protected _loadFileExtension As String
		Protected _imagePrecedence As Pair()
		Protected _loadFilesPrefix As String
		Private _exportNativesToFileNamedFrom As kCura.WinEDDS.ExportNativeWithFilenameFrom = ExportNativeWithFilenameFrom.Identifier

#Region "Public Properties"

		Public Property LoadFilesPrefix() As String
			Get
				Return _loadFilesPrefix
			End Get
			Set(ByVal value As String)
				_loadFilesPrefix = kCura.WinEDDS.Utility.GetFilesystemSafeName(value)
			End Set
		End Property

		Public Property ImagePrecedence() As Pair()
			Get
				Return _imagePrecedence
			End Get
			Set(ByVal value As Pair())
				_imagePrecedence = value
			End Set
		End Property

		Public Property CaseInfo() As kCura.EDDS.Types.CaseInfo
			Get
				Return _caseInfo
			End Get
			Set(ByVal value As kCura.EDDS.Types.CaseInfo)
				_caseInfo = value
			End Set
		End Property

		Public ReadOnly Property CaseArtifactID() As Int32
			Get
				Return Me.CaseInfo.ArtifactID
			End Get
		End Property

		Public Property DataTable() As System.Data.DataTable
			Get
				Return _dataTable
			End Get
			Set(ByVal value As System.Data.DataTable)
				_dataTable = value
			End Set
		End Property

		Public Property TypeOfExport() As ExportType
			Get
				Return _typeOfExport
			End Get
			Set(ByVal value As ExportType)
				_typeOfExport = value
			End Set
		End Property

		Public Property FolderPath() As String
			Get
				Return _folderPath
			End Get
			Set(ByVal value As String)
				_folderPath = value
			End Set
		End Property

		Public Property ArtifactID() As Int32
			Get
				Return _artifactID
			End Get
			Set(ByVal value As Int32)
				_artifactID = value
			End Set
		End Property

		Public Property ViewID() As Int32
			Get
				Return _viewID
			End Get
			Set(ByVal value As Int32)
				_viewID = value
			End Set
		End Property

		Public Property Overwrite() As Boolean
			Get
				Return _overwrite
			End Get
			Set(ByVal value As Boolean)
				_overwrite = value
			End Set
		End Property

		Public Property RecordDelimiter() As Char
			Get
				Return _recordDelimiter
			End Get
			Set(ByVal value As Char)
				_recordDelimiter = value
			End Set
		End Property

		Public Property QuoteDelimiter() As Char
			Get
				Return _quoteDelimiter
			End Get
			Set(ByVal value As Char)
				_quoteDelimiter = value
			End Set
		End Property

		Public Property NewlineDelimiter() As Char
			Get
				Return _newlineDelimiter
			End Get
			Set(ByVal value As Char)
				_newlineDelimiter = value
			End Set
		End Property

		Public Property MultiRecordDelimiter() As Char
			Get
				Return _multiRecordDelimiter
			End Get
			Set(ByVal value As Char)
				_multiRecordDelimiter = value
			End Set
		End Property

		Public Property Credential() As Net.NetworkCredential
			Get
				Return _credential
			End Get
			Set(ByVal value As Net.NetworkCredential)
				_credential = value
			End Set
		End Property

		Public Property CookieContainer() As System.Net.CookieContainer
			Get
				Return _cookieContainer
			End Get
			Set(ByVal value As System.Net.CookieContainer)
				_cookieContainer = value
			End Set
		End Property

		Public Property ExportFullText() As Boolean
			Get
				Return _exportFullText
			End Get
			Set(ByVal value As Boolean)
				_exportFullText = value
			End Set
		End Property

		Public Property ExportNative() As Boolean
			Get
				Return _exportNative
			End Get
			Set(ByVal value As Boolean)
				_exportNative = value
			End Set
		End Property

		Public Property LogFileFormat() As kCura.WinEDDS.LoadFileType.FileFormat
			Get
				Return _logFileFormat
			End Get
			Set(ByVal value As kCura.WinEDDS.LoadFileType.FileFormat)
				_logFileFormat = value
			End Set
		End Property

		Public Property UseAbsolutePaths() As Boolean
			Get
				Return _useAbsolutePaths
			End Get
			Set(ByVal value As Boolean)
				_useAbsolutePaths = value
			End Set
		End Property

		Public Property RenameFilesToIdentifier() As Boolean
			Get
				Return _renameFilesToIdentifier
			End Get
			Set(ByVal value As Boolean)
				_renameFilesToIdentifier = value
			End Set
		End Property

		Public Property IdentifierColumnName() As String
			Get
				Return _identifierColumnName
			End Get
			Set(ByVal value As String)
				_identifierColumnName = value
			End Set
		End Property

		Public Property LoadFileExtension() As String
			Get
				Return _loadFileExtension
			End Get
			Set(ByVal value As String)
				_loadFileExtension = value
			End Set
		End Property

		Public Property VolumeInfo() As kCura.WinEDDS.Exporters.VolumeInfo
			Get
				Return _volumeInfo
			End Get
			Set(ByVal value As kCura.WinEDDS.Exporters.VolumeInfo)
				_volumeInfo = value
			End Set
		End Property

		Public Property ExportImages() As Boolean
			Get
				Return _exportImages
			End Get
			Set(ByVal value As Boolean)
				_exportImages = value
			End Set
		End Property
		Public Property ExportNativesToFileNamedFrom() As kCura.WinEDDS.ExportNativeWithFilenameFrom
			Get
				Return _exportNativesToFileNamedFrom
			End Get
			Set(ByVal value As kCura.WinEDDS.ExportNativeWithFilenameFrom)
				_exportNativesToFileNamedFrom = value
			End Set
		End Property

#End Region

		Public Sub New()
			Me.RecordDelimiter = Chr(20)
			Me.QuoteDelimiter = Chr(254)
			Me.NewlineDelimiter = Chr(174)
			Me.MultiRecordDelimiter = Chr(59)
		End Sub

		'FOR DIRECT UPLOAD ONLY
		'Public Sub New(ByVal identity As kCura.EDDS.EDDSIdentity)
		'	Me.RecordDelimiter = Chr(20)
		'	Me.QuoteDelimiter = Chr(254)
		'	Me.NewlineDelimiter = Chr(174)
		'	Me.MultiRecordDelimiter = Chr(59)
		'	_identity = identity
		'End Sub

		Public Enum ExportType
			Production
			ArtifactSearch
			ParentSearch
			AncestorSearch
		End Enum

	End Class
End Namespace