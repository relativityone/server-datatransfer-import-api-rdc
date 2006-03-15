Namespace kCura.WinEDDS
	Public Class FileDownloader

		Public Enum Type
			Web
			Direct
		End Enum

		Private _gateway As kCura.WinEDDS.Service.FileIO
		Private _credentials As Net.NetworkCredential
		Private _type As Type
		Private _destinationFolderPath As String
		Private _webClient As Net.WebClient
		Private _downloadUrl As String

		Public Sub SetDesintationFolderName(ByVal value As String)
			_destinationFolderPath = value
		End Sub

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal destinationFolderPath As String, ByVal downloadHandlerUrl As String)
			_gateway = New kCura.WinEDDS.Service.FileIO(credentials)
			_gateway.Credentials = credentials
			_gateway.Timeout = Int32.MaxValue
			_credentials = credentials
			If destinationFolderPath.Chars(destinationFolderPath.Length - 1) <> "\"c Then
				destinationFolderPath &= "\"
			End If
			_destinationFolderPath = destinationFolderPath
			_webClient = New Net.WebClient
			_webClient.Credentials = credentials
			_downloadUrl = downloadHandlerUrl
			'Dim documentManager As kCura.EDDS.WebAPI.DocumentManagerBase.DocumentManager
			SetType(_destinationFolderPath)

		End Sub

		Private Sub SetType(ByVal destinationFolderPath As String)
			Try
				System.IO.File.Create(destinationFolderPath & "123").Close()
				System.IO.File.Delete(destinationFolderPath & "123")
				Me.UploaderType = Type.Direct
			Catch ex As Exception
				Me.UploaderType = Type.Web
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

		Public Property UploaderType() As Type
			Get
				Return _type
			End Get
			Set(ByVal value As Type)
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

		Public Function DownloadFile(ByVal localFilePath As String, ByVal remoteFileGuid As String, ByVal artifactID As Int32) As Boolean
			If Me.UploaderType = Type.Web Then
				Me.UploaderType = Type.Web
				Return WebDownloadFile(localFilePath, artifactID, remoteFileGuid)
			Else
				Me.UploaderType = Type.Direct
				Dim remoteFilePath As String = _destinationFolderPath & remoteFileGuid
				Try
					System.IO.File.Copy(remoteFilePath, localFilePath)
					Return True
				Catch ex As Exception
					RaiseEvent UploadStatusEvent("Error Uploading File")					'TODO: Change this to a separate error-type event'
					Throw New ApplicationException("Error Uplaoding File", ex)
				End Try
			End If
		End Function

		Private Function WebDownloadFile(ByVal localFilePath As String, ByVal artifactID As Int32, ByVal remoteFileGuid As String) As Boolean
			Try
				Dim remoteuri As String
				remoteuri = String.Format("{0}Download.aspx?ArtifactID={1}&GUID={2}", _downloadUrl, artifactID, remoteFileGuid)
				_webClient.DownloadFile(remoteuri, localFilePath)
				Return True
			Catch ex As Exception
				RaiseEvent UploadStatusEvent("Error Uploading File")				 'TODO: Change this to a separate error-type event'
				Throw New ApplicationException("Error Uplaoding File", ex)
			End Try

		End Function

		Public Event UploadStatusEvent(ByVal message As String)
		Public Event UploadModeChangeEvent(ByVal mode As String)


	End Class
End Namespace
