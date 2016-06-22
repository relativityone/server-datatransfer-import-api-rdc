Namespace kCura.WinEDDS
	Public Class NativeSettingsFactory

		Inherits SettingsFactoryBase
		Private _loadFile As LoadFile
		Private _docFields As DocumentFieldCollection


		Public Sub New(ByVal credential As System.Net.NetworkCredential, ByVal caseArtifactID As Int32)
			MyBase.New(credential)
			Me.InitloadFile(caseArtifactID)
		End Sub

		Public Sub New(ByVal login As String, ByVal password As String, ByVal caseArtifactID As Int32)
			MyBase.New(login, password)
			Me.InitloadFile(caseArtifactID)
		End Sub

		Private Sub InitloadFile(ByVal caseArtifactID As Int32)
			_loadFile = New LoadFile
			_loadFile.ArtifactTypeID = Relativity.ArtifactType.Document
			_loadFile.CookieContainer = Me.CookieContainer
			_loadFile.CopyFilesToDocumentRepository = True
			_loadFile.CreateFolderStructure = False
			_loadFile.Credentials = Me.Credential
			_loadFile.ExtractedTextFileEncoding = System.Text.Encoding.Default
			_loadFile.ExtractedTextFileEncodingName = Relativity.SqlNameHelper.GetSqlFriendlyName(_loadFile.ExtractedTextFileEncoding.EncodingName).ToLower
			_loadFile.FieldMap = New kCura.WinEDDS.LoadFileFieldMap
			_loadFile.FirstLineContainsHeaders = True
			_loadFile.FolderStructureContainedInColumn = Nothing
			_loadFile.FullTextColumnContainsFileLocation = False
			_loadFile.GroupIdentifierColumn = ""
			_loadFile.DataGridIDColumn = ""
			_loadFile.HierarchicalValueDelimiter = "\"c
			_loadFile.LoadNativeFiles = False
			_loadFile.MultiRecordDelimiter = ";"c
			_loadFile.NativeFilePathColumn = Nothing
			_loadFile.NewlineDelimiter = ChrW(174)
			_loadFile.OverwriteDestination = WinEDDS.OverwriteModeEnum.Append.ToString()
			_loadFile.QuoteDelimiter = ChrW(254)
			_loadFile.RecordDelimiter = ChrW(20)
			_loadFile.SourceFileEncoding = System.Text.Encoding.Default
			Me.CaseArtifactID = caseArtifactID
			_loadFile.SelectedCasePath = _loadFile.CaseInfo.DocumentPath
			_loadFile.SelectedIdentifierField = _docFields.IdentifierFields(0)
		End Sub

#Region " Required "

		Public WriteOnly Property CaseArtifactID() As Int32
			Set(ByVal value As Int32)
				_loadFile.CaseInfo = Me.CaseManager.Read(value)
				_loadFile.CaseDefaultPath = _loadFile.CaseInfo.DocumentPath
				_loadFile.DestinationFolderID = _loadFile.CaseInfo.RootFolderID
				_docFields = Me.FieldManager.Query.RetrieveAllAsDocumentFieldCollection(value, Relativity.ArtifactType.Document)
			End Set
		End Property

#End Region

#Region " No default "

		Public WriteOnly Property SourceFilePath() As String
			Set(ByVal Value As String)
				_loadFile.FilePath = Value
			End Set
		End Property

#End Region

		Public WriteOnly Property CopyFilesToDocumentRepository() As Boolean
			Set(ByVal value As Boolean)
				_loadFile.CopyFilesToDocumentRepository = value
			End Set
		End Property

		Public WriteOnly Property CreateFolderStructure() As Boolean
			Set(ByVal value As Boolean)
				_loadFile.CreateFolderStructure = value
			End Set
		End Property

		Public WriteOnly Property DestinationFolderID() As Int32
			Set(ByVal value As Int32)
				_loadFile.DestinationFolderID = value
			End Set
		End Property

		Public WriteOnly Property ExtractedTextFileEncoding() As System.Text.Encoding
			Set(ByVal Value As System.Text.Encoding)
				_loadFile.ExtractedTextFileEncoding = Value
				_loadFile.ExtractedTextFileEncodingName = Relativity.SqlNameHelper.GetSqlFriendlyName(_loadFile.ExtractedTextFileEncoding.EncodingName).ToLower
			End Set
		End Property

#Region " Field Map "

		Public Sub AddMappedItem(ByVal sourceColumnIndex As Int32, ByVal fieldArtifactID As Int32)
			Me.AddMappedItem(sourceColumnIndex, _docFields.Item(fieldArtifactID))
		End Sub

		Public Sub AddMappedItem(ByVal sourceColumnIndex As Int32, ByVal fieldDisplayName As String)
			Me.AddMappedItem(sourceColumnIndex, _docFields.Item(fieldDisplayName))
		End Sub

		Public Sub AddMappedItem(ByVal sourceColumnIndex As Int32, ByVal field As DocumentField)
			_loadFile.FieldMap.Add(New kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem(field, sourceColumnIndex))
		End Sub

#End Region

		Public WriteOnly Property FirstLineContainsHeaders() As Boolean
			Set(ByVal value As Boolean)
				_loadFile.FirstLineContainsHeaders = value
			End Set
		End Property

		Public WriteOnly Property FolderStructureContainedInColumn() As String
			Set(ByVal value As String)
				_loadFile.FolderStructureContainedInColumn = value
			End Set
		End Property

		Public WriteOnly Property FullTextColumnContainsFileLocation() As Boolean
			Set(ByVal value As Boolean)
				_loadFile.FullTextColumnContainsFileLocation = value
			End Set
		End Property

		Public WriteOnly Property HierarchicalValueDelimiter() As Char
			Set(ByVal value As Char)
				_loadFile.HierarchicalValueDelimiter = value
			End Set
		End Property

		Public WriteOnly Property LoadNativeFiles() As Boolean
			Set(ByVal value As Boolean)
				_loadFile.LoadNativeFiles = value
			End Set
		End Property

		Public WriteOnly Property MultiRecordDelimiter() As Char
			Set(ByVal value As Char)
				_loadFile.MultiRecordDelimiter = value
			End Set
		End Property

		Public WriteOnly Property NativeFilePathColumnIndex() As Int32
			Set(ByVal value As Int32)
				If value = -1 Then
					_loadFile.NativeFilePathColumn = Nothing
				Else
					_loadFile.NativeFilePathColumn = "(" & value + 1 & ")"
				End If
			End Set
		End Property

		Public WriteOnly Property NewlineProxy() As Char
			Set(ByVal value As Char)
				_loadFile.NewlineDelimiter = value
			End Set
		End Property

		Public WriteOnly Property OverwriteDestination() As OverwriteType
			Set(ByVal value As OverwriteType)
				Select Case value
					Case SettingsFactoryBase.OverwriteType.Append
						_loadFile.OverwriteDestination = WinEDDS.OverwriteModeEnum.Append.ToString()
					Case SettingsFactoryBase.OverwriteType.AppendOverlay
						_loadFile.OverwriteDestination = WinEDDS.OverwriteModeEnum.AppendOverlay.ToString()
					Case SettingsFactoryBase.OverwriteType.Overlay
						_loadFile.OverwriteDestination = WinEDDS.OverwriteModeEnum.Overlay.ToString()
				End Select
			End Set
		End Property

		Public WriteOnly Property QuoteProxy() As Char
			Set(ByVal value As Char)
				_loadFile.QuoteDelimiter = value
			End Set
		End Property

		Public WriteOnly Property RecordDelimiter() As Char
			Set(ByVal value As Char)
				_loadFile.RecordDelimiter = value
			End Set
		End Property

		Public WriteOnly Property SelectedDestinationRepository() As String
			Set(ByVal value As String)
				_loadFile.SelectedCasePath = value
			End Set
		End Property

		Public WriteOnly Property SourceFileEncoding() As System.Text.Encoding
			Set(ByVal value As System.Text.Encoding)
				_loadFile.SourceFileEncoding = value
			End Set
		End Property

		Public Overrides Sub Save(ByVal location As String)
			MyBase.SaveObject(location, _loadFile)
		End Sub

		Public Function ToLoadFile() As LoadFile
			Return _loadFile
		End Function

	End Class
End Namespace

