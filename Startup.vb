
Imports System.Threading.Tasks
Imports kCura.EDDS.WinForm.Exceptions
Imports kCura.WinEDDS.Container
Imports kCura.WinEDDS.Core.Export.VolumeManagerV2.Container

Namespace kCura.EDDS.WinForm

	Public Module Startup

#Region " Library Functions "

		Private Declare Function GetConsoleWindow Lib "kernel32" () As Integer
		Private Declare Function CloseWindow Lib "user32" (ByVal hwnd As Integer) As Integer
		Private Declare Function FreeConsole Lib "kernel32" () As Integer

#End Region

#Region " Members "
		Friend _application As kCura.EDDS.WinForm.Application
		Friend HasSetUsername As Boolean = False
		Friend HasSetPassword As Boolean = False
		Private _importOptions As ImportOptions = New ImportOptions()
		Private _authOptions As AuthenticationOptions = new AuthenticationOptions()
#End Region

#Region " Enumerations "

		Public Enum LoadMode
			Image
			Native
			DynamicObject
			Application
			Export
		End Enum

#End Region

		Public Sub Main()
			ContainerFactoryProvider.ContainerFactory = new ContainerFactory()
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
				Task.Run(Async Function() As Task
							 Await RunInConsoleMode().ConfigureAwait(False)
						 End Function).Wait()
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

		Private Async Function RunInConsoleMode() As Task
			Try
				_application = kCura.EDDS.WinForm.Application.Instance
			    Dim _import As ImportManager = New ImportManager(new ExportConfig(_application.RelativityVersion))

				Dim commandList As kCura.CommandLine.CommandList = kCura.CommandLine.CommandLineParser.Parse
				For Each command As kCura.CommandLine.Command In commandList
					If command.Directive.ToLower.Replace("-", "").Replace("/", "") = "h" Then
						If command.Value Is Nothing OrElse command.Value = "" Then
							GetHelpPage()
							Return
						End If
						If command.Value.ToLower = "encoding" Then
							GetEncodingList()
							Return
						End If
						GetHelpPage()
						Return
					End If
				Next

				_authOptions.SetCredentials(commandList)

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
					Return
				ElseIf Not defaultCredentialResult = Application.CredentialCheckResult.Success Then

					_authOptions.CredentialsAreSet()

					Dim loginResult As Application.CredentialCheckResult = Application.CredentialCheckResult.NotSet
					Try
						loginResult = _application.Login(_authOptions)
					Catch ex As Exception
						loginResult = Application.CredentialCheckResult.Fail
					End Try
					If loginResult = Application.CredentialCheckResult.AccessDisabled Then
						Console.WriteLine(Application.ACCESS_DISABLED_MESSAGE)
						Return
					ElseIf loginResult = Application.CredentialCheckResult.InvalidClientCredentials Then
						Throw New ClientCrendentialsException
					ElseIf loginResult = Application.CredentialCheckResult.FailToConnectToIdentityServer Then
						Throw New ConnectToIdentityServerException
					ElseIf Not loginResult = Application.CredentialCheckResult.Success Then
						Throw New CredentialsException
					End If

				End If

				Await _importOptions.SetOptions(commandList)

				Select Case _importOptions.LoadMode
					Case LoadMode.Image
						Await _import.RunImageImport(_importOptions)
					Case LoadMode.Native
						Await _import.RunNativeImport(_importOptions)
					Case LoadMode.DynamicObject
						_import.RunDynamicObjectImport(_importOptions)
					Case LoadMode.Application
						Await _import.RunApplicationImport(_importOptions)
					Case LoadMode.Export
						Await _import.RunExport(_importOptions.SelectedExportSettings)
				End Select

				Await _application.Logout()
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

		End Function

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
