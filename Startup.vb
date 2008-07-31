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
		Private SelectedCaseInfo As kCura.EDDS.Types.CaseInfo
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
#End Region

#Region " Enumerations "

		Public Enum LoadMode
			Image
			Native
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
				SetSourceFileEncoding(GetValueFromCommandListByFlag(commandList, "e"))
				SetFullTextFileEncoding(GetValueFromCommandListByFlag(commandList, "x"))
				SetSelectedCasePath(GetValueFromCommandListByFlag(commandList, "r"))
				SetCopyFilesToDocumentRepository(GetValueFromCommandListByFlag(commandList, "l"))
				SetDestinationFolderID(GetValueFromCommandListByFlag(commandList, "d"))
				Select Case CurrentLoadMode
					Case LoadMode.Image
						RunImageImport()
					Case LoadMode.Native
						RunNativeImport()
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

		Private Sub RunNativeImport()
			Dim folderManager As New kCura.WinEDDS.Service.FolderManager(_application.Credential, _application.CookieContainer)
			If folderManager.Exists(SelectedCaseInfo.ArtifactID, SelectedCaseInfo.RootFolderID) Then
				Dim frm As New kCura.Windows.Process.ProgressForm
				Dim importer As New kCura.WinEDDS.ImportLoadFileProcess
				SelectedNativeLoadFile.SourceFileEncoding = SourceFileEncoding
				SelectedNativeLoadFile.ExtractedTextFileEncoding = ExtractedTextFileEncoding
				SelectedNativeLoadFile.SelectedCasePath = SelectedCasePath
				SelectedNativeLoadFile.CopyFilesToDocumentRepository = CopyFilesToDocumentRepository
				SelectedNativeLoadFile.DestinationFolderID = DestinationFolderID
				importer.LoadFile = SelectedNativeLoadFile
				importer.TimeZoneOffset = _application.TimeZoneOffset
				_application.SetWorkingDirectory(SelectedNativeLoadFile.FilePath)
				Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController)
				_application.StartProcess(importer)
			End If
		End Sub

		Private Sub RunImageImport()
			Dim importer As New kCura.WinEDDS.ImportImageFileProcess
			SelectedImageLoadFile.CookieContainer = _application.CookieContainer
			SelectedImageLoadFile.Credential = _application.Credential
			SelectedImageLoadFile.SelectedCasePath = SelectedCasePath
			SelectedImageLoadFile.CaseDefaultPath = SelectedCaseInfo.DocumentPath
			SelectedImageLoadFile.CopyFilesToDocumentRepository = CopyFilesToDocumentRepository
			SelectedImageLoadFile.FullTextEncoding = ExtractedTextFileEncoding
			importer.ImageLoadFile = SelectedImageLoadFile
			_application.SetWorkingDirectory(SelectedImageLoadFile.FileName)
			Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController)
			_application.StartProcess(importer)
		End Sub

#End Region

#Region " Input Validation "

		Private Sub SetFileLocation(ByVal path As String)
			If Not System.IO.File.Exists(path) Then Throw New LoadFilePathException(path)
			_loadFilePath = path
			HasSetLoadFileLocation = True
		End Sub

		Private Sub SetCaseInfo(ByVal caseID As String)
			Try
				Dim caseManager As New kCura.WinEDDS.Service.CaseManager(_application.Credential, _application.CookieContainer)
				SelectedCaseInfo = caseManager.Read(Int32.Parse(caseID))
			Catch ex As Exception
				Throw New CaseArtifactIdException(caseID)
			End Try
			If SelectedCaseInfo Is Nothing Then Throw New CaseArtifactIdException(caseID)
			_application.RefreshSelectedCaseInfo(SelectedCaseInfo)
			HasSetCaseInfo = True
		End Sub

		Private Sub SetSavedMapLocation(ByVal path As String)
			Try
				If Not System.IO.File.Exists(path) Then Throw New SavedSettingsFilePathException(path)
				Select Case CurrentLoadMode
					Case LoadMode.Image
						SelectedImageLoadFile = _application.ReadImageLoadFile(path)
					Case LoadMode.Native
						Dim sr As New System.IO.StreamReader(path)
						Dim tempLoadFile As WinEDDS.LoadFile
						Dim deserializer As New System.Runtime.Serialization.Formatters.Soap.SoapFormatter
						tempLoadFile = DirectCast(deserializer.Deserialize(sr.BaseStream), WinEDDS.LoadFile)
						sr.Close()
						tempLoadFile.FilePath = _loadFilePath
						tempLoadFile.CaseInfo = SelectedCaseInfo
						tempLoadFile.CopyFilesToDocumentRepository = True						'LoadFile.CopyFilesToDocumentRepository
						tempLoadFile.SelectedCasePath = SelectedCaseInfo.DocumentPath						''''''''''
						tempLoadFile.CaseDefaultPath = SelectedCaseInfo.DocumentPath
						tempLoadFile.Credentials = _application.Credential
						tempLoadFile.DestinationFolderID = 35
						tempLoadFile.ExtractedTextFileEncoding = System.Text.Encoding.Unicode
						tempLoadFile.SourceFileEncoding = System.Text.Encoding.Default
						tempLoadFile.SelectedIdentifierField = _application.CurrentFields(True).IdentifierFields(0)
						Dim mapItemToRemove As LoadFileFieldMap.LoadFileFieldMapItem
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
						SelectedNativeLoadFile = tempLoadFile
						SelectedNativeLoadFile.SelectedCasePath = SelectedCaseInfo.DocumentPath
						SelectedNativeLoadFile.Credentials = _application.Credential
						SelectedNativeLoadFile.CookieContainer = _application.CookieContainer
				End Select
			Catch ex As Exception
				Throw New SavedSettingsRehydrationException(path)
			End Try
		End Sub

		Private Sub SetSourceFileEncoding(ByVal value As String)
			If value Is Nothing OrElse value = "" Then
				SourceFileEncoding = System.Text.Encoding.Default
			Else
				Try
					SourceFileEncoding = System.Text.Encoding.GetEncoding(Int32.Parse(value))
				Catch
					Throw New EncodingException(value, "source file")
				End Try
			End If
		End Sub
		Private Sub SetFullTextFileEncoding(ByVal value As String)
			If value Is Nothing OrElse value = "" Then
				ExtractedTextFileEncoding = System.Text.Encoding.Default
			Else
				Try
					ExtractedTextFileEncoding = System.Text.Encoding.GetEncoding(Int32.Parse(value))
				Catch
					Throw New EncodingException(value, "extracted text files")
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
				If Not folderExists Then
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
			End Select
			If Not HasSetLoadMode Then Throw New NoLoadTypeModeSetException
		End Sub

#End Region

#Region " Utility "

		Private Sub CloseConsole()
			Dim ActiveWindowHandle As Integer = GetConsoleWindow
			FreeConsole()
			CloseWindow(ActiveWindowHandle)
		End Sub

		Private Function GetHelpPage() As String
			Console.WriteLine(kCura.Resources.Helper.RetrieveDataFromResource("StringConstants", "HelpPage"))
		End Function

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
			MyBase.New(message)
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
			MyBase.New(String.Format("The case specified by the following ID does not exist: " & caseArtifactID))
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
			MyBase.New("The saved settings file specified is in an invalid format: " & path)
		End Sub
	End Class

	Public Class NoLoadTypeModeSetException
		Inherits RdcBaseException
		Public Sub New()
			MyBase.New("No load type or invalid load type set. Available options are ""image"" or ""native""")
		End Sub
	End Class

	Public Class EncodingException
		Inherits RdcBaseException
		Public Sub New(ByVal id As String, ByVal destination As String)
			MyBase.New(String.Format("Invalid encoding set for {1}.  Encoding id '{0}' not supported.  Use -h:encoding for a list of supported encoding ids", id, destination))
		End Sub
	End Class

	Public Class FolderIdException
		Inherits RdcBaseException
		Public Sub New(ByVal id As String)
			MyBase.New(String.Format("There is no folder in the selected case with the id '{0}'", id))
		End Sub
	End Class

#End Region

End Namespace
