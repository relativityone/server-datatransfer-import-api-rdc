Namespace kCura.WinEDDS
	Public Class ExportFile
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

#Region "Public Properties"

		Public Property CaseInfo() As kCura.EDDS.Types.CaseInfo
			Get
				Return _caseInfo
			End Get
			Set(ByVal value As kCura.EDDS.Types.CaseInfo)
				_caseInfo = value
			End Set
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

#End Region

		Public Sub New()
			Me.RecordDelimiter = Chr(20)
			Me.QuoteDelimiter = Chr(254)
			Me.NewlineDelimiter = Chr(174)
			Me.MultiRecordDelimiter = Chr(59)
		End Sub

		Public Enum ExportType
			Production
			ArtifactSearch
			ParentSearch
			AncestorSearch
		End Enum

	End Class
End Namespace