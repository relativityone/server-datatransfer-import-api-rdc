Imports System.Security.AccessControl
Imports kCura.EDDS.WebAPI.RelativityManagerBase
Imports kCura.Utility
Imports kCura.WinEDDS.Credentials
Imports Relativity
Imports RelativityManager = kCura.WinEDDS.Service.RelativityManager
Imports kCura.EDDS.WinForm.Exceptions

Namespace kCura.EDDS.WinForm

	Public Module Startup

#Region " Library Functions "

		Private Declare Function GetConsoleWindow Lib "kernel32" () As Integer
		Private Declare Function CloseWindow Lib "user32" (ByVal hwnd As Integer) As Integer
		Private Declare Function FreeConsole Lib "kernel32" () As Integer

#End Region

#Region " Members "

		Private _application As kCura.EDDS.WinForm.Application
		Private _loadFilePath As String
		Private SelectedCaseInfo As Relativity.CaseInfo
		Private SelectedNativeLoadFile As New kCura.WinEDDS.LoadFile
		Private SelectedImageLoadFile As kCura.WinEDDS.ImageLoadFile
		Private CurrentLoadMode As LoadMode
		Private HasSetLoadFileLocation As Boolean = False
		Private HasSetCaseInfo As Boolean = False
		Private HasSetUsername As Boolean = False
		Private HasSetPassword As Boolean = False
		Private HasSetSavedMapLocation As Boolean = False
		Private HasSetLoadMode As Boolean = False
		Private SourceFileEncoding As System.Text.Encoding
		Private ExtractedTextFileEncoding As System.Text.Encoding
		Private SelectedCasePath As String = ""
		Private CopyFilesToDocumentRepository As Boolean = True
		Private DestinationFolderID As Int32
		Private RunningDirectory As String = System.IO.Directory.GetCurrentDirectory
		Private ExportErrorReportFile As Boolean = False
		Private ExportErrorLoadFile As Boolean = False
		Private ErrorReportFileLocation As String = ""
		Private ErrorLoadFileLocation As String = ""
		Private ArtifactTypeID As Int32 = -1
		Private StartLineNumber As Int64
#End Region

#Region " Enumerations "

		Public Enum LoadMode
			Image
			Native
			DynamicObject
			Application
		End Enum

#End Region

		Public Sub Main()
			Dim handler As ThreadExceptionHandler = New ThreadExceptionHandler()
			AddHandler System.Windows.Forms.Application.ThreadException, AddressOf handler.Application_ThreadException

			Dim args As String() = System.Environment.GetCommandLineArgs

			If args.Length = 1 Then
				CloseConsole()
				Dim mainForm As New kCura.EDDS.WinForm.MainForm()

				mainForm.Show()
				mainForm.Refresh()
				System.Windows.Forms.Application.Run()
			Else
				RunInConsoleMode()
			End If
		End Sub

		Private Function GetValueFromCommandListByFlag(ByVal commandList As kCura.CommandLine.CommandList, ByVal flag As String) As String
			For Each command As kCura.CommandLine.Command In commandList
				If command.Directive.ToLower.Replace("-", "").Replace("/", "") = flag.ToLower Then
					If command.Value Is Nothing Then Return ""
					Return command.Value
				End If
			Next
			Return ""
		End Function

		Private Function UrlIsValid(ByVal url As String) As Boolean
			Try
				url = url.TrimEnd("\"c).TrimEnd("/"c) & "/"
				Dim req As System.Net.HttpWebRequest = DirectCast(System.Net.WebRequest.Create(url & "RelativityManager.asmx"), System.Net.HttpWebRequest)
				req.Credentials = System.Net.CredentialCache.DefaultCredentials
				Dim resp As System.Net.HttpWebResponse = DirectCast(req.GetResponse(), System.Net.HttpWebResponse)
				Return True
			Catch ex As Exception
				Return False
			End Try
		End Function

		Private Sub RunInConsoleMode()
			Try
				_application = kCura.EDDS.WinForm.Application.Instance

				Dim commandList As kCura.CommandLine.CommandList = kCura.CommandLine.CommandLineParser.Parse
				For Each command As kCura.CommandLine.Command In commandList
					If command.Directive.ToLower.Replace("-", "").Replace("/", "") = "h" Then
						If command.Value Is Nothing OrElse command.Value = "" Then
							GetHelpPage()
							Exit Sub
						End If
						If command.Value.ToLower = "encoding" Then
							GetEncodingList()
							Exit Sub
						End If
						GetHelpPage()
						Exit Sub
					End If
				Next
				If kCura.WinEDDS.Config.WebServiceURL = "" OrElse Not UrlIsValid(kCura.WinEDDS.Config.WebServiceURL) Then
					Console.WriteLine("Web Service URL not set or not accessible.  Please enter:")
					Dim webserviceurl As String = Console.ReadLine
					While Not UrlIsValid(webserviceurl)
						Console.WriteLine("Invalid Web Service URL set.  Retry:")
						webserviceurl = Console.ReadLine.Trim
					End While
					kCura.WinEDDS.Config.WebServiceURL = webserviceurl
				End If
				Dim defaultCredentialResult As Application.CredentialCheckResult = _application.AttemptWindowsAuthentication()
				If defaultCredentialResult = Application.CredentialCheckResult.AccessDisabled Then
					Console.WriteLine(Application.ACCESS_DISABLED_MESSAGE)
					Exit Sub
				ElseIf Not defaultCredentialResult = Application.CredentialCheckResult.Success Then
					Dim clientID As String = ""
					Dim clientSecret As String = ""
					clientID = GetValueFromCommandListByFlag(commandList, "clientID")
					clientSecret = GetValueFromCommandListByFlag(commandList, "clientSecret")

					Dim userName As String = ""
					Dim password As String = ""
					userName = GetValueFromCommandListByFlag(commandList, "u")
					password = GetValueFromCommandListByFlag(commandList, "p")

					HandleConsoleAuthErrors(userName, password, clientID, clientSecret)
					Dim loginResult As Application.CredentialCheckResult = Application.CredentialCheckResult.NotSet
					Try
						If Not String.IsNullOrEmpty(userName)
							Dim cred As New UserCredentialsProvider(userName, password)
							RelativityWebApiCredentialsProvider.Instance().SetProvider(cred)
							loginResult = _application.DoLogin()
						Else 

							loginResult = _application.DoOAuthLogin(clientID, clientSecret)
						End If
						
					Catch ex As Exception
						loginResult = Application.CredentialCheckResult.Fail
					End Try
					If loginResult = Application.CredentialCheckResult.AccessDisabled Then
						Console.WriteLine(Application.ACCESS_DISABLED_MESSAGE)
						Exit Sub
					ElseIf loginResult = Application.CredentialCheckResult.InvalidClientCredentials Then
						throw new ClientCrendentialsException
					ElseIf loginResult = Application.CredentialCheckResult.FailToConnectToIdentityServer Then
						throw new ConnectToIdentityServerException
					ElseIf Not loginResult = Application.CredentialCheckResult.Success Then
						Throw New CredentialsException
					Else

					End If

				End If
				SetLoadType(GetValueFromCommandListByFlag(commandList, "m"))
				SetCaseInfo(GetValueFromCommandListByFlag(commandList, "c"))
				SetFileLocation(GetValueFromCommandListByFlag(commandList, "f"))
				SetSavedMapLocation(GetValueFromCommandListByFlag(commandList, "k"))
				EnsureLoadFileLocation()
				SetSourceFileEncoding(GetValueFromCommandListByFlag(commandList, "e"))
				SetFullTextFileEncoding(GetValueFromCommandListByFlag(commandList, "x"))
				SetSelectedCasePath(GetValueFromCommandListByFlag(commandList, "r"))
				SetCopyFilesToDocumentRepository(GetValueFromCommandListByFlag(commandList, "l"))
				SetDestinationFolderID(GetValueFromCommandListByFlag(commandList, "d"))
				SetStartLineNumber(GetValueFromCommandListByFlag(commandList, "s"))
				SetExportErrorReportLocation(commandList)
				SetExportErrorFileLocation(commandList)
				Select Case CurrentLoadMode
					Case LoadMode.Image
						RunImageImport()
					Case LoadMode.Native
						RunNativeImport()
					Case LoadMode.DynamicObject
						RunDynamicObjectImport(commandList)
					Case LoadMode.Application
						RunApplicationImport()
				End Select

				_application.Logout()
			Catch ex As RdcBaseException
				Console.WriteLine("--------------------------")
				Console.WriteLine("ERROR: " & ex.Message)
				Console.WriteLine("")
				Console.WriteLine("Use kCura.EDDS.WinForm.exe -h for help")
				Console.WriteLine("--------------------------")
			Catch ex As Exception
				Console.WriteLine("--------------------------")
				Console.WriteLine("FATAL ERROR:")
				Console.WriteLine(ex.ToString)
				Console.WriteLine("--------------------------")
			End Try

		End Sub

		Private Sub HandleConsoleAuthErrors(username As String, password As String, clientID As String, clientSecret As String)
			Dim usernameExists As Boolean = Not String.IsNullOrEmpty(username)
			Dim passwordExists As Boolean = Not String.IsNullOrEmpty(password)
			Dim clientIDExists As Boolean = Not String.IsNullOrEmpty(clientID)
			Dim clientSecretExists As Boolean = Not String.IsNullOrEmpty(clientSecret)

			If (usernameExists Or passwordExists) AndAlso (clientIDExists Or clientSecretExists)Then
				Throw New MultipleCredentialException
			End If

			If Not clientIDExists AndAlso Not clientSecretExists Then
				If Not usernameExists Then Throw New UsernameException
				If Not passwordExists Then Throw New PasswordException
			Else
				If Not clientIDExists Then Throw New ClientIDException
				If Not clientSecretExists Then Throw New ClientSecretException
			End If
		End Sub


#Region " Run Import "

		Private Sub RunApplicationImport()
			Dim packageData As Byte()
			packageData = System.IO.File.ReadAllBytes(_loadFilePath)
			Dim importer As New kCura.WinEDDS.ApplicationDeploymentProcess(New Int32() {}, Nothing, packageData, _application.Credential, _application.CookieContainer, New Relativity.CaseInfo() {SelectedCaseInfo})
			Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController, ErrorLoadFileLocation, ErrorReportFileLocation)
			_application.StartProcess(importer)
		End Sub

		Private Sub RunDynamicObjectImport(ByVal commandList As kCura.CommandLine.CommandList)
			Dim importer As New kCura.WinEDDS.ImportLoadFileProcess
			SelectedNativeLoadFile.SourceFileEncoding = SourceFileEncoding
			SelectedNativeLoadFile.ExtractedTextFileEncoding = ExtractedTextFileEncoding
			SelectedNativeLoadFile.SelectedCasePath = SelectedCasePath
			SelectedNativeLoadFile.CopyFilesToDocumentRepository = CopyFilesToDocumentRepository
			SelectedNativeLoadFile.DestinationFolderID = DestinationFolderID
			SelectedNativeLoadFile.StartLineNumber = StartLineNumber
			importer.LoadFile = SelectedNativeLoadFile
			importer.TimeZoneOffset = _application.TimeZoneOffset
			importer.BulkLoadFileFieldDelimiter = Config.BulkLoadFileFieldDelimiter
			importer.CloudInstance = Config.CloudInstance
			importer.ExecutionSource = Relativity.ExecutionSource.Rdc
			_application.SetWorkingDirectory(SelectedNativeLoadFile.FilePath)
			Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController, ErrorLoadFileLocation, ErrorReportFileLocation)
			_application.StartProcess(importer)
		End Sub

		Private Sub RunNativeImport()
			If EnsureEncoding() Then
				Dim folderManager As New kCura.WinEDDS.Service.FolderManager(_application.Credential, _application.CookieContainer)
				If folderManager.Exists(SelectedCaseInfo.ArtifactID, SelectedCaseInfo.RootFolderID) Then
					Dim importer As New kCura.WinEDDS.ImportLoadFileProcess
					SelectedNativeLoadFile.SourceFileEncoding = SourceFileEncoding
					SelectedNativeLoadFile.ExtractedTextFileEncoding = ExtractedTextFileEncoding
					SelectedNativeLoadFile.SelectedCasePath = SelectedCasePath
					SelectedNativeLoadFile.CopyFilesToDocumentRepository = CopyFilesToDocumentRepository
					SelectedNativeLoadFile.DestinationFolderID = DestinationFolderID
					SelectedNativeLoadFile.StartLineNumber = StartLineNumber
					importer.LoadFile = SelectedNativeLoadFile
					importer.TimeZoneOffset = _application.TimeZoneOffset
					importer.BulkLoadFileFieldDelimiter = Config.BulkLoadFileFieldDelimiter
					importer.CloudInstance = Config.CloudInstance
					importer.ExecutionSource = Relativity.ExecutionSource.Rdc
					SelectedNativeLoadFile.ArtifactTypeID = Relativity.ArtifactType.Document
					_application.SetWorkingDirectory(SelectedNativeLoadFile.FilePath)
					Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController, ErrorLoadFileLocation, ErrorReportFileLocation)
					_application.StartProcess(importer)
				End If
			Else
				Throw New EncodingMisMatchException(SourceFileEncoding.CodePage, kCura.WinEDDS.Utility.DetectEncoding(_loadFilePath, True).DeterminedEncoding.CodePage)
			End If
		End Sub

		Private Sub RunImageImport()
			If EnsureEncoding() Then
				Dim importer As New kCura.WinEDDS.ImportImageFileProcess
				SelectedImageLoadFile.CookieContainer = _application.CookieContainer
				SelectedImageLoadFile.Credential = _application.Credential
				SelectedImageLoadFile.SelectedCasePath = SelectedCasePath
				SelectedImageLoadFile.CaseDefaultPath = SelectedCaseInfo.DocumentPath
				SelectedImageLoadFile.CopyFilesToDocumentRepository = CopyFilesToDocumentRepository
				SelectedImageLoadFile.FullTextEncoding = ExtractedTextFileEncoding
				SelectedImageLoadFile.StartLineNumber = StartLineNumber
				importer.ImageLoadFile = SelectedImageLoadFile
				importer.CloudInstance = Config.CloudInstance
				importer.ExecutionSource = ExecutionSource.Rdc
				_application.SetWorkingDirectory(SelectedImageLoadFile.FileName)
				Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController, ErrorLoadFileLocation, ErrorReportFileLocation)
				_application.StartProcess(importer)
			Else
				Throw New EncodingMisMatchException(SourceFileEncoding.CodePage, kCura.WinEDDS.Utility.DetectEncoding(_loadFilePath, True).DeterminedEncoding.CodePage)
			End If
		End Sub

#End Region

#Region " Input Validation "
		Private Sub EnsureLoadFileLocation()
			If String.IsNullOrEmpty(_loadFilePath) Then Throw New LoadFilePathException
			If Not System.IO.File.Exists(_loadFilePath) Then Throw New LoadFilePathException(_loadFilePath)
		End Sub

		Private Function EnsureEncoding() As Boolean
			Dim determinedEncoding As System.Text.Encoding = kCura.WinEDDS.Utility.DetectEncoding(_loadFilePath, True).DeterminedEncoding
			If determinedEncoding Is Nothing Then Return True
			Return (determinedEncoding.Equals(SourceFileEncoding))
		End Function

		Private Sub SetFileLocation(ByVal path As String)
			_loadFilePath = path
			HasSetLoadFileLocation = Not String.IsNullOrEmpty(_loadFilePath)
		End Sub

		Private Sub SetCaseInfo(ByVal caseID As String)
			Try
				Dim caseManager As New kCura.WinEDDS.Service.CaseManager(_application.Credential, _application.CookieContainer)
				SelectedCaseInfo = caseManager.Read(Int32.Parse(caseID))
			Catch ex As Exception
				Throw New CaseArtifactIdException(caseID, ex)
			End Try
			If SelectedCaseInfo Is Nothing Then Throw New CaseArtifactIdException(caseID)
			_application.RefreshSelectedCaseInfo(SelectedCaseInfo)
			HasSetCaseInfo = True
		End Sub

		Private Sub SetExportErrorReportLocation(ByVal commandLine As kCura.CommandLine.CommandList)
			For Each command As kCura.CommandLine.Command In commandLine
				If command.Directive.ToLower.Replace("-", "").Replace("/", "") = "er" Then
					ExportErrorReportFile = True
					If command.Value Is Nothing OrElse command.Value = "" Then
						ErrorReportFileLocation = RunningDirectory.TrimEnd("\".ToCharArray) & "\ErrorReport_" & System.DateTime.Now.Ticks.ToString & ".csv"
					Else
						ErrorReportFileLocation = String.Copy(command.Value)
					End If
					Try
						If Not System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(ErrorReportFileLocation)) Then System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(ErrorReportFileLocation))
						System.IO.File.Create(ErrorReportFileLocation).Close()
						System.IO.File.Delete(ErrorReportFileLocation)
					Catch ex As Exception
						Throw New InvalidPathLocationException(ErrorReportFileLocation, "error report", ex)
					End Try
					'Return command.Value
				End If
			Next
			If Not ExportErrorReportFile Then ErrorReportFileLocation = ""
		End Sub
		Private Sub SetExportErrorFileLocation(ByVal commandLine As kCura.CommandLine.CommandList)
			For Each command As kCura.CommandLine.Command In commandLine
				If command.Directive.ToLower.Replace("-", "").Replace("/", "") = "ef" Then
					ExportErrorLoadFile = True
					If command.Value Is Nothing OrElse command.Value = "" Then
						Dim extension As String = System.IO.Path.GetExtension(_loadFilePath)
						If extension Is Nothing Then extension = "txt"
						extension = extension.TrimStart(".".ToCharArray)
						ErrorLoadFileLocation = RunningDirectory.TrimEnd("\".ToCharArray) & "\ErrorLines_" & System.DateTime.Now.Ticks.ToString & "." & extension
					Else
						ErrorLoadFileLocation = String.Copy(command.Value)
					End If
					Try
						If Not System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(ErrorLoadFileLocation)) Then System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(ErrorLoadFileLocation))
						System.IO.File.Create(ErrorLoadFileLocation).Close()
						System.IO.File.Delete(ErrorLoadFileLocation)
					Catch ex As Exception
						Throw New InvalidPathLocationException(ErrorReportFileLocation, "error file", ex)
					End Try
					'Return command.Value
				End If
			Next
			If Not ExportErrorLoadFile Then ErrorLoadFileLocation = ""
		End Sub

		Private Sub SetSavedMapLocation(ByVal path As String)
			Try
				Select Case CurrentLoadMode
					Case LoadMode.Image
						If Not System.IO.File.Exists(path) Then Throw New SavedSettingsFilePathException(path)
						SelectedImageLoadFile = _application.ReadImageLoadFile(path)
						If HasSetLoadFileLocation Then SelectedImageLoadFile.FileName = _loadFilePath
					Case LoadMode.Native, LoadMode.DynamicObject
						If Not System.IO.File.Exists(path) Then Throw New SavedSettingsFilePathException(path)
						Dim sr As New System.IO.StreamReader(path)
						Dim doc As String = sr.ReadToEnd
						sr.Close()
						Dim xmlDoc As New System.Xml.XmlDocument
						xmlDoc.LoadXml(doc)

						sr = New System.IO.StreamReader(path)
						Dim stringr As New System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(_application.CleanLoadFile(xmlDoc)))
						Dim tempLoadFile As WinEDDS.LoadFile
						Dim deserializer As New System.Runtime.Serialization.Formatters.Soap.SoapFormatter
						tempLoadFile = DirectCast(deserializer.Deserialize(stringr), WinEDDS.LoadFile)
						sr.Close()
						If Not String.IsNullOrEmpty(_loadFilePath) Then
							tempLoadFile.FilePath = _loadFilePath
						Else
							_loadFilePath = tempLoadFile.FilePath
						End If
						tempLoadFile.OverwriteDestination = Utility.ConvertLegacyOverwriteDestinationValueToEnum(tempLoadFile.OverwriteDestination)
						tempLoadFile.ForceFolderPreview = False
						tempLoadFile.CaseInfo = SelectedCaseInfo
						tempLoadFile.CopyFilesToDocumentRepository = True						'LoadFile.CopyFilesToDocumentRepository
						tempLoadFile.SelectedCasePath = SelectedCaseInfo.DocumentPath						''''''''''
						tempLoadFile.CaseDefaultPath = SelectedCaseInfo.DocumentPath
						tempLoadFile.Credentials = _application.Credential
						tempLoadFile.DestinationFolderID = 35
						tempLoadFile.ExtractedTextFileEncoding = System.Text.Encoding.Unicode
						tempLoadFile.SourceFileEncoding = System.Text.Encoding.Default
						'TODO: Have ArtifactTypeID be passed in on command line, currently hardcoding to 10
						Dim artifactTypeID As Int32
						If CurrentLoadMode = LoadMode.Native Then
							artifactTypeID = Relativity.ArtifactType.Document
						Else
							artifactTypeID = tempLoadFile.ArtifactTypeID
						End If
						tempLoadFile.SelectedIdentifierField = _application.CurrentFields(artifactTypeID, True).IdentifierFields(0)
						Dim mapItemToRemove As LoadFileFieldMap.LoadFileFieldMapItem = Nothing
						If tempLoadFile.GroupIdentifierColumn = "" AndAlso System.IO.File.Exists(tempLoadFile.FilePath) Then
							Dim fieldMapItem As kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem
							For Each fieldMapItem In tempLoadFile.FieldMap
								If Not fieldMapItem.DocumentField Is Nothing AndAlso _
								 fieldMapItem.NativeFileColumnIndex >= 0 AndAlso _
								 fieldMapItem.DocumentField.FieldName.ToLower = "group identifier" Then
									tempLoadFile.GroupIdentifierColumn = _application.GetColumnHeadersFromLoadFile(tempLoadFile, tempLoadFile.FirstLineContainsHeaders)(fieldMapItem.NativeFileColumnIndex)
									'mapItemToRemove = fieldMapItem
								End If
							Next
						End If
						If Not mapItemToRemove Is Nothing Then tempLoadFile.FieldMap.Remove(mapItemToRemove)

						For Each fieldMapItem As kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem In tempLoadFile.FieldMap
							If Not fieldMapItem.DocumentField Is Nothing Then
								Try
									Dim thisField As DocumentField = _application.CurrentFields(tempLoadFile.ArtifactTypeID).Item(fieldMapItem.DocumentField.FieldID)
									fieldMapItem.DocumentField.AssociatedObjectTypeID = thisField.AssociatedObjectTypeID
									fieldMapItem.DocumentField.UseUnicode = thisField.UseUnicode
									fieldMapItem.DocumentField.CodeTypeID = thisField.CodeTypeID
									fieldMapItem.DocumentField.FieldLength = thisField.FieldLength
									fieldMapItem.DocumentField.ImportBehavior = thisField.ImportBehavior
								Catch
								End Try
							End If
						Next
						SelectedNativeLoadFile = tempLoadFile
						SelectedNativeLoadFile.SelectedCasePath = SelectedCaseInfo.DocumentPath
						SelectedNativeLoadFile.Credentials = _application.Credential
						SelectedNativeLoadFile.CookieContainer = _application.CookieContainer
					Case LoadMode.Application
						If System.IO.File.Exists(path) Then Throw New InvalidOperationException("Load file is not supported for application imports")
				End Select
			Catch ex As Exception
				Throw New SavedSettingsRehydrationException(path, ex)
			End Try
		End Sub

		Private Sub SetSourceFileEncoding(ByVal value As String)
			If value Is Nothing OrElse value = "" Then
				SourceFileEncoding = System.Text.Encoding.Default
			Else
				Try
					SourceFileEncoding = System.Text.Encoding.GetEncoding(Int32.Parse(value))
				Catch ex As Exception
					Throw New EncodingException(value, "source file", ex)
				End Try
			End If
		End Sub
		Private Sub SetFullTextFileEncoding(ByVal value As String)
			If value Is Nothing OrElse value = "" Then
				ExtractedTextFileEncoding = System.Text.Encoding.Default
			Else
				Try
					ExtractedTextFileEncoding = System.Text.Encoding.GetEncoding(Int32.Parse(value))
				Catch ex As Exception
					Throw New EncodingException(value, "extracted text files", ex)
				End Try
			End If
		End Sub
		Private Sub SetSelectedCasePath(ByVal value As String)
			If value Is Nothing OrElse value = "" Then
				SelectedCasePath = SelectedCaseInfo.DocumentPath
			Else
				SelectedCasePath = value.TrimEnd("\"c) & "\"
			End If
		End Sub
		Private Sub SetCopyFilesToDocumentRepository(ByVal value As String)
			If value Is Nothing OrElse value = "" Then
				CopyFilesToDocumentRepository = True
			Else
				CopyFilesToDocumentRepository = True
				Try
					CopyFilesToDocumentRepository = Boolean.Parse(value)
				Catch ex As Exception
					Select Case value.ToLower.Substring(0, 1)
						Case "t", "y", "1"
							CopyFilesToDocumentRepository = True
						Case "f", "n", "0"
							CopyFilesToDocumentRepository = False
					End Select
					If value.ToLower = "on" Then
						CopyFilesToDocumentRepository = True
					ElseIf value.ToLower = "off" Then
						CopyFilesToDocumentRepository = False
					End If
				End Try
			End If
		End Sub

		Private Sub SetDestinationFolderID(ByVal value As String)
			If value Is Nothing OrElse value = "" Then
				DestinationFolderID = SelectedCaseInfo.RootFolderID
			Else
				Dim folderManager As New kCura.WinEDDS.Service.FolderManager(_application.Credential, _application.CookieContainer)
				Dim folderExists As Boolean = False
				Try
					folderExists = folderManager.Exists(SelectedCaseInfo.ArtifactID, Int32.Parse(value))
				Catch
				End Try
				If folderExists Then
					DestinationFolderID = Int32.Parse(value)
				Else
					Throw New FolderIdException(value)
				End If
			End If

		End Sub

		Private Sub SetLoadType(ByVal loadTypeString As String)
			Select Case loadTypeString.ToLower.Trim
				Case "i", "image"
					CurrentLoadMode = LoadMode.Image
					HasSetLoadMode = True
				Case "n", "native"
					CurrentLoadMode = LoadMode.Native
					HasSetLoadMode = True
				Case "o", "object"
					CurrentLoadMode = LoadMode.DynamicObject
					HasSetLoadMode = True
				Case "a", "application"
					CurrentLoadMode = LoadMode.Application
					HasSetLoadMode = True
			End Select
			If Not HasSetLoadMode Then Throw New NoLoadTypeModeSetException
		End Sub

		Private Sub SetStartLineNumber(ByVal value As String)
			If value Is Nothing OrElse value = "" Then
				StartLineNumber = 0
			Else
				Try
					StartLineNumber = Int64.Parse(value)
					If StartLineNumber < 0 Then
						Throw New StartLineNumberException(value)
					End If
				Catch ex As Exception
					Throw New StartLineNumberException(value, ex)
				End Try
			End If
		End Sub

#End Region

#Region " Utility "

		Private Sub CloseConsole()
			Dim ActiveWindowHandle As Integer = GetConsoleWindow
			FreeConsole()
			CloseWindow(ActiveWindowHandle)
		End Sub

		Private Sub GetHelpPage()
			Console.WriteLine(kCura.Utility.Resources.Helper.RetrieveDataFromResource("StringConstants", "HelpPage"))
		End Sub

		Private Sub GetEncodingList()
			For Each encodingItem As kCura.EDDS.WinForm.EncodingItem In kCura.EDDS.WinForm.Constants.AllEncodings
				Console.WriteLine(encodingItem.CodePageId.ToString.PadLeft(6, " "c) & "  " & encodingItem.ToString)
			Next
		End Sub
#End Region

	End Module

End Namespace
