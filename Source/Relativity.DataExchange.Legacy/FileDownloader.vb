Imports Relativity.DataExchange
Imports Relativity.DataExchange.Service

Namespace kCura.WinEDDS
	Public Class FileDownloader
		Private ReadOnly _fileIoServiceProxy As kCura.WinEDDS.Service.FileIO
		Private ReadOnly _credentials As Net.NetworkCredential
		Private ReadOnly _downloadUrl As String
		Private ReadOnly _cookieContainer As System.Net.CookieContainer
		Private ReadOnly _userManagerServiceProxy As kCura.WinEDDS.Service.UserManager
		
		Private _fileHelper As Global.Relativity.DataExchange.Io.IFile
		Private ReadOnly Property FileHelper() As Global.Relativity.DataExchange.Io.IFile
			Get
				If(_fileHelper Is Nothing) Then
					_fileHelper = Global.Relativity.DataExchange.Io.FileSystem.Instance.File
				End If
				
				Return _fileHelper
			End Get
		End Property

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal downloadHandlerUrl As String, ByVal cookieContainer As System.Net.CookieContainer)
			_fileIoServiceProxy = New kCura.WinEDDS.Service.FileIO(credentials, cookieContainer) With {
				.Timeout = Int32.MaxValue
			}

			_cookieContainer = cookieContainer
			_credentials = credentials
			_downloadUrl = UrlHelper.GetBaseUrlAndCombine(AppSettings.Instance.WebApiServiceUrl, downloadHandlerUrl)
			_userManagerServiceProxy = New kCura.WinEDDS.Service.UserManager(credentials, cookieContainer)
		End Sub

		Public Function MoveTempFileToLocal(ByVal localFilePath As String, ByVal remoteFileGuid As String, ByVal caseInfo As CaseInfo) As Boolean
			Return MoveTempFileToLocal(localFilePath, remoteFileGuid, caseInfo, True)
		End Function

		Public Function MoveTempFileToLocal(ByVal localFilePath As String, ByVal remoteFileGuid As String, ByVal caseInfo As CaseInfo, ByVal removeRemoteTempFile As Boolean) As Boolean
			Dim retval As Boolean = WebDownloadFile(localFilePath, remoteFileGuid, caseInfo.ArtifactID.ToString)

			If removeRemoteTempFile Then
				_fileIoServiceProxy.RemoveTempFile(caseInfo.ArtifactID, remoteFileGuid)
			End If

			Return retval
		End Function

		Public Sub RemoveRemoteTempFile(ByVal remoteFileGuid As String, ByVal caseInfo As CaseInfo)
			_fileIoServiceProxy.RemoveTempFile(caseInfo.ArtifactID, remoteFileGuid)
		End Sub

		Private Function WebDownloadFile(ByVal localFilePath As String, ByVal remoteFileGuid As String, ByVal appID As String) As Boolean
			Dim tries As Int32 = 0
			While tries < AppSettings.Instance.MaxReloginTries
				Try
					Return DoWebDownloadFile(localFilePath, remoteFileGuid, appID)
				Catch ex As DistributedReLoginException
					tries += 1
					RaiseEvent UploadStatusEvent(String.Format("Download Manager credentials failed.  Attempting to re-login ({0} of {1})", tries, AppSettings.Instance.MaxReloginTries))
					_userManagerServiceProxy.AttemptReLogin()
				End Try
			End While
			RaiseEvent UploadStatusEvent("Error Downloading File")
			Throw New ApplicationException("Error Downloading File: Unable to authenticate against Distributed server" & vbNewLine, New DistributedReLoginException)
		End Function

		Private Function DoWebDownloadFile(ByVal localFilePath As String, ByVal remoteFileGuid As String, ByVal appID As String) As Boolean
			Dim localStream As System.IO.Stream = Nothing
			Try
				Dim downloadUrl As String = _downloadUrl.TrimEnd("/"c) & "/"
				Dim remoteuri As String = $"{downloadUrl}Download.aspx?ArtifactID=-1&GUID={remoteFileGuid}&AppID={appID}"
				If kCura.WinEDDS.Service.Settings.AuthenticationToken <> String.Empty Then
					remoteuri &= String.Format("&AuthenticationToken={0}", kCura.WinEDDS.Service.Settings.AuthenticationToken)
				End If
				Dim httpWebRequest As System.Net.HttpWebRequest = CType(System.Net.HttpWebRequest.Create(remoteuri), System.Net.HttpWebRequest)
				httpWebRequest.Credentials = _credentials
				httpWebRequest.CookieContainer = _cookieContainer
				httpWebRequest.UnsafeAuthenticatedConnectionSharing = True
				Dim webResponse As System.Net.HttpWebResponse = DirectCast(httpWebRequest.GetResponse(), System.Net.HttpWebResponse)
				Dim length As Int64 = 0
				If Not webResponse Is Nothing Then
					length = System.Math.Max(webResponse.ContentLength, 0)
					Dim responseStream As System.IO.Stream = webResponse.GetResponseStream()
					Try
						localStream =FileHelper.Create(localFilePath)
					Catch ex As Exception
						localStream = FileHelper.Create(localFilePath)
					End Try
					Dim buffer(AppSettings.Instance.WebBasedFileDownloadChunkSize - 1) As Byte
					Dim bytesRead As Int32
					While True
						bytesRead = responseStream.Read(buffer, 0, AppSettings.Instance.WebBasedFileDownloadChunkSize)
						If bytesRead <= 0 Then
							Exit While
						End If
						localStream.Write(buffer, 0, bytesRead)
					End While
				End If
				localStream.Close()
				Dim actualLength As Int64 = FileHelper.GetFileSize(localFilePath)
				If length <> actualLength AndAlso length > 0 Then
					Throw New kCura.WinEDDS.Exceptions.WebDownloadCorruptException("Error retrieving data from distributed server; expecting " & length & " bytes and received " & actualLength)
				End If
				Return True
			Catch ex As DistributedReLoginException
				Me.CloseStream(localStream)
				Throw
			Catch ex As System.Net.WebException
				Me.CloseStream(localStream)
				If TypeOf ex.Response Is System.Net.HttpWebResponse Then
					Dim r As System.Net.HttpWebResponse = DirectCast(ex.Response, System.Net.HttpWebResponse)
					If r.StatusCode = Net.HttpStatusCode.Forbidden AndAlso r.StatusDescription.ToLower = "kcuraaccessdeniedmarker" Then
						Throw New DistributedReLoginException
					End If
				End If
				If ex.Message.IndexOf("409") <> -1 Then
					RaiseEvent UploadStatusEvent("Error Downloading File")                  'TODO: Change this to a separate error-type event'
					Throw New ApplicationException("Error Downloading File: the file associated with the guid " & remoteFileGuid & " cannot be found" & vbNewLine, ex)
				Else
					RaiseEvent UploadStatusEvent("Error Downloading File")                  'TODO: Change this to a separate error-type event'
					Throw New ApplicationException("Error Downloading File:", ex)
				End If
			Catch ex As System.Exception
				Me.CloseStream(localStream)
				RaiseEvent UploadStatusEvent("Error Downloading File")               'TODO: Change this to a separate error-type event'
				Throw New ApplicationException("Error Downloading File", ex)
			End Try
		End Function

		''' <summary>
		''' The exception thrown when a Web API failure occurs because the user login must be initialized or has expired.
		''' </summary>
		<Serializable>
		Public Class DistributedReLoginException
			Inherits System.Exception

			''' <summary>
			''' Initializes a new instance of the <see cref="DistributedReLoginException"/> class.
			''' </summary>
			Public Sub New()
				MyBase.New()
			End Sub

			''' <inheritdoc />
			<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
			Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
				MyBase.New(info, context)
			End Sub
		End Class

		Private Sub CloseStream(ByVal stream As System.IO.Stream)
			If stream Is Nothing Then Exit Sub
			Try
				stream.Close()
			Catch
			End Try
		End Sub

		Public Event UploadStatusEvent(ByVal message As String)
	End Class
End Namespace
