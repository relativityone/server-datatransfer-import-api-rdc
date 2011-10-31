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

			Dim args As String() = System.Environment.GetCommandLineArgs
			If args.Length = 1 Then
				CloseConsole()
				Dim frm As New kCura.EDDS.WinForm.MainForm
				frm.Show()
				frm.Refresh()
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
				If kCura.WinEDDS.Config.WebServiceURL = "" Then
					Console.WriteLine("Web Service URL not set.  Please enter:")
					Dim webserviceurl As String = Console.ReadLine
					While Not UrlIsValid(webserviceurl)
						Console.WriteLine("Invalid Web Service URL set.  Retry:")
						webserviceurl = Console.ReadLine.Trim
					End While
					kCura.WinEDDS.Config.WebServiceURL = webserviceurl
				End If
				If Not _application.DefaultCredentialsAreGood Then
					Dim userName As String = ""
					Dim password As String = ""
					userName = GetValueFromCommandListByFlag(commandList, "u")
					password = GetValueFromCommandListByFlag(commandList, "p")
					If userName Is Nothing OrElse userName = "" Then Throw New UsernameException
					If password Is Nothing OrElse password = "" Then Throw New PasswordException
					Dim isValidLogin As Boolean = False
					Dim cred As New System.Net.NetworkCredential(userName, password)
					Try
						isValidLogin = _application.DoLogin(cred)
					Catch
						isValidLogin = False
					End Try
					If Not isValidLogin Then Throw New CredentialsException
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


#Region " Run Import "

		Private Sub RunApplicationImport()
			Dim template As New Xml.XmlDocument
			Dim stream As System.IO.FileStream = System.IO.File.Open(_loadFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read)
			template.Load(stream)
			stream.Close()

			Dim importer As New kCura.WinEDDS.ApplicationDeploymentProcess(New Int32() {}, Nothing, template, _application.Credential, _application.CookieContainer, New Relativity.CaseInfo() {SelectedCaseInfo})
			Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController, ErrorLoadFileLocation, ErrorReportFileLocation)
			_application.StartProcess(importer)
		End Sub

		Private Sub RunDynamicObjectImport(ByVal commandList As kCura.CommandLine.CommandList)
			Dim frm As New kCura.Windows.Process.ProgressForm
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
			_application.SetWorkingDirectory(SelectedNativeLoadFile.FilePath)
			Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController, ErrorLoadFileLocation, ErrorReportFileLocation)
			_application.StartProcess(importer)
		End Sub

		Private Sub RunNativeImport()
			If EnsureEncoding() Then
				Dim folderManager As New kCura.WinEDDS.Service.FolderManager(_application.Credential, _application.CookieContainer)
				If folderManager.Exists(SelectedCaseInfo.ArtifactID, SelectedCaseInfo.RootFolderID) Then
					Dim frm As New kCura.Windows.Process.ProgressForm
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

		Private Function GetArtifactTypeID(ByVal commandLine As kCura.CommandLine.CommandList) As Int32
			For Each command As kCura.CommandLine.Command In commandLine
				If command.Directive.ToLower.Replace("-", "").Replace("/", "") = "ot" Then
					If command.Value Is Nothing Then command.Value = ""
					If command.Value = "" Then Throw New InvalidArtifactTypeException(command.Value)
					For Each row As System.Data.DataRow In _application.AllUploadableArtifactTypes.Rows
						If Relativity.SqlNameHelper.GetSqlFriendlyName(row("Name").ToString).ToLower = Relativity.SqlNameHelper.GetSqlFriendlyName(command.Value).ToLower Then
							Return CType(row("DescriptorArtifactTypeID"), Int32)
						End If
					Next
					For Each row As System.Data.DataRow In _application.AllUploadableArtifactTypes.Rows
						If row("DescriptorArtifactTypeID").ToString.ToLower = command.Value.ToLower Then
							Return CType(row("DescriptorArtifactTypeID"), Int32)
						End If
					Next
					Throw New InvalidArtifactTypeException(command.Value)
				End If
			Next
		End Function


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
			Console.WriteLine(kCura.Resources.Helper.RetrieveDataFromResource("StringConstants", "HelpPage"))
		End Sub

		Private Sub GetEncodingList()
			For Each encodingItem As kCura.EDDS.WinForm.EncodingItem In kCura.EDDS.WinForm.Constants.AllEncodings
				Console.WriteLine(encodingItem.CodePageId.ToString.PadLeft(6, " "c) & "  " & encodingItem.ToString)
			Next
		End Sub
#End Region

	End Module

#Region " Exception "

	Public MustInherit Class RdcBaseException
		Inherits System.Exception
		Public Sub New(ByVal message As String)
			Me.New(message, Nothing)
		End Sub

		Protected Sub New(ByVal message As String, ByVal innerException As System.Exception)
			MyBase.New(message, innerException)
		End Sub
	End Class

	Public Class FlagException
		Inherits RdcBaseException
		Public Sub New(ByVal flag As String)
			MyBase.New(String.Format("Flag {0} is invalid", flag))
		End Sub
	End Class

	Public Class LoadFilePathException
		Inherits RdcBaseException
		Public Sub New()
			MyBase.New("No load file specified")
		End Sub

		Public Sub New(ByVal loadFilePath As String)
			MyBase.New(String.Format("The load file specified does not exist: " & loadFilePath))
		End Sub
	End Class

	Public Class SavedSettingsFilePathException
		Inherits RdcBaseException
		Public Sub New(ByVal loadFilePath As String)
			MyBase.New(String.Format("The saved settings file specified does not exist: " & loadFilePath))
		End Sub
	End Class

	Public Class CaseArtifactIdException
		Inherits RdcBaseException
		Public Sub New(ByVal caseArtifactID As String)
			Me.New(caseArtifactID, Nothing)
		End Sub

		Public Sub New(ByVal caseArtifactID As String, ByVal innerException As System.Exception)
			MyBase.New(String.Format("The case specified by the following ID does not exist: " & caseArtifactID), innerException)
		End Sub
	End Class

	Public Class UsernameException
		Inherits RdcBaseException
		Public Sub New()
			MyBase.New("Invalid or no username specified")
		End Sub
	End Class

	Public Class PasswordException
		Inherits RdcBaseException
		Public Sub New()
			MyBase.New("Invalid or no password specified")
		End Sub
	End Class

	Public Class CredentialsException
		Inherits RdcBaseException
		Public Sub New()
			MyBase.New("Invalid credentials specified.  Please specify an active Relativity account's username and password.")
		End Sub
	End Class

	Public Class SavedSettingsRehydrationException
		Inherits RdcBaseException
		Public Sub New(ByVal path As String)
			Me.New(path, Nothing)
		End Sub

		Public Sub New(ByVal path As String, ByVal innerException As System.Exception)
			MyBase.New("The saved settings file specified is in an invalid format: " & path, innerException)
		End Sub
	End Class

	Public Class NoLoadTypeModeSetException
		Inherits RdcBaseException
		Public Sub New()
			MyBase.New("No load type or invalid load type set. Available options are ""image"", ""native"" or ""object""")
		End Sub
	End Class

	Public Class EncodingMisMatchException
		Inherits RdcBaseException
		Public Sub New(ByVal encoding As Int32, ByVal detectedEncoding As Int32)
			Me.New(encoding, detectedEncoding, Nothing)
		End Sub

		Public Sub New(ByVal encoding As Int32, ByVal detectedEncoding As Int32, ByVal innerException As System.Exception)
			MyBase.New(String.Format("The Encoding id - {0} - selected for your load file does not match the detected Encoding - {1}.  Please select the correct Encoding for your load file.", encoding, detectedEncoding, innerException))
		End Sub
	End Class

	Public Class EncodingException
		Inherits RdcBaseException
		Public Sub New(ByVal id As String, ByVal destination As String)
			Me.New(id, destination, Nothing)
		End Sub

		Public Sub New(ByVal id As String, ByVal destination As String, ByVal innerException As System.Exception)
			MyBase.New(String.Format("Invalid encoding set for {1}.  Encoding id '{0}' not supported.  Use -h:encoding for a list of supported encoding ids", id, destination), innerException)
		End Sub
	End Class

	Public Class FolderIdException
		Inherits RdcBaseException
		Public Sub New(ByVal id As String)
			MyBase.New(String.Format("There is no folder in the selected case with the id '{0}'", id))
		End Sub
	End Class

	Public Class InvalidPathLocationException
		Inherits RdcBaseException
		Public Sub New(ByVal path As String, ByVal type As String)
			Me.New(path, type, Nothing)
		End Sub

		Public Sub New(ByVal path As String, ByVal type As String, ByVal innerException As System.Exception)
			MyBase.New(String.Format("The {0} path {1} is invalid", type, path), innerException)
		End Sub
	End Class

	Public Class InvalidArtifactTypeException
		Inherits RdcBaseException
		Public Sub New(ByVal type As String)
			MyBase.New(String.Format("'{0}' is neither the name or ID of any dynamic object type in the system", type))
		End Sub
	End Class

	Public Class StartLineNumberException
		Inherits RdcBaseException
		Public Sub New(ByVal value As String)
			Me.New(value, Nothing)
		End Sub

		Public Sub New(ByVal value As String, ByVal innerException As System.Exception)
			MyBase.New(String.Format("The specified start line number is not valid: {0}.", value), innerException)
		End Sub
	End Class

#End Region

End Namespace
