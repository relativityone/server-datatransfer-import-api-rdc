Imports System.Configuration
Imports System.Web.Services.Protocols
Imports System.Security.Cryptography.X509Certificates
Imports System.Net
Imports System.Net.Security
Imports System.Linq
Imports System.Threading.Tasks

Imports kCura.EDDS.WinForm.Forms
Imports kCura.Windows.Forms
Imports kCura.WinEDDS.Core.Export
Imports kCura.WinEDDS.Credentials
Imports kCura.WinEDDS.Service
Imports Relativity
Imports Relativity.OAuth2Client.Exceptions
Imports Relativity.OAuth2Client.Interfaces
Imports Relativity.OAuth2Client.Interfaces.Events
Imports Relativity.Services.ServiceProxy
Imports Relativity.StagingExplorer.Services.StagingManager

Namespace kCura.EDDS.WinForm
    Public Class Application

#Region "Singleton Methods"
        Private Shared _instance As Application

        Protected Sub New()
            _processPool = New kCura.Windows.Process.ProcessPool
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls Or SecurityProtocolType.Ssl3
            _CookieContainer = New System.Net.CookieContainer
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
        Public Event ReCheckCertificate()

        Public OpenCaseSelector As Boolean = True

        Public Const ACCESS_DISABLED_MESSAGE As String = "Your Relativity account has been disabled.  Please contact your Relativity Administrator to activate your account."
        Public Const ROSE_STARTUP_PERMISSIONS_FAILURE As String = "The RelativityOne Staging Explorer failed to run due to insufficient permissions. Please contact you Relativity Administrator."
        Public Const ROSE_STARTUP_ALREADY_RUNNING As String = "Only one Staging Explorer session is allowed per one logged in user."
        Public Const RDC_ERROR_TITLE As String = "Relativity Desktop Client Error"
        Public Const RDC_TITLE As String = "Relativity Desktop Client"

        Private _caseSelected As Boolean = True
        Private _processPool As kCura.Windows.Process.ProcessPool
        Private _selectedCaseInfo As Relativity.CaseInfo
        Private _selectedCaseFolderID As Int32
        Private _fieldProviderCache As IFieldProviderCache
        Private _selectedCaseFolderPath As String
        Private _timeZoneOffset As Int32
        Private WithEvents _certificatePromptForm As CertificatePromptForm
        Private WithEvents _optionsForm As OptionsForm
        Private _documentRepositoryList As String()
#End Region

#Region "Properties"

        Public Sub SetImplicitCredentialProvider()
            If RelativityWebApiCredentialsProvider.Instance().CredentialsSet() AndAlso RelativityWebApiCredentialsProvider.Instance().ProviderType() = GetType(OAuth2ImplicitCredentials) Then
                Dim tempImplicitProvider As OAuth2ImplicitCredentials = CType(RelativityWebApiCredentialsProvider.Instance().GetProvider(), OAuth2ImplicitCredentials)
                tempImplicitProvider.CloseLoginView()
            End If
            Dim authEndpoint As String = String.Format("{0}/{1}", GetIdentityServerLocation(), "connect/authorize")
            Dim implicitProvider = New OAuth2ImplicitCredentials(New Uri(authEndpoint), "Relativity Desktop Client", AddressOf On_TokenRetrieved)
            RelativityWebApiCredentialsProvider.Instance().SetProvider(implicitProvider)
        End Sub

        Friend Async Function GetCredentialsAsync() As Task(Of System.Net.NetworkCredential)
            If Not RelativityWebApiCredentialsProvider.Instance().CredentialsSet() Then
                SetImplicitCredentialProvider()
            End If
            Return Await RelativityWebApiCredentialsProvider.Instance().GetCredentialsAsync()
        End Function

        Private Async Function GetFieldProviderCacheAsync() As Task(Of IFieldProviderCache)
            If (_fieldProviderCache Is Nothing) Then
                _fieldProviderCache = New FieldProviderCache(Await GetCredentialsAsync(), _CookieContainer)
            End If
            Return _fieldProviderCache
        End Function

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

        Public Async Function RefreshSelectedCaseInfo(Optional ByVal caseInfo As Relativity.CaseInfo = Nothing) As Task
            Dim caseManager As New kCura.WinEDDS.Service.CaseManager(Await Me.GetCredentialsAsync(), _CookieContainer)
            If caseInfo Is Nothing Then
                _selectedCaseInfo = caseManager.Read(_selectedCaseInfo.ArtifactID)
            Else
                _selectedCaseInfo = caseManager.Read(caseInfo.ArtifactID)
            End If
            _documentRepositoryList = caseManager.GetAllDocumentFolderPathsForCase(_selectedCaseInfo.ArtifactID)
        End Function

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

        Public Async Function GetSendLoadNotificationEmailEnabledAsync() As Task(Of Boolean)
            Return New kCura.WinEDDS.Service.RelativityManager(Await Me.GetCredentialsAsync, Me.CookieContainer).IsImportEmailNotificationEnabled
        End Function

        Public Async Function CurrentFields(ByVal artifactTypeID As Int32, Optional ByVal refresh As Boolean = False) As Task(Of DocumentFieldCollection)
            Try
                Return (Await GetFieldProviderCacheAsync()).CurrentFields(artifactTypeID, SelectedCaseInfo.ArtifactID, refresh)
            Catch ex As System.Exception
                If ex.Message.IndexOf("Need To Re Login") <> -1 Then
                    NewLogin(False)
                Else
                    Throw
                End If
            End Try
            Return Nothing
        End Function

        Public Async Function CurrentNonFileFields(ByVal artifactTypeID As Int32, Optional ByVal refresh As Boolean = False) As Task(Of DocumentFieldCollection)
            Try
                Return (Await GetFieldProviderCacheAsync()).CurrentNonFileFields(artifactTypeID, SelectedCaseInfo.ArtifactID, refresh)
            Catch ex As System.Exception
                If ex.Message.IndexOf("Need To Re Login") <> -1 Then
                    NewLogin(False)
                Else
                    Throw
                End If
            End Try
            Return Nothing
        End Function

        Public Property TemporaryWebServiceURL() As String

        Public Property TemporaryForceFolderPreview() As Boolean

        Public Property CookieContainer() As System.Net.CookieContainer

        Public Property ArtifactTypeID() As Int32

        Public Property UserHasImportPermission() As Boolean

        Public Property UserHasExportPermission() As Boolean

        Public Property UserHasStagingPermission() As Boolean

#End Region
 
#Region "Event Throwers"
        Public Sub LogOn()
            RaiseEvent OnEvent(New AppEvent(AppEvent.AppEventType.LogOn))
        End Sub

        Public Sub LogOnForm()
            RaiseEvent OnEvent(New AppEvent(AppEvent.AppEventType.LogOnForm))
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
                _caseSelected = False
                '' Turn off our trust of bad certificates! This needs to happen here (references need to be added to add it to MainForm - bad practice).
                ServicePointManager.ServerCertificateValidationCallback = Function(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) sslPolicyErrors.Equals(SslPolicyErrors.None)
                RaiseEvent ReCheckCertificate()
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
        Public Async Function GetDocumentFieldFromName(ByVal fieldName As String) As Task(Of DocumentField)
            Return (Await CurrentFields(Me.ArtifactTypeID)).Item(fieldName)
        End Function

        Public Async Function GetCaseIdentifierFields(ByVal artifactTypeID As Int32) As Task(Of String())
            Return (Await CurrentFields(artifactTypeID)).IdentifierFieldNames
        End Function

        Public Async Function IdentiferFieldDropdownPopulator() As Task(Of String())
            Return (Await CurrentFields(Me.ArtifactTypeID)).NamesForIdentifierDropdown
        End Function

        Public Async Function GetCaseFields(ByVal caseID As Int32, ByVal artifactTypeID As Int32, Optional ByVal refresh As Boolean = False) As Task(Of String())
            Dim retval As String() = Nothing
            Dim fields As DocumentFieldCollection = Await CurrentFields(artifactTypeID, refresh)
            If Not fields Is Nothing Then
                retval = fields.Names()
            End If
            Return retval
        End Function

        Public Async Function GetNonFileCaseFields(ByVal caseID As Int32, ByVal artifactTypeID As Int32, Optional ByVal refresh As Boolean = False) As Task(Of String())
            Dim retval As String() = Nothing
            Dim fields As DocumentFieldCollection = Await CurrentNonFileFields(artifactTypeID, refresh)
            If Not fields Is Nothing Then
                retval = fields.Names()
            End If
            Return retval
        End Function

        Friend Async Function IsConnected() As Task(Of Boolean)
            Dim retval = False
            Try
                Dim userManager As New kCura.WinEDDS.Service.UserManager(Await GetCredentialsAsync(), _CookieContainer)
                retval = userManager.LoggedIn()
            Catch ex As System.Exception
                If ex.Message.IndexOf("Need To Re Login") <> -1 Then
                    NewLogin(False)
                Else
                    Throw
                End If
            End Try
            Return retval
        End Function

        Public Async Function GetSelectedIdentifier(ByVal selectedField As DocumentField) As Task(Of String)
            Try
                Return (Await CurrentFields(Me.ArtifactTypeID)).Item(selectedField.FieldName).FieldName
            Catch ex As System.Exception
                Return String.Empty
            End Try
        End Function

        Friend Async Function ReadyToLoad(ByVal unselectedFieldNames As String()) As Task(Of Boolean)
            Dim identifierFieldName As String
            Dim unselectedFieldName As String
            Dim unselectedIDFieldNames As New System.Text.StringBuilder
            For Each identifierFieldName In (Await CurrentFields(Me.ArtifactTypeID)).IdentifierFieldNames()
                For Each unselectedFieldName In unselectedFieldNames
                    If identifierFieldName.ToLower & " [identifier]" = unselectedFieldName Then
                        unselectedIDFieldNames.AppendFormat(unselectedFieldName & ChrW(13))
                    End If
                Next
            Next
            If unselectedIDFieldNames.Length = 0 Then
                Return True
            Else
                MsgBox("The following identifier fields have not been mapped: " & ChrW(13) & unselectedIDFieldNames.ToString &
                "Do you wish to continue?", MsgBoxStyle.Critical, "Warning")
                Return False
            End If
        End Function

        Friend Async Function ReadyToLoad(ByVal loadFile As WinEDDS.LoadFile, ByVal forPreview As Boolean) As Task(Of Boolean)
            Dim isIdentifierMapped As Boolean = False
            For Each fieldMapItem As LoadFileFieldMap.LoadFileFieldMapItem In loadFile.FieldMap
                If Not fieldMapItem.DocumentField Is Nothing AndAlso fieldMapItem.DocumentField.FieldID = loadFile.IdentityFieldId AndAlso fieldMapItem.NativeFileColumnIndex <> -1 Then
                    isIdentifierMapped = True
                End If
            Next
            Dim fieldName As String = (Await Me.CurrentFields(ArtifactTypeID, True)).Item(loadFile.IdentityFieldId).FieldName
            If Not forPreview AndAlso Me.IdentifierFieldIsMappedButNotKey(loadFile.FieldMap, loadFile.IdentityFieldId) Then
                MsgBox("The field marked [identifier] cannot be part of a field map when it's not the Overlay Identifier field", MsgBoxStyle.Critical, "Relativity Desktop Client")
                Return False
            End If
            If Not isIdentifierMapped Then
                MsgBox("The key field [" & fieldName & "] is unmapped.  Please map it to continue", MsgBoxStyle.Critical, "Relativity Desktop Client")
                Return isIdentifierMapped
            End If
            If Not forPreview AndAlso Not New kCura.WinEDDS.Service.FieldQuery(Await GetCredentialsAsync(), _CookieContainer).IsFieldIndexed(Me.SelectedCaseInfo.ArtifactID, loadFile.IdentityFieldId) Then
                Return MsgBox("There is no SQL index on the selected Overlay Identifier field.  " & vbNewLine & "Performing a load on an un-indexed SQL field will be drastically slower, " & vbNewLine & "and may negatively impact Relativity performance for all users." & vbNewLine & "Contact your SQL Administrator to have an index applied to the selected Overlay Identifier field.", MsgBoxStyle.OkCancel, "Relativity Desktop Client") = MsgBoxResult.Ok
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

        Friend Async Function ReadyToLoad(ByVal imageArgs As WinEDDS.ImageLoadFile, ByVal forPreview As Boolean) As Task(Of Boolean)
            Dim id As Int32
            If imageArgs.ProductionArtifactID > 0 Then
                id = imageArgs.BeginBatesFieldArtifactID
            Else
                id = imageArgs.IdentityFieldId
            End If
            If Not forPreview AndAlso Not New kCura.WinEDDS.Service.FieldQuery(Await GetCredentialsAsync(), _CookieContainer).IsFieldIndexed(Me.SelectedCaseInfo.ArtifactID, id) Then
                Return MsgBox("There is no SQL index on the selected Overlay Identifier field.  " & vbNewLine & "Performing a load on an un-indexed SQL field will be drastically slower, " & vbNewLine & "and may negatively impact Relativity performance for all users." & vbNewLine & "Contact your SQL Administrator to have an index applied to the selected Overlay Identifier field.", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok
            Else
                Return True
            End If
        End Function
#End Region

#Region "Folder Management"
        Public Async Function CreateNewFolder(ByVal parentFolderID As Int32) As Task
            Dim name As String = InputBox("Enter Folder Name", "Relativity Review")
            If name <> String.Empty Then
                Try
                    Dim folderManager As New kCura.WinEDDS.Service.FolderManager(Await Me.GetCredentialsAsync(), _CookieContainer)
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

        Public Async Function GetCaseFolders(ByVal caseID As Int32) As Task(Of System.Data.DataSet)
            Try
                Dim folderManager As New kCura.WinEDDS.Service.FolderManager(Await GetCredentialsAsync(), _CookieContainer)
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
        End Function
#End Region

#Region "Case Management"
        Public Async Function GetCases() As Task(Of System.Data.DataSet)
            Dim cache As IFieldProviderCache = Await GetFieldProviderCacheAsync()
            cache.ResetCache()
            Try
                Dim csMgr As New kCura.WinEDDS.Service.CaseManager(Await GetCredentialsAsync(), _CookieContainer)
                _documentRepositoryList = csMgr.GetAllDocumentFolderPaths()
                Dim dataset As System.Data.DataSet = csMgr.RetrieveAll()

#If EnableInjections Then
                Dim dt As System.Data.DataTable = dataset.Tables(0)
                If dt.Rows.Count > 0 Then
                    Dim numberOfMockWorkspacesToAdd As Int32? = Config.NumberOfFakeWorkspacesToAdd
                    If numberOfMockWorkspacesToAdd.HasValue AndAlso numberOfMockWorkspacesToAdd.Value > 0 Then
                        Dim rows As System.Data.DataRow() = dt.Select()
                        Dim maxID As Int32 = 0
                        For Each row As System.Data.DataRow In rows
                            maxID = Math.Max(CInt(row("ArtifactID")), maxID)
                        Next
                        Dim sampleRow As Object() = rows(0).ItemArray()
                        For i As Int32 = 1 To numberOfMockWorkspacesToAdd.Value
                            maxID += 1
                            sampleRow(0) = maxID
                            sampleRow(1) = "Workspace " & maxID.ToString().PadLeft(20, "0"c)
                            dt.Rows.Add(sampleRow)
                        Next
                    End If
                End If
#End If

                Return dataset
            Catch ex As System.Exception
                If ex.Message.IndexOf("Need To Re Login") <> -1 Then
                    NewLogin(False)
                Else
                    Throw
                End If
            End Try
            Return Nothing
        End Function

        Public Sub SelectCaseFolder(ByVal folderInfo As WinEDDS.FolderInfo)
            _selectedCaseFolderID = folderInfo.ArtifactID
            _selectedCaseFolderPath = folderInfo.Path
            _caseSelected = True
            RaiseEvent OnEvent(New AppEvent(AppEvent.AppEventType.WorkspaceFolderSelected))
            _caseSelected = True
        End Sub

        Public Async Function OpenCase() As Task
            Try
                Dim caseInfo As Relativity.CaseInfo = Me.GetCase
                If Not caseInfo Is Nothing Then
                    _selectedCaseInfo = caseInfo
                    Await Me.RefreshSelectedCaseInfo()
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
        End Function

        Public Function GetCase() As Relativity.CaseInfo
            Dim frm As New CaseSelectForm
            frm.MultiSelect = False
            frm.ShowDialog()
            If frm.SelectedCaseInfo Is Nothing OrElse frm.SelectedCaseInfo.Count = 0 Then
                Return Nothing
            Else
                Return frm.SelectedCaseInfo.Item(0)
            End If
        End Function

        Public Async Function GetConnectionStatus() As Task(Of String)
            Dim parameters = CreateTapiParametersAsync()
            Dim clientName = Await kCura.WinEDDS.TApi.TapiWinEddsHelper.GetWorkspaceClientDisplayNameAsync(await parameters)
            Return clientName
        End Function

        Public Async Function GetConnectionMode() As Task(Of Guid)
            Dim parameters = CreateTapiParametersAsync()
            Dim clientName = Await kCura.WinEDDS.TApi.TapiWinEddsHelper.GetWorkspaceClientIdAsync(await parameters)
            Return clientName
        End Function

        Private Async Function CreateTapiParametersAsync() As Task(Of TApi.TapiBridgeParameters)
            Dim credentials = Await Me.GetCredentialsAsync()
            Dim parameters = New TApi.TapiBridgeParameters
            parameters.Credentials = credentials
            parameters.AsperaDocRootLevels = WinEDDS.Config.TapiAsperaNativeDocRootLevels
            parameters.FileShare = Me.SelectedCaseInfo.DocumentPath
            parameters.ForceAsperaClient = WinEDDS.Config.TapiForceAsperaClient
            parameters.ForceClientCandidates = WinEDDS.Config.TapiForceClientCandidates
            parameters.ForceFileShareClient = WinEDDS.Config.TapiForceFileShareClient
            parameters.ForceHttpClient = WinEDDS.Config.ForceWebUpload OrElse WinEDDS.Config.TapiForceHttpClient                                                            
            parameters.WebCookieContainer = Me.CookieContainer
            parameters.WebServiceUrl = WinEDDS.Config.WebServiceURL
            parameters.WorkspaceId = Me.SelectedCaseInfo.ArtifactID

            return parameters
        End Function
#End Region

#Region "Security Methods"
        ''' <summary>
        ''' Checks that the https certificate is trusted for this connection. Throws an exception if anything but trust failure happens
        ''' </summary>
        ''' <returns>True if the certificate is trusted. False otherwise.</returns>
        ''' <remarks></remarks>
        Public Function CertificateTrusted() As Boolean
            Dim isCertificateTrusted As Boolean = True
            Dim cred As NetworkCredential = DirectCast(CredentialCache.DefaultCredentials, NetworkCredential)
            Dim relativityManager As New Service.RelativityManager(cred, _CookieContainer)

            Try
                ' Only if this line bombs do we say the cert is untrusted
                relativityManager.ValidateSuccessfulLogin()
            Catch ex As WebException
                If (ex.Status = WebExceptionStatus.TrustFailure) Then
                    isCertificateTrusted = False
                Else
                    Throw
                End If
            End Try

            Return isCertificateTrusted
        End Function

        Public Async Function IsAssociatedSearchProviderAccessible(ByVal caseContextArtifactID As Int32, ByVal searchArtifactID As Int32) As Task(Of Boolean)
            Dim searchManager As New kCura.WinEDDS.Service.SearchManager(Await Me.GetCredentialsAsync(), Me.CookieContainer)
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
        Public Async Function AllUploadableArtifactTypes() As Task(Of System.Data.DataTable)
            Return New kCura.WinEDDS.Service.ObjectTypeManager(Await Me.GetCredentialsAsync(), Me.CookieContainer).RetrieveAllUploadable(Me.SelectedCaseInfo.ArtifactID).Tables(0)
        End Function

        Public Async Function HasFileField(ByVal artifactTypeID As Int32, Optional ByVal refresh As Boolean = False) As Task(Of Boolean)
            Dim retval As Boolean = False
            Dim docFieldCollection As DocumentFieldCollection = Await CurrentFields(artifactTypeID, refresh)
            Dim allFields As ICollection = docFieldCollection.AllFields
            For Each field As DocumentField In allFields
                If field.FieldTypeID = Relativity.FieldTypeHelper.FieldType.File Then
                    retval = True
                End If
            Next
            Return retval
        End Function

        Public Async Function GetObjectTypeName(ByVal artifactTypeID As Int32) As Task(Of String)
            Dim objectTypeManager As New kCura.WinEDDS.Service.ObjectTypeManager(Await Me.GetCredentialsAsync(), Me.CookieContainer)
            Dim uploadableObjectTypes As System.Data.DataRowCollection = objectTypeManager.RetrieveAllUploadable(Me.SelectedCaseInfo.ArtifactID).Tables(0).Rows
            For Each objectType As System.Data.DataRow In uploadableObjectTypes
                With New kCura.WinEDDS.ObjectTypeListItem(CType(objectType("DescriptorArtifactTypeID"), Int32), CType(objectType("Name"), String), CType(objectType("HasAddPermission"), Boolean))
                    If .Value = artifactTypeID Then Return .Display
                End With
            Next
            Return String.Empty
        End Function

        Public Function GetColumnHeadersFromLoadFile(ByVal loadfile As kCura.WinEDDS.LoadFile, ByVal firstLineContainsColumnHeaders As Boolean) As String()
            loadfile.CookieContainer = Me.CookieContainer
            Dim logger As Relativity.Logging.ILog = RelativityLogFactory.CreateLog("WinEDDS")
            Dim parser As New kCura.WinEDDS.BulkLoadFileImporter(loadfile, Nothing, Nothing, logger, _timeZoneOffset, False, Nothing, False, Config.BulkLoadFileFieldDelimiter, Config.EnforceDocumentLimit, Nothing, ExecutionSource.Rdc)
            Return parser.GetColumnNames(loadfile)
        End Function

        Public Function GetCaseFolderPath(ByVal caseFolderArtifactID As Int32) As String
            Return "\" & _selectedCaseInfo.Name & _selectedCaseFolderPath
        End Function

        'Worker function for PreviewLoadFile
        Private Sub ManageRowWideError(ByVal dt As System.Data.DataTable, ByVal err As System.Exception, ByRef rowcount As Int32)
            Dim errorRow As System.Data.DataRow = dt.NewRow
            rowcount += 1
            For Each column As System.Data.DataColumn In dt.Columns
                Dim errorMessage As LoadFilePreviewColumnItem = New LoadFilePreviewColumnItem(New WinEDDS.Exceptions.ErrorMessage(If((column.ColumnName = "Record Number"), rowcount.ToString, "Row-wide error: " & err.Message)))
                errorRow(column.ColumnName) = errorMessage
            Next
            dt.Rows.Add(errorRow)
        End Sub

        Public Async Function BuildLoadFileDataSource(ByVal al As ArrayList) As Task(Of DataTable)
            Try
                Await Me.GetCaseFields(_selectedCaseInfo.ArtifactID, Me.ArtifactTypeID, True)
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
                                        dt.Columns(field.DisplayName).DataType = GetType(LoadFilePreviewColumnItem)
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
                    row.Add(New LoadFilePreviewColumnItem(field.Value))
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
                Dim previewChoices As New PreviewChoicesHelper
                Return previewChoices.BuildFoldersAndCodesDataSource(al, previewCodeCount)
            Catch ex As Exception
                kCura.EDDS.WinForm.Utility.ThrowExceptionToGUI(ex)
            End Try
            Return Nothing
        End Function

        Friend Async Function EnsureConnection() As Task(Of Boolean)
            Dim retval = False
            If Not Me.SelectedCaseInfo Is Nothing Then
                Try
                    Dim userManager As New kCura.WinEDDS.Service.UserManager(Await GetCredentialsAsync(), _CookieContainer)
                    retval = userManager.LoggedIn()
                Catch ex As System.Exception
                    If ex.Message.IndexOf("Need To Re Login") <> -1 Then
                        retval = False
                    Else
                        Throw
                    End If
                End Try
            Else
                retval = True
            End If
            Return retval
        End Function

        Public Async Function RefreshCaseFolders() As Task
            If Await Me.EnsureConnection Then
                RaiseEvent OnEvent(New kCura.WinEDDS.LoadCaseEvent(SelectedCaseInfo))
            End If
        End Function

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
        Public Async Function NewLoadFile(ByVal destinationArtifactID As Int32, ByVal caseInfo As Relativity.CaseInfo) As Task
            If Not Await Me.IsConnected() Then
                CursorDefault()
                Return
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
            loadFile.Credentials = Await Me.GetCredentialsAsync()
            loadFile.CookieContainer = Me.CookieContainer
            loadFile.OverwriteDestination = Relativity.ImportOverwriteType.Append.ToString
            loadFile.ArtifactTypeID = Me.ArtifactTypeID
            frm.LoadFile = loadFile
            Await frm.LoadFormControls(False)
            frm.Show()
        End Function

        Public Async Function NewProductionExport(ByVal caseInfo As Relativity.CaseInfo) As Task
            Await Me.NewSearchExport(caseInfo.RootFolderID, caseInfo, ExportFile.ExportType.Production)
        End Function

        Public Async Function NewSearchExport(ByVal selectedFolderId As Int32, ByVal caseInfo As Relativity.CaseInfo, ByVal typeOfExport As kCura.WinEDDS.ExportFile.ExportType) As Task
            Dim frm As ExportForm = New ExportForm()
            Dim exportFile As WinEDDS.ExportFile
            Try
                exportFile = Await Me.GetNewExportFileSettingsObject(selectedFolderId, caseInfo, typeOfExport, Me.ArtifactTypeID)
                If exportFile.DataTable.Rows.Count = 0 Then
                    Dim s As New System.Text.StringBuilder
                    s.Append("There are no exportable ")
                    Select Case exportFile.TypeOfExport
                        Case ExportFile.ExportType.Production
                            s.Append("productions ")
                        Case ExportFile.ExportType.ArtifactSearch
                            s.Append("saved searches ")
                        Case Else
                            s.Append("views ")
                    End Select
                    s.Append("in this case")
                    MsgBox(s.ToString, MsgBoxStyle.Critical, "Relativity Desktop Client")
                    Return
                End If
                frm.Application = Me
                frm.ExportFile = exportFile
                frm.Show()
            Catch ex As System.Exception
                If ex.Message.IndexOf("Need To Re Login") <> -1 Then
                    NewLogin(False)
                    Return
                Else
                    Throw
                End If
            End Try
        End Function

        Public Async Sub NewFileTransfer(mainForm As MainForm)
            Await NewLoginAsync()
            
            Dim credentials = Await Me.GetCredentialsAsync()

            StartStagingExplorer(credentials, mainForm)
        End Sub

        Private Sub StartStagingExplorer(credentials As NetworkCredential, mainForm As MainForm)
            Dim filename = GetApplicationFilePath()
            Dim arguments = $"-t {credentials.Password} -w {Me.SelectedCaseInfo.ArtifactID}  -u {kCura.WinEDDS.Config.WebServiceURL}"

            Dim appProcess = New Process()
            appProcess.StartInfo.FileName = filename
            appProcess.StartInfo.Arguments = arguments
            appProcess.EnableRaisingEvents = True

            AddHandler appProcess.Exited, Sub(s, e) OnStagingExplorerProcessExited(s, mainForm)

            appProcess.Start()
        End Sub

        Private Sub OnStagingExplorerProcessExited(sender As Object, mainForm As MainForm)
            Dim appProcess = TryCast(sender, Process)
            If appProcess Is Nothing Then
                Return
            End If

            Dim handleStagingExplorerProcessExited =
                    Sub(exitCode As Integer)
                        If exitCode = 403 Then
                            MessageBox.Show(ROSE_STARTUP_PERMISSIONS_FAILURE, RDC_ERROR_TITLE)
                        ElseIf exitCode = 423 Then
                            MessageBox.Show(ROSE_STARTUP_ALREADY_RUNNING, RDC_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If
                    End Sub

            If mainForm.InvokeRequired Then
                mainForm.Invoke(handleStagingExplorerProcessExited, appProcess.ExitCode)
            Else 
                handleStagingExplorerProcessExited(appProcess.ExitCode)
            End If
        End Sub

        Private Function GetApplicationFilePath() As String
            Dim appPath = ConfigurationManager.AppSettings("Relativity.StagingExplorer.ApplicationFile")
            If String.IsNullOrEmpty(appPath) Then
                Return "Relativity.StagingExplorer.exe"
            Else
                Return appPath
            End If
        End Function

        Public Async Function GetListOfProductionsForCase(ByVal caseInfo As Relativity.CaseInfo) As Task(Of System.Data.DataTable)
            Dim productionManager As New kCura.WinEDDS.Service.ProductionManager(Await Me.GetCredentialsAsync(), _CookieContainer)
            Return productionManager.RetrieveProducedByContextArtifactID(caseInfo.ArtifactID).Tables(0)
        End Function


        Public Async Function GetNewExportFileSettingsObject(ByVal selectedFolderId As Int32, ByVal caseInfo As Relativity.CaseInfo, ByVal typeOfExport As kCura.WinEDDS.ExportFile.ExportType, ByVal artifactTypeID As Int32) As Task(Of WinEDDS.ExportFile)
            Dim exportFile As New WinEDDS.ExportFile(artifactTypeID)
            Dim searchManager As New kCura.WinEDDS.Service.SearchManager(Await Me.GetCredentialsAsync(), _CookieContainer)
            Dim productionManager As New kCura.WinEDDS.Service.ProductionManager(Await Me.GetCredentialsAsync(), _CookieContainer)
            exportFile.ArtifactID = selectedFolderId
            exportFile.CaseInfo = caseInfo
            exportFile.Credential = Await Me.GetCredentialsAsync()
            exportFile.TypeOfExport = typeOfExport
            exportFile.ObjectTypeName = Await Me.GetObjectTypeName(exportFile.ArtifactTypeID)
            Select Case typeOfExport
                Case ExportFile.ExportType.Production
                    exportFile.DataTable = productionManager.RetrieveProducedByContextArtifactID(caseInfo.ArtifactID).Tables(0)
                Case Else
                    exportFile.DataTable = Me.GetSearchExportDataSource(searchManager, caseInfo.ArtifactID, typeOfExport = ExportFile.ExportType.ArtifactSearch, exportFile.ArtifactTypeID)
            End Select
            Dim ids As New System.Collections.ArrayList
            For Each row As System.Data.DataRow In exportFile.DataTable.Rows
                ids.Add(row("ArtifactID"))
            Next
            For Each field As DocumentField In Await Me.CurrentFields(exportFile.ArtifactTypeID, True)
                If field.FieldTypeID = Relativity.FieldTypeHelper.FieldType.File Then
                    exportFile.FileField = field
                    Exit For
                End If
            Next
            If ids.Count = 0 Then
                exportFile.ArtifactAvfLookup = New System.Collections.Specialized.HybridDictionary
                exportFile.AllExportableFields = New WinEDDS.ViewFieldInfo() {}
            Else
                exportFile.ArtifactAvfLookup = searchManager.RetrieveDefaultViewFieldsForIdList(caseInfo.ArtifactID, exportFile.ArtifactTypeID, DirectCast(ids.ToArray(GetType(Int32)), Int32()), typeOfExport = ExportFile.ExportType.Production)
                exportFile.AllExportableFields = searchManager.RetrieveAllExportableViewFields(caseInfo.ArtifactID, exportFile.ArtifactTypeID)
            End If
            Return exportFile
        End Function

        Friend Function GetSearchExportDataSource(ByVal searchManager As kCura.WinEDDS.Service.SearchManager, ByVal caseArtifactID As Int32, ByVal isArtifactSearch As Boolean, ByVal artifactType As Int32) As System.Data.DataTable
            Dim searchExportDataSet As System.Data.DataSet
            If isArtifactSearch Then
                searchExportDataSet = searchManager.RetrieveViewsByContextArtifactID(caseArtifactID, artifactType, True)
            Else
                searchExportDataSet = searchManager.RetrieveViewsByContextArtifactID(caseArtifactID, artifactType, False)
            End If
            Return searchExportDataSet.Tables(0)
        End Function

        Public Async Function NewImageFile(ByVal destinationArtifactID As Int32, ByVal caseinfo As Relativity.CaseInfo) As Task
            CursorWait()
            If Not Await Me.IsConnected() Then
                CursorDefault()
                Return
            End If
            Dim frm As New ImageLoad
            Try
                Dim imageFile As New ImageLoadFile
                imageFile.Credential = Await Me.GetCredentialsAsync()
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
        End Function

        Public Async Function NewProductionFile(ByVal destinationArtifactID As Int32, ByVal caseinfo As Relativity.CaseInfo) As Task
            CursorWait()
            If Not Await Me.IsConnected() Then
                CursorDefault()
                Return
            End If
            Dim frm As New ImageLoad
            Try
                Dim imageFile As New ImageLoadFile
                imageFile.Credential = Await Me.GetCredentialsAsync()
                imageFile.CaseInfo = caseinfo
                imageFile.DestinationFolderID = destinationArtifactID
                imageFile.ForProduction = True
                imageFile.CookieContainer = Me.CookieContainer
                imageFile.SelectedCasePath = caseinfo.DocumentPath
                imageFile.FullTextEncoding = System.Text.Encoding.Default
                imageFile.CopyFilesToDocumentRepository = Config.CopyFilesToRepository
                Dim productionManager As New kCura.WinEDDS.Service.ProductionManager(Await Me.GetCredentialsAsync(), _CookieContainer)
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
        End Function

        Public Sub NewOptions()
            CursorWait()

            If (Not _optionsForm Is Nothing AndAlso Not _optionsForm.Visible) Then
                _optionsForm = Nothing
            End If

            If (_optionsForm Is Nothing) Then
                _optionsForm = New OptionsForm

                If Not _certificatePromptForm Is Nothing AndAlso _certificatePromptForm.Visible Then
                    _certificatePromptForm.Close()
                    _optionsForm.Show()
                Else
                    _optionsForm.Show()
                End If
            End If

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
            Dim frm As New Forms.SetWebServiceURL
            frm.Show()
            CursorDefault()
        End Sub

        ''' <summary>
        ''' Prompts the user to Allow or Deny the untrusted connection.
        ''' </summary>;
        ''' <remarks></remarks>
        Public Sub CertificateCheckPrompt()
            CursorWait()
            '' Mimicing NewLogin for now.. url specific logic here?
            If Not _certificatePromptForm Is Nothing Then
                If Not _certificatePromptForm.IsDisposed Then
                    _certificatePromptForm.Close()
                End If
            End If

            _certificatePromptForm = New CertificatePromptForm
            _certificatePromptForm.Show()

            CursorDefault()
        End Sub

        Public Sub NewLogin(Optional ByVal openCaseSelector As Boolean = True)
            NewLoginAsync(openCaseSelector).Wait()
        End Sub

        Public Async Function NewLoginAsync(Optional ByVal openCaseSelector As Boolean = True) As Task
            Try
                Me.OpenCaseSelector = openCaseSelector
                Await Me.GetCredentialsAsync()
            Catch ex As LoginCanceledException
                'Login form was closed, ignore
            End Try
        End Function
#End Region

#Region "Process Management"
        Private Function CreateProgressForm() As kCura.Windows.Process.ProgressForm
            Return New kCura.Windows.Process.ProgressForm() With {.StatusRefreshRate = WinEDDS.Config.ProcessFormRefreshRate}
        End Function

        Public Async Function QueryConnectivity() As Task
            CursorWait()
            If Not Await Me.IsConnected() Then
                CursorDefault()
                Return
            End If
            Dim proc As New kCura.WinEDDS.ConnectionDetailsProcess(Await Me.GetCredentialsAsync(), Me.CookieContainer, Me.SelectedCaseInfo)
            Dim form As New TextDisplayForm
            form.ProcessObserver = proc.ProcessObserver
            form.Text = "Relativity Desktop Client | Connectivity Tests"
            form.Show()
            _processPool.StartProcess(proc)
            CursorDefault()
        End Function

        Public Async Function PreviewLoadFile(ByVal loadFileToPreview As LoadFile, ByVal errorsOnly As Boolean, ByVal formType As Int32) As Task
            CursorWait()
            If Not Await Me.IsConnected() Then
                CursorDefault()
                Return
            End If
            If Not CheckFieldMap(loadFileToPreview) Then
                CursorDefault()
                Return
            End If
            Dim frm As kCura.Windows.Process.ProgressForm = CreateProgressForm()
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

        Public Function StartProcess(ByVal process As Windows.Process.IRunable) As System.Guid
            Return _processPool.StartProcess(process)
        End Function

        Public Async Function ImportLoadFile(ByVal loadFile As LoadFile) As Task
            CursorWait()
            'Dim folderManager As New kCura.WinEDDS.Service.FolderManager(Credential, _cookieContainer, _identity)
            If Not Await Me.IsConnected() Then
                CursorDefault()
                Return
            End If
            Dim folderManager As New kCura.WinEDDS.Service.FolderManager(Await GetCredentialsAsync(), _CookieContainer)
            If folderManager.Exists(SelectedCaseInfo.ArtifactID, SelectedCaseInfo.RootFolderID) Then
                If CheckFieldMap(loadFile) Then
                    Dim frm As kCura.Windows.Process.ProgressForm = CreateProgressForm()
                    Dim importer As New kCura.WinEDDS.ImportLoadFileProcess
                    importer.LoadFile = loadFile
                    importer.TimeZoneOffset = _timeZoneOffset
                    importer.BulkLoadFileFieldDelimiter = Config.BulkLoadFileFieldDelimiter
                    importer.CloudInstance = Config.CloudInstance
					importer.EnforceDocumentLimit = Config.EnforceDocumentLimit
                    importer.ExecutionSource = Relativity.ExecutionSource.Rdc
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
                End If
            Else
                CursorDefault()
                MsgBox("Selected folder no longer exists.  Please reselect.")
            End If
        End Function

        Public Async Function PreviewImageFile(ByVal loadfile As ImageLoadFile) As Task
            CursorWait()
            'Dim folderManager As New kCura.WinEDDS.Service.FolderManager(Credential, _cookieContainer, _identity)
            If Not Await Me.IsConnected() Then
                CursorDefault()
                Return
            End If
            Dim frm As kCura.Windows.Process.ProgressForm = CreateProgressForm()
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
        End Function

        Public Async Function ImportImageFile(ByVal ImageLoadFile As ImageLoadFile) As Task
            CursorWait()
            If Not Await Me.IsConnected() Then
                CursorDefault()
                Return
            End If
            Dim frm As kCura.Windows.Process.ProgressForm = CreateProgressForm()
            Dim importer As New kCura.WinEDDS.ImportImageFileProcess
            ImageLoadFile.CookieContainer = Me.CookieContainer
            importer.ImageLoadFile = ImageLoadFile
            importer.CloudInstance = Config.CloudInstance
			importer.EnforceDocumentLimit = Config.EnforceDocumentLimit
            importer.ExecutionSource = Relativity.ExecutionSource.Rdc
            SetWorkingDirectory(ImageLoadFile.FileName)
            frm.ProcessObserver = importer.ProcessObserver
            frm.ProcessController = importer.ProcessController
            frm.Text = "Import Image File Progress ..."
            frm.ErrorFileExtension = "OPT"
            frm.Show()
            frm.ProcessID = _processPool.StartProcess(importer)
            CursorDefault()
        End Function

        Public Async Function StartSearch(ByVal exportFile As ExportFile) As Task
            CursorWait()
            If Not Await Me.IsConnected() Then
                CursorDefault()
                Return
            End If
            Dim frm As kCura.Windows.Process.ProgressForm = CreateProgressForm()
            frm.StatusRefreshRate = 0
            Dim exporter As New kCura.WinEDDS.ExportSearchProcess(new ExportFileFormatterFactory(), New ExportConfig)
            exporter.UserNotification = New FormsUserNotification()
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
            _processPool.StartProcess(exporter)
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
        Public Sub SaveImageLoadFile(ByVal imageLoadFile As ImageLoadFile, ByVal path As String)
            SaveFileObject(imageLoadFile, path)
        End Sub

        Public Sub SaveExportFile(ByVal exportFile As ExportFile, ByVal path As String)
            SaveFileObject(exportFile, path)
        End Sub

        Public Sub SaveLoadFile(ByVal loadFile As LoadFile, ByVal path As String)
            SaveFileObject(Utility.ConvertOverwriteDestinationToLegacyValues(loadFile), path)
        End Sub



        Private Sub SaveFileObject(ByVal fileObject As Object, ByVal path As String)
            Dim sw As New System.IO.StreamWriter(path)
            Dim serializer As New System.Runtime.Serialization.Formatters.Soap.SoapFormatter
            Try
                serializer.Serialize(sw.BaseStream, fileObject)
                sw.Close()
            Catch ex As System.Exception
                MsgBox("Save Failed" + vbCrLf + ex.Message, MsgBoxStyle.Critical)
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

        Public Async Function ReadLoadFile(ByVal loadFile As LoadFile, ByVal path As String, ByVal isSilent As Boolean) As Task(Of LoadFile)
            kCura.WinEDDS.Service.Settings.SendEmailOnLoadCompletion = Config.SendNotificationOnImportCompletionByDefault
            If Not Await Me.EnsureConnection Then Return Nothing
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
                If Not isSilent Then MsgBox("Load Failed", MsgBoxStyle.Critical, "Relativity Desktop Client")
                'TODO: Log Exception
                Return Nothing
            End Try
            tempLoadFile.OverwriteDestination = Utility.ConvertLegacyOverwriteDestinationValueToEnum(tempLoadFile.OverwriteDestination)
            tempLoadFile.CaseInfo = Me.SelectedCaseInfo
            tempLoadFile.CopyFilesToDocumentRepository = loadFile.CopyFilesToDocumentRepository
            tempLoadFile.SelectedCasePath = Me.SelectedCaseInfo.DocumentPath
            tempLoadFile.Credentials = Await Me.GetCredentialsAsync()
            tempLoadFile.DestinationFolderID = loadFile.DestinationFolderID
            tempLoadFile.SelectedIdentifierField = (Await Me.CurrentFields(ArtifactTypeID, True)).Item((Await Me.GetCaseIdentifierFields(ArtifactTypeID))(0))
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
                    If Not fieldMapItem.DocumentField Is Nothing AndAlso
                    fieldMapItem.NativeFileColumnIndex >= 0 AndAlso
                    fieldMapItem.DocumentField.FieldName.ToLower = "group identifier" Then
                        tempLoadFile.GroupIdentifierColumn = Me.GetColumnHeadersFromLoadFile(tempLoadFile, tempLoadFile.FirstLineContainsHeaders)(fieldMapItem.NativeFileColumnIndex)
                        'mapItemToRemove = fieldMapItem
                    End If
                    If Not fieldMapItem.DocumentField Is Nothing Then
                        Try
                            Dim thisField As DocumentField = (Await Me.CurrentFields(tempLoadFile.ArtifactTypeID)).Item(fieldMapItem.DocumentField.FieldID)
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

        Public Async Function ReadImageLoadFile(ByVal path As String) As Task(Of ImageLoadFile)
            kCura.WinEDDS.Service.Settings.SendEmailOnLoadCompletion = Config.SendNotificationOnImportCompletionByDefault
            Dim sr As New System.IO.StreamReader(path)
            Dim retval As ImageLoadFile
            Dim deserializer As New System.Runtime.Serialization.Formatters.Soap.SoapFormatter
            Try
                retval = DirectCast(deserializer.Deserialize(sr.BaseStream), ImageLoadFile)
                sr.Close()
            Catch ex As System.Exception
                MsgBox("Load Failed", MsgBoxStyle.Critical, "Relativity Desktop Client")
                'TODO: Log Exception
                Return Nothing
            End Try
            retval.CaseInfo = Me.SelectedCaseInfo
            retval.DestinationFolderID = Me.SelectedCaseInfo.RootFolderID
            retval.Credential = Await Me.GetCredentialsAsync()
            Return retval
        End Function

#End Region

        Public Enum CredentialCheckResult
            NotSet = 0
            Success = 1
            Fail = 2
            AccessDisabled = 3
            InvalidClientCredentials = 4
            FailToConnectToIdentityServer = 5
        End Enum

        Private _lastCredentialCheckResult As CredentialCheckResult = CredentialCheckResult.NotSet

        Public ReadOnly Property LastCredentialCheckResult As CredentialCheckResult
            Get
                Return _lastCredentialCheckResult
            End Get
        End Property

#Region "Login"
        ''' <summary>
        ''' Attempt to establish a trusted and authenticated connection to Relativity. Success = Case List, Failure = Login Prompt
        ''' </summary>
        ''' <param name="callingForm">The calling form to be Hooked (what is hooked?) by the Enhance Menu Provider</param>
        ''' <returns>A fresh LoginForm</returns>
        ''' <remarks></remarks>
        Friend Async Function AttemptLogin(ByVal callingForm As Form) As Task
            Dim defaultCredentialResult As Application.CredentialCheckResult = AttemptWindowsAuthentication()
            SetImplicitCredentialProvider()
            Select Case (defaultCredentialResult)
                Case Application.CredentialCheckResult.AccessDisabled
                    MessageBox.Show(Application.ACCESS_DISABLED_MESSAGE, Application.RDC_ERROR_TITLE)
                Case Application.CredentialCheckResult.Fail
                    CheckVersion(System.Net.CredentialCache.DefaultCredentials)
                    Await NewLoginAsync()
                Case Application.CredentialCheckResult.Success
                    LogOn()
                    If (Not _caseSelected) Then
                        Await OpenCase()
                    End If
                    EnhancedMenuProvider.Hook(callingForm)
            End Select

        End Function


        ''' <summary>
        ''' Try to log in using Windows Authentication
        ''' </summary>
        ''' <returns>
        ''' true if successful, else false
        ''' </returns>
        Friend Function AttemptWindowsAuthentication() As CredentialCheckResult
            Dim myHttpWebRequest As System.Net.HttpWebRequest
            Dim cred As System.Net.NetworkCredential
            Dim relativityManager As kCura.WinEDDS.Service.RelativityManager

            cred = DirectCast(System.Net.CredentialCache.DefaultCredentials, System.Net.NetworkCredential)
            myHttpWebRequest = DirectCast(System.Net.WebRequest.Create(kCura.WinEDDS.Config.WebServiceURL & "\RelativityManager.asmx"), System.Net.HttpWebRequest)
            myHttpWebRequest.Credentials = System.Net.CredentialCache.DefaultCredentials
            Try
                relativityManager = New kCura.WinEDDS.Service.RelativityManager(cred, _CookieContainer)
                If relativityManager.ValidateSuccessfulLogin Then
                    CheckVersion(System.Net.CredentialCache.DefaultCredentials)
                    RelativityWebApiCredentialsProvider.Instance().SetProvider(New UserCredentialsProvider(cred))
                    kCura.WinEDDS.Service.Settings.AuthenticationToken = New kCura.WinEDDS.Service.UserManager(cred, _CookieContainer).GenerateDistributedAuthenticationToken()
                    _lastCredentialCheckResult = CredentialCheckResult.Success
                Else
                    _lastCredentialCheckResult = CredentialCheckResult.Fail
                End If
            Catch ex As System.Exception
                If IsAccessDisabledException(ex) Then
                    _lastCredentialCheckResult = CredentialCheckResult.AccessDisabled
                Else
                    _lastCredentialCheckResult = CredentialCheckResult.Fail
                End If
            End Try

            Return _lastCredentialCheckResult
        End Function

        ''' <summary>
        ''' Loads the workspace permissions into UserHasExportPermission and UserHasImportPermission.
        ''' </summary>
        Public Async Function LoadWorkspacePermissions() As Task
            UserHasExportPermission = New kCura.WinEDDS.Service.ExportManager(Await GetCredentialsAsync(), CookieContainer).HasExportPermissions(SelectedCaseInfo.ArtifactID)
            UserHasImportPermission = New kCura.WinEDDS.Service.BulkImportManager(Await GetCredentialsAsync(), CookieContainer).HasImportPermissions(SelectedCaseInfo.ArtifactID)

            'additionally load config and permissions for staging explorer
			Try
				UserHasStagingPermission = Await Me.CanUserAccessStagingExplorer(Await GetCredentialsAsync())
			Catch ex As Exception
				UserHasStagingPermission = False
			End Try
            
        End Function

        Private Sub CertificatePromptForm_Deny_Click() Handles _certificatePromptForm.DenyUntrustedCertificates
            '' The user does not trust the certificate. Prompt them for a new server by showing the settings dialog
            NewOptions()
        End Sub

        Private Async Sub CertificatePromptForm_Allow_Click() Handles _certificatePromptForm.AllowUntrustedCertificates
            '' The magical line that allows untrusted certificates
            ServicePointManager.ServerCertificateValidationCallback = Function(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) True
            Await AttemptLogin(_certificatePromptForm)
        End Sub

        Private Async Sub On_TokenRetrieved(source As ITokenProvider, args As ITokenResponseEventArgs)
            Dim credential = Await GetCredentialsAsync()
            Dim userManager As New kCura.WinEDDS.Service.UserManager(credential, _CookieContainer)
            Dim relativityManager As New kCura.WinEDDS.Service.RelativityManager(credential, _CookieContainer)
            Try
                CheckVersion(credential)
            Catch ex As System.Net.WebException
                HandleWebException(ex)
                _lastCredentialCheckResult = CredentialCheckResult.Fail
                Return
            Catch ex As System.Exception
                Me.ChangeWebServiceUrl("An error occurred while validating the Relativity WebAPI URL.  Check the URL and try again?")
                _lastCredentialCheckResult = CredentialCheckResult.Fail
                Return
            End Try

            Try
                If userManager.Login(credential.UserName, credential.Password) Then

                    Dim locale As New System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.LCID, True)
                    locale.NumberFormat.CurrencySymbol = relativityManager.RetrieveCurrencySymbol
                    System.Threading.Thread.CurrentThread.CurrentCulture = locale

                    kCura.WinEDDS.Service.Settings.AuthenticationToken = userManager.GenerateDistributedAuthenticationToken()
                    If OpenCaseSelector Then Await OpenCase()
                    _timeZoneOffset = 0                                                         'New kCura.WinEDDS.Service.RelativityManager(cred, _cookieContainer).GetServerTimezoneOffset
                    _lastCredentialCheckResult = CredentialCheckResult.Success
                    'This was created specifically for raising an event after login success for RDC forms authentication 
                    LogOnForm()
                Else
                    Await Me.NewLoginAsync()
                    _lastCredentialCheckResult = CredentialCheckResult.Fail
                End If
            Catch ex As System.Exception
                Dim errorDialog As New ErrorDialog
                If IsAccessDisabledException(ex) Then
                    errorDialog.Text = "Account Disabled"
                    errorDialog.InitializeSoapExceptionWithCustomMessage(DirectCast(ex, System.Web.Services.Protocols.SoapException),
                     ACCESS_DISABLED_MESSAGE)
                    _lastCredentialCheckResult = CredentialCheckResult.AccessDisabled
                Else
                    If Not ex.Message.IndexOf("Invalid License.") = -1 Then
                        errorDialog.Text = "Invalid License."
                    ElseIf (Not ex.Message.IndexOf("A library (dll)") = -1) OrElse (Not ex.Message.IndexOf("Relativity is temporarily unavailable.") = -1) Then
                        errorDialog.Text = "Invalid Assembly."
                    Else
                        errorDialog.Text = "Unrecognized login error."
                    End If
                    _lastCredentialCheckResult = CredentialCheckResult.Fail
                    errorDialog.Initialize(ex, errorDialog.Text)
                End If
                If errorDialog.ShowDialog = DialogResult.OK Then
                    NewLogin()
                Else
                    ExitApplication()
                End If
            End Try
        End Sub

        Public Sub HandleWebException(ex As WebException)

            If Not ex.Message.IndexOf("The remote name could not be resolved") = -1 AndAlso ex.Source = "System" Then
                Me.ChangeWebServiceUrl("The current Relativity WebAPI URL could not be resolved. Try a new URL?")
            ElseIf Not ex.Message.IndexOf("The request failed with HTTP status 401") = -1 AndAlso ex.Source = "System.Web.Services" Then
                Me.ChangeWebServiceUrl("The current Relativity WebAPI URL was resolved but is not configured correctly. Try a new URL?")
            ElseIf Not ex.Message.IndexOf("The request failed with HTTP status 404") = -1 AndAlso ex.Source = "System.Web.Services" Then
                Me.ChangeWebServiceUrl("The current Relativity WebAPI URL was not found. Try a new URL?")
            Else
                Me.ChangeWebServiceUrl("An error occurred while validating the Relativity WebAPI URL.  Check the URL and try again?")
            End If
        End Sub

        Public Function IsAccessDisabledException(ByVal ex As System.Exception) As Boolean
            If TypeOf ex Is System.Web.Services.Protocols.SoapException Then
                Dim soapEx As System.Web.Services.Protocols.SoapException = DirectCast(ex, System.Web.Services.Protocols.SoapException)
                Dim exceptionType As String = String.Empty
                Try
                    exceptionType = soapEx.Detail("ExceptionType").InnerText
                Catch caughtEx As Exception
                    ' Not a properly formatted SoapException
                    Return False
                End Try
                Return exceptionType = "Relativity.Core.Exception.RelativityAccessDisabledException"
            Else
                Return False
            End If
        End Function

        Public Function DoLogin() As CredentialCheckResult
            Try
                Dim netCreds As System.Net.NetworkCredential = RelativityWebApiCredentialsProvider.Instance.GetCredentials()
                Dim userManager As New kCura.WinEDDS.Service.UserManager(netCreds, _CookieContainer)
                CheckVersion(netCreds)
                If userManager.Login(netCreds.UserName, netCreds.Password) Then
                    kCura.WinEDDS.Service.Settings.AuthenticationToken = userManager.GenerateDistributedAuthenticationToken()
                    _timeZoneOffset = 0                                          'New kCura.WinEDDS.Service.RelativityManager(cred, _cookieContainer).GetServerTimezoneOffset
                    _lastCredentialCheckResult = CredentialCheckResult.Success
                Else
                    _lastCredentialCheckResult = CredentialCheckResult.Fail
                End If
            Catch ex As IdentityProviderConnectionException
                _lastCredentialCheckResult = CredentialCheckResult.FailToConnectToIdentityServer
            Catch ex As OAuth2ClientException
                _lastCredentialCheckResult = CredentialCheckResult.InvalidClientCredentials
            Catch ex As Exception
                If IsAccessDisabledException(ex) Then
                    _lastCredentialCheckResult = CredentialCheckResult.AccessDisabled
                Else
                    _lastCredentialCheckResult = CredentialCheckResult.Fail
                End If
            End Try
            Return _lastCredentialCheckResult
        End Function

        Public Function DoOAuthLogin(ByVal clientId As String, ByVal clientSecret As String, ByVal stsUrl As Uri) As CredentialCheckResult
            Dim credProvider As ICredentialsProvider = New OAuth2ClientCredentials(stsUrl, clientId, clientSecret)
            RelativityWebApiCredentialsProvider.Instance().SetProvider(credProvider)

            _lastCredentialCheckResult = DoLogin()

            Return _lastCredentialCheckResult
        End Function

        Public Function DoOAuthLogin(ByVal clientId As String, ByVal clientSecret As String) As CredentialCheckResult
            Dim urlString As String = String.Format("{0}/{1}", GetIdentityServerLocation(), "connect/token")
            Dim stsUrl As Uri = New Uri(urlString)

            _lastCredentialCheckResult = DoOAuthLogin(clientId, clientSecret, stsUrl)

            Return _lastCredentialCheckResult
        End Function

        Public Function GetIdentityServerLocation() As String
            Dim tempCred As System.Net.NetworkCredential = DirectCast(System.Net.CredentialCache.DefaultCredentials, System.Net.NetworkCredential)
            Dim relManager As Service.RelativityManager = New RelativityManager(tempCred, _CookieContainer)
            Dim urlString As String = String.Format("{0}/{1}", relManager.GetRelativityUrl(), "Identity")
            Return urlString
        End Function

        Private Async Function Reconnect() As Task
            Dim credentials As NetworkCredential = Await GetCredentialsAsync()
            Dim userManager As New kCura.WinEDDS.Service.UserManager(credentials, _CookieContainer)
            If userManager.Login(credentials.UserName, credentials.Password) Then
                kCura.WinEDDS.Service.Settings.AuthenticationToken = userManager.GenerateDistributedAuthenticationToken()
            End If

        End Function

        Public Sub ChangeWebServiceUrl(ByVal message As String)
            If MsgBox(message, MsgBoxStyle.YesNo, "Relativity Desktop Client") = MsgBoxResult.Yes Then
                Dim url As String = InputBox("Enter New URL:", DefaultResponse:=kCura.WinEDDS.Config.WebServiceURL)
                If url <> String.Empty Then
                    kCura.WinEDDS.Config.WebServiceURL = url
                    OpenCaseSelector = True
                    RaiseEvent OnEvent(New AppEvent(AppEvent.AppEventType.LogOnRequested))
                Else
                    ExitApplication()
                End If
            Else
                ExitApplication()
            End If
        End Sub

        Private Sub CheckVersion(ByVal credential As Net.ICredentials)
            Dim relativityManager As New kCura.WinEDDS.Service.RelativityManager(DirectCast(credential, System.Net.NetworkCredential), _CookieContainer)
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
                MsgBox(String.Format("Your version of the Relativity Desktop Client ({0}) is out of date. Please make sure you are running correct RDC version ({1}) or specified correct WebService URL for Relativity.", Me.GetDisplayAssemblyVersion(), relVersionString), MsgBoxStyle.Critical, "WinRelativity Version Mismatch")
                ExitApplication()
            Else
                Exit Sub
            End If
        End Sub
#End Region

#Region "Logout"
        Public Async Function Logout() As Task
            Try
                Dim userManager As New kCura.WinEDDS.Service.UserManager(Await GetCredentialsAsync(), _CookieContainer)
                userManager.Logout()
            Catch ex As Exception

            End Try

        End Function
#End Region

#Region "System Configuration"
        Public Function GetDisplayAssemblyVersion() As String
            Return System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString
        End Function

        Public Async Function GetSystemConfiguration() As Task(Of System.Data.DataTable)
            Return New kCura.WinEDDS.Service.RelativityManager(Await Me.GetCredentialsAsync(), Me.CookieContainer).RetrieveRdcConfiguration().Tables(0)
        End Function
#End Region

        Public Overridable Async Function GetProductionPrecendenceList(ByVal caseInfo As Relativity.CaseInfo) As Task(Of System.Data.DataTable)
            Dim productionManager As kCura.WinEDDS.Service.ProductionManager
            Dim dt As System.Data.DataTable
            Try
                productionManager = New kCura.WinEDDS.Service.ProductionManager(Await Me.GetCredentialsAsync(), _CookieContainer)
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

            Dim aboutFrm As New AboutForm()
            aboutFrm.ShowDialog()

        End Sub

        Public Async Function DoHelp() As Task
            Dim cloudIsEnabled As Boolean = Await GetIsCloudInstance()
            'Default cloud setting

            Dim urlPrefix As String = "https://help.kcura.com/"

            'Go to appropriate documentation site
            If cloudIsEnabled Then
                Process.Start(urlPrefix & "RelativityOne/Content/Relativity/Relativity_Desktop_Client/Relativity_Desktop_Client.htm")
            Else
                Dim v As System.Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version
                Dim majMin As String = String.Format("{0}.{1}", v.Major, v.Minor)
                Process.Start(urlPrefix & majMin & "/#Relativity/Relativity_Desktop_Client/Relativity_Desktop_Client.htm")
            End If

        End Function

        Public Async Function GetIsCloudInstance()  As Task(Of System.Boolean) 
            Dim cloudIsEnabled As Boolean = False
            'Get configuration information
            Dim configTable As System.Data.DataTable = Await GetSystemConfiguration()

            'Get cloud instance setting
            Dim foundRows() As System.Data.DataRow = configTable.Select("Name = 'CloudInstance'")

            If foundRows.Length > 0 Then
                Dim foundRow As System.Data.DataRow = foundRows.ElementAt(0)
                cloudIsEnabled = CType(foundRow.ItemArray.ElementAt(2), Boolean)
            End If
           
            Return cloudIsEnabled
        End Function

        Public Function Login(authOptions As AuthenticationOptions) As Application.CredentialCheckResult
            Dim loginResult As Application.CredentialCheckResult

            If Not String.IsNullOrEmpty(authOptions.UserName) Then
                Dim cred As New UserCredentialsProvider(authOptions.UserName, authOptions.Password)
                RelativityWebApiCredentialsProvider.Instance().SetProvider(cred)
                loginResult = DoLogin()
            Else

                loginResult = DoOAuthLogin(authOptions.ClientId, authOptions.ClientSecret)
            End If
            Return loginResult
        End Function

        Private Async Function CanUserAccessStagingExplorer(credentials As NetworkCredential) As Task(Of System.Boolean)
            Dim relativityCredentials = New BearerTokenCredentials(credentials.Password)

            Dim baseUri = New Uri(WinEDDS.Config.WebServiceURL)
            Dim settings = New ServiceFactorySettings(New Uri($"https://{baseUri.Host}/Relativity.Services"), New Uri($"https://{baseUri.Host}/Relativity.Rest/api"), relativityCredentials)
            Dim factory = New ServiceFactory(settings)

            Using manager As IStagingPermissionsService = factory.CreateProxy(Of IStagingPermissionsService)
                Dim userCanRunStagingExplorer = Await manager.UserCanRunStagingExplorer()
                Return userCanRunStagingExplorer
            End Using
        End Function
    End Class
End Namespace

