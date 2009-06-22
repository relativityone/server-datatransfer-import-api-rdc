Namespace kCura.WinEDDS
	Public Class FileUploader
		Public Enum Type
			Web
			Direct
		End Enum

		'TODO: REWRITE ALL OF THIS FOR THE LOVE OF CHRIST

		Private _gateway As kCura.WinEDDS.Service.FileIO
		Private _credentials As Net.NetworkCredential
		Private _type As Type
		Private _destinationFolderPath As String
		Private _caseArtifactID As Int32
		Private _isBulkEnabled As Boolean = True
		Private _repositoryPathManager As kCura.Edds.Types.RepositoryPathManager
		Private _sortIntoVolumes As Boolean = False

		Public ReadOnly Property CurrentDestinationDirectory() As String
			Get
				Return _repositoryPathManager.CurrentDestinationDirectory
			End Get
		End Property
		Public Sub SetDesintationFolderName(ByVal value As String)
			_destinationFolderPath = value
		End Sub

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal caseArtifactID As Int32, ByVal destinationFolderPath As String, ByVal cookieContainer As System.Net.CookieContainer, Optional ByVal sortIntoVolumes As Boolean = True)
			_gateway = New kCura.WinEDDS.Service.FileIO(credentials, cookieContainer)
			_gateway.Credentials = credentials
			_gateway.Timeout = Int32.MaxValue
			_credentials = credentials
			_caseArtifactID = caseArtifactID
			_destinationFolderPath = destinationFolderPath
			_repositoryPathManager = New kCura.EDDS.Types.RepositoryPathManager(_gateway.RepositoryVolumeMax)
			_sortIntoVolumes = sortIntoVolumes
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
				'RaiseEvent UploadModeChangeEvent(value.ToString, _isBulkEnabled)
			End Set
		End Property

		Public ReadOnly Property IsBulkEnabled() As Boolean
			Get
				Return _isBulkEnabled
			End Get
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

		Public Function UploadBcpFile(ByVal appID As Int32, ByVal localFilePath As String) As FileUploadReturnArgs
			Return UploadBcpFileWrapper(appID, localFilePath, True)
		End Function

		Public Function ValidateBcpPath(ByVal appID As Int32, ByVal localFilePath As String) As FileUploadReturnArgs
			Return UploadBcpFileWrapper(appID, localFilePath, False)
		End Function

		Public Function UploadBcpFileWrapper(ByVal appID As Int32, ByVal localFilePath As String, ByVal upload As Boolean) As FileUploadReturnArgs
			'This function catches a potential intermittent network issue, when UploadBcpFile returns an arg object of type Warning
			Dim args As FileUploadReturnArgs
			Dim tries As Int32 = 0
			While tries < 20
				tries += 1
				Try
					args = UploadBcpFile(appID, localFilePath, upload)
					If args.Type <> FileUploadReturnArgs.FileUploadReturnType.Warning Then
						Return args
					End If
				Catch ex As System.Exception
					Return New FileUploadReturnArgs(FileUploadReturnArgs.FileUploadReturnType.UploadError, "Error accessing BCP Path, could be caused by network connectivity issues: " & ex.Message)
				End Try
			End While
			Return New FileUploadReturnArgs(FileUploadReturnArgs.FileUploadReturnType.UploadError, "Error accessing BCP Path, could be caused by network connectivity issues.")
		End Function

		Public Function UploadBcpFile(ByVal appID As Int32, ByVal localFilePath As String, ByVal upload As Boolean) As FileUploadReturnArgs
			Dim oldDestinationFolderPath As String = String.Copy(_destinationFolderPath)
			Try
				_destinationFolderPath = _gateway.GetBcpSharePath(appID)
				If Not System.IO.Directory.Exists(_destinationFolderPath) Then
					System.IO.Directory.CreateDirectory(_destinationFolderPath)
				End If
				Dim retVal As String = ""
				If upload Then retVal = Me.UploadFile(localFilePath, appID, True)
				_destinationFolderPath = oldDestinationFolderPath
				Return New FileUploadReturnArgs(FileUploadReturnArgs.FileUploadReturnType.ValidUploadKey, retVal)
			Catch ex As Exception
				If ex.ToString.ToLower.IndexOf("nobcpdirectoryexception") <> -1 Then
					_isBulkEnabled = False
					Me.UploaderType = _type
					_destinationFolderPath = oldDestinationFolderPath
					Return New FileUploadReturnArgs(FileUploadReturnArgs.FileUploadReturnType.UploadError, ex.Message)
				Else
					Try
						If _destinationFolderPath = oldDestinationFolderPath Then
							_isBulkEnabled = False
							Me.UploaderType = _type
							Return New FileUploadReturnArgs(FileUploadReturnArgs.FileUploadReturnType.Warning, "Invalid BCP Path Specified.")
						End If
						Dim r As String = ""
						If upload Then r = Me.WebUploadFile(New System.IO.FileStream(localFilePath, IO.FileMode.Open, IO.FileAccess.Read), appID, System.Guid.NewGuid.ToString)
						_destinationFolderPath = oldDestinationFolderPath
						Return New FileUploadReturnArgs(FileUploadReturnArgs.FileUploadReturnType.ValidUploadKey, r)
					Catch e As Exception
						_isBulkEnabled = False
						Me.UploaderType = _type
						_destinationFolderPath = oldDestinationFolderPath
						Return New FileUploadReturnArgs(FileUploadReturnArgs.FileUploadReturnType.UploadError, ex.Message)
					End Try
					Throw
				End If
			End Try
		End Function

		Public Function UploadFile(ByVal filePath As String, ByVal contextArtifactID As Int32, Optional ByVal internalUse As Boolean = False) As String
			Return UploadFile(filePath, contextArtifactID, System.Guid.NewGuid.ToString, internalUse)
		End Function

		Public Function UploadFile(ByVal filePath As String, ByVal contextArtifactID As Int32, ByVal newFileName As String, ByVal internalUse As Boolean) As String
			Dim tries As Int32 = 20
			While tries > 0
				Try
					If Me.UploaderType = Type.Web Then
						Me.UploaderType = Type.Web
						Return WebUploadFile(New System.IO.FileStream(filePath, IO.FileMode.Open, IO.FileAccess.Read), contextArtifactID, newFileName)
					Else
						Return Me.DirectUploadFile(filePath, contextArtifactID, newFileName, internalUse, tries < 20)
					End If
				Catch ex As System.Exception
					tries -= 1
					If TypeOf ex Is System.IO.IOException AndAlso tries > 0 Then
						RaiseEvent UploadWarningEvent(Me.UploaderType.ToString & " upload failed: " & ex.Message & " - Retrying in 30 seconds. " & tries & " tries left.")
						System.Threading.Thread.CurrentThread.Join(30000)
					Else
						If Me.UploaderType = Type.Direct And _sortIntoVolumes Then _repositoryPathManager.Rollback()
						If internalUse Then
							Throw
						Else
							RaiseEvent UploadStatusEvent("Error Uploading File: " & ex.Message & System.Environment.NewLine & ex.ToString)							 'TODO: Change this to a separate error-type event'
							Throw New ApplicationException("Error Uploading File", ex)
						End If
					End If
				End Try
			End While
		End Function

		Private Function DirectUploadFile(ByVal filePath As String, ByVal contextArtifactID As Int32, ByVal newFileName As String, ByVal internalUse As Boolean, ByVal overwrite As Boolean) As String
			Dim destinationDirectory As String = _repositoryPathManager.GetNextDestinationDirectory(_destinationFolderPath)
			If Not _sortIntoVolumes Then destinationDirectory = _destinationFolderPath
			If Not System.IO.Directory.Exists(destinationDirectory) Then System.IO.Directory.CreateDirectory(destinationDirectory)
			System.IO.File.Copy(filePath, destinationDirectory & newFileName, overwrite)
			Return newFileName
		End Function

		Public Function UploadTextAsFile(ByVal content As String, ByVal contextArtifactID As Int32, ByVal fileGuid As String) As String
			If Me.UploaderType = Type.Web Then
				Me.UploaderType = Type.Web
				Return WebUploadFile(New System.IO.MemoryStream(System.Text.Encoding.Unicode.GetBytes(content)), contextArtifactID, fileGuid)
			Else
				Me.UploaderType = Type.Direct
				Dim newFileName As String = System.Guid.NewGuid.ToString
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
			Dim readLimit As Int32 = Settings.ChunkSize
			Dim destinationDirectory As String = _repositoryPathManager.GetNextDestinationDirectory(_destinationFolderPath)
			If Not _sortIntoVolumes Then destinationDirectory = _destinationFolderPath
			Dim response As kCura.EDDS.WebAPI.FileIOBase.IoResponse
			If fileStream.Length < Settings.ChunkSize Then
				readLimit = CType(fileStream.Length, Int32)
			End If
			If fileStream.Length > 0 Then
				trips = CType(Math.Ceiling(fileStream.Length / Settings.ChunkSize), Int32)
			Else
				trips = 1
			End If
			For i = 1 To trips
				Dim b(readLimit) As Byte
				fileStream.Read(b, 0, readLimit)
				If i = 1 Then
					With Gateway.BeginFill(_caseArtifactID, b, destinationDirectory, fileGuid)
						If .Success Then
							fileGuid = .Filename
						Else
							Throw New System.IO.IOException(.ErrorMessage)
						End If
					End With
				End If
				If i <= trips And i > 1 Then
					RaiseEvent UploadStatusEvent("Trip " & i & " of " & trips)
					With Gateway.FileFill(_caseArtifactID, destinationDirectory, fileGuid, b, contextArtifactID)
						If Not .Success Then
							Gateway.RemoveFill(_caseArtifactID, destinationDirectory, fileGuid)
							Throw New System.IO.IOException(.ErrorMessage)
						End If
					End With
				End If
				If (fileStream.Position + Settings.ChunkSize) > fileStream.Length Then
					readLimit = CType(fileStream.Length - fileStream.Position, Int32)
				Else
					readLimit = Settings.ChunkSize
				End If
				b = Nothing
			Next i
			fileStream.Close()
			Return fileGuid
		End Function

		Public Event UploadStatusEvent(ByVal message As String)
		Public Event UploadWarningEvent(ByVal message As String)
		Public Event UploadModeChangeEvent(ByVal mode As String, ByVal isBulkEnabled As Boolean)

	End Class

	Public Class FileUploadReturnArgs

		Private _value As String
		Private _type As FileUploadReturnType

		Public Enum FileUploadReturnType
			UploadError
			ValidUploadKey
			Warning
		End Enum

		Public ReadOnly Property Value() As String
			Get
				Return _value
			End Get
		End Property

		Public ReadOnly Property Type() As FileUploadReturnType
			Get
				Return _type
			End Get
		End Property

		Sub New(ByVal type As kCura.WinEDDS.FileUploadReturnArgs.FileUploadReturnType, ByVal value As String)
			_value = value
			_type = type
		End Sub

	End Class

End Namespace
