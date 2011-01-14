Imports System.Web.Services.Protocols

Namespace kCura.EDDS.WinForm
	Public Class Application

#Region "Singleton Methods"
		Private Shared _instance As Application

		Protected Sub New()
			_processPool = New kCura.Windows.Process.ProcessPool
			Dim currentZone As System.TimeZone = System.TimeZone.CurrentTimeZone

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
		Private _selectedCaseInfo As Relativity.CaseInfo
		Private _selectedCaseFolderID As Int32
		Private _credential As System.Net.NetworkCredential
		Private _fields As kCura.WinEDDS.DocumentFieldCollection
		Private _selectedCaseFolderPath As String
		Private _timeZoneOffset As Int32
		Private WithEvents _loginForm As LoginForm
		Private Shared _cache As New Hashtable
		Private _temporaryWebServiceURL As String
		Private _temporaryForceFolderPreview As Boolean
		Private _cookieContainer As System.Net.CookieContainer
		Private _documentRepositoryList As String()
		Private _artifactTypeID As Int32
		Private _totalFolders As New System.Collections.Specialized.HybridDictionary

		'Private _identity As kCura.EDDS.EDDSIdentity
#End Region

#Region "Properties"
		Public Property TimeZoneOffset() As Int32
			Get
				Return 0
				Return _timeZoneOffset
			End Get
			Set(ByVal value As Int32)
				_timeZoneOffset = value
			End Set
		End Property

		Public ReadOnly Property SelectedCaseInfo() As Relativity.CaseInfo
			Get
				Return _selectedCaseInfo
			End Get
		End Property

		Public Sub RefreshSelectedCaseInfo(Optional ByVal caseInfo As Relativity.CaseInfo = Nothing)
			Dim caseManager As New kCura.WinEDDS.Service.CaseManager(Me.Credential, _cookieContainer)
			If caseInfo Is Nothing Then
				_selectedCaseInfo = caseManager.Read(_selectedCaseInfo.ArtifactID)
			Else
				_selectedCaseInfo = caseManager.Read(caseInfo.ArtifactID)
			End If
			_documentRepositoryList = caseManager.GetAllDocumentFolderPathsForCase(_selectedCaseInfo.ArtifactID)
		End Sub

		Public ReadOnly Property SelectedCaseFolderID() As Int32
			Get
				Return _selectedCaseFolderID
			End Get
		End Property

		Public ReadOnly Property DocumentRepositoryList() As String()
			Get
				Return _documentRepositoryList
			End Get
		End Property

		Public ReadOnly Property LoggedInUser() As String
			Get
				Dim winIdent As System.Security.Principal.WindowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent
				Return winIdent.Name
			End Get
		End Property

		Public ReadOnly Property SendLoadNotificationEmailEnabled() As Boolean
			Get
				Return New kCura.WinEDDS.Service.RelativityManager(Me.Credential, Me.CookieContainer).IsImportEmailNotificationEnabled
			End Get
		End Property

		Public ReadOnly Property CurrentFields(ByVal artifactTypeID As Int32, Optional ByVal refresh As Boolean = False) As DocumentFieldCollection
			Get
				Try
					If _fields Is Nothing OrElse refresh Then
						_fields = New DocumentFieldCollection
						Dim fieldManager As New kCura.WinEDDS.Service.FieldQuery(Credential, _cookieContainer)
						Dim fields() As kCura.EDDS.WebAPI.DocumentManagerBase.Field
						fields = fieldManager.RetrieveAllAsArray(SelectedCaseInfo.ArtifactID, artifactTypeID)
						Dim i As Int32
						For i = 0 To fields.Length - 1
							With fields(i)
								_fields.Add(New DocumentField(.DisplayName, .ArtifactID, .FieldTypeID, .FieldCategoryID, .CodeTypeID, .MaxLength, .AssociativeArtifactTypeID, .UseUnicodeEncoding, .ImportBehavior))
							End With
						Next
					End If
					Return _fields
				Catch ex As System.Exception
					If ex.Message.IndexOf("Need To Re Login") <> -1 Then
						NewLogin(False)
					Else
						Throw
					End If
				End Try
				Return Nothing
			End Get
		End Property

		Public ReadOnly Property CurrentNonFileFields(ByVal artifactTypeID As Int32, Optional ByVal refresh As Boolean = False) As DocumentFieldCollection
			Get
				Try
					If _fields Is Nothing OrElse refresh Then
						_fields = New DocumentFieldCollection
						Dim fieldManager As New kCura.WinEDDS.Service.FieldQuery(Credential, _cookieContainer)
						Dim fields() As kCura.EDDS.WebAPI.DocumentManagerBase.Field
						fields = fieldManager.RetrieveAllAsArray(SelectedCaseInfo.ArtifactID, artifactTypeID)
						Dim i As Int32
						For i = 0 To fields.Length - 1
							With fields(i)
								If fields(i).FieldTypeID <> 9 Then
									_fields.Add(New DocumentField(.DisplayName, .ArtifactID, .FieldTypeID, .FieldCategoryID, .CodeTypeID, .MaxLength, .AssociativeArtifactTypeID, .UseUnicodeEncoding, .ImportBehavior))
								End If
							End With
						Next
					End If
					Return _fields
				Catch ex As System.Exception
					If ex.Message.IndexOf("Need To Re Login") <> -1 Then
						NewLogin(False)
					Else
						Throw
					End If
				End Try
				Return Nothing
			End Get
		End Property

		Public Function HasFileField(ByVal artifactTypeID As Int32, Optional ByVal refresh As Boolean = False) As Boolean
			Dim retval As Boolean = False
			Dim docFieldCollection As DocumentFieldCollection = CurrentFields(artifactTypeID, refresh)
			Dim allFields As ICollection = docFieldCollection.AllFields
			For Each field As DocumentField In allFields
				If field.FieldTypeID = Relativity.FieldTypeHelper.FieldType.File Then
					retval = True
				End If
			Next
			Return retval
		End Function

		Public Function GetObjectTypeName(ByVal artifactTypeID As Int32) As String
			Dim objectTypeManager As New kCura.WinEDDS.Service.ObjectTypeManager(Me.Credential, Me.CookieContainer)
			Dim uploadableObjectTypes As System.Data.DataRowCollection = objectTypeManager.RetrieveAllUploadable(Me.SelectedCaseInfo.ArtifactID).Tables(0).Rows
			For Each objectType As System.Data.DataRow In uploadableObjectTypes
				With New kCura.WinEDDS.ObjectTypeListItem(CType(objectType("DescriptorArtifactTypeID"), Int32), CType(objectType("Name"), String), CType(objectType("HasAddPermission"), Boolean))
					If .Value = artifactTypeID Then Return .Display
				End With
			Next
			Return String.Empty
		End Function

		Public Function AllUploadableArtifactTypes() As System.Data.DataTable
			Return New kCura.WinEDDS.Service.ObjectTypeManager(Me.Credential, Me.CookieContainer).RetrieveAllUploadable(Me.SelectedCaseInfo.ArtifactID).Tables(0)
		End Function

		'Public ReadOnly Property CurrentGroupIdentifierField() As DocumentField
		'	Get
		'		Dim fieldManager As New kCura.WinEDDS.Service.FieldQuery(Credential, _cookieContainer)
		'		Dim fields() As kCura.EDDS.WebAPI.DocumentManagerBase.Field = fieldManager.RetrieveAllAsArray(SelectedCaseInfo.ArtifactID, True)
		'		Dim i As Int32
		'		For i = 0 To fields.Length - 1
		'			With fields(i)
		'				If fields(i).DisplayName.ToLower = "group identifier" Then
		'					Return New DocumentField(.DisplayName, .ArtifactID, .FieldTypeID, .FieldCategoryID, .CodeTypeID, .MaxLength, .UseUnicodeEncoding)
		'				End If
		'			End With
		'		Next
		'	End Get
		'End Property

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

		Public Property TemporaryForceFolderPreview() As Boolean
			Get
				Return _temporaryForceFolderPreview
			End Get
			Set(ByVal value As Boolean)
				_temporaryForceFolderPreview = value
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

		Public Property ArtifactTypeID() As Int32
			Get
				Return _artifactTypeID
			End Get
			Set(ByVal value As Int32)
				_artifactTypeID = value
			End Set
		End Property

#End Region

#Region "Event Throwers"
		Public Sub LogOn()
			RaiseEvent OnEvent(New AppEvent(AppEvent.AppEventType.LogOn))
		End Sub

		Public Sub ExitApplication()
			UpdateWebServiceURL(False)
			UpdateForceFolderPreview()
			RaiseEvent OnEvent(New AppEvent(AppEvent.AppEventType.ExitApplication))
		End Sub

		Public Sub UpdateForceFolderPreview()
			kCura.WinEDDS.Config.ForceFolderPreview = Me.TemporaryForceFolderPreview
		End Sub

		Public Sub UpdateWebServiceURL(ByVal relogin As Boolean)
			If Not Me.TemporaryWebServiceURL Is Nothing AndAlso Not Me.TemporaryWebServiceURL = String.Empty AndAlso Not Me.TemporaryWebServiceURL.Equals(kCura.WinEDDS.Config.WebServiceURL) Then
				kCura.WinEDDS.Config.WebServiceURL = Me.TemporaryWebServiceURL
				If relogin Then
					Me.NewLogin(True)
				End If
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
			Return CurrentFields(Me.ArtifactTypeID).Item(fieldName)
		End Function

		Public Function GetCaseIdentifierFields(ByVal artifactTypeID As Int32) As String()
			Return CurrentFields(artifactTypeID).IdentifierFieldNames
		End Function

		Public Function IdentiferFieldDropdownPopulator() As String()
			Return CurrentFields(Me.ArtifactTypeID).NamesForIdentifierDropdown
		End Function

		Public Function GetCaseFields(ByVal caseID As Int32, ByVal artifactTypeID As Int32, Optional ByVal refresh As Boolean = False) As String()
			Dim retval As DocumentFieldCollection = CurrentFields(artifactTypeID, refresh)
			If Not retval Is Nothing Then
				Return CurrentFields(artifactTypeID, refresh).Names()
			Else
				Return Nothing
			End If
		End Function

		Public Function GetNonFileCaseFields(ByVal caseID As Int32, ByVal artifactTypeID As Int32, Optional ByVal refresh As Boolean = False) As String()
			Dim retval As DocumentFieldCollection = CurrentNonFileFields(artifactTypeID, refresh)
			If Not retval Is Nothing Then
				Return CurrentNonFileFields(artifactTypeID, refresh).Names()
			Else
				Return Nothing
			End If
		End Function

		Friend Function IsConnected(ByVal caseID As Int32, ByVal artifactTypeID As Int32) As Boolean
			Return Not Me.GetCaseFields(caseID, artifactTypeID, True) Is Nothing
		End Function

		Public Function GetSelectedIdentifier(ByVal selectedField As DocumentField) As String
			Try
				Return CurrentFields(Me.ArtifactTypeID).Item(selectedField.FieldName).FieldName
			Catch ex As System.Exception
				Return String.Empty
			End Try
		End Function

		Friend Function ReadyToLoad(ByVal unselectedFieldNames As String()) As Boolean
			Dim identifierFieldName As String
			Dim unselectedFieldName As String
			Dim unselectedIDFieldNames As New System.Text.StringBuilder
			For Each identifierFieldName In CurrentFields(Me.ArtifactTypeID).IdentifierFieldNames()
				For Each unselectedFieldName In unselectedFieldNames
					If identifierFieldName.ToLower & " [identifier]" = unselectedFieldName Then
						unselectedIDFieldNames.AppendFormat(unselectedFieldName & ChrW(13))
					End If
				Next
			Next
			If unselectedIDFieldNames.Length = 0 Then
				Return True
			Else
				MsgBox("The following identifier fields have not been mapped: " & ChrW(13) & unselectedIDFieldNames.ToString & _
				 "Do you wish to continue?", MsgBoxStyle.Critical, "Warning!")
				Return False
			End If
		End Function

		Friend Function ReadyToLoad(ByVal loadFile As WinEDDS.LoadFile, ByVal forPreview As Boolean) As Boolean
			Dim isIdentifierMapped As Boolean = False
			For Each fieldMapItem As LoadFileFieldMap.LoadFileFieldMapItem In loadFile.FieldMap
				If Not fieldMapItem.DocumentField Is Nothing AndAlso fieldMapItem.DocumentField.FieldID = loadFile.IdentityFieldId AndAlso fieldMapItem.NativeFileColumnIndex <> -1 Then
					isIdentifierMapped = True
				End If
			Next
			Dim fieldName As String = Me.CurrentFields(ArtifactTypeID, True).Item(loadFile.IdentityFieldId).FieldName
			If Not forPreview AndAlso Me.IdentifierFieldIsMappedButNotKey(loadFile.FieldMap, loadFile.IdentityFieldId) Then
				MsgBox("The field marked [identifier] cannot be part of a field map when it's not the Overlay Identifier field", MsgBoxStyle.Critical)
				Return False
			End If
			If Not isIdentifierMapped Then
				MsgBox("The key field [" & fieldName & "] is unmapped.  Please map it to continue", MsgBoxStyle.Critical)
				Return isIdentifierMapped
			End If
			If Not forPreview AndAlso Not New kCura.WinEDDS.Service.FieldQuery(Credential, _cookieContainer).IsFieldIndexed(Me.SelectedCaseInfo.ArtifactID, loadFile.IdentityFieldId) Then
				Return MsgBox("There is no SQL index on the selected Overlay Identifier field.  " & vbNewLine & "Performing a load on an un-indexed SQL field will be drastically slower, " & vbNewLine & "and may negatively impact Relativity performance for all users." & vbNewLine & "Contact your SQL Administrator to have an index applied to the selected Overlay Identifier field.", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok
			Else
				Return True
			End If
		End Function

		Private Function IdentifierFieldIsMappedButNotKey(ByVal fieldMap As WinEDDS.LoadFileFieldMap, ByVal keyFieldID As Int32) As Boolean
			Dim idField As DocumentField = Nothing
			For Each item As LoadFileFieldMap.LoadFileFieldMapItem In fieldMap
				If Not item.DocumentField Is Nothing AndAlso Not item.NativeFileColumnIndex = -1 And item.DocumentField.FieldCategory = Relativity.FieldCategory.Identifier Then
					idField = item.DocumentField
					Exit For
				End If
			Next
			If Not idField Is Nothing AndAlso idField.FieldID <> keyFieldID Then
				Return True
			Else
				Return False
			End If
		End Function

		Friend Function ReadyToLoad(ByVal imageArgs As WinEDDS.ImageLoadFile, ByVal forPreview As Boolean) As Boolean
			Dim id As Int32
			If imageArgs.ProductionArtifactID > 0 Then
				id = imageArgs.BeginBatesFieldArtifactID
			Else
				id = imageArgs.IdentityFieldId
			End If
			If Not forPreview AndAlso Not New kCura.WinEDDS.Service.FieldQuery(Credential, _cookieContainer).IsFieldIndexed(Me.SelectedCaseInfo.ArtifactID, id) Then
				Return MsgBox("There is no SQL index on the selected Overlay Identifier field.  " & vbNewLine & "Performing a load on an un-indexed SQL field will be drastically slower, " & vbNewLine & "and may negatively impact Relativity performance for all users." & vbNewLine & "Contact your SQL Administrator to have an index applied to the selected Overlay Identifier field.", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok
			Else
				Return True
			End If
		End Function
#End Region

#Region "Folder Management"
		Public Function CreateNewFolder(ByVal parentFolderID As Int32) As Int32
			Dim name As String = InputBox("Enter Folder Name", "Relativity Review")
			If name <> String.Empty Then
				Try
					Dim folderManager As New kCura.WinEDDS.Service.FolderManager(Me.Credential, _cookieContainer)
					Dim folderID As Int32 = folderManager.Create(Me.SelectedCaseInfo.ArtifactID, parentFolderID, name)
					RaiseEvent OnEvent(New NewFolderEvent(parentFolderID, folderID, name))
				Catch ex As System.Exception
					If ex.Message.IndexOf("Need To Re Login") <> -1 Then
						NewLogin(False)
					Else
						Throw
					End If
				End Try
			End If
		End Function

		Public Function GetCaseRootFolderForErrorState(ByVal caseID As Int32) As System.Data.DataSet
			Dim dt As New System.Data.DataTable
			dt.Columns.Add("ArtifactID")
			dt.Columns.Add("ParentArtifactID")
			dt.Columns.Add("Name")
			Dim row As System.Data.DataRow = dt.NewRow
			row("ArtifactID") = Me.SelectedCaseInfo.RootFolderID
			row("ParentArtifactID") = System.DBNull.Value
			row("Name") = Me.SelectedCaseInfo.Name
			dt.Rows.Add(row)
			Dim retval As New System.Data.DataSet
			retval.Tables.Add(dt)
			Return retval
		End Function


		Public Function GetCaseFolders(ByVal caseID As Int32) As System.Data.DataSet
			Try
				Dim folderManager As New kCura.WinEDDS.Service.FolderManager(Credential, _cookieContainer)
				Dim retval As System.Data.DataSet = folderManager.RetrieveIntitialChunk(caseID)
				Dim dt As System.Data.DataTable = retval.Tables(0)
				Dim addOn As System.Data.DataTable
				Dim lastId As Int32
				Do
					If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then lastId = CType(dt.Rows(dt.Rows.Count - 1)("ArtifactID"), Int32)
					addOn = folderManager.RetrieveNextChunk(caseID, lastId).Tables(0)
					For Each row As System.Data.DataRow In addOn.Rows
						dt.Rows.Add(row.ItemArray)
					Next
				Loop Until addOn Is Nothing OrElse addOn.Rows.Count = 0
				Return retval
			Catch ex As System.Exception
				If ex.Message.IndexOf("Need To Re Login") <> -1 Then
					NewLogin(False)
				Else
					Throw
				End If
			End Try
			Return Nothing
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
			Try
				Dim csMgr As New kCura.WinEDDS.Service.CaseManager(Credential, _cookieContainer)
				_documentRepositoryList = csMgr.GetAllDocumentFolderPaths()
				Return csMgr.RetrieveAll()
			Catch ex As System.Exception
				If ex.Message.IndexOf("Need To Re Login") <> -1 Then
					NewLogin(False)
				Else
					Throw
				End If
			End Try
			Return Nothing
		End Function

		Public Sub SelectCaseFolder(ByVal folderInfo As FolderInfo)
			_selectedCaseFolderID = folderInfo.ArtifactID
			_selectedCaseFolderPath = folderInfo.Path
			RaiseEvent OnEvent(New AppEvent(AppEvent.AppEventType.CaseFolderSelected))
		End Sub

		Public Sub OpenCase()
			Try
				Dim caseInfo As Relativity.CaseInfo = Me.GetCase
				If Not caseInfo Is Nothing Then
					_selectedCaseInfo = caseInfo
					Me.RefreshSelectedCaseInfo()
					RaiseEvent OnEvent(New LoadCaseEvent(caseInfo))
				End If
			Catch MrSoapy As SoapException
				Select Case MrSoapy.Detail("ExceptionType").InnerText
					Case "Relativity.Core.Exception.WorkspaceVersion"
						Dim x As New ErrorDialog With {.Text = "Relativity Desktop Client Error"}
						x.Initialize(MrSoapy)
						If x.ShowDialog() <> DialogResult.OK Then
							Environment.Exit(1)
						End If
					Case "kCura.EDDS.WebAPI.ServiceBase.NeedToReLoginException"
						NewLogin(True)
					Case Else
						Me.ChangeWebServiceURL()
				End Select
			Catch ex As System.Exception
				Throw
			End Try
		End Sub

		Public Function GetCase() As Relativity.CaseInfo
			Dim frm As New CaseSelectForm
			frm.MultiSelect = False
			frm.ShowDialog()
			If frm.SelectedCaseInfo.Count = 0 Then
				Return Nothing
			Else
				Return frm.SelectedCaseInfo.Item(0)
			End If
		End Function

		Public Function GetConnectionStatus() As String
			Dim x As New kCura.WinEDDS.FileUploader(Me.Credential, Me.SelectedCaseInfo.ArtifactID, Me.SelectedCaseInfo.DocumentPath, Me.CookieContainer)
			Return x.UploaderType.ToString
		End Function
#End Region

#Region "Security Methods"
		Public Function IsAssociatedSearchProviderAccessible(ByVal caseContextArtifactID As Int32, ByVal searchArtifactID As Int32) As Boolean
			Dim searchManager As New kCura.WinEDDS.Service.SearchManager(Me.Credential, Me.CookieContainer)
			Dim values As Boolean() = searchManager.IsAssociatedSearchProviderAccessible(caseContextArtifactID, searchArtifactID)
			Dim isSearchProviderValid As Boolean = values(0) And values(1)
			Dim message As New System.Text.StringBuilder
			If Not values(0) Then message.Append("You do not have the rights to view the search provider associated with this selected search" & vbNewLine)
			If Not values(1) AndAlso values(0) Then message.Append("The search provider associated with this selected search is not active" & vbNewLine)
			If Not isSearchProviderValid Then MsgBox(message.ToString & "Search Export Halted", MsgBoxStyle.Exclamation, "Search Provider Error")
			Return isSearchProviderValid
		End Function
#End Region

#Region "Utility"
		Public Function GetColumnHeadersFromLoadFile(ByVal loadfile As kCura.WinEDDS.LoadFile, ByVal firstLineContainsColumnHeaders As Boolean) As String()
			loadfile.CookieContainer = Me.CookieContainer
			Dim parser As New kCura.WinEDDS.BulkLoadFileImporter(loadfile, Nothing, _timeZoneOffset, False, Nothing, False)
			Return parser.GetColumnNames(loadfile)
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
		Private Sub ManageRowWideError(ByVal dt As System.Data.DataTable, ByVal err As System.Exception, ByRef rowcount As Int32)
			Dim errorRow As System.Data.DataRow = dt.NewRow
			rowcount += 1
			For Each column As System.Data.DataColumn In dt.Columns
				If column.ColumnName = "Record Number" Then
					errorRow(column.ColumnName) = rowcount.ToString
				Else
					errorRow(column.ColumnName) = "Row-wide error: " & err.Message
				End If
			Next
			dt.Rows.Add(errorRow)
		End Sub

		Public Function BuildLoadFileDataSource(ByVal al As ArrayList) As DataTable
			Try
				Me.GetCaseFields(_selectedCaseInfo.ArtifactID, Me.ArtifactTypeID, True)
				'Dim previewer As New kCura.WinEDDS.LoadFilePreviewer(loadFile, _timeZoneOffset, errorsOnly)
				'Dim al As ArrayList = DirectCast(previewer.ReadFile(loadFile.FilePath), ArrayList)
				'previewer.Close()
				Dim item As Object
				Dim field As Api.ArtifactField
				Dim fields As Api.ArtifactField()
				Dim firstTimeThrough As Boolean = True
				Dim row As ArrayList
				Dim dt As New DataTable
				Dim errorQueue As New System.Collections.ArrayList
				Dim i As Int32 = 0
				If al.Count = 0 Then
					dt.Columns.Add("  ")
					dt.Rows.Add(New String() {"No errors"})
				Else
					For Each item In al
						If Not item Is Nothing Then
							If TypeOf item Is System.Exception Then
								If dt.Columns.Count = 0 Then
									errorQueue.Add(item)
								Else
									Me.ManageRowWideError(dt, DirectCast(item, System.Exception), i)
								End If
							Else
								row = New ArrayList
								fields = DirectCast(item, Api.ArtifactField())
								If firstTimeThrough Then
									dt.Columns.Add("Record Number")
									For Each field In fields
										dt.Columns.Add(field.DisplayName)
										If field.DisplayName.ToLower.Contains("extracted text") Then
											'dt.Columns.Add("Extracted Text Encoding")
										End If
									Next
									firstTimeThrough = False
									For Each err As System.Exception In errorQueue
										Me.ManageRowWideError(dt, err, i)
									Next
									errorQueue.Clear()
								End If
								AddRow(dt, row, fields, i)
							End If
						End If
					Next
					If errorQueue.Count > 0 Then
						dt.Columns.Add(" ")
						For Each err As System.Exception In errorQueue
							Me.ManageRowWideError(dt, err, i)
						Next
					End If
				End If
				Return dt
			Catch ex As System.Exception
				kCura.EDDS.WinForm.Utility.ThrowExceptionToGUI(ex)
			End Try
			Return Nothing
		End Function

		Private Sub AddRow(ByVal dt As DataTable, ByVal row As System.Collections.ArrayList, ByVal fields As Api.ArtifactField(), ByRef counter As Int32)
			Try
				counter += 1
				Dim field As Api.ArtifactField
				row.Add(counter.ToString())
				For Each field In fields
					row.Add(field.ValueAsString)
					If field.DisplayName.ToLower.Contains("extracted text") Then
						'row.Add("...Encoding will go here...")
					End If
				Next
				dt.Rows.Add(row.ToArray)
			Catch x As System.Exception
				Throw
			End Try
		End Sub

		'Worker function for Previewing choice and folder counts
		Public Function BuildFoldersAndCodesDataSource(ByVal al As ArrayList, ByVal previewCodeCount As System.Collections.Specialized.HybridDictionary) As DataTable
			Try
				Dim dt As New DataTable
				Dim folderCount As Int32 = GetFolderCount(al)
				Dim codeFieldColumnIndexes As ArrayList = GetCodeFieldColumnIndexes(DirectCast(al(0), System.Array))

				'setup columns
				dt.Columns.Add("Field Name")
				dt.Columns.Add("Count")

				'setup folder row
				Dim folderRow As New System.Collections.ArrayList
				folderRow.Add("Folders")
				If folderCount <> -1 Then
					folderRow.Add(folderCount.ToString)
				Else
					folderRow.Add("0")
				End If
				dt.Rows.Add(folderRow.ToArray)

				'insert spacer
				Dim blankRow As New System.Collections.ArrayList
				blankRow.Add("")
				blankRow.Add("")
				dt.Rows.Add(blankRow.ToArray)

				'setup choice rows
				If codeFieldColumnIndexes.Count = 0 Then
					dt.Columns.Add("     ")
					dt.Rows.Add(New String() {"No choice fields have been mapped"})
				Else
					For Each key As String In previewCodeCount.Keys
						Dim row As New System.Collections.ArrayList
						row.Add(key.Split("_".ToCharArray, 2)(1))
						Dim currentFieldHashTable As System.Collections.Specialized.HybridDictionary = DirectCast(previewCodeCount(key), System.Collections.Specialized.HybridDictionary)
						row.Add(currentFieldHashTable.Count)
						dt.Rows.Add(row.ToArray)
						currentFieldHashTable.Clear()
					Next
				End If

				Return dt
			Catch ex As Exception
				kCura.EDDS.WinForm.Utility.ThrowExceptionToGUI(ex)
			End Try
			Return Nothing
		End Function

		Public Function GetFolderCount(ByVal al As ArrayList) As Int32
			_totalFolders.Clear()
			Dim fieldValue As String
			Dim item As Object
			Dim fields As Api.ArtifactField()
			Dim folderColumnIndex As Int32 = GetFolderColumnIndex(DirectCast(al(0), System.Array))

			If folderColumnIndex <> -1 Then
				For Each item In al
					If Not item Is Nothing Then
						fields = DirectCast(item, Api.ArtifactField())
						If folderColumnIndex <> -1 Then
							fieldValue = fields(folderColumnIndex).ValueAsString
							AddFoldersToTotalFolders(fieldValue)
						End If
					End If
				Next
				Return _totalFolders.Count
			End If
			Return -1
		End Function

		Private Function GetFolderColumnIndex(ByVal firstRow As Array) As Int32
			Dim folderColumnIndex As Int32 = -1
			Dim currentIndex As Int32 = 0
			For Each field As Api.ArtifactField In firstRow
				If field.ArtifactID = -2 And field.DisplayName = "Parent Folder Identifier" Then folderColumnIndex = currentIndex
				currentIndex += 1
			Next
			Return folderColumnIndex
		End Function

		Private Function GetCodeFieldColumnIndexes(ByVal firstRow As Array) As ArrayList
			Dim codeFieldColumnIndexes As New ArrayList
			Dim currentIndex As Int32 = 0
			For Each field As Api.ArtifactField In firstRow
				If field.Type = Relativity.FieldTypeHelper.FieldType.Code OrElse field.Type = Relativity.FieldTypeHelper.FieldType.MultiCode Then
					codeFieldColumnIndexes.Add(currentIndex)
				End If
				currentIndex += 1
			Next
			Return codeFieldColumnIndexes
		End Function

#Region "go away"
		''Worker function for Previewing choice and folder counts
		'Public Function BuildFoldersAndCodesDataSource2(ByVal al As ArrayList, ByVal previewCodeCount As System.Collections.Specialized.HybridDictionary) As DataTable
		'	_totalFolders.Clear()
		'	Try
		'		Dim item As Object
		'		Dim fields As Api.ArtifactField()
		'		Dim codeFieldColumnIndexes As New ArrayList
		'		Dim folderColumnIndex As Int32 = -1
		'		Dim dt As New DataTable
		'		'get the choice field column indicies
		'		If al.Count > 0 Then
		'			Dim firstRow As System.Array = DirectCast(al(0), System.Array)
		'			Dim currentIndex As Int32 = 0
		'			For Each field As Api.ArtifactField In firstRow
		'				If field.Type = Relativity.FieldTypeHelper.FieldType.Code OrElse field.Type = Relativity.FieldTypeHelper.FieldType.MultiCode Then
		'					codeFieldColumnIndexes.Add(currentIndex)
		'				End If
		'				If field.ArtifactID = -2 And field.DisplayName = "Parent Folder Identifier" Then folderColumnIndex = currentIndex
		'				currentIndex += 1
		'			Next
		'		End If
		'		dt.Columns.Add("Field Name")
		'		dt.Columns.Add("Count")
		'		Dim fieldValue As String
		'		For Each item In al
		'			If Not item Is Nothing Then
		'				fields = DirectCast(item, Api.ArtifactField())
		'				If folderColumnIndex <> -1 Then
		'					fieldValue = fields(folderColumnIndex).ValueAsString
		'					AddFoldersToTotalFolders(fieldValue)
		'				End If
		'			End If
		'		Next
		'		Dim folderRow As New System.Collections.ArrayList
		'		folderRow.Add("Folders")
		'		If folderColumnIndex <> -1 Then
		'			folderRow.Add(_totalFolders.Count.ToString)
		'		Else
		'			folderRow.Add("0")
		'		End If
		'		dt.Rows.Add(folderRow.ToArray)
		'		Dim blankRow As New System.Collections.ArrayList
		'		blankRow.Add("")
		'		blankRow.Add("")
		'		dt.Rows.Add(blankRow.ToArray)
		'		If codeFieldColumnIndexes.Count = 0 Then
		'			dt.Columns.Add("     ")
		'			dt.Rows.Add(New String() {"No choice fields have been mapped"})
		'		Else
		'			For Each key As String In previewCodeCount.Keys
		'				Dim row As New System.Collections.ArrayList
		'				row.Add(key.Split("_".ToCharArray, 2)(1))
		'				Dim currentFieldHashTable As System.Collections.Specialized.HybridDictionary = DirectCast(previewCodeCount(key), System.Collections.Specialized.HybridDictionary)
		'				row.Add(currentFieldHashTable.Count)
		'				dt.Rows.Add(row.ToArray)
		'				currentFieldHashTable.Clear()
		'			Next
		'		End If
		'		Return dt
		'	Catch ex As Exception
		'		kCura.EDDS.WinForm.Utility.ThrowExceptionToGUI(ex)
		'	End Try
		'	Return Nothing
		'End Function
#End Region

		Private Sub AddFoldersToTotalFolders(ByVal folderPath As String)
			If folderPath <> String.Empty AndAlso folderPath <> "\" Then
				If folderPath.LastIndexOf("\"c) < 1 Then
					If Not _totalFolders.Contains(folderPath) Then _totalFolders.Add(folderPath, String.Empty)
				Else
					If Not _totalFolders.Contains(folderPath) Then _totalFolders.Add(folderPath, String.Empty)
					AddFoldersToTotalFolders(folderPath.Substring(0, folderPath.LastIndexOf("\"c)))
				End If
			End If
		End Sub

		Private Function EnsureConnection() As Boolean
			If Not Me.SelectedCaseInfo Is Nothing Then
				Dim casefields As String() = Nothing
				Dim [continue] As Boolean = True
				Try
					casefields = Me.GetCaseFields(Me.SelectedCaseInfo.ArtifactID, 10, True)
					Return Not casefields Is Nothing
				Catch ex As System.Exception
					If ex.Message.IndexOf("Need To Re Login") <> -1 Then
						Return False
					Else
						Throw
					End If
				End Try
			Else
				Return True
			End If
		End Function

		Public Sub RefreshCaseFolders()
			If Me.EnsureConnection Then
				RaiseEvent OnEvent(New kCura.WinEDDS.LoadCaseEvent(SelectedCaseInfo))
			End If
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
				sb.Append("If you continue, any existing values in them will be wiped out in any records that are overwritten." & System.Environment.NewLine)
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

		Public Sub SetWorkingDirectory(ByVal filePath As String)
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
		Public Sub NewLoadFile(ByVal destinationArtifactID As Int32, ByVal caseInfo As Relativity.CaseInfo)
			If Not Me.IsConnected(caseInfo.ArtifactID, Me.ArtifactTypeID) Then
				CursorDefault()
				Exit Sub
			End If
			Dim frm As New LoadFileForm
			Dim loadFile As New LoadFile
			frm._application = Me
			loadFile.SelectedCasePath = caseInfo.DocumentPath
			If Me.ArtifactTypeID = Relativity.ArtifactType.Document Then
				loadFile.DestinationFolderID = destinationArtifactID
			Else
				loadFile.DestinationFolderID = caseInfo.RootArtifactID
			End If
			loadFile.SendEmailOnLoadCompletion = Config.SendNotificationOnImportCompletionByDefault
			loadFile.CopyFilesToDocumentRepository = Config.CopyFilesToRepository
			loadFile.CaseInfo = caseInfo
			loadFile.Credentials = Me.Credential
			loadFile.CookieContainer = Me.CookieContainer
			loadFile.OverwriteDestination = "None"
			loadFile.ArtifactTypeID = Me.ArtifactTypeID
			frm.LoadFile = loadFile
			frm.Show()
		End Sub

		Public Sub NewProductionExport(ByVal caseInfo As Relativity.CaseInfo)
			Me.NewSearchExport(caseInfo.RootFolderID, caseInfo, ExportFile.ExportType.Production)
		End Sub

		Public Sub NewSearchExport(ByVal selectedFolderId As Int32, ByVal caseInfo As Relativity.CaseInfo, ByVal typeOfExport As kCura.WinEDDS.ExportFile.ExportType)
			Dim frm As New ExportForm
			Dim exportFile As WinEDDS.ExportFile
			Try
				exportFile = Me.GetNewExportFileSettingsObject(selectedFolderId, caseInfo, typeOfExport, Me.ArtifactTypeID)
				If exportFile.DataTable.Rows.Count = 0 Then
					Dim s As New System.Text.StringBuilder
					s.Append("There are no exportable ")
					Select Case exportFile.TypeOfExport
						Case exportFile.ExportType.Production
							s.Append("productions ")
						Case exportFile.ExportType.ArtifactSearch
							s.Append("saved searches ")
						Case Else
							s.Append("views ")
					End Select
					s.Append("in this case")
					MsgBox(s.ToString, MsgBoxStyle.Critical)
					Exit Sub
				End If
				frm.Application = Me
				frm.ExportFile = exportFile
				frm.Show()
			Catch ex As System.Exception
				If ex.Message.IndexOf("Need To Re Login") <> -1 Then
					NewLogin(False)
					Exit Sub
				Else
					Throw
				End If
			End Try
		End Sub

		Public Function GetNewExportFileSettingsObject(ByVal selectedFolderId As Int32, ByVal caseInfo As Relativity.CaseInfo, ByVal typeOfExport As kCura.WinEDDS.ExportFile.ExportType, ByVal artifactTypeID As Int32) As WinEDDS.ExportFile
			Dim exportFile As New WinEDDS.ExportFile(artifactTypeID)
			Dim searchManager As New kCura.WinEDDS.Service.SearchManager(Me.Credential, _cookieContainer)
			Dim productionManager As New kCura.WinEDDS.Service.ProductionManager(Me.Credential, _cookieContainer)
			exportFile.ArtifactID = selectedFolderId
			exportFile.CaseInfo = caseInfo
			exportFile.Credential = Me.Credential
			exportFile.TypeOfExport = typeOfExport
			Select Case typeOfExport
				Case exportFile.ExportType.Production
					exportFile.DataTable = productionManager.RetrieveProducedByContextArtifactID(caseInfo.ArtifactID).Tables(0)
				Case Else
					exportFile.DataTable = Me.GetSearchExportDataSource(searchManager, caseInfo.ArtifactID, typeOfExport = exportFile.ExportType.ArtifactSearch, exportFile.ArtifactTypeID)
			End Select
			Dim ids As New System.Collections.ArrayList
			For Each row As System.Data.DataRow In exportFile.DataTable.Rows
				ids.Add(row("ArtifactID"))
			Next
			For Each field As DocumentField In Me.CurrentFields(exportFile.ArtifactTypeID, True)
				If field.FieldTypeID = Relativity.FieldTypeHelper.FieldType.File Then
					exportFile.FileField = field
					Exit For
				End If
			Next
			If ids.Count = 0 Then
				exportFile.ArtifactAvfLookup = New System.Collections.Specialized.HybridDictionary
				exportFile.AllExportableFields = New WinEDDS.ViewFieldInfo() {}
			Else
				exportFile.ArtifactAvfLookup = searchManager.RetrieveDefaultViewFieldsForIdList(caseInfo.ArtifactID, exportFile.ArtifactTypeID, DirectCast(ids.ToArray(GetType(Int32)), Int32()), typeOfExport = exportFile.ExportType.Production)
				exportFile.AllExportableFields = searchManager.RetrieveAllExportableViewFields(caseInfo.ArtifactID, exportFile.ArtifactTypeID)
			End If
			Return exportFile
		End Function

		Public Sub NewApplicationFile(ByVal caseInfo As Relativity.CaseInfo)
			CursorWait()
			If Not Me.IsConnected(caseInfo.ArtifactID, Me.ArtifactTypeID) Then
				CursorDefault()
				Exit Sub
			End If
			Dim applicationForm As New ApplicationFileForm
			applicationForm.Application = Me
			Dim list As New Generic.List(Of Relativity.CaseInfo)
			list.Add(caseInfo)
			applicationForm.CaseInfos = list
			applicationForm.CookieContainer = Me.CookieContainer
			applicationForm.Credentials = Me.Credential
			applicationForm.Show()
			CursorDefault()
		End Sub

		Friend Function GetSearchExportDataSource(ByVal searchManager As kCura.WinEDDS.Service.SearchManager, ByVal caseArtifactID As Int32, ByVal isArtifactSearch As Boolean, ByVal artifactType As Int32) As System.Data.DataTable
			Dim searchExportDataSet As System.Data.DataSet
			If isArtifactSearch Then
				searchExportDataSet = searchManager.RetrieveViewsByContextArtifactID(caseArtifactID, artifactType, True)
			Else
				searchExportDataSet = searchManager.RetrieveViewsByContextArtifactID(caseArtifactID, artifactType, False)
			End If
			Return searchExportDataSet.Tables(0)
		End Function

		Public Sub NewOutlookImport(ByVal destinationArtifactID As Int32, ByVal caseInfo As Relativity.CaseInfo)
			Dim importerAssembly As System.Reflection.Assembly
			importerAssembly = System.Reflection.Assembly.LoadFrom(kCura.WinEDDS.Config.OutlookImporterLocation)
			Dim hostImporterGateway As kCura.EDDS.Import.ImporterGatewayBase
			hostImporterGateway = CType(importerAssembly.CreateInstance("kCura.EDDS.Import.Outlook.ImporterGateway"), kCura.EDDS.Import.ImporterGatewayBase)
			Dim frm As Form = hostImporterGateway.GetSettingsForm(New Import.CaseInfo(caseInfo.ArtifactID, caseInfo.RootArtifactID), destinationArtifactID, New WinEDDSGateway)
			frm.Show()
		End Sub

		Public Sub NewSQLImport(ByVal destinationArtifactID As Int32, ByVal caseInfo As Relativity.CaseInfo)
			Dim frm As New SQLImportForm
			Dim sQLImportSettings As New SQLImportSettings
			sQLImportSettings.DestinationFolderID = destinationArtifactID
			sQLImportSettings.CaseInfo = caseInfo
			frm.SQLImportSettings = sQLImportSettings
			frm.Show()
		End Sub

		Public Sub NewImageFile(ByVal destinationArtifactID As Int32, ByVal caseinfo As Relativity.CaseInfo)
			CursorWait()
			If Not Me.IsConnected(caseinfo.ArtifactID, Me.ArtifactTypeID) Then
				CursorDefault()
				Exit Sub
			End If
			Dim frm As New ImageLoad
			Try
				Dim imageFile As New ImageLoadFile
				imageFile.Credential = Me.Credential
				imageFile.CaseInfo = caseinfo
				imageFile.SelectedCasePath = caseinfo.DocumentPath
				imageFile.DestinationFolderID = destinationArtifactID
				imageFile.CookieContainer = Me.CookieContainer
				imageFile.ForProduction = False
				imageFile.FullTextEncoding = Nothing
				imageFile.CopyFilesToDocumentRepository = Config.CopyFilesToRepository
				imageFile.SendEmailOnLoadCompletion = Config.SendNotificationOnImportCompletionByDefault
				frm.ImageLoadFile = imageFile
			Catch ex As System.Exception
				Throw
			Finally
				CursorDefault()
			End Try
			frm.Show()
			CursorDefault()
		End Sub

		Public Sub NewProductionFile(ByVal destinationArtifactID As Int32, ByVal caseinfo As Relativity.CaseInfo)
			CursorWait()
			If Not Me.IsConnected(caseinfo.ArtifactID, ArtifactTypeID) Then
				CursorDefault()
				Exit Sub
			End If
			Dim frm As New ImageLoad
			Try
				Dim imageFile As New ImageLoadFile
				imageFile.Credential = Me.Credential
				imageFile.CaseInfo = caseinfo
				imageFile.DestinationFolderID = destinationArtifactID
				imageFile.ForProduction = True
				imageFile.CookieContainer = Me.CookieContainer
				imageFile.SelectedCasePath = caseinfo.DocumentPath
				imageFile.FullTextEncoding = System.Text.Encoding.Default
				imageFile.CopyFilesToDocumentRepository = Config.CopyFilesToRepository
				Dim productionManager As New kCura.WinEDDS.Service.ProductionManager(Me.Credential, _cookieContainer)
				imageFile.ProductionTable = productionManager.RetrieveImportEligibleByContextArtifactID(caseinfo.ArtifactID).Tables(0)
				imageFile.SendEmailOnLoadCompletion = Config.SendNotificationOnImportCompletionByDefault
				frm.ImageLoadFile = imageFile
			Catch ex As System.Exception
				Throw
			Finally
				CursorDefault()
			End Try
			frm.Show()
			CursorDefault()
		End Sub

		Public Sub NewDirectoryImport(ByVal destinationArtifactID As Int32, ByVal caseInfo As Relativity.CaseInfo)
			CursorWait()
			If Not Me.IsConnected(caseInfo.ArtifactID, ArtifactTypeID) Then
				CursorDefault()
				Exit Sub
			End If
			Dim frm As New ImportFileSystemForm
			Dim importFileDirectorySettings As New ImportFileDirectorySettings
			importFileDirectorySettings.DestinationFolderID = destinationArtifactID
			importFileDirectorySettings.CaseInfo = caseInfo
			frm.ImportFileDirectorySettings = importFileDirectorySettings
			frm.Show()
			CursorDefault()
		End Sub

		Public Sub NewEnronImport(ByVal destinationArtifactID As Int32, ByVal caseInfo As Relativity.CaseInfo)
			CursorWait()
			If Not Me.IsConnected(caseInfo.ArtifactID, ArtifactTypeID) Then
				CursorDefault()
				Exit Sub
			End If
			Dim frm As New ImportFileSystemForm
			Dim importFileDirectorySettings As New ImportFileDirectorySettings
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

		Public Sub NewLogin(Optional ByVal openCaseSelector As Boolean = True)
			CursorWait()
			If Not _loginForm Is Nothing Then
				If Not _loginForm.IsDisposed Then
					_loginForm.Close()
				End If
			End If
			_loginForm = New LoginForm
			_loginForm.OpenCaseSelector = openCaseSelector
			_loginForm.Show()
			CursorDefault()
		End Sub
#End Region

#Region "Process Management"

		Public Function QueryConnectivity() As Guid
			CursorWait()
			If Not Me.IsConnected(Me.SelectedCaseInfo.ArtifactID, 10) Then
				CursorDefault()
				Exit Function
			End If
			Dim proc As New kCura.WinEDDS.ConnectionDetailsProcess(Me.Credential, Me.CookieContainer, Me.SelectedCaseInfo)
			Dim form As New TextDisplayForm
			form.ProcessObserver = proc.ProcessObserver
			form.Text = "Connectivity Tests"
			form.Show()
			_processPool.StartProcess(proc)
			CursorDefault()
		End Function

		Public Function PreviewLoadFile(ByVal loadFileToPreview As LoadFile, ByVal errorsOnly As Boolean, ByVal formType As Int32) As Guid
			CursorWait()
			If Not Me.IsConnected(loadFileToPreview.CaseInfo.ArtifactID, ArtifactTypeID) Then
				CursorDefault()
				Exit Function
			End If
			If Not CheckFieldMap(loadFileToPreview) Then
				CursorDefault()
				Exit Function
			End If
			Dim frm As New kCura.Windows.Process.ProgressForm
			Dim previewer As New kCura.WinEDDS.PreviewLoadFileProcess(formType)
			loadFileToPreview.PreviewCodeCount.Clear()
			Dim previewform As New LoadFilePreviewForm(formType, loadFileToPreview.MultiRecordDelimiter, loadFileToPreview.PreviewCodeCount)
			Dim thrower As New ValueThrower
			previewer.Thrower = thrower
			previewer.TimeZoneOffset = _timeZoneOffset
			previewer.ErrorsOnly = errorsOnly
			previewform.Thrower = previewer.Thrower
			previewer.LoadFile = loadFileToPreview
			SetWorkingDirectory(loadFileToPreview.FilePath)
			frm.ProcessObserver = previewer.ProcessObserver
			frm.ProcessController = previewer.ProcessController
			If errorsOnly Then
				frm.Text = "Preview Load File Errors Progress ..."
			Else
				frm.Text = "Preview Load File Progress ..."
			End If
			previewform.Show()
			frm.Show()
			_processPool.StartProcess(previewer)
			CursorDefault()
		End Function

		Public Function StartProcess(ByVal process As kCura.Windows.Process.ProcessBase) As System.Guid
			Return _processPool.StartProcess(process)
		End Function
		Public Function ImportDirectory(ByVal importFileDirectorySettings As ImportFileDirectorySettings) As Guid
			CursorWait()
			If Not Me.IsConnected(importFileDirectorySettings.CaseInfo.ArtifactID, ArtifactTypeID) Then
				CursorDefault()
				Exit Function
			End If
			Dim frm As New kCura.Windows.Process.ProgressForm
			Dim importer As New kCura.WinEDDS.ImportFileDirectoryProcess(Credential, CookieContainer)
			'Dim importer As New kCura.WinEDDS.ImportFileDirectoryProcess(Credential, CookieContainer, Me.Identity)
			importer.ImportFileDirectorySettings = importFileDirectorySettings
			frm.ProcessObserver = importer.ProcessObserver
			frm.Text = "Import File Directory Progress ..."
			frm.Show()
			CursorDefault()
			Return _processPool.StartProcess(importer)
		End Function

		Public Function ImportGeneric(ByVal settings As Object) As Guid
			CursorWait()
			If Not Me.IsConnected(_selectedCaseInfo.ArtifactID, ArtifactTypeID) Then
				CursorDefault()
				Exit Function
			End If
			Dim frm As New kCura.Windows.Process.ProgressForm
			Dim importer As New kCura.WinEDDS.GenericImportProcess(New WinEDDSGateway)
			importer.Settings = settings
			frm.ProcessObserver = importer.ProcessObserver
			frm.Text = "Import Generic Progress ..."
			frm.Show()
			CursorDefault()
			Return _processPool.StartProcess(importer)
		End Function

		Public Function ImportSQL(ByVal sqlimportsettings As SQLImportSettings) As Guid
			CursorWait()
			If Not Me.IsConnected(_selectedCaseInfo.ArtifactID, ArtifactTypeID) Then
				CursorDefault()
				Exit Function
			End If
			Dim frm As New kCura.Windows.Process.ProgressForm
			Dim imporProcess As New kCura.WinEDDS.SQLImportProcess
			imporProcess.SQLImportSettings = sqlimportsettings
			frm.ProcessObserver = imporProcess.ProcessObserver
			frm.ProcessController = imporProcess.ProcessController
			frm.Text = "Import SQL Progress ..."
			frm.Show()
			CursorDefault()
			Return _processPool.StartProcess(imporProcess)
		End Function

		Public Function ImportLoadFile(ByVal loadFile As LoadFile) As Guid
			CursorWait()
			'Dim folderManager As New kCura.WinEDDS.Service.FolderManager(Credential, _cookieContainer, _identity)
			If Not Me.IsConnected(_selectedCaseInfo.ArtifactID, ArtifactTypeID) Then
				CursorDefault()
				Exit Function
			End If
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
					frm.Text = "Import Load File Progress ..."
					frm.ErrorFileExtension = System.IO.Path.GetExtension(loadFile.FilePath).TrimStart("."c).ToUpper
					If frm.ErrorFileExtension Is Nothing Then frm.ErrorFileExtension = "TXT"
					If frm.ErrorFileExtension = String.Empty Then frm.ErrorFileExtension = "TXT"
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

		Public Sub PreviewImageFile(ByVal loadfile As ImageLoadFile)
			CursorWait()
			'Dim folderManager As New kCura.WinEDDS.Service.FolderManager(Credential, _cookieContainer, _identity)
			If Not Me.IsConnected(_selectedCaseInfo.ArtifactID, ArtifactTypeID) Then
				CursorDefault()
				Exit Sub
			End If
			Dim frm As New kCura.Windows.Process.ProgressForm
			Dim previewer As New kCura.WinEDDS.PreviewImageFileProcess
			previewer.TimeZoneOffset = _timeZoneOffset
			previewer.LoadFile = loadfile
			SetWorkingDirectory(loadfile.FileName)
			frm.ProcessObserver = previewer.ProcessObserver
			frm.ProcessController = previewer.ProcessController
			frm.Text = "Preview Image File Progress ..."
			frm.Show()
			_processPool.StartProcess(previewer)
			CursorDefault()
		End Sub

		Public Sub ImportImageFile(ByVal ImageLoadFile As ImageLoadFile)
			CursorWait()
			If Not Me.IsConnected(_selectedCaseInfo.ArtifactID, ArtifactTypeID) Then
				CursorDefault()
				Exit Sub
			End If
			Dim frm As New kCura.Windows.Process.ProgressForm
			Dim importer As New kCura.WinEDDS.ImportImageFileProcess
			ImageLoadFile.CookieContainer = Me.CookieContainer
			importer.ImageLoadFile = ImageLoadFile
			SetWorkingDirectory(ImageLoadFile.FileName)
			frm.ProcessObserver = importer.ProcessObserver
			frm.ProcessController = importer.ProcessController
			frm.Text = "Import Image File Progress ..."
			frm.ErrorFileExtension = "OPT"
			frm.Show()
			frm.ProcessID = _processPool.StartProcess(importer)
			CursorDefault()
		End Sub

		'Public Function StartProduction(ByVal exportFile As ExportFile) As Guid
		'	CursorWait()
		'	If Not Me.IsConnected(_selectedCaseInfo.ArtifactID) Then
		'		CursorDefault()
		'		Exit Function
		'	End If
		'	Dim frm As New kCura.Windows.Process.ProgressForm
		'	Dim exporter As New kCura.WinEDDS.ExportProductionProcess

		'	exporter.ExportFile = exportFile
		'	frm.ProcessObserver = exporter.ProcessObserver
		'	frm.ProcessController = exporter.ProcessController
		'	frm.Text = "Export Progress..."
		'	frm.Show()
		'	CursorDefault()
		'	Return _processPool.StartProcess(exporter)
		'End Function

		Public Function StartSearch(ByVal exportFile As ExportFile) As Guid
			CursorWait()
			If Not Me.IsConnected(_selectedCaseInfo.ArtifactID, ArtifactTypeID) Then
				CursorDefault()
				Exit Function
			End If
			Dim frm As New kCura.Windows.Process.ProgressForm
			Dim exporter As New kCura.WinEDDS.ExportSearchProcess

			exporter.ExportFile = exportFile
			frm.ProcessObserver = exporter.ProcessObserver
			frm.ProcessController = exporter.ProcessController
			frm.Text = "Export Progress..."
			Select Case exportFile.TypeOfExport
				Case exportFile.ExportType.AncestorSearch
					frm.Text = "Export Folders and Subfolders Progress ..."
				Case exportFile.ExportType.ArtifactSearch
					frm.Text = "Export Saved Search Progress ..."
				Case exportFile.ExportType.ParentSearch
					frm.Text = "Export Folder Progress ..."
				Case exportFile.ExportType.Production
					frm.Text = "Export Production Set Progress ..."
			End Select
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

		Public Sub ImportApplicationFile(ByVal caseInfos As Generic.IEnumerable(Of Relativity.CaseInfo), ByVal application As Xml.XmlDocument)
			CursorWait()
			If Not Me.IsConnected(Me.SelectedCaseInfo.ArtifactID, 10) Then
				CursorDefault()
				Exit Sub
			End If
			Dim applicationDeploymentProcess As New kCura.WinEDDS.ApplicationDeploymentProcess(application, Me.Credential, Me.CookieContainer, caseInfos)
			Dim form As New TextDisplayForm
			form.ProcessObserver = applicationDeploymentProcess.ProcessObserver
			form.Text = "Application Deployment System"
			form.Show()
			_processPool.StartProcess(applicationDeploymentProcess)
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
				Try
					sw.Close()
				Catch
				End Try
			End Try
		End Sub
		Public Function CleanLoadFile(ByVal doc As System.Xml.XmlDocument) As String
			For Each node As System.Xml.XmlNode In doc.ChildNodes(0).ChildNodes(0)
				If node.Name = "a1:DocumentField" Then
					Dim nodesToRemove As New System.Collections.ArrayList
					For Each dfNode As System.Xml.XmlNode In node.ChildNodes
						If dfNode.Name = "_codeTypeID" Then nodesToRemove.Add(dfNode)
						If dfNode.Name = "_fieldLength" Then nodesToRemove.Add(dfNode)
					Next
					For Each nodeToRemove As System.Xml.XmlNode In nodesToRemove
						node.RemoveChild(nodeToRemove)
					Next
				End If
			Next
			Return doc.OuterXml
		End Function

		Public Function ReadLoadFile(ByVal loadFile As LoadFile, ByVal path As String, ByVal isSilent As Boolean) As LoadFile
			kCura.WinEDDS.Service.Settings.SendEmailOnLoadCompletion = Config.SendNotificationOnImportCompletionByDefault
			If Not Me.EnsureConnection Then Return Nothing
			Dim sr As New System.IO.StreamReader(path)
			Dim doc As String = sr.ReadToEnd
			sr.Close()
			Dim xmlDoc As New System.Xml.XmlDocument
			xmlDoc.LoadXml(doc)

			sr = New System.IO.StreamReader(path)
			Dim stringr As New System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(Me.CleanLoadFile(xmlDoc)))
			Dim tempLoadFile As WinEDDS.LoadFile
			Dim deserializer As New System.Runtime.Serialization.Formatters.Soap.SoapFormatter

			Try
				tempLoadFile = DirectCast(deserializer.Deserialize(stringr), WinEDDS.LoadFile)
				sr.Close()
			Catch ex As System.Exception
				If Not isSilent Then MsgBox("Load Failed", MsgBoxStyle.Critical)
				'TODO: Log Exception
				Return Nothing
			End Try
			tempLoadFile.CaseInfo = Me.SelectedCaseInfo
			tempLoadFile.CopyFilesToDocumentRepository = loadFile.CopyFilesToDocumentRepository
			tempLoadFile.SelectedCasePath = Me.SelectedCaseInfo.DocumentPath
			tempLoadFile.Credentials = Me.Credential
			tempLoadFile.DestinationFolderID = loadFile.DestinationFolderID
			tempLoadFile.SelectedIdentifierField = Me.CurrentFields(ArtifactTypeID, True).Item(Me.GetCaseIdentifierFields(ArtifactTypeID)(0))
			Dim x As New System.Windows.Forms.OpenFileDialog
			If Not loadFile.FilePath = String.Empty AndAlso System.IO.File.Exists(loadFile.FilePath) Then
				x.FileName = loadFile.FilePath
			ElseIf Not tempLoadFile.FilePath = String.Empty AndAlso System.IO.File.Exists(tempLoadFile.FilePath) Then
				x.FileName = tempLoadFile.FilePath
			End If
			MsgBox("Please Choose a Load File", MsgBoxStyle.OkOnly)
			x.Title = "Choose Load File"
			x.Filter = "All files (*.*)|*.*|CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt|DAT Files|*.dat"
			Select Case x.ShowDialog()
				Case DialogResult.Cancel
					Return Nothing
			End Select
			tempLoadFile.FilePath = x.FileName
			Dim mapItemToRemove As LoadFileFieldMap.LoadFileFieldMapItem = Nothing
			If tempLoadFile.GroupIdentifierColumn = String.Empty AndAlso System.IO.File.Exists(tempLoadFile.FilePath) Then
				Dim fieldMapItem As kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem
				For Each fieldMapItem In tempLoadFile.FieldMap
					If Not fieldMapItem.DocumentField Is Nothing AndAlso _
					 fieldMapItem.NativeFileColumnIndex >= 0 AndAlso _
					 fieldMapItem.DocumentField.FieldName.ToLower = "group identifier" Then
						tempLoadFile.GroupIdentifierColumn = Me.GetColumnHeadersFromLoadFile(tempLoadFile, tempLoadFile.FirstLineContainsHeaders)(fieldMapItem.NativeFileColumnIndex)
						'mapItemToRemove = fieldMapItem
					End If
					If Not fieldMapItem.DocumentField Is Nothing Then
						Try
							Dim thisField As DocumentField = Me.CurrentFields(tempLoadFile.ArtifactTypeID).Item(fieldMapItem.DocumentField.FieldID)
							fieldMapItem.DocumentField.AssociatedObjectTypeID = thisField.AssociatedObjectTypeID
							fieldMapItem.DocumentField.UseUnicode = thisField.UseUnicode
							fieldMapItem.DocumentField.CodeTypeID = thisField.CodeTypeID
							fieldMapItem.DocumentField.FieldLength = thisField.FieldLength
							fieldMapItem.DocumentField.ImportBehavior = thisField.ImportBehavior
						Catch
						End Try
					End If
				Next
			End If
			If Not mapItemToRemove Is Nothing Then tempLoadFile.FieldMap.Remove(mapItemToRemove)
			Return tempLoadFile
		End Function

		Public Function ReadImageLoadFile(ByVal path As String) As ImageLoadFile
			kCura.WinEDDS.Service.Settings.SendEmailOnLoadCompletion = Config.SendNotificationOnImportCompletionByDefault
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

		Friend Function DefaultCredentialsAreGood() As Boolean
			Dim myHttpWebRequest As System.Net.HttpWebRequest
			Dim myHttpWebResponse As System.Net.HttpWebResponse
			Dim cred As System.Net.NetworkCredential
			Dim relativityManager As kCura.WinEDDS.Service.RelativityManager

			cred = DirectCast(System.Net.CredentialCache.DefaultCredentials, System.Net.NetworkCredential)
			myHttpWebRequest = DirectCast(System.Net.WebRequest.Create(kCura.WinEDDS.Config.WebServiceURL & "\RelativityManager.asmx"), System.Net.HttpWebRequest)
			myHttpWebRequest.Credentials = System.Net.CredentialCache.DefaultCredentials
			Try
				myHttpWebResponse = DirectCast(myHttpWebRequest.GetResponse(), System.Net.HttpWebResponse)
				relativityManager = New kCura.WinEDDS.Service.RelativityManager(cred, _cookieContainer)
				If relativityManager.ValidateSuccesfulLogin Then
					CheckVersion(System.Net.CredentialCache.DefaultCredentials)
					_credential = cred
					kCura.WinEDDS.Service.Settings.AuthenticationToken = New kCura.WinEDDS.Service.UserManager(cred, _cookieContainer).GenerateDistributedAuthenticationToken()
					Return True
				Else
					Return False
				End If
			Catch ex As System.Exception
				Return False
			End Try
		End Function

		Private Sub _loginForm_OK_Click(ByVal cred As System.Net.NetworkCredential, ByVal openCaseSelector As Boolean) Handles _loginForm.OK_Click
			_loginForm.Close()
			Dim userManager As New kCura.WinEDDS.Service.UserManager(cred, _cookieContainer)
			Dim relativityManager As New kCura.WinEDDS.Service.RelativityManager(cred, _cookieContainer)
			Try
				CheckVersion(cred)
				If userManager.Login(cred.UserName, cred.Password) Then

					Dim locale As New System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.LCID, True)
					locale.NumberFormat.CurrencySymbol = relativityManager.RetrieveCurrencySymbol
					System.Threading.Thread.CurrentThread.CurrentCulture = locale

					_credential = cred
					kCura.WinEDDS.Service.Settings.AuthenticationToken = userManager.GenerateDistributedAuthenticationToken()
					If openCaseSelector Then OpenCase()
					_timeZoneOffset = 0					'New kCura.WinEDDS.Service.RelativityManager(cred, _cookieContainer).GetServerTimezoneOffset
				Else
					Me.ReLogin("Invalid login. Try again?")
				End If
			Catch ex As System.Net.WebException
				If Not ex.Message.IndexOf("The remote name could not be resolved") = -1 AndAlso ex.Source = "System" Then
					Me.ChangeWebServiceURL("The current Web Service URL could not be resolved. Try a new URL?")
				ElseIf Not ex.Message.IndexOf("The request failed with HTTP status 401") = -1 AndAlso ex.Source = "System.Web.Services" Then
					Me.ChangeWebServiceURL("The current Web Service URL was resolved but is not configured correctly. Try a new URL?")
				End If
			Catch ex As System.Exception
				Dim x As New ErrorDialog
				If Not ex.Message.IndexOf("Invalid License.") = -1 Then
					x.Text = "Invalid License."
				ElseIf (Not ex.Message.IndexOf("A library (dll)") = -1) OrElse (Not ex.Message.IndexOf("Relativity is temporarily unavailable.") = -1) Then
					x.Text = "Invalid Assembly."
				Else
					x.Text = "Unrecognized login error.  Try again?"
				End If
				x.Initialize(ex, x.Text)
				If x.ShowDialog = DialogResult.OK Then
					NewLogin()
				Else
					ExitApplication()
				End If
			End Try
		End Sub

		Public Function DoLogin(ByVal cred As System.Net.NetworkCredential) As Boolean
			Dim userManager As New kCura.WinEDDS.Service.UserManager(cred, _cookieContainer)
			CheckVersion(cred)
			If userManager.Login(cred.UserName, cred.Password) Then
				_credential = cred
				kCura.WinEDDS.Service.Settings.AuthenticationToken = userManager.GenerateDistributedAuthenticationToken()
				_timeZoneOffset = 0				'New kCura.WinEDDS.Service.RelativityManager(cred, _cookieContainer).GetServerTimezoneOffset
				Return True
			Else
				Return False
			End If
		End Function

		Private Sub ReLogin(ByVal message As String)
			If MsgBox(message, MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
				NewLogin()
			Else
				ExitApplication()
			End If
		End Sub

		Private Sub Reconnect()
			Dim userManager As New kCura.WinEDDS.Service.UserManager(_credential, _cookieContainer)
			If userManager.Login(_credential.UserName, _credential.Password) Then
				_credential = _credential
				kCura.WinEDDS.Service.Settings.AuthenticationToken = userManager.GenerateDistributedAuthenticationToken()
			End If

		End Sub

		Private Sub ChangeWebServiceUrl(ByVal message As String)
			If MsgBox(message, MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
				Dim url As String = InputBox("Enter New URL:", DefaultResponse:=kCura.WinEDDS.Config.WebServiceURL)
				If url <> String.Empty Then
					kCura.WinEDDS.Config.WebServiceURL = url
					NewLogin()
				Else
					ExitApplication()
				End If
			End If
		End Sub

		Private Sub CheckVersion(ByVal credential As Net.ICredentials)
			Dim relativityManager As New kCura.WinEDDS.Service.RelativityManager(DirectCast(credential, System.Net.NetworkCredential), _cookieContainer)
			Dim winVersionString As String = System.Reflection.Assembly.GetExecutingAssembly.FullName.Split(","c)(1).Split("="c)(1)
			Dim winRelativityVersion As String() = winVersionString.Split("."c)
			Dim relVersionString As String = relativityManager.RetrieveRelativityVersion
			Dim relativityWebVersion As String() = relVersionString.Split("."c)
			Dim match As Boolean = True
			Dim i As Int32
			For i = 0 To System.Math.Max(winRelativityVersion.Length - 1, relativityWebVersion.Length - 1)
				Dim winv As String = "*"
				Dim relv As String = "*"
				If i <= winRelativityVersion.Length - 1 Then winv = winRelativityVersion(i)
				If i <= relativityWebVersion.Length - 1 Then relv = relativityWebVersion(i)
				If Not (relv = "*" OrElse winv = "*" OrElse relv.ToLower = winv.ToLower) Then
					match = False
					Exit For
				End If
			Next
			If Not match Then
				MsgBox(String.Format("Your version of WinRelativity is out of date. You are running version {0}, but version {1} is required.", Me.GetDisplayAssemblyVersion(), relVersionString), MsgBoxStyle.Critical, "WinRelativity Version Mismatch")
				ExitApplication()
			Else
				Exit Sub
			End If
		End Sub
#End Region

#Region " System Configuration "

		Public Function GetDisplayAssemblyVersion() As String
			Return System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString
		End Function

		Public Function GetSystemConfiguration() As System.Data.DataTable
			Return New kCura.WinEDDS.Service.RelativityManager(Me.Credential, Me.CookieContainer).RetrieveRdcConfiguration().Tables(0)
		End Function

#End Region


		Public Function GetProductionPrecendenceList(ByVal caseInfo As Relativity.CaseInfo) As System.Data.DataTable
			Dim productionManager As kCura.WinEDDS.Service.ProductionManager
			Dim dt As System.Data.DataTable
			Try
				productionManager = New kCura.WinEDDS.Service.ProductionManager(Me.Credential, _cookieContainer)
				dt = productionManager.RetrieveProducedByContextArtifactID(caseInfo.ArtifactID).Tables(0)
			Catch ex As System.Exception
				If ex.Message.IndexOf("Need To Re Login") <> -1 Then
					NewLogin(False)
					'productionManager = New kCura.WinEDDS.Service.ProductionManager(Me.Credential, _cookieContainer)
					Return Nothing
				Else
					Throw
				End If
			End Try
			Dim retval As New System.Data.DataTable
			retval.Columns.Add("Display")
			retval.Columns.Add("Value")
			Dim row As System.Data.DataRow
			For Each row In dt.Rows
				retval.Rows.Add(New String() {row("Name").ToString, row("ArtifactID").ToString})
			Next
			Return retval
		End Function

		Public Sub DoAbout()
			If Not _loginForm Is Nothing AndAlso Not _loginForm.IsDisposed Then
				_loginForm.TopMost = False
			End If

			Dim sb As New System.Text.StringBuilder
			Dim nl As String = System.Environment.NewLine & System.Environment.NewLine
			Dim bitness As String = String.Empty

			' Determine build bitness
			If IntPtr.Size = 8 Then
				bitness = "64-bit"
			ElseIf IntPtr.Size = 4 Then
				bitness = "32-bit"
			End If

			sb.Append(String.Format("Relativity Desktop Client {0} {1}", bitness, nl))
			sb.Append("Version " & Me.GetDisplayAssemblyVersion() & nl)
			sb.Append(Relativity.Constants.LICENSE_AGREEMENT_TEXT & nl)
			sb.Append("Copyright © " & System.DateTime.Now.Year & " kCura Corporation")
			MsgBox(sb.ToString, MsgBoxStyle.OkOnly, "About Relativity Desktop Client")
			If Not _loginForm Is Nothing AndAlso Not _loginForm.IsDisposed Then
				_loginForm.TopMost = True
			End If
		End Sub
	End Class
End Namespace