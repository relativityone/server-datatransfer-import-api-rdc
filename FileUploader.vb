Namespace kCura.WinEDDS
	Public Class FileUploader
		Public Enum Type
			Web
			Direct
		End Enum

		Private _gateway As kCura.WinEDDS.Service.FileIO
		Private _credentials As Net.NetworkCredential
		Private _type As Type
		Private _destinationFolderPath As String
		Private _caseArtifactID As Int32

		Public Sub SetDesintationFolderName(ByVal value As String)
			_destinationFolderPath = value
		End Sub

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal caseArtifactID As Int32, ByVal destinationFolderPath As String, ByVal cookieContainer As System.Net.CookieContainer)
			_gateway = New kCura.WinEDDS.Service.FileIO(credentials, cookieContainer)
			_gateway.Credentials = credentials
			_gateway.Timeout = Int32.MaxValue
			_credentials = credentials
			_caseArtifactID = caseArtifactID
			_destinationFolderPath = destinationFolderPath
			SetType(_destinationFolderPath)
		End Sub

		Private Sub SetType(ByVal destinationFolderPath As String)
			Try
				If Not System.IO.Directory.Exists(destinationFolderPath) Then
					System.IO.Directory.CreateDirectory(destinationFolderPath)
				End If
				System.IO.File.Create(destinationFolderPath & "123").Close()
				System.IO.File.Delete(destinationFolderPath & "123")
				Me.UploaderType = Type.Direct
			Catch ex As System.Exception
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

		Public ReadOnly Property CaseArtifactID() As Int32
			Get
				Return _caseArtifactID
			End Get
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

		Public Function UploadBcpFile(ByVal appID As Int32, ByVal localFilePath As String) As String		'upload return args
			Try
				_destinationFolderPath = _gateway.GetBcpSharePath(appID)
				Return Me.UploadFile(localFilePath, appID)
			Catch ex As Exception
				If ex.ToString.ToLower.IndexOf("nobcpdirectoryexception") <> -1 Then
					Return String.Empty
				Else
					Throw
				End If
			End Try
		End Function

		Public Function UploadFile(ByVal filePath As String, ByVal contextArtifactID As Int32) As String
			Return UploadFile(filePath, contextArtifactID, System.Guid.NewGuid.ToString)
		End Function

		Public Function UploadFile(ByVal filePath As String, ByVal contextArtifactID As Int32, ByVal newFileName As String) As String
			If Me.UploaderType = Type.Web Then
				Me.UploaderType = Type.Web
				Return WebUploadFile(New System.IO.FileStream(filePath, IO.FileMode.Open, IO.FileAccess.Read), contextArtifactID, newFileName)
			Else
				Me.UploaderType = Type.Direct
				Dim tries As Int32 = 20
				'Dim newFileName As String = System.Guid.NewGuid.ToString
				'Dim documentManager As New kCura.EDDS.WebAPI.DocumentManagerBase.DocumentManager
				While tries > 0
					Try
						If Not System.IO.Directory.Exists(_destinationFolderPath) Then
							System.IO.Directory.CreateDirectory(_destinationFolderPath)
						End If
						System.IO.File.Copy(filePath, String.Format("{0}{1}", _destinationFolderPath, newFileName))
						Return newFileName
					Catch ex As System.Exception
						tries -= 1
						If TypeOf ex Is System.IO.IOException AndAlso tries > 0 Then
							RaiseEvent UploadStatusEvent("Network file transfer error. Retrying in 30 seconds. " & tries & " tries left.")
							System.Threading.Thread.CurrentThread.Join(30000)
						Else
							RaiseEvent UploadStatusEvent("Error Uploading File: " & ex.Message & System.Environment.NewLine & ex.ToString)							'TODO: Change this to a separate error-type event'
							Throw New ApplicationException("Error Uploading File", ex)
						End If
					End Try
				End While
			End If
		End Function

		Public Function UploadTextAsFile(ByVal content As String, ByVal contextArtifactID As Int32, ByVal fileGuid As String) As String
			If Me.UploaderType = Type.Web Then
				Me.UploaderType = Type.Web
				Return WebUploadFile(New System.IO.MemoryStream(System.Text.Encoding.Unicode.GetBytes(content)), contextArtifactID, fileGuid)
			Else
				Me.UploaderType = Type.Direct
				Dim newFileName As String = System.Guid.NewGuid.ToString
				'Dim documentManager As New kCura.EDDS.WebAPI.DocumentManagerBase.DocumentManager
				Try
					Dim sw As New System.IO.StreamWriter(String.Format("{0}{1}", _destinationFolderPath, newFileName), False, System.Text.Encoding.Unicode)
					sw.Write(content)
					sw.Close()
					sw = Nothing
					content = Nothing
					Return newFileName
				Catch ex As System.Exception
					RaiseEvent UploadStatusEvent("Error Uploading File")					'TODO: Change this to a separate error-type event'
					Throw New ApplicationException("Error Uploading File", ex)
				End Try
			End If
		End Function

		Private Function WebUploadFile(ByVal fileStream As System.IO.Stream, ByVal contextArtifactID As Int32, ByVal fileGuid As String) As String
			Dim i As Int32
			Dim trips As Int32
			Dim artifactID As Int32
			'Dim file As System.IO.File
			'Dim fileStream As System.IO.FileStream
			Dim readLimit As Int32 = Settings.ChunkSize
			'Dim fileGuid As String
			Try
				'	fileStream = file.Open(filePath, IO.FileMode.Open, IO.FileAccess.Read)
				If fileStream.Length < Settings.ChunkSize Then
					readLimit = CType(fileStream.Length, Int32)
				End If
				If fileStream.Length > 0 Then
					trips = CType(Math.Ceiling(fileStream.Length / Settings.ChunkSize), Int32)
				Else
					trips = 1
				End If
				'UpdateStatus(System.IO.Path.GetFileName(filePath), 1, fileStream.Length)
				For i = 1 To trips
					Dim b(readLimit) As Byte
					fileStream.Read(b, 0, readLimit)
					If i = 1 Then
						fileGuid = Gateway.BeginFill(_caseArtifactID, b, _destinationFolderPath, fileGuid)
						If fileGuid = String.Empty Then
							Return String.Empty
						End If
					End If
					If i <= trips And i > 1 Then
						RaiseEvent UploadStatusEvent("Trip " & i & " of " & trips)
						If Not Gateway.FileFill(_caseArtifactID, _destinationFolderPath, fileGuid, b, contextArtifactID) Then
							Gateway.RemoveFill(_caseArtifactID, _destinationFolderPath, fileGuid)
							Return String.Empty
						End If
					End If

					'_totalBytesUp += b.Length
					'UpdateStatus(System.IO.Path.GetFileName(withApostrophe(filePath)), fileStream.Position, fileStream.Length)
					If (fileStream.Position + Settings.ChunkSize) > fileStream.Length Then
						readLimit = CType(fileStream.Length - fileStream.Position, Int32)
					Else
						readLimit = Settings.ChunkSize
					End If
					b = Nothing
				Next i
				'file = Nothing
				fileStream.Close()
				Return fileGuid
			Catch ex As System.Exception
				Throw ex
			End Try
		End Function

		Public Event UploadStatusEvent(ByVal message As String)
		Public Event UploadModeChangeEvent(ByVal mode As String)


	End Class
End Namespace
