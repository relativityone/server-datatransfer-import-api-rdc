Namespace kCura.WinEDDS
	Public Class OpticonFileReader
		Inherits kCura.Utility.DelimitedFileImporter
		Implements Api.IImageReader
		Private _settings As ImageLoadFile
		Public ReadOnly Property Settings() As ImageLoadFile
			Get
				Return _settings
			End Get
		End Property


		Public Sub New(ByVal folderID As Int32, ByVal args As ImageLoadFile, ByVal controller As kCura.Windows.Process.Controller, ByVal processID As Guid, ByVal doRetryLogic As Boolean)
			MyBase.New(New Char() {","c}, doRetryLogic)
			_settings = args
			'_docManager = New kCura.WinEDDS.Service.DocumentManager(args.Credential, args.CookieContainer)
			'_fieldQuery = New kCura.WinEDDS.Service.FieldQuery(args.Credential, args.CookieContainer)
			'_folderManager = New kCura.WinEDDS.Service.FolderManager(args.Credential, args.CookieContainer)
			'_auditManager = New kCura.WinEDDS.Service.AuditManager(args.Credential, args.CookieContainer)
			'_fileManager = New kCura.WinEDDS.Service.FileManager(args.Credential, args.CookieContainer)
			'_productionManager = New kCura.WinEDDS.Service.ProductionManager(args.Credential, args.CookieContainer)
			'_bulkImportManager = New kCura.WinEDDS.Service.BulkImportManager(args.Credential, args.CookieContainer)
			'Dim suffix As String = "\EDDS" & args.CaseInfo.ArtifactID & "\"
			'If args.SelectedCasePath = "" Then
			'	_repositoryPath = args.CaseDefaultPath.TrimEnd("\"c) & suffix
			'Else
			'	_repositoryPath = args.SelectedCasePath.TrimEnd("\"c) & suffix
			'End If
			'_textRepositoryPath = args.CaseDefaultPath & "EDDS" & args.CaseInfo.ArtifactID & "\"
			'_fileUploader = New kCura.WinEDDS.FileUploader(args.Credential, args.CaseInfo.ArtifactID, _repositoryPath, args.CookieContainer)
			'_bcpuploader = New kCura.WinEDDS.FileUploader(args.Credential, args.CaseInfo.ArtifactID, _repositoryPath, args.CookieContainer, False)
			'_folderID = folderID
			'_productionArtifactID = args.ProductionArtifactID
			'If _productionArtifactID <> 0 Then
			'	_productionDTO = _productionManager.Read(args.CaseInfo.ArtifactID, _productionArtifactID)
			'	_keyFieldDto = New kCura.WinEDDS.Service.FieldManager(args.Credential, args.CookieContainer).Read(args.CaseInfo.ArtifactID, args.BeginBatesFieldArtifactID)
			'ElseIf args.IdentityFieldId <> -1 Then
			'	_keyFieldDto = New kCura.WinEDDS.Service.FieldManager(args.Credential, args.CookieContainer).Read(args.CaseInfo.ArtifactID, args.IdentityFieldId)
			'Else
			'	Dim fieldID As Int32 = _fieldQuery.RetrieveAllAsDocumentFieldCollection(args.CaseInfo.ArtifactID, Relativity.ArtifactType.Document).IdentifierFields(0).FieldID
			'	_keyFieldDto = New kCura.WinEDDS.Service.FieldManager(args.Credential, args.CookieContainer).Read(args.CaseInfo.ArtifactID, fieldID)
			'End If
			'_overwrite = args.Overwrite
			'_replaceFullText = args.ReplaceFullText
			'_selectedIdentifierField = args.ControlKeyField
			'_processController = controller
			'_copyFilesToRepository = args.CopyFilesToDocumentRepository
			'_continue = True
			'_autoNumberImages = args.AutoNumberImages
			'_caseInfo = args.CaseInfo
			'_settings = args
			'_processID = processID
			'_startLineNumber = args.StartLineNumber
		End Sub

		Public Sub AdvanceRecord() Implements Api.IImageReader.AdvanceRecord
			Me.AdvanceLine()
		End Sub

		Public Overloads Sub Close() Implements Api.IImageReader.Close
			MyBase.Close()
		End Sub

		Public ReadOnly Property CurrentRecordNumber() As Integer Implements Api.IImageReader.CurrentRecordNumber
			Get
				Return MyBase.CurrentLineNumber
			End Get
		End Property

		Private Enum Columns
			BatesNumber = 0
			FileLocation = 2
			MultiPageIndicator = 3
		End Enum

		Public Function GetImageRecord() As Api.ImageRecord Implements Api.IImageReader.GetImageRecord
			Dim val As String() = Me.GetLine
			If val.Length < 4 Then Throw New InvalidLineFormatException(Me.CurrentLineNumber, val.Length)
			Dim retval As New Api.ImageRecord
			retval.BatesNumber = val(Columns.BatesNumber)
			retval.FileLocation = val(Columns.FileLocation)
			retval.IsNewDoc = val(Columns.MultiPageIndicator).ToLower = "y"
			Return retval
		End Function

		Public ReadOnly Property HasMoreRecords() As Boolean Implements Api.IImageReader.HasMoreRecords
			Get
				Return Not Me.HasReachedEOF
			End Get
		End Property

		Public Overrides Function ReadFile(ByVal path As String) As Object
			Throw New MethodAccessException("Unsupported Operation")
		End Function

		Public Function CountRecords() As Long Implements Api.IImageReader.CountRecords
			Return kCura.Utility.File.CountLinesInFile(Me.Settings.FileName)
		End Function

		Public Sub Cancel() Implements Api.IImageReader.Cancel
			Me.DoRetryLogic = False
		End Sub

#Region " Exceptions - Fatal "

		Public Class InvalidLineFormatException
			Inherits System.Exception
			Public Sub New(ByVal lineNumber As Int32, ByVal numberOfColumns As Int32)
				MyBase.New(String.Format("Invalid opticon file line {0}.  There must be at least 4 columns per line in an opticon file, there are {1} in the current line", lineNumber, numberOfColumns))
			End Sub
		End Class

#End Region

		Public Sub Initialize() Implements Api.IImageReader.Initialize
			Me.Reader = New System.IO.StreamReader(_settings.FileName, System.Text.Encoding.Default, True)
			Me.Rewind()
		End Sub
	End Class
End Namespace

