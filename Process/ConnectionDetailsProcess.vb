Namespace kCura.WinEDDS
	Public Class ConnectionDetailsProcess
		Inherits kCura.Windows.Process.ProcessBase

		Private WithEvents _searchExporter As kCura.WinEDDS.Exporter
		Private _startTime As System.DateTime
		Private _errorCount As Int32
		Private _warningCount As Int32
		Private _uploadModeText As String = Nothing
		Private _defaultDestinationDirectory As String
		Private _credential As Net.NetworkCredential
		Private _cookieContainer As Net.CookieContainer
		Private _caseInfo As kCura.EDDS.Types.CaseInfo

		Public Sub New(ByVal credential As Net.NetworkCredential, ByVal cookieContainer As Net.CookieContainer, ByVal caseInfo As kCura.EDDS.Types.CaseInfo)
			MyBase.New()
			_credential = credential
			_cookieContainer = cookieContainer
			_caseInfo = caseInfo
			_defaultDestinationDirectory = caseInfo.DocumentPath
		End Sub

		Public Property DefaultDestinationDirectory() As String
			Get
				Return _defaultDestinationDirectory
			End Get
			Set(ByVal value As String)
				_defaultDestinationDirectory = value
			End Set
		End Property

		Protected Overrides Sub Execute()
			Dim bcpEnabled As Boolean = Me.CheckBcp()
			Me.WriteStatus("")
			Me.CheckDownloadHandlerURL()
			Me.WriteStatus("")
			Me.CheckRepositoryConnectivity()
			Me.WriteStatus("")
			Me.CheckWebBasedRepositoryLifecycle()
			Me.WriteStatus("")
		End Sub

		Private Function CheckRepositoryConnectivity() As String
			Me.WriteStatus("Checking default repository access: " & _caseInfo.DocumentPath)
			Dim isDirect As Boolean = False
			Try
				If Not System.IO.Directory.Exists(_caseInfo.DocumentPath) Then
					System.IO.Directory.CreateDirectory(_caseInfo.DocumentPath)
				End If
				Me.WriteStatus("Default repository detected in Direct Mode")
				isDirect = True
			Catch ex As Exception
				Me.WriteStatus("No direct access to repository - proceeding in Web Mode")
				Me.WriteStatus("Actual error: " & ex.ToString)
			End Try
			Dim id As String = System.Guid.NewGuid.ToString
			If isDirect Then
				Try
					System.IO.File.Create(id).Close()
					Me.WriteStatus("Temporary file created - proceeding in Direct Mode")
				Catch ex As Exception
					Me.WriteStatus("Cannot create file in repository - proceeding in Web Mode")
					Me.WriteStatus("Actual error: " & ex.ToString)
					isDirect = False
				End Try
			End If
			If isDirect Then
				Try
					System.IO.File.Delete(id)
					Me.WriteStatus("Temporary file deleted - proceeding in Direct Mode")
				Catch ex As Exception
					Me.WriteStatus("Cannot delete file in repository - proceeding in Web Mode")
					Me.WriteStatus("Actual error: " & ex.ToString)
					isDirect = False
				End Try
			End If
			If isDirect Then
				Try
					System.IO.Directory.CreateDirectory(id)
					Me.WriteStatus("Subdirectory created - proceeding in Direct Mode")
				Catch ex As Exception
					Me.WriteStatus("Cannot create file in repository - proceeding in Web Mode")
					Me.WriteStatus("Actual error: " & ex.ToString)
					isDirect = False
				End Try
			End If
			If isDirect Then
				Try
					System.IO.Directory.Delete(id)
					Me.WriteStatus("Subdirectory deleted - proceeding in Direct Mode")
				Catch ex As Exception
					Me.WriteStatus("Cannot delete directory - proceeding in Web Mode")
					Me.WriteStatus("Actual error: " & ex.ToString)
					isDirect = False
				End Try
			End If
			If isDirect Then
				Me.WriteStatus("Direct Mode test: Passed")
				Me.WriteStatus("Uploading in Direct Mode")
			Else
				Me.WriteStatus("Direct Mode test: Failed")
				Me.WriteStatus("Uploading in Web Mode")
			End If
		End Function

		Private Function CheckWebBasedRepositoryLifecycle() As Boolean
			Me.WriteStatus("Web Mode Connectivity:")
			Dim uploader As kCura.WinEDDS.FileUploader
			Try
				uploader = New kCura.WinEDDS.FileUploader(_credential, _caseInfo.ArtifactID, _caseInfo.DocumentPath, _cookieContainer, False)
			Catch ex As Exception
				Me.WriteStatus("Error creating uploader object: " & ex.ToString)
				Return False
			End Try
			Try
				'uploader.
				uploader.UploaderType = FileUploader.Type.Web
			Catch ex As Exception
				Me.WriteStatus("Cannot change uploader type to web")
				Me.WriteStatus("Actual error: " & ex.ToString)
				Return False
			End Try
			Dim path As String
			Try
				path = System.IO.Path.GetTempFileName
				Dim sw As New System.IO.StreamWriter(path)
				sw.WriteLine("This is only a test")
				sw.Close()
			Catch ex As Exception
				Me.WriteStatus("Error creating local temp file")
				Me.WriteStatus("Actual error: " & ex.ToString)
				Return False
			End Try
			Dim dest As String
			Try
				dest = uploader.UploadFile(path, _caseInfo.ArtifactID)
				Me.WriteStatus("Web mode upload successful")
			Catch ex As Exception
				Me.WriteStatus("Error uploading local temp file in web mode")
				Me.WriteStatus("Actual error: " & ex.ToString)
				Return False
			End Try
			Dim downloader As kCura.WinEDDS.FileDownloader
			Try
				downloader = New kCura.WinEDDS.FileDownloader(_credential, _caseInfo.DocumentPath, kCura.Utility.URI.GetFullyQualifiedPath(_caseInfo.DownloadHandlerURL, New System.Uri(kCura.WinEDDS.Config.WebServiceURL)), _cookieContainer, kCura.WinEDDS.Service.Settings.AuthenticationToken)
			Catch ex As Exception
				Me.WriteStatus("Error initializing file downloader")
				Me.WriteStatus("Actual error: " & ex.ToString)
				Return False
			End Try
			Try
				downloader.DownloadTempFile(path, dest, _caseInfo.ArtifactID.ToString)
				Me.WriteStatus("Temporary file successfully downloaded")
			Catch ex As Exception
				Me.WriteStatus("Error downloading file from repository through Distributed")
				Me.WriteStatus("Actual error: " & ex.ToString)
				Return False
			End Try
			Try
				Dim gateway As New kCura.WinEDDS.Service.FileIO(_credential, _cookieContainer)
				gateway.RemoveTempFile(_caseInfo.ArtifactID, dest)
				Me.WriteStatus("Temp file successfully removed from repository")
			Catch ex As Exception
				Me.WriteStatus("Error downloading file from repository through Distributed")
				Me.WriteStatus("Actual error: " & ex.ToString)
				Return False
			End Try
			Try
				kCura.Utility.File.Delete(path)
			Catch ex As Exception
			End Try
			Try
				Me.WriteStatus("Retrieving default repository drive information:")
				Dim s As String()() = New kCura.WinEDDS.Service.FileIO(_credential, _cookieContainer).GetDefaultRepositorySpaceReport(_caseInfo.ArtifactID)
				Me.WriteStatus("Success - report follows:")
				Me.WriteOutReportString(s)
			Catch ex As Exception
				Me.WriteStatus("Error retrieving space information on default repository - make sure relativity service account has full rights to the repository if you want to see this report")
				Me.WriteStatus("Exact error: " & ex.ToString)
				Return False
			End Try
			Me.WriteStatus("Web based file repository lifecycle successfully completed")
			Return True
		End Function

		Private Function CheckDownloadHandlerURL() As String
			Me.WriteStatus("Validate Download URL:")
			Dim downloadUrl As String = kCura.Utility.URI.GetFullyQualifiedPath(_caseInfo.DownloadHandlerURL, New System.Uri(kCura.WinEDDS.Config.WebServiceURL))
			Dim token As String = kCura.WinEDDS.Service.Settings.AuthenticationToken
			Me.WriteStatus(downloadUrl)
			Dim myReq As System.Net.HttpWebRequest = DirectCast(System.Net.WebRequest.Create(downloadUrl & "AccessDenied.aspx"), System.Net.HttpWebRequest)
			Try
				myReq.GetResponse()
				Me.WriteStatus("URL validated")
			Catch ex As System.Exception
				Me.WriteStatus("Cannot find URL")
				Me.WriteStatus(ex.ToString)
			End Try
		End Function

		Private Function CheckBcp() As Boolean
			Me.WriteStatus("Checking Bulk Share Configuration")
			Dim gateway As New kCura.WinEDDS.Service.FileIO(_credential, _cookieContainer)
			Dim bcpPath As String
			Try
				bcpPath = gateway.GetBcpSharePath(_caseInfo.ArtifactID)
				Me.WriteStatus("Selected BCP Path: " & bcpPath)
			Catch ex As System.Exception
				Me.WriteStatus("Error retrieving BCP share path - WebAPI error")
				Dim ensure As String = "Ensure"
				If WinEDDS.Config.EnableSingleModeImport Then
					Me.WriteStatus("Loads will happen in Single mode - this is less than optimal.  To upgrade,")
					ensure = "ensure"
				End If
				Me.WriteStatus(ensure & " that relativity service account has rights to create/delete files and subdirectories in the bcp folder")
				Me.WriteStatus("Exact error: " & ex.ToString)
				Return False
			End Try
			Try
				If Not System.IO.Directory.Exists(bcpPath) Then
					System.IO.Directory.CreateDirectory(bcpPath)
				End If
				Dim path As String = bcpPath.TrimEnd("\"c) & "\"
				System.IO.File.Create(path & "123").Close()
				System.IO.File.Delete(path & "123")
				Me.WriteStatus("Your account (" & System.Security.Principal.WindowsIdentity.GetCurrent.Name & ") has direct access to BCPPath folder")
			Catch ex As Exception
				Me.WriteStatus("Valid: No direct access to bcp folder from direct account - using relativity service account and web services for access")
				'Me.WriteStatus("Exact error: " & ex.ToString)
			End Try
			Try
				Me.WriteStatus("Validating bulk insert rights")
				If gateway.ValidateBcpShare(_caseInfo.ArtifactID) Then
					Me.WriteStatus("Bulk insert rights validated")
				Else
					Me.WriteStatus("Bulk share configured incorrectly or does not exist")
				End If
			Catch ex As Exception
				Me.WriteStatus("Error running bulk insert")
				Me.WriteStatus("Ensure that EDDSDBO login has bulk admin rights on case DB")
				Me.WriteStatus("Ensure that the security account that SQL Server is running under has rights to the selected BCP share")
				Dim text As String = ex.ToString
				If TypeOf ex Is System.Web.Services.Protocols.SoapException Then text = System.Web.HttpUtility.HtmlDecode(text)
				Me.WriteStatus("Exact error: " & text)
				Return False
			End Try
			Try
				Me.WriteStatus("Retrieving bulk directory drive information:")
				Dim s As String()() = gateway.GetBcpShareSpaceReport(_caseInfo.ArtifactID)
				Me.WriteStatus("Success - report follows:")
				Me.WriteOutReportString(s)
			Catch ex As Exception
				Me.WriteStatus("Error retrieving space information on bulk share - make sure relativity service account has full rights to the share if you want to see this report")
				Me.WriteStatus("Exact error: " & ex.ToString)
				Return False
			End Try
			Return True
		End Function

		Private Sub WriteOutReportString(ByVal input As String()())
			For Each line As String() In input
				Me.WriteStatus(vbTab & line(0) & ": " & line(1))
			Next
		End Sub

		Private Sub WriteStatus(ByVal message As String)
			Me.ProcessObserver.RaiseStatusEvent("", message)
		End Sub

	End Class
End Namespace

