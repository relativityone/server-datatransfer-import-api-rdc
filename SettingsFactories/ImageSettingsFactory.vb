Namespace kCura.WinEDDS
	Public Class ImageSettingsFactory
		Inherits SettingsFactoryBase

		Private _loadFile As ImageLoadFile
		Private _docFields As DocumentFieldCollection

		Public Sub New(ByVal login As String, ByVal password As String, ByVal caseArtifactID As Int32)
			MyBase.New(login, password)
			Me.InitLoadFile(caseArtifactID)
		End Sub

		Public Sub New(ByVal credential As System.Net.NetworkCredential, ByVal caseArtifactID As Int32)
			MyBase.New(credential)
			Me.InitLoadFile(caseArtifactID)
		End Sub

		Private Sub InitLoadFile(ByVal caseArtifactID As Int32)
			_loadFile = New ImageLoadFile
			_loadFile.AutoNumberImages = False
			_loadFile.CookieContainer = Me.CookieContainer
			_loadFile.CopyFilesToDocumentRepository = True
			_loadFile.Credential = Me.Credential
			_loadFile.ForProduction = False
			_loadFile.FullTextEncoding = System.Text.Encoding.Default
			_loadFile.Overwrite = ImportOverwriteModeEnum.Append.ToString
			_loadFile.ProductionArtifactID = 0
			_loadFile.ProductionTable = Nothing
			_loadFile.ReplaceFullText = False
			Me.CaseArtifactID = caseArtifactID
			_loadFile.BeginBatesFieldArtifactID = 0
			_loadFile.ControlKeyField = ""
			_loadFile.SelectedCasePath = _loadFile.CaseInfo.DocumentPath
			_loadFile.StartLineNumber = 0
		End Sub

		Public Function ToLoadFile() As ImageLoadFile
			Return _loadFile
		End Function

		Public WriteOnly Property CaseArtifactID() As Int32
			Set(ByVal value As Int32)
				_loadFile.CaseInfo = Me.CaseManager.Read(value)
				_loadFile.CaseDefaultPath = _loadFile.CaseInfo.DocumentPath
				_loadFile.DestinationFolderID = _loadFile.CaseInfo.RootFolderID
				_docFields = Me.FieldManager.Query.RetrieveAllAsDocumentFieldCollection(value, Relativity.ArtifactType.Document)
			End Set
		End Property

		Public WriteOnly Property AutoNumberImages() As Boolean
			Set(ByVal value As Boolean)
				_loadFile.AutoNumberImages = value
			End Set
		End Property

		Public WriteOnly Property BeginBatesFieldDisplayName() As String
			Set(ByVal value As String)
				If value Is Nothing OrElse value = "" Then
					_loadFile.BeginBatesFieldArtifactID = 0
				Else
					_loadFile.BeginBatesFieldArtifactID = _docFields.Item(value).FieldID
				End If
			End Set
		End Property

		Public WriteOnly Property BeginBatesFieldArtifactID() As Int32
			Set(ByVal value As Int32)
				_loadFile.BeginBatesFieldArtifactID = _docFields.Item(value).FieldID
			End Set
		End Property

		Public WriteOnly Property ControlKeyFieldDisplayName() As String
			Set(ByVal value As String)
				_loadFile.ControlKeyField = value
			End Set
		End Property

		Public WriteOnly Property CopyFilesToDocumentRepository() As Boolean
			Set(ByVal value As Boolean)
				_loadFile.CopyFilesToDocumentRepository = value
			End Set
		End Property

		Public WriteOnly Property DestinationFolderID() As Int32
			Set(ByVal value As Int32)
				_loadFile.DestinationFolderID = value
			End Set
		End Property

		Public WriteOnly Property SourceFilePath() As String
			Set(ByVal value As String)
				_loadFile.FileName = value
			End Set
		End Property

		Public WriteOnly Property ForProduction() As Boolean
			Set(ByVal value As Boolean)
				_loadFile.ForProduction = value
			End Set
		End Property

		Public WriteOnly Property ExtractedTextFileEncoding() As System.Text.Encoding
			Set(ByVal Value As System.Text.Encoding)
				_loadFile.FullTextEncoding = Value
			End Set
		End Property

		Public WriteOnly Property OverwriteDestination() As OverwriteType
			Set(ByVal value As OverwriteType)
				Select Case value
					Case SettingsFactoryBase.OverwriteType.Append
						_loadFile.Overwrite = ImportOverwriteModeEnum.Append.ToString
					Case SettingsFactoryBase.OverwriteType.AppendOverlay
						_loadFile.Overwrite = ImportOverwriteModeEnum.AppendOverlay.ToString
					Case SettingsFactoryBase.OverwriteType.Overlay
						_loadFile.Overwrite = ImportOverwriteModeEnum.Overlay.ToString
				End Select
			End Set
		End Property

		Public WriteOnly Property ProductionArtifactID() As Int32
			Set(ByVal Value As Int32)
				_loadFile.ProductionArtifactID = Value
			End Set
		End Property

		Public WriteOnly Property ReplaceFullText() As Boolean
			Set(ByVal Value As Boolean)
				_loadFile.ReplaceFullText = Value
			End Set
		End Property

		Public WriteOnly Property SelectedDestinationRepository() As String
			Set(ByVal value As String)
				_loadFile.SelectedCasePath = value
			End Set
		End Property

		Public WriteOnly Property StartLineNumber() As Int64
			Set(ByVal value As Int64)
				_loadFile.StartLineNumber = value
			End Set
		End Property

		Public WriteOnly Property IdentityFieldId() As Int32
			Set(value As Int32)
				_loadFile.IdentityFieldId = value
			End Set
		End Property

		Public Overrides Sub Save(ByVal location As String)
			MyBase.SaveObject(location, _loadFile)
		End Sub

	End Class
End Namespace

