Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports kCura.WinEDDS.Service.Export
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Service
Imports Relativity.DataExchange.Transfer

Namespace kCura.WinEDDS
	Public Class FileDownloader
		Implements Service.Export.IExportFileDownloader

		Private _gateway As kCura.WinEDDS.Service.FileIO
		Private _credentials As Net.NetworkCredential
		Private _tapiClient As TapiClient?
		Private _downloadUrl As String
		Private _cookieContainer As System.Net.CookieContainer
		Private _userManager As kCura.WinEDDS.Service.UserManager
		Private Shared _locationAccessMatrix As New ConcurrentDictionary(Of String, TapiClient)
		
		Private _fileHelper As Global.Relativity.DataExchange.Io.IFile
		Public Property FileHelper() As Global.Relativity.DataExchange.Io.IFile Implements IExportFileDownloader.FileHelper
			Get
				If(_fileHelper Is Nothing) Then
					_fileHelper = Global.Relativity.DataExchange.Io.FileSystem.Instance.File
				End If
				
				Return _fileHelper
			End Get
		    Set
				_fileHelper = value
		    End Set
		End Property
		

		Public Sub SetDesintationFolderName(ByVal value As String)
			DestinationFolderPath = value
		End Sub

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal destinationFolderPath As String, ByVal downloadHandlerUrl As String, ByVal cookieContainer As System.Net.CookieContainer)
			_gateway = New kCura.WinEDDS.Service.FileIO(credentials, cookieContainer)

			_cookieContainer = cookieContainer
			_gateway.Credentials = credentials
			_gateway.Timeout = Int32.MaxValue
			_credentials = credentials
			If destinationFolderPath.Chars(destinationFolderPath.Length - 1) <> "\"c Then
				destinationFolderPath &= "\"
			End If
			Me.DestinationFolderPath = destinationFolderPath
			_downloadUrl = UrlHelper.GetBaseUrlAndCombine(AppSettings.Instance.WebApiServiceUrl, downloadHandlerUrl)
			_userManager = New kCura.WinEDDS.Service.UserManager(credentials, cookieContainer)

			If _locationAccessMatrix Is Nothing Then _locationAccessMatrix = New ConcurrentDictionary(Of String, TapiClient)
		End Sub

		Private Function SetTransferMode(ByVal destFolderPath As String) As TapiClient
			Try
				Dim dummyText As String = System.Guid.NewGuid().ToString().Replace("-", String.Empty).Substring(0, 5)
				FileHelper.Create(destFolderPath & dummyText).Close()
				FileHelper.Delete(destFolderPath & dummyText)
				Me.TransferMode = TapiClient.Direct
			Catch ex As System.Exception
				Me.TransferMode = TapiClient.Web
			End Try
			Return Me.TransferMode
		End Function

		Public Property DestinationFolderPath() As String

		Public Property TransferMode As TapiClient
			Get
				Return If(_tapiClient, SetTransferMode(DestinationFolderPath))
			End Get
			Set(ByVal value As TapiClient)
				Dim doevent As Boolean = Not _tapiClient.HasValue OrElse _tapiClient.Value <> value
				_tapiClient = value
				If doevent Then
					RaiseEvent TransferModesChangeEvent(Me, New TapiMultiClientEventArgs(value))
				End If
			End Set
		End Property

		Public ReadOnly Property TransferModes As IList(Of TapiClient) Implements IExportFileDownloaderStatus.TransferModes
			Get
				Dim transferModesList As New List(Of TapiClient)
				If (_tapiClient.HasValue) Then
					transferModesList.Add(_tapiClient.Value)
				End If

				Return transferModesList
			End Get
		End Property



		Public Function DownloadFullTextFile(ByVal localFilePath As String, ByVal artifactID As Int32, ByVal appID As String) As Boolean Implements IExportFileDownloader.DownloadFullTextFile
			Return WebDownloadFile(localFilePath, artifactID, "", appID, Nothing, True, -1, -1, -1)
		End Function

		Public Function DownloadLongTextFile(ByVal localFilePath As String, ByVal artifactID As Int32, ByVal field As ViewFieldInfo, ByVal appId As String) As Boolean Implements IExportFileDownloader.DownloadLongTextFile
			Return WebDownloadFile(localFilePath, artifactID, "", appId, Nothing, False, field.FieldArtifactId, -1, -1)
		End Function

		Public Function DownloadLongTextFile(ByVal localFilePath As String, ByVal artifactID As Int32, ByVal fieldId As Int32, ByVal appId As String) As Boolean
			Return WebDownloadFile(localFilePath, artifactID, "", appId, Nothing, False, fieldId, -1, -1)
		End Function

		Private Function DownloadFile(ByVal localFilePath As String, ByVal remoteFileGuid As String, ByVal remoteLocation As String, ByVal artifactID As Int32, ByVal appID As String, ByVal fileFieldArtifactID As Int32, ByVal fileID As Int32) As Boolean
			If remoteLocation.Length > 7 Then
				If remoteLocation.Substring(0, 7).ToLower = "file://" Then
					remoteLocation = remoteLocation.Substring(7)
				End If
			End If
			Dim remoteLocationKey As String = remoteLocation.Substring(0, remoteLocation.LastIndexOf("\")).TrimEnd("\"c) & "\"
			Dim tapiClient As TapiClient = TapiClient.None
			Dim keyExists As Boolean = _locationAccessMatrix.TryGetValue(remoteLocationKey, tapiClient)
			If keyExists Then
				Select tapiClient
					Case TapiClient.Direct
						Me.TransferMode = tapiClient
						FileHelper.Copy(remoteLocation, localFilePath, True)
						Return True
					Case TapiClient.None
					Case TapiClient.Web
						Me.TransferMode = tapiClient
						Return WebDownloadFile(localFilePath, artifactID, remoteFileGuid, appID, Nothing, False, -1, fileID, fileFieldArtifactID)
				End Select
			Else
				Try
					FileHelper.Copy(remoteLocation, localFilePath, True)
					_locationAccessMatrix.TryAdd(remoteLocationKey, TapiClient.Direct)
					Return True
				Catch ex As Exception
					Return Me.WebDownloadFile(localFilePath, artifactID, remoteFileGuid, appID, remoteLocationKey, False, -1, fileID, fileFieldArtifactID)
				End Try
			End If
			Return Nothing
		End Function

		Public Function DownloadFileForDocument(ByVal localFilePath As String, ByVal remoteFileGuid As String, ByVal remoteLocation As String, ByVal artifactID As Int32, ByVal appID As String) As Boolean Implements IExportFileDownloader.DownloadFileForDocument
			Return Me.DownloadFile(localFilePath, remoteFileGuid, remoteLocation, artifactID, appID, -1, -1)
		End Function

		Public Function DownloadFileForDynamicObject(ByVal localFilePath As String, ByVal remoteLocation As String, ByVal artifactID As Int32, ByVal appID As String, ByVal fileID As Int32, ByVal fileFieldArtifactID As Int32) As Boolean Implements IExportFileDownloader.DownloadFileForDynamicObject
			Return Me.DownloadFile(localFilePath, Nothing, remoteLocation, artifactID, appID, fileFieldArtifactID, fileID)
		End Function

		Public Function DownloadTempFile(ByVal localFilePath As String, ByVal remoteFileGuid As String, ByVal appID As String) As Boolean
			Me.TransferMode = TapiClient.Web
			Return WebDownloadFile(localFilePath, -1, remoteFileGuid, appID, Nothing, False, -1, -1, -1)
		End Function

		Public Function MoveTempFileToLocal(ByVal localFilePath As String, ByVal remoteFileGuid As String, ByVal caseInfo As CaseInfo) As Boolean
			Return MoveTempFileToLocal(localFilePath, remoteFileGuid, caseInfo, True)
		End Function

		Public Function MoveTempFileToLocal(ByVal localFilePath As String, ByVal remoteFileGuid As String, ByVal caseInfo As CaseInfo, ByVal removeRemoteTempFile As Boolean) As Boolean
			Dim retval As Boolean = Me.DownloadTempFile(localFilePath, remoteFileGuid, caseInfo.ArtifactID.ToString)

			If removeRemoteTempFile Then
				_gateway.RemoveTempFile(caseInfo.ArtifactID, remoteFileGuid)
			End If

			Return retval
		End Function

		Public Sub RemoveRemoteTempFile(ByVal remoteFileGuid As String, ByVal caseInfo As CaseInfo)
			_gateway.RemoveTempFile(caseInfo.ArtifactID, remoteFileGuid)
		End Sub

		Public Shared TotalWebTime As Long = 0

		Private Function WebDownloadFile(ByVal localFilePath As String, ByVal artifactID As Int32, ByVal remoteFileGuid As String, ByVal appID As String, ByVal remotelocationkey As String, ByVal forFullText As Boolean, ByVal longTextFieldArtifactID As Int32, ByVal fileID As Int32, ByVal fileFieldArtifactID As Int32) As Boolean
			Dim tries As Int32 = 0
			While tries < AppSettings.Instance.MaxReloginTries
				Try
					Return DoWebDownloadFile(localFilePath, artifactID, remoteFileGuid, appID, remotelocationkey, forFullText, longTextFieldArtifactID, fileID, fileFieldArtifactID)
				Catch ex As DistributedReLoginException
					tries += 1
					RaiseEvent UploadStatusEvent(String.Format("Download Manager credentials failed.  Attempting to re-login ({0} of {1})", tries, AppSettings.Instance.MaxReloginTries))
					_userManager.AttemptReLogin()
				End Try
			End While
			RaiseEvent UploadStatusEvent("Error Downloading File")
			Throw New ApplicationException("Error Downloading File: Unable to authenticate against Distributed server" & vbNewLine, New DistributedReLoginException)
		End Function

		Private Function DoWebDownloadFile(ByVal localFilePath As String, ByVal artifactID As Int32, ByVal remoteFileGuid As String, ByVal appID As String, ByVal remotelocationkey As String, ByVal forFullText As Boolean, ByVal longTextFieldArtifactID As Int32, ByVal fileID As Int32, ByVal fileFieldArtifactID As Int32) As Boolean
			Dim now As Long = System.DateTime.Now.Ticks
			Dim localStream As System.IO.Stream = Nothing
			Try
				Dim remoteuri As String
				Dim downloadUrl As String = _downloadUrl.TrimEnd("/"c) & "/"
				If forFullText Then
					remoteuri = String.Format("{0}Download.aspx?ArtifactID={1}&AppID={2}&ExtractedText=True", downloadUrl, artifactID, appID)
				ElseIf longTextFieldArtifactID > 0 Then
					remoteuri = String.Format("{0}Download.aspx?ArtifactID={1}&AppID={2}&LongTextFieldArtifactID={3}", downloadUrl, artifactID, appID, longTextFieldArtifactID)
				ElseIf fileFieldArtifactID > 0 Then
					remoteuri = String.Format("{0}Download.aspx?ObjectArtifactID={1}&FileID={2}&AppID={3}&FileFieldArtifactID={4}", downloadUrl, artifactID, fileID, appID, fileFieldArtifactID)
				Else
					remoteuri = String.Format("{0}Download.aspx?ArtifactID={1}&GUID={2}&AppID={3}", downloadUrl, artifactID, remoteFileGuid, appID)
				End If
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
				If Not remotelocationkey Is Nothing Then _locationAccessMatrix.TryAdd(remotelocationkey, TapiClient.Web)
				TotalWebTime += System.DateTime.Now.Ticks - now
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
		Public Event TransferModesChangeEvent(ByVal sender As Object, ByVal args As Global.Relativity.DataExchange.Transfer.TapiMultiClientEventArgs) Implements IExportFileDownloaderStatus.TransferModesChangeEvent

	End Class
End Namespace
