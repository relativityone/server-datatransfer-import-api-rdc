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
		Private _repositoryPathManager As Relativity.RepositoryPathManager
		Private _sortIntoVolumes As Boolean = False
		Private _doRetry As Boolean = True
		Public Property DoRetry() As Boolean
			Get
				Return _doRetry
			End Get
			Set(ByVal Value As Boolean)
				_doRetry = Value
			End Set
		End Property

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
			_repositoryPathManager = New Relativity.RepositoryPathManager(_gateway.RepositoryVolumeMax)
			_sortIntoVolumes = sortIntoVolumes
			SetType(_destinationFolderPath)
		End Sub

		Private Sub SetType(ByVal destFolderPath As String)
			Try
				Dim dummyText As String = System.Guid.NewGuid().ToString().Replace("-", String.Empty).Substring(0, 5)
				'If the destination folder path is empty, we only need to test file Read/Write permissions
				If Not String.IsNullOrEmpty(destFolderPath) Then
					If Not System.IO.Directory.Exists(destFolderPath) Then
						System.IO.Directory.CreateDirectory(destFolderPath)
					End If
				End If
				System.IO.File.Create(destFolderPath & dummyText).Close()
				System.IO.File.Delete(destFolderPath & dummyText)
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

		Private Function UploadBcpFileWrapper(ByVal appID As Int32, ByVal localFilePath As String, ByVal upload As Boolean) As FileUploadReturnArgs
			'This function catches a potential intermittent network issue, when UploadBcpFile returns an arg object of type Warning
			Dim args As FileUploadReturnArgs
			Dim tries As Int32 = 0
			'If Not _doRetry Then Return Nothing
			While tries < 20 AndAlso (_doRetry OrElse tries = 0)
				tries += 1
				Try
					args = UploadBcpFile(appID, localFilePath, upload)
					If args.Type = FileUploadReturnArgs.FileUploadReturnType.ValidUploadKey Then
						Return args
					Else
						Throw New System.Exception(args.Value)
					End If
				Catch ex As System.Exception
					RaiseEvent UploadWarningEvent("Error accessing BCP Path, could be caused by network connectivity issues: " & ex.ToString)
					If Config.EnableSingleModeImport AndAlso tries < 19 Then
						Return New FileUploadReturnArgs(FileUploadReturnArgs.FileUploadReturnType.UploadError, "Error accessing BCP Path, could be caused by network connectivity issues: " & ex.Message)
					ElseIf Not Config.EnableSingleModeImport AndAlso tries = 3 AndAlso Not upload Then
						Return New FileUploadReturnArgs(FileUploadReturnArgs.FileUploadReturnType.UploadError, "Error accessing BCP Path, could be caused by network connectivity issues: " & ex.Message)
					End If
					If tries = 1 AndAlso Not Config.EnableSingleModeImport Then
						RaiseEvent UploadWarningEvent("Retrying bulk upload")
					Else
						RaiseEvent UploadWarningEvent("Retrying bulk upload in " & kCura.Utility.Config.Settings.IoErrorWaitTimeInSeconds & " seconds")
						System.Threading.Thread.CurrentThread.Join(1000 * kCura.Utility.Config.Settings.IoErrorWaitTimeInSeconds)
					End If
				End Try
			End While
			Return New FileUploadReturnArgs(FileUploadReturnArgs.FileUploadReturnType.UploadError, "Error accessing BCP Path, could be caused by network connectivity issues.")
		End Function

		Private Function UploadBcpFile(ByVal appID As Int32, ByVal localFilePath As String, ByVal upload As Boolean) As FileUploadReturnArgs
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
			Dim tries As Int32 = kCura.Utility.Config.Settings.IoErrorNumberOfRetries
			While tries > 0 And (DoRetry OrElse tries = kCura.Utility.Config.Settings.IoErrorNumberOfRetries)
				Try
					If Me.UploaderType = Type.Web Then
						Me.UploaderType = Type.Web
						Return Me.WebUploadFile(New System.IO.FileStream(filePath, IO.FileMode.Open, IO.FileAccess.Read), contextArtifactID, newFileName)
					Else
						Return Me.DirectUploadFile(filePath, contextArtifactID, newFileName, internalUse, tries < kCura.Utility.Config.Settings.IoErrorNumberOfRetries)
					End If
				Catch ex As System.Exception
					tries -= 1
					Dim wait As Int32 = kCura.Utility.Config.Settings.IoErrorWaitTimeInSeconds
					If Me.IsWarningException(ex) AndAlso tries > 0 Then
						'RaiseEvent UploadWarningEvent(Me.UploaderType.ToString & " upload failed: " & ex.Message & " - Retrying in 30 seconds. " & tries & " tries left.")

						RaiseEvent UploadWarningEvent(String.Format("{0} upload failed: {1} - Retrying in {2} seconds.  {3} tries left.", Me.UploaderType.ToString, ex.Message, wait, tries))
						System.Threading.Thread.CurrentThread.Join(wait * 1000)
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
			Return Nothing
		End Function

		Private Function IsWarningException(ByVal ex As System.Exception) As Boolean
			If Me.UploaderType = Type.Direct And TypeOf ex Is System.IO.IOException Then Return True
			If Me.UploaderType = Type.Web Then Return True
			Return False
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
			Try
				Dim i As Int32
				Dim trips As Int32
				Dim readLimit As Int32 = Settings.ChunkSize
				Dim destinationDirectory As String = _repositoryPathManager.GetNextDestinationDirectory(_destinationFolderPath)
				If Not _sortIntoVolumes Then destinationDirectory = _destinationFolderPath
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
						With _gateway.BeginFill(_caseArtifactID, b, destinationDirectory, fileGuid)
							If .Success Then
								fileGuid = .Filename
							Else
								Throw New System.IO.IOException(.ErrorMessage)
							End If
						End With
					End If
					If i <= trips And i > 1 Then
						RaiseEvent UploadStatusEvent("Trip " & i & " of " & trips)
						With _gateway.FileFill(_caseArtifactID, destinationDirectory, fileGuid, b, contextArtifactID)
							If Not .Success Then
								_gateway.RemoveFill(_caseArtifactID, destinationDirectory, fileGuid)
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
			Catch ex As System.Exception
				Try
					fileStream.Close()
				Catch
				End Try
				Select Case Me.GetStatusCode(ex)
					Case 503
						Throw New System.InvalidOperationException("Web API server unavailable; if problem persists, contact your system administrator", ex)
					Case Else
						Throw
				End Select
			End Try
			Return fileGuid
		End Function

		Private Function GetStatusCode(ByVal ex As System.Exception) As Int32
			If Not TypeOf ex Is System.Net.WebException Then Return -1
			Dim webex As System.Net.WebException = DirectCast(ex, System.Net.WebException)
			Try
				Return DirectCast(webex.Response, System.Net.HttpWebResponse).StatusCode
			Catch
				Return -1
			End Try
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
