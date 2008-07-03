Namespace kCura.WinEDDS
	Public Class FileDownloader

		Public Enum FileAccessType
			Web
			Direct
		End Enum

		Private _gateway As kCura.WinEDDS.Service.FileIO
		Private _credentials As Net.NetworkCredential
		Private _type As FileAccessType
		Private _destinationFolderPath As String
		Private _downloadUrl As String
		Private _cookieContainer As System.Net.CookieContainer
		Private _authenticationToken As String
		Private _userManager As kCura.WinEDDS.Service.UserManager
		Private Shared _locationAccessMatrix As New System.Collections.Hashtable

		Public Sub SetDesintationFolderName(ByVal value As String)
			_destinationFolderPath = value
		End Sub

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal destinationFolderPath As String, ByVal downloadHandlerUrl As String, ByVal cookieContainer As System.Net.CookieContainer, ByVal authenticationToken As String)
			_gateway = New kCura.WinEDDS.Service.FileIO(credentials, cookieContainer)
			_cookieContainer = cookieContainer
			_gateway.Credentials = credentials
			_gateway.Timeout = Int32.MaxValue
			_credentials = credentials
			If destinationFolderPath.Chars(destinationFolderPath.Length - 1) <> "\"c Then
				destinationFolderPath &= "\"
			End If
			_destinationFolderPath = destinationFolderPath
			_downloadUrl = downloadHandlerUrl
			'Dim documentManager As kCura.EDDS.WebAPI.DocumentManagerBase.DocumentManager
			SetType(_destinationFolderPath)
			_authenticationToken = authenticationToken
			_userManager = New kCura.WinEDDS.Service.UserManager(credentials, cookieContainer)
			If _locationAccessMatrix Is Nothing Then _locationAccessMatrix = New System.Collections.Hashtable
		End Sub

		Private Sub SetType(ByVal destinationFolderPath As String)
			Try
				System.IO.File.Create(destinationFolderPath & "123").Close()
				System.IO.File.Delete(destinationFolderPath & "123")
				Me.UploaderType = FileAccessType.Direct
			Catch ex As System.Exception
				Me.UploaderType = FileAccessType.Web
			End Try
		End Sub

		Public Property DestinationFolderPath() As String
			Get
				Return _destinationFolderPath
			End Get
			Set(ByVal value As String)
				_destinationFolderPath = value
			End Set
		End Property

		Public Property UploaderType() As FileAccessType
			Get
				Return _type
			End Get
			Set(ByVal value As FileAccessType)
				_type = value
				RaiseEvent UploadModeChangeEvent(value.ToString)
			End Set
		End Property

		Private ReadOnly Property Gateway() As kCura.WinEDDS.Service.FileIO
			Get
				Return _gateway
			End Get
		End Property

		Friend Class Settings

			Friend Shared ReadOnly Property ChunkSize() As Int32
				Get
					Return 1024000
				End Get
			End Property
		End Class

		'Public Function DownloadFile(ByVal filePath As String, ByVal fileGuid As String) As String
		'	Return UploadFile(filePath, contextArtifactID, System.Guid.NewGuid.ToString)
		'End Function

		Public Function DownloadFullTextFile(ByVal localFilePath As String, ByVal artifactID As Int32, ByVal appID As String) As Boolean
			Return WebDownloadFile(localFilePath, artifactID, "", appID, Nothing, True)
		End Function


		Public Function DownloadFile(ByVal localFilePath As String, ByVal remoteFileGuid As String, ByVal remoteLocation As String, ByVal artifactID As Int32, ByVal appID As String) As Boolean
			'If Me.UploaderType = Type.Web Then
			Dim remoteLocationKey As String = remoteLocation.Substring(0, remoteLocation.LastIndexOf("\")).TrimEnd("\"c) & "\"
			If _locationAccessMatrix.Contains(remoteLocationKey) Then
				Select Case CType(_locationAccessMatrix(remoteLocationKey), FileAccessType)
					Case FileAccessType.Direct
						Me.UploaderType = FileAccessType.Direct
						System.IO.File.Copy(remoteLocation, localFilePath, True)
						Return True
					Case FileAccessType.Web
						Me.UploaderType = FileAccessType.Web
						Return WebDownloadFile(localFilePath, artifactID, remoteFileGuid, appID, Nothing)
				End Select
			Else
				Try
					System.IO.File.Copy(remoteLocation, localFilePath, True)
					_locationAccessMatrix.Add(remoteLocationKey, FileAccessType.Direct)
					Return True
				Catch ex As Exception
					Return Me.WebDownloadFile(localFilePath, artifactID, remoteFileGuid, appID, remoteLocationKey)
				End Try
			End If
		End Function

		Public Function DownloadFile(ByVal localFilePath As String, ByVal remoteFileGuid As String, ByVal appID As String) As Boolean
			Me.UploaderType = FileAccessType.Web
			Return WebDownloadFile(localFilePath, -1, remoteFileGuid, appID, Nothing)
		End Function

		Private Function WebDownloadFile(ByVal localFilePath As String, ByVal artifactID As Int32, ByVal remoteFileGuid As String, ByVal appID As String, ByVal remotelocationkey As String, Optional ByVal forFullText As Boolean = False) As Boolean
			Try
				Dim remoteuri As String
				Dim downloadUrl As String = _downloadUrl.TrimEnd("/"c) & "/"
				If forFullText Then
					remoteuri = String.Format("{0}Download.aspx?ArtifactID={1}&AppID={2}&ExtractedText=True", downloadUrl, artifactID, appID)
				Else
					remoteuri = String.Format("{0}Download.aspx?ArtifactID={1}&GUID={2}&AppID={3}", downloadUrl, artifactID, remoteFileGuid, appID)
				End If
				If _authenticationToken <> String.Empty Then
					remoteuri &= String.Format("&AuthenticationToken={0}", _authenticationToken)
				End If
				Dim httpWebRequest As System.Net.HttpWebRequest = CType(System.Net.HttpWebRequest.Create(remoteuri), System.Net.HttpWebRequest)
				httpWebRequest.Credentials = _credentials
				httpWebRequest.CookieContainer = _cookieContainer
				httpWebRequest.UnsafeAuthenticatedConnectionSharing = True
				httpWebRequest.Headers.Add("SOURCEID", "9AAC98ED-01A4-4111-B66E-D25130875E5D")				'Verifies WinEDDS as a trusted source with the Distributed environment.
				Dim webResponse As System.Net.WebResponse = httpWebRequest.GetResponse()
				Dim localStream As System.IO.Stream
				If Not webResponse Is Nothing Then
					Dim responseStream As System.IO.Stream = webResponse.GetResponseStream()
					localStream = System.IO.File.Create(localFilePath)
					Dim buffer(1023) As Byte
					Dim bytesRead As Int32
					While True
						bytesRead = responseStream.Read(buffer, 0, 1024)
						If bytesRead <= 0 Then
							Exit While
						End If
						localStream.Write(buffer, 0, bytesRead)
					End While
				End If
				localStream.Close()
				If Not remotelocationkey Is Nothing Then _locationAccessMatrix.Add(remotelocationkey, FileAccessType.Web)
				Return True
			Catch ex As System.Exception
				RaiseEvent UploadStatusEvent("Error Downloading File")				 'TODO: Change this to a separate error-type event'
				Throw New ApplicationException("Error Downloading File", ex)
			End Try
		End Function

		Public Event UploadStatusEvent(ByVal message As String)
		Public Event UploadModeChangeEvent(ByVal mode As String)

	End Class
End Namespace
