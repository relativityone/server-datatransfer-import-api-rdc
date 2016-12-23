Imports System.Security.AccessControl
Imports kCura.EDDS.WebAPI.RelativityManagerBase
Imports kCura.Utility
Imports kCura.WinEDDS.Credentials
Imports Relativity
Imports RelativityManager = kCura.WinEDDS.Service.RelativityManager

Namespace kCura.EDDS.WinForm

	Public Module Startup

#Region " Library Functions "

		Private Declare Function GetConsoleWindow Lib "kernel32" () As Integer
		Private Declare Function CloseWindow Lib "user32" (ByVal hwnd As Integer) As Integer
		Private Declare Function FreeConsole Lib "kernel32" () As Integer

#End Region

#Region " Members "
		Private _application As kCura.EDDS.WinForm.Application
		Friend HasSetUsername As Boolean = False
		Friend HasSetPassword As Boolean = False
		Private _importOptions As ImportOptions = New ImportOptions()
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

				_importOptions.SetOptions(commandList, _application)

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

					_importOptions.CredentialsAreSet()

					Dim loginResult As Application.CredentialCheckResult = Application.CredentialCheckResult.NotSet
					Try
						If Not String.IsNullOrEmpty(_importOptions.UserName)
							Dim cred As New UserCredentialsProvider(_importOptions.UserName, _importOptions.Password)
							RelativityWebApiCredentialsProvider.Instance().SetProvider(cred)
							loginResult = _application.DoLogin()
						Else 

							loginResult = _application.DoOAuthLogin(_importOptions.ClientId, _importOptions.ClientSecret)
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
					End If

				End If

				Select Case _importOptions.LoadMode
					Case LoadMode.Image
						RunImageImport()
					Case LoadMode.Native
						RunNativeImport()
					Case LoadMode.DynamicObject
						RunDynamicObjectImport()
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

#Region " Run Import "

		Private Sub RunApplicationImport()
			Dim packageData As Byte()
			packageData = System.IO.File.ReadAllBytes(_importOptions.LoadFilePath)
			Dim importer As New kCura.WinEDDS.ApplicationDeploymentProcess(New Int32() {}, Nothing, packageData, _application.Credential, _application.CookieContainer, New Relativity.CaseInfo() {_importOptions.SelectedCaseInfo})
			Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController, _importOptions.ErrorLoadFileLocation, _importOptions.ErrorReportFileLocation)
			_application.StartProcess(importer)
		End Sub

		Private Sub RunDynamicObjectImport()
			Dim importer As New kCura.WinEDDS.ImportLoadFileProcess
			_importOptions.SelectedNativeLoadFile.SourceFileEncoding = _importOptions.SourceFileEncoding
			_importOptions.SelectedNativeLoadFile.ExtractedTextFileEncoding = _importOptions.ExtractedTextFileEncoding
			_importOptions.SelectedNativeLoadFile.SelectedCasePath = _importOptions.SelectedCasePath
			_importOptions.SelectedNativeLoadFile.CopyFilesToDocumentRepository = _importOptions.CopyFilesToDocumentRepository
			_importOptions.SelectedNativeLoadFile.DestinationFolderID = _importOptions.DestinationFolderID
			_importOptions.SelectedNativeLoadFile.StartLineNumber = _importOptions.StartLineNumber
			importer.LoadFile = _importOptions.SelectedNativeLoadFile
			importer.TimeZoneOffset = _application.TimeZoneOffset
			importer.BulkLoadFileFieldDelimiter = Config.BulkLoadFileFieldDelimiter
			importer.CloudInstance = Config.CloudInstance
			importer.ExecutionSource = Relativity.ExecutionSource.Rdc
			_application.SetWorkingDirectory(_importOptions.SelectedNativeLoadFile.FilePath)
			Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController, _importOptions.ErrorLoadFileLocation, _importOptions.ErrorReportFileLocation)
			_application.StartProcess(importer)
		End Sub

		Private Sub RunNativeImport()
			If _importOptions.EnsureEncoding() Then
				Dim folderManager As New kCura.WinEDDS.Service.FolderManager(_application.Credential, _application.CookieContainer)
				If folderManager.Exists(_importOptions.SelectedCaseInfo.ArtifactID, _importOptions.SelectedCaseInfo.RootFolderID) Then
					Dim importer As New kCura.WinEDDS.ImportLoadFileProcess
					_importOptions.SelectedNativeLoadFile.SourceFileEncoding = _importOptions.SourceFileEncoding
					_importOptions.SelectedNativeLoadFile.ExtractedTextFileEncoding = _importOptions.ExtractedTextFileEncoding
					_importOptions.SelectedNativeLoadFile.SelectedCasePath = _importOptions.SelectedCasePath
					_importOptions.SelectedNativeLoadFile.CopyFilesToDocumentRepository = _importOptions.CopyFilesToDocumentRepository
					_importOptions.SelectedNativeLoadFile.DestinationFolderID = _importOptions.DestinationFolderID
					_importOptions.SelectedNativeLoadFile.StartLineNumber = _importOptions.StartLineNumber
					importer.LoadFile = _importOptions.SelectedNativeLoadFile
					importer.TimeZoneOffset = _application.TimeZoneOffset
					importer.BulkLoadFileFieldDelimiter = Config.BulkLoadFileFieldDelimiter
					importer.CloudInstance = Config.CloudInstance
					importer.ExecutionSource = Relativity.ExecutionSource.Rdc
					_importOptions.SelectedNativeLoadFile.ArtifactTypeID = Relativity.ArtifactType.Document
					_application.SetWorkingDirectory(_importOptions.SelectedNativeLoadFile.FilePath)
					Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController, _importOptions.ErrorLoadFileLocation, _importOptions.ErrorReportFileLocation)
					_application.StartProcess(importer)
				End If
			Else
				Throw New EncodingMisMatchException(_importOptions.SourceFileEncoding.CodePage, kCura.WinEDDS.Utility.DetectEncoding(_importOptions.LoadFilePath, True).DeterminedEncoding.CodePage)
			End If
		End Sub

		Private Sub RunImageImport()
			If _importOptions.EnsureEncoding() Then
				Dim importer As New kCura.WinEDDS.ImportImageFileProcess
				_importOptions.SelectedImageLoadFile.CookieContainer = _application.CookieContainer
				_importOptions.SelectedImageLoadFile.Credential = _application.Credential
				_importOptions.SelectedImageLoadFile.SelectedCasePath = _importOptions.SelectedCasePath
				_importOptions.SelectedImageLoadFile.CaseDefaultPath = _importOptions.SelectedCaseInfo.DocumentPath
				_importOptions.SelectedImageLoadFile.CopyFilesToDocumentRepository = _importOptions.CopyFilesToDocumentRepository
				_importOptions.SelectedImageLoadFile.FullTextEncoding = _importOptions.ExtractedTextFileEncoding
				_importOptions.SelectedImageLoadFile.StartLineNumber = _importOptions.StartLineNumber
				importer.ImageLoadFile = _importOptions.SelectedImageLoadFile
				importer.CloudInstance = Config.CloudInstance
				importer.ExecutionSource = ExecutionSource.Rdc
				_application.SetWorkingDirectory(_importOptions.SelectedImageLoadFile.FileName)
				Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController, _importOptions.ErrorLoadFileLocation, _importOptions.ErrorReportFileLocation)
				_application.StartProcess(importer)
			Else
				Throw New EncodingMisMatchException(_importOptions.SourceFileEncoding.CodePage, kCura.WinEDDS.Utility.DetectEncoding(_importOptions.LoadFilePath, True).DeterminedEncoding.CodePage)
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

	Public Class ClientIDException
		Inherits RdcBaseException
		Public Sub New()
			MyBase.New("Invalid or no Client ID specified")
		End Sub
	End Class

	Public Class ClientSecretException
		Inherits RdcBaseException
		Public Sub New()
			MyBase.New("Invalid or no Client Secret specified")
		End Sub
	End Class

	Public Class MultipleCredentialException
		Inherits RdcBaseException

		Public Sub New()
			MyBase.New("Mutiple credentials specified.  Please only specify Username and Password or Client ID and Secret.")
		End Sub
	End Class

	public Class ClientCrendentialsException
		Inherits RdcBaseException

		Public Sub New()
			MyBase.New("Invalid credentials specified. Please specify a valid ClientID and ClientSecret combination")
		End Sub
	End Class

	Public Class ConnectToIdentityServerException
		Inherits RdcBaseException

		Public Sub New()
			Mybase.New("Failed to connect to Identity server. Ensure your Identity server is running and accessible from this location.")
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
