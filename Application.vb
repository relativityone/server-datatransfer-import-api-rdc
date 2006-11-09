Namespace kCura.EDDS.WinForm
	Public Class Application

#Region "Singleton Methods"
		Private Shared _instance As Application

		Protected Sub New()
			_processPool = New kCura.Windows.Process.ProcessPool
			Dim currentZone As System.TimeZone = System.TimeZone.CurrentTimeZone
			_timeZoneOffset = currentZone.GetUtcOffset(DateTime.Now).Hours

			'If currentZone.IsDaylightSavingTime(DateTime.Now) Then
			'	_timeZoneOffset -= 1
			'End If
			System.Net.ServicePointManager.CertificatePolicy = New TrustAllCertificatePolicy
			_cookieContainer = New System.Net.CookieContainer
		End Sub

		Public Shared ReadOnly Property Instance() As Application
			Get
				If _instance Is Nothing Then
					_instance = New Application
				End If
				Return _instance
			End Get
		End Property
#End Region

#Region "Members"
		Public Event OnEvent(ByVal appEvent As AppEvent)
		Public Event ChangeCursor(ByVal cursorStyle As System.Windows.Forms.Cursor)

		Private _processPool As kCura.Windows.Process.ProcessPool
		Private _selectedCaseInfo As kCura.EDDS.Types.CaseInfo
		Private _selectedCaseFolderID As Int32
		Private _credential As System.Net.NetworkCredential
		Private _fields As kCura.WinEDDS.DocumentFieldCollection
		Private _selectedCaseFolderPath As String
		Private _timeZoneOffset As Int32
		Private WithEvents _loginForm As LoginForm
		Private Shared _cache As New Hashtable
		Private _temporaryWebServiceURL As String
		Private _cookieContainer As System.Net.CookieContainer
		'Private _identity As kCura.EDDS.EDDSIdentity
#End Region

#Region "Properties"
		Public Property TimeZoneOffset() As Int32
			Get
				Return _timeZoneOffset
			End Get
			Set(ByVal value As Int32)
				_timeZoneOffset = value
			End Set
		End Property

		'Public ReadOnly Property Identity() As kCura.EDDS.EDDSIdentity
		'	Get
		'		Return _identity
		'	End Get
		'End Property

		Public ReadOnly Property SelectedCaseInfo() As kCura.EDDS.Types.CaseInfo
			Get
				Return _selectedCaseInfo
			End Get
		End Property

		'Public ReadOnly Property SelectedCaseArtifactID() As Int32
		'	Get
		'		Return _selectedCaseInfo.ArtifactID
		'	End Get
		'End Property

		'Public ReadOnly Property SelectedCaseRootArtifactID() As Int32
		'	Get
		'		Return _selectedCaseInfo.RootArtifactID
		'	End Get
		'End Property

		'Public ReadOnly Property SelectedCaseFolderID() As Int32
		'	Get
		'		Return _selectedCaseFolderID
		'	End Get
		'End Property

		Public ReadOnly Property LoggedInUser() As String
			Get
				Dim winIdent As System.Security.Principal.WindowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent
				Return winIdent.Name
			End Get
		End Property

		Public ReadOnly Property CurrentFields() As DocumentFieldCollection
			Get
				If _fields Is Nothing Then
					_fields = New DocumentFieldCollection
					'Dim fieldManager As New kCura.WinEDDS.Service.FieldQuery(Credential, _cookieContainer, _identity)
					Dim fieldManager As New kCura.WinEDDS.Service.FieldQuery(Credential, _cookieContainer)
					Dim fields() As kCura.EDDS.WebAPI.DocumentManagerBase.Field = fieldManager.RetrieveAllAsArray(SelectedCaseInfo.ArtifactID)
					Dim i As Int32
					For i = 0 To fields.Length - 1
						With fields(i)
							_fields.Add(New DocumentField(.DisplayName, .ArtifactID, .FieldTypeID, .FieldCategoryID, .CodeTypeID, .MaxLength))
						End With
					Next
				End If
				Return _fields
			End Get
		End Property

		Friend ReadOnly Property Credential() As System.Net.NetworkCredential
			Get
				If _credential Is Nothing Then
					NewLogin()
				End If
				Return _credential
			End Get
		End Property

		Public Property TemporaryWebServiceURL() As String
			Get
				Return _temporaryWebServiceURL
			End Get
			Set(ByVal value As String)
				_temporaryWebServiceURL = value
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
#End Region

#Region "Event Throwers"
		Public Sub LogOn()
			RaiseEvent OnEvent(New AppEvent(AppEvent.AppEventType.LogOn))
		End Sub

		Public Sub ExitApplication()
			UpdateWebServiceURL()
			RaiseEvent OnEvent(New AppEvent(AppEvent.AppEventType.ExitApplication))
		End Sub

		Public Sub UpdateWebServiceURL()
			If Not Me.TemporaryWebServiceURL Is Nothing AndAlso Not Me.TemporaryWebServiceURL = "" Then
				kCura.WinEDDS.Config.WebServiceURL = Me.TemporaryWebServiceURL
			End If
		End Sub

		Public Sub CursorDefault()
			RaiseEvent ChangeCursor(System.Windows.Forms.Cursors.Default)
		End Sub

		Public Sub CursorWait()
			RaiseEvent ChangeCursor(System.Windows.Forms.Cursors.WaitCursor)
		End Sub
#End Region

#Region "Document Field Collection"
		Public Function GetDocumentFieldFromName(ByVal fieldName As String) As DocumentField
			Return CurrentFields.Item(fieldName)
		End Function

		Public Function GetCaseIdentifierFields() As String()
			Return CurrentFields.IdentifierFieldNames
		End Function

		Public Function IdentiferFieldDropdownPopulator() As String()
			Return CurrentFields.NamesForIdentifierDropdown
		End Function

		Public Function GetCaseFields(ByVal caseID As Int32) As String()
			Return CurrentFields.Names
		End Function

		Public Function GetSelectedIdentifier(ByVal selectedField As DocumentField) As String
			Try
				Return CurrentFields.Item(selectedField.FieldName).FieldName
			Catch ex As System.Exception
				Return String.Empty
			End Try
		End Function

		Friend Function ReadyToLoad(ByVal unselectedFieldNames As String()) As Boolean
			Dim identifierFieldName As String
			Dim unselectedFieldName As String
			Dim unselectedIDFieldNames As New System.Text.StringBuilder
			For Each identifierFieldName In CurrentFields.IdentifierFieldNames()
				For Each unselectedFieldName In unselectedFieldNames
					If identifierFieldName = unselectedFieldName Then
						unselectedIDFieldNames.AppendFormat(unselectedFieldName & ChrW(13))
					End If
				Next
			Next
			If unselectedIDFieldNames.Length = 0 Then
				Return True
			Else
				Return MsgBox("The following identifier fields have not been mapped: " & ChrW(13) & unselectedIDFieldNames.ToString & _
				"Do you wish to continue?", MsgBoxStyle.YesNo, "Warning!") = MsgBoxResult.Yes
			End If
		End Function
#End Region

#Region "Folder Management"
		Public Function CreateNewFolder(ByVal parentFolderID As Int32) As Int32
			Dim name As String = InputBox("Enter Folder Name", "Relativity Review")
			If name <> "" Then
				Dim folderManager As New kCura.WinEDDS.Service.FolderManager(Me.Credential, _cookieContainer)
				Dim folderID As Int32 = folderManager.Create(Me.SelectedCaseInfo.ArtifactID, parentFolderID, name)
				RaiseEvent OnEvent(New NewFolderEvent(parentFolderID, folderID, name))
			End If
		End Function

		Public Function GetCaseFolders(ByVal caseID As Int32) As System.Data.DataSet
			Dim folderManager As New kCura.WinEDDS.Service.FolderManager(Credential, _cookieContainer)
			Return folderManager.RetrieveAllByCaseID(caseID)
			'Dim dsFactory As New kCura.Utility.DataSetFactory
			'dsFactory.AddColumn("ArtifactID", kCura.Utility.DataSetFactory.DataType.Integer)
			'dsFactory.AddColumn("ParentArtifactID", kCura.Utility.DataSetFactory.DataType.Integer)
			'dsFactory.AddColumn("Name", kCura.Utility.DataSetFactory.DataType.String)
			'dsFactory.AddRow(1, System.DBNull.Value, "Case " + caseID.ToString)
			'dsFactory.AddRow(2, 1, "Collection 1")
			'dsFactory.AddRow(3, 1, "Collection 2")
			'dsFactory.AddRow(4, 1, "Collection 3")
			'dsFactory.AddRow(5, 3, "Folder 1")
			'dsFactory.AddRow(6, 3, "Folder 2")
			'dsFactory.AddRow(7, 3, "Folder 3")
			'Return dsFactory.BuildDataSet
		End Function
#End Region

#Region "Case Management"
		Public Function GetCases() As System.Data.DataSet
			_fields = Nothing
			Dim csMgr As New kCura.WinEDDS.Service.CaseManager(Credential, _cookieContainer)
			Return csMgr.RetrieveAll()

			'Dim dsFactory As New kCura.Utility.DataSetFactory
			'dsFactory.AddColumn("CaseID", kCura.Utility.DataSetFactory.DataType.Integer)
			'dsFactory.AddColumn("Name", kCura.Utility.DataSetFactory.DataType.String)
			'dsFactory.AddRow(1, "Repository 2")
			'dsFactory.AddRow(2, "Repository 1")
			'dsFactory.AddRow(3, "Repository 3")
			'dsFactory.AddRow(4, "Repository 4")
			'Return dsFactory.BuildDataSet
		End Function

		Public Sub SelectCaseFolder(ByVal folderInfo As FolderInfo)
			_selectedCaseFolderID = folderInfo.ArtifactID
			_selectedCaseFolderPath = folderInfo.Path
			RaiseEvent OnEvent(New AppEvent(AppEvent.AppEventType.CaseFolderSelected))
		End Sub

		Public Sub OpenCase()
			Try
				Dim caseInfo As kCura.EDDS.Types.CaseInfo = Me.GetCase
				If Not caseInfo Is Nothing Then
					_selectedCaseInfo = caseInfo
					RaiseEvent OnEvent(New LoadCaseEvent(caseInfo))
				End If
			Catch ex As System.Exception
				Me.ChangeWebServiceURL()
			End Try
		End Sub

		'TODO : consider renaming this to something more GUI specific
		Public Function GetCase() As kCura.EDDS.Types.CaseInfo
			Dim frm As New CaseSelectForm
			frm.ShowDialog()
			Return frm.SelectedCaseInfo
		End Function
#End Region

#Region "Utility"
		Public Function GetColumnHeadersFromLoadFile(ByVal loadfile As kCura.WinEDDS.LoadFile, ByVal firstLineContainsColumnHeaders As Boolean) As String()
			Dim parser As New kCura.WinEDDS.LoadFileImporter(loadfile, Nothing, _timeZoneOffset)
			Return parser.GetColumnNames(loadfile.FilePath)
			'Dim retValue(3) As String
			'retValue(0) = "Filed 0"
			'retValue(1) = "Field 1"
			'retValue(2) = "Field 2"
			'retValue(3) = "Field 3"
			'Return retValue
		End Function

		Public Function GetCaseFolderPath(ByVal caseFolderArtifactID As Int32) As String
			Return "\" & _selectedCaseInfo.Name & _selectedCaseFolderPath
		End Function

		'Worker function for PreviewLoadFile
		Public Function BuildLoadFileDataSource(ByVal al As ArrayList) As DataTable
			Try
				'Dim previewer As New kCura.WinEDDS.LoadFilePreviewer(loadFile, _timeZoneOffset, errorsOnly)
				'Dim al As ArrayList = DirectCast(previewer.ReadFile(loadFile.FilePath), ArrayList)
				'previewer.Close()
				Dim item As Object
				Dim field As DocumentField
				Dim fields As DocumentField()
				Dim firstTimeThrough As Boolean = True
				Dim row As ArrayList
				Dim dt As New DataTable
				Dim i As Int32 = 0
				If al.Count = 0 Then
					dt.Columns.Add("  ")
					dt.Rows.Add(New String() {"No errors"})
				Else
					For Each item In al
						If Not item Is Nothing Then
							row = New ArrayList
							fields = DirectCast(item, DocumentField())
							If firstTimeThrough Then
								dt.Columns.Add("Record Number")
								For Each field In fields
									dt.Columns.Add(field.FieldName)
								Next
								firstTimeThrough = False
							End If
							AddRow(dt, row, fields, i)
						End If
					Next
				End If
				Return dt
			Catch ex As System.Exception
				kCura.EDDS.WinForm.Utility.ThrowExceptionToGUI(ex)
			End Try
		End Function

		Private Sub AddRow(ByVal dt As DataTable, ByVal row As System.collections.ArrayList, ByVal fields As DocumentField(), ByRef counter As Int32)
			Try
				counter += 1
				Dim field As DocumentField
				row.Add(counter.ToString())
				For Each field In fields
					row.Add(field.Value)
				Next
				dt.Rows.Add(row.ToArray)
			Catch x As System.Exception
				Throw
			End Try
		End Sub

		Public Sub RefreshCaseFolders()
			RaiseEvent OnEvent(New kCura.WinEDDS.LoadCaseEvent(SelectedCaseInfo))
		End Sub

		Public Function CheckFieldMap(ByVal loadFile As LoadFile) As Boolean
			Dim unmapped As String()
			Dim fmi As LoadFileFieldMap.LoadFileFieldMapItem
			Dim fieldsNotColumnsMapped As Boolean
			Dim values As New ArrayList
			For Each fmi In loadFile.FieldMap
				If fmi.DocumentField Is Nothing AndAlso fmi.NativeFileColumnIndex <> -1 Then
					values.Add("Column " & fmi.NativeFileColumnIndex + 1)
					fieldsNotColumnsMapped = False
				ElseIf Not fmi.DocumentField Is Nothing AndAlso fmi.NativeFileColumnIndex = -1 Then
					values.Add(fmi.DocumentField.FieldName)
					fieldsNotColumnsMapped = True
				End If
			Next
			If values.Count > 0 Then
				unmapped = DirectCast(values.ToArray(GetType(String)), String())
				Dim sb As New System.Text.StringBuilder
				Dim nl As String = System.Environment.NewLine
				If fieldsNotColumnsMapped Then
					sb.Append("The following fields are unmapped:" & nl)
				Else
					sb.Append("The following file columns are unmapped:" & nl)
				End If
				Dim s As String
				For Each s In unmapped
					sb.Append(" - " & s & nl)
				Next
				sb.Append("Do you wish to continue?")
				If MsgBox(sb.ToString, MsgBoxStyle.YesNo, "Warning") = MsgBoxResult.No Then
					Return False
				Else
					Return True
				End If
			Else
				Return True
			End If
		End Function

		Private Sub SetWorkingDirectory(ByVal filePath As String)
			Dim directory As String
			If Not filePath.LastIndexOf("\") = filePath.Length - 1 Then
				directory = filePath.Substring(0, filePath.LastIndexOf("\") + 1)
			Else
				directory = String.Copy(filePath)
			End If
			System.IO.Directory.SetCurrentDirectory(directory)
		End Sub
#End Region

#Region "Form Initializers"
		Public Sub NewLoadFile(ByVal destinationArtifactID As Int32, ByVal caseInfo As kCura.EDDS.Types.CaseInfo)
			Dim frm As New LoadFileForm
			Dim loadFile As New loadFile
			frm._application = Me
			loadFile.DestinationFolderID = destinationArtifactID
			loadFile.CaseInfo = caseInfo
			loadFile.Credentials = Me.Credential
			loadFile.CookieContainer = Me.CookieContainer
			frm.LoadFile = loadFile
			frm.Show()
		End Sub

		Public Sub NewProductionExport(ByVal caseInfo As kCura.EDDS.Types.CaseInfo)
			Dim frm As New ProductionExportForm
			Dim exportFile As New exportFile
			Dim productionManager As New kCura.WinEDDS.Service.ProductionManager(Me.Credential, _cookieContainer)
			exportFile.CaseInfo = caseInfo
			exportFile.DataTable = productionManager.RetrieveProducedByContextArtifactID(caseInfo.ArtifactID).Tables(0)
			exportFile.Credential = Me.Credential
			exportFile.TypeOfExport = exportFile.ExportType.Production
			frm.Application = Me
			frm.ExportFile = exportFile
			frm.Show()
		End Sub

		Public Sub NewSearchExport(ByVal rootFolderID As Int32, ByVal caseInfo As kCura.EDDS.Types.CaseInfo, ByVal typeOfExport As kCura.WinEDDS.ExportFile.ExportType)
			Dim frm As New SearchExportForm
			Dim exportFile As New exportFile
			Dim searchManager As New kCura.WinEDDS.Service.SearchManager(Me.Credential, _cookieContainer)
			exportFile.ArtifactID = rootFolderID
			exportFile.CaseInfo = caseInfo
			If typeOfExport = exportFile.ExportType.ArtifactSearch Then
				exportFile.DataTable = searchManager.RetrieveViewsByContextArtifactID(caseInfo.ArtifactID, True).Tables(0)
			Else
				exportFile.DataTable = searchManager.RetrieveViewsByContextArtifactID(caseInfo.ArtifactID, False).Tables(0)
			End If
			exportFile.Credential = Me.Credential
			exportFile.TypeOfExport = typeOfExport
			frm.Application = Me
			frm.ExportFile = exportFile
			frm.Show()
		End Sub

		Public Sub NewOutlookImport(ByVal destinationArtifactID As Int32, ByVal caseInfo As kCura.EDDS.Types.CaseInfo)
			Dim importerAssembly As System.Reflection.Assembly
			importerAssembly = System.Reflection.Assembly.LoadFrom(kCura.WinEDDS.Config.OutlookImporterLocation)
			Dim hostImporterGateway As kCura.EDDS.Import.ImporterGatewayBase
			hostImporterGateway = CType(importerAssembly.CreateInstance("kCura.EDDS.Import.Outlook.ImporterGateway"), kCura.EDDS.Import.ImporterGatewayBase)
			Dim frm As Form = hostImporterGateway.GetSettingsForm(New Import.CaseInfo(caseInfo.ArtifactID, caseInfo.RootArtifactID), destinationArtifactID, New WinEDDSGateway)
			frm.Show()
		End Sub

		Public Sub NewSQLImport(ByVal destinationArtifactID As Int32, ByVal caseInfo As kCura.EDDS.Types.CaseInfo)
			Dim frm As New SQLImportForm
			Dim sQLImportSettings As New sQLImportSettings
			sQLImportSettings.DestinationFolderID = destinationArtifactID
			sQLImportSettings.CaseInfo = caseInfo
			frm.SQLImportSettings = sQLImportSettings
			frm.Show()
		End Sub

		Public Sub NewImageFile(ByVal destinationArtifactID As Int32, ByVal caseinfo As kCura.EDDS.Types.CaseInfo)
			CursorWait()
			Dim frm As New ImageLoad
			'Dim imageFile As New ImageLoadFile(Me.Identity)
			Dim imageFile As New ImageLoadFile
			imageFile.Credential = Me.Credential
			imageFile.CaseInfo = caseinfo
			frm.ImageLoadFile = imageFile
			frm.Show()
			CursorDefault()
		End Sub

		Public Sub NewDirectoryImport(ByVal destinationArtifactID As Int32, ByVal caseInfo As kCura.EDDS.Types.CaseInfo)
			CursorWait()
			Dim frm As New ImportFileSystemForm
			Dim importFileDirectorySettings As New importFileDirectorySettings
			importFileDirectorySettings.DestinationFolderID = destinationArtifactID
			importFileDirectorySettings.CaseInfo = caseInfo
			frm.ImportFileDirectorySettings = importFileDirectorySettings
			frm.Show()
			CursorDefault()
		End Sub

		Public Sub NewEnronImport(ByVal destinationArtifactID As Int32, ByVal caseInfo As kCura.EDDS.Types.CaseInfo)
			CursorWait()
			Dim frm As New ImportFileSystemForm
			Dim importFileDirectorySettings As New importFileDirectorySettings
			importFileDirectorySettings.EnronImport = True
			importFileDirectorySettings.DestinationFolderID = destinationArtifactID
			importFileDirectorySettings.CaseInfo = caseInfo
			frm.ImportFileDirectorySettings = importFileDirectorySettings
			frm.Show()
			CursorDefault()
		End Sub

		Public Sub NewOptions()
			CursorWait()
			Dim frm As New OptionsForm
			frm.Show()
			CursorDefault()
		End Sub

		Public Sub SetWebServiceURL()
			CursorWait()
			Dim frm As New SetWebServiceURL
			AddHandler frm.ExitApplication, AddressOf Me.ExitApplication
			frm.Required = True
			frm.ShowDialog()
			CursorDefault()
		End Sub

		Public Sub ChangeWebServiceURL()
			CursorWait()
			Dim frm As New SetWebServiceURL
			frm.Show()
			CursorDefault()
		End Sub

		Public Sub NewLogin()
			CursorWait()
			If Not _loginForm Is Nothing Then
				If Not _loginForm.IsDisposed Then
					_loginForm.Close()
				End If
			End If
			_loginForm = New LoginForm
			_loginForm.Show()
			CursorDefault()
		End Sub
#End Region

#Region "Process Management"
		Public Function PreviewLoadFile(ByVal loadFileToPreview As LoadFile, ByVal errorsOnly As Boolean) As Guid
			CursorWait()
			If Not CheckFieldMap(loadFileToPreview) Then
				CursorDefault()
				Exit Function
			End If
			Dim frm As New kCura.Windows.Process.ProgressForm
			Dim previewer As New kCura.WinEDDS.PreviewLoadFileProcess
			Dim previewfrm As New LoadFilePreviewForm
			Dim thrower As New ValueThrower
			previewer.Thrower = thrower
			previewer.TimeZoneOffset = _timeZoneOffset
			previewer.ErrorsOnly = errorsOnly
			previewfrm.Thrower = previewer.Thrower
			previewer.LoadFile = loadFileToPreview
			frm.ProcessObserver = previewer.ProcessObserver
			frm.ProcessController = previewer.ProcessController
			previewfrm.Show()
			frm.Show()
			_processPool.StartProcess(previewer)
			CursorDefault()
		End Function

		Public Function ImportDirectory(ByVal importFileDirectorySettings As ImportFileDirectorySettings) As Guid
			CursorWait()
			Dim frm As New kCura.Windows.Process.ProgressForm
			Dim importer As New kCura.WinEDDS.ImportFileDirectoryProcess(Credential, CookieContainer)
			'Dim importer As New kCura.WinEDDS.ImportFileDirectoryProcess(Credential, CookieContainer, Me.Identity)
			importer.ImportFileDirectorySettings = importFileDirectorySettings
			frm.ProcessObserver = importer.ProcessObserver
			frm.Show()
			CursorDefault()
			Return _processPool.StartProcess(importer)
		End Function

		Public Function ImportGeneric(ByVal settings As Object) As Guid
			CursorWait()
			Dim frm As New kCura.Windows.Process.ProgressForm
			Dim importer As New kCura.WinEDDS.GenericImportProcess(New WinEDDSGateway)
			importer.Settings = settings
			frm.ProcessObserver = importer.ProcessObserver
			frm.Show()
			CursorDefault()
			Return _processPool.StartProcess(importer)
		End Function

		Public Function ImportSQL(ByVal sqlimportsettings As SQLImportSettings) As Guid
			CursorWait()
			Dim frm As New kCura.Windows.Process.ProgressForm
			Dim imporProcess As New kCura.WinEDDS.SQLImportProcess
			imporProcess.SQLImportSettings = sqlimportsettings
			frm.ProcessObserver = imporProcess.ProcessObserver
			frm.ProcessController = imporProcess.ProcessController
			frm.Show()
			CursorDefault()
			Return _processPool.StartProcess(imporProcess)
		End Function

		Public Function ImportLoadFile(ByVal loadFile As LoadFile) As Guid
			CursorWait()
			'Dim folderManager As New kCura.WinEDDS.Service.FolderManager(Credential, _cookieContainer, _identity)
			Dim folderManager As New kCura.WinEDDS.Service.FolderManager(Credential, _cookieContainer)
			If folderManager.Exists(SelectedCaseInfo.ArtifactID, SelectedCaseInfo.RootFolderID) Then
				If CheckFieldMap(loadFile) Then
					Dim frm As New kCura.Windows.Process.ProgressForm
					Dim importer As New kCura.WinEDDS.ImportLoadFileProcess
					importer.LoadFile = loadFile
					importer.TimeZoneOffset = _timeZoneOffset
					SetWorkingDirectory(loadFile.FilePath)
					frm.ProcessObserver = importer.ProcessObserver
					frm.ProcessController = importer.ProcessController
					frm.StopImportButtonText = "Stop"
					frm.Show()
					frm.ProcessID = _processPool.StartProcess(importer)
					CursorDefault()
					Return frm.ProcessID
				End If
			Else
				CursorDefault()
				MsgBox("Selected folder no longer exists.  Please reselect.")
			End If
		End Function

		Public Function ImportImageFile(ByVal ImageLoadFile As ImageLoadFile) As Guid
			CursorWait()
			Dim frm As New kCura.Windows.Process.ProgressForm
			Dim importer As New kCura.WinEDDS.ImportImageFileProcess
			ImageLoadFile.CookieContainer = Me.CookieContainer
			importer.ImageLoadFile = ImageLoadFile
			SetWorkingDirectory(ImageLoadFile.FileName)
			frm.ProcessObserver = importer.ProcessObserver
			frm.ProcessController = importer.ProcessController
			frm.Show()
			CursorDefault()
			Return _processPool.StartProcess(importer)
		End Function

		Public Function StartProduction(ByVal exportFile As ExportFile) As Guid
			CursorWait()
			Dim frm As New kCura.Windows.Process.ProgressForm
			Dim exporter As New kCura.WinEDDS.ExportProductionProcess

			exporter.ExportFile = exportFile
			frm.ProcessObserver = exporter.ProcessObserver
			frm.ProcessController = exporter.ProcessController
			frm.Text = "Export Progress..."
			frm.Show()
			CursorDefault()
			Return _processPool.StartProcess(exporter)
		End Function

		Public Function StartSearch(ByVal exportFile As ExportFile) As Guid
			CursorWait()
			Dim frm As New kCura.Windows.Process.ProgressForm
			Dim exporter As New kCura.WinEDDS.ExportSearchProcess

			exporter.ExportFile = exportFile
			frm.ProcessObserver = exporter.ProcessObserver
			frm.ProcessController = exporter.ProcessController
			frm.Text = "Export Progress..."
			frm.Show()
			CursorDefault()
			Return _processPool.StartProcess(exporter)
		End Function

		Public Sub CancelImport(ByVal importProcessId As Guid)
			CursorWait()
			_processPool.AbortProcess(importProcessId)
			CursorDefault()
		End Sub

		Public Sub DeleteThread(ByVal processID As Guid)
			CursorWait()
			_processPool.RemoveProcess(processID)
			CursorDefault()
		End Sub
#End Region

#Region "Save/Load Settings Objects"
		Public Sub SaveLoadFile(ByVal loadFile As LoadFile, ByVal path As String)
			SaveFileObject(loadFile, path)
		End Sub

		Private Sub SaveFileObject(ByVal fileObject As Object, ByVal path As String)
			Dim sw As New System.IO.StreamWriter(path)
			Dim serializer As New System.Runtime.Serialization.Formatters.Soap.SoapFormatter
			Try
				serializer.Serialize(sw.BaseStream, fileObject)
				sw.Close()
			Catch ex As System.Exception
				MsgBox("Save Falied", MsgBoxStyle.Critical)
				'TODO: Log exception
			End Try
		End Sub

		Public Function ReadLoadFile(ByVal loadFile As LoadFile, ByVal path As String) As LoadFile
			Dim sr As New System.IO.StreamReader(path)
			Dim tempLoadFile As loadFile
			Dim deserializer As New System.Runtime.Serialization.Formatters.Soap.SoapFormatter
			Try
				tempLoadFile = DirectCast(deserializer.Deserialize(sr.BaseStream), loadFile)
				sr.Close()
			Catch ex As System.Exception
				MsgBox("Load Failed", MsgBoxStyle.Critical)
				'TODO: Log Exception
				Return Nothing
			End Try
			tempLoadFile.CaseInfo = Me.SelectedCaseInfo
			tempLoadFile.Credentials = Me.Credential
			tempLoadFile.DestinationFolderID = Me.SelectedCaseInfo.RootFolderID
			Return tempLoadFile
			'If tempLoadFile.CaseInfo.ArtifactID = loadFile.CaseInfo.ArtifactID Then
			'	tempLoadFile.Credentials = Me.Credential
			'	tempLoadFile.DestinationFolderID = Me.SelectedCaseFolderID
			'	Return tempLoadFile
			'Else
			'	MsgBox("Selected load file was created" & ControlChars.NewLine & "for a different case.  Load aborted.", MsgBoxStyle.Exclamation)
			'	Return Nothing
			'End If
		End Function

		Public Function ReadImageLoadFile(ByVal path As String) As ImageLoadFile
			Dim sr As New System.IO.StreamReader(path)
			Dim retval As ImageLoadFile
			Dim deserializer As New System.Runtime.Serialization.Formatters.Soap.SoapFormatter
			Try
				retval = DirectCast(deserializer.Deserialize(sr.BaseStream), ImageLoadFile)
				sr.Close()
			Catch ex As System.Exception
				MsgBox("Load Failed", MsgBoxStyle.Critical)
				'TODO: Log Exception
				Return Nothing
			End Try
			retval.CaseInfo = Me.SelectedCaseInfo
			retval.DestinationFolderID = Me.SelectedCaseInfo.RootFolderID
			retval.Credential = Me.Credential
			Return retval
		End Function

		Public Sub SaveImageLoadFile(ByVal imageLoadFile As ImageLoadFile, ByVal path As String)
			SaveFileObject(imageLoadFile, path)
		End Sub
#End Region

#Region "Login"
		Private Sub _loginForm_OK_Click(ByVal cred As System.Net.NetworkCredential) Handles _loginForm.OK_Click
			_loginForm.Close()
			Dim userManager As New kCura.WinEDDS.Service.UserManager(cred, _cookieContainer)
			Try
				'_identity = userManager.Login(cred.UserName, cred.Password)
				userManager.Login(cred.UserName, cred.Password)
				_credential = cred
				OpenCase()
			Catch ex As kCura.WinEDDS.Exception.InvalidLoginException
				If MsgBox("Invalid login. Try again?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
					NewLogin()
				Else
					ExitApplication()
				End If
			Catch ex As System.Net.WebException
				If MsgBox("Invalid URL. Try Again?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
					Dim url As String = InputBox("Enter New URL:", DefaultResponse:=kCura.WinEDDS.Config.WebServiceURL)
					If url <> "" Then
						kCura.WinEDDS.Config.WebServiceURL = url
						NewLogin()
					End If
				Else
					ExitApplication()
				End If
			End Try
		End Sub

		'Friend Function DefaultCredentialsAreGood() As Boolean
		'	Dim myHttpWebRequest As System.Net.HttpWebRequest
		'	Dim myHttpWebResponse As System.Net.HttpWebResponse
		'	Dim cred As System.Net.NetworkCredential
		'	cred = DirectCast(System.Net.CredentialCache.DefaultCredentials, System.Net.NetworkCredential)
		'	Try
		'		'	myHttpWebRequest = DirectCast(System.Net.WebRequest.Create(kCura.WinEDDS.Config.WebServiceURL), System.Net.HttpWebRequest)
		'		'	myHttpWebRequest.Credentials = System.Net.CredentialCache.DefaultCredentials
		'		'	myHttpWebResponse = DirectCast(myHttpWebRequest.GetResponse(), System.Net.HttpWebResponse)
		'		'Catch ex As System.Net.WebException
		'		myHttpWebRequest = DirectCast(System.Net.WebRequest.Create(kCura.WinEDDS.Config.WebServiceURL & "\RelativityManager.asmx"), System.Net.HttpWebRequest)
		'		myHttpWebRequest.Credentials = System.Net.CredentialCache.DefaultCredentials
		'		myHttpWebResponse = DirectCast(myHttpWebRequest.GetResponse(), System.Net.HttpWebResponse)
		'	Catch ex2 As System.Exception
		'		Return False
		'	End Try
		'	CheckVersion(System.Net.CredentialCache.DefaultCredentials)
		'	_credential = cred
		'	Return True
		'End Function

		Friend Function DefaultCredentialsAreGood() As Boolean
			Dim myHttpWebRequest As System.Net.HttpWebRequest
			Dim cred As System.Net.NetworkCredential = DirectCast(System.Net.CredentialCache.DefaultCredentials, System.Net.NetworkCredential)
			Try
				If kCura.WinEDDS.Config.AuthenticationMode = "Forms" Then
					myHttpWebRequest = DirectCast(System.Net.WebRequest.Create(kCura.WinEDDS.Config.WebServiceURL), System.Net.HttpWebRequest)
				ElseIf kCura.WinEDDS.Config.AuthenticationMode = "Windows" Then
					myHttpWebRequest = DirectCast(System.Net.WebRequest.Create(kCura.WinEDDS.Config.WebServiceURL & "\RelativityManager.asmx"), System.Net.HttpWebRequest)
				End If
				myHttpWebRequest.Credentials = System.Net.CredentialCache.DefaultCredentials
				Dim myHttpWebResponse As System.Net.HttpWebResponse = DirectCast(myHttpWebRequest.GetResponse(), System.Net.HttpWebResponse)
				CheckVersion(System.Net.CredentialCache.DefaultCredentials)
				_credential = cred
				Return True
			Catch ex As System.Exception
				Return False
			End Try
		End Function

		Private Sub CheckVersion(ByVal credential As Net.ICredentials)
			'Dim relativityManager As New kCura.WinEDDS.Service.RelativityManager(DirectCast(credential, System.Net.NetworkCredential), _cookieContainer)
			'Dim winRelativityVersion As String = System.Reflection.Assembly.GetExecutingAssembly.FullName.Split(","c)(1).Split("="c)(1)
			'Dim relativityWebVersion As String = relativityManager.RetrieveRelativityVersion()

			'If winRelativityVersion <> relativityWebVersion Then
			'	MsgBox(String.Format("Your version of WinRelativity is out of date. You are running version {0}, but version {1} is required.", winRelativityVersion, relativityWebVersion), MsgBoxStyle.Critical, "WinRelativity Version Mismatch")
			'	ExitApplication()
			'Else
			'	Exit Sub
			'End If
		End Sub
#End Region

	End Class
End Namespace