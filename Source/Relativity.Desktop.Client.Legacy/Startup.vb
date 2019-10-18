Imports kCura.WinEDDS.Container
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Export.VolumeManagerV2.Container


Namespace Relativity.Desktop.Client

	Public Module Startup

#Region " Library Functions "

		Private Declare Function GetConsoleWindow Lib "kernel32" () As Integer
		Private Declare Function CloseWindow Lib "user32" (ByVal hwnd As Integer) As Integer
		Private Declare Function FreeConsole Lib "kernel32" () As Integer

#End Region

#Region " Members "
		Friend _application As Global.Relativity.Desktop.Client.Application
		Private _importOptions As ImportOptions = New ImportOptions()
		Private _authOptions As AuthenticationOptions = New AuthenticationOptions()
		Private _logger As Relativity.Logging.ILog
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
			SetupRelativityLogging()
			ContainerFactoryProvider.ContainerFactory = New ContainerFactory(_logger)
			Dim handler As ThreadExceptionHandler = New ThreadExceptionHandler()
			AddHandler System.Windows.Forms.Application.ThreadException, AddressOf handler.Application_ThreadException

			Dim args As String() = System.Environment.GetCommandLineArgs

			If args.Length = 1 Then
				CloseConsole()
				Dim mainForm As New MainForm()

				mainForm.Show()
				mainForm.Refresh()
				System.Windows.Forms.Application.Run()
			Else
				Task.Run(Async Function() As Task
							 Await RunInConsoleMode().ConfigureAwait(False)
						 End Function).Wait()
			End If
		End Sub

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
				_application = Global.Relativity.Desktop.Client.Application.Instance
				Dim _import As ImportManager = New ImportManager(_logger)

				Dim commandList As CommandList = CommandLineParser.Parse
				For Each command As Command In commandList
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

				If AppSettings.Instance.WebApiServiceUrl = "" OrElse Not UrlIsValid(AppSettings.Instance.WebApiServiceUrl) Then
					Console.WriteLine("Web Service URL not set or not accessible.  Please enter:")
					Dim webserviceurl As String = Console.ReadLine
					While Not UrlIsValid(webserviceurl)
						Console.WriteLine("Invalid Web Service URL set.  Retry:")
						webserviceurl = Console.ReadLine.Trim
					End While
					AppSettings.Instance.WebApiServiceUrl = webserviceurl
				End If
				Dim defaultCredentialResult As Global.Relativity.Desktop.Client.Application.CredentialCheckResult = _application.AttemptWindowsAuthentication()
				If defaultCredentialResult = Global.Relativity.Desktop.Client.Application.CredentialCheckResult.AccessDisabled Then
					Console.WriteLine(Global.Relativity.Desktop.Client.Application.ACCESS_DISABLED_MESSAGE)
					Return
				ElseIf Not defaultCredentialResult = Global.Relativity.Desktop.Client.Application.CredentialCheckResult.Success Then

					_authOptions.CredentialsAreSet()

					Dim loginResult As Global.Relativity.Desktop.Client.Application.CredentialCheckResult = Global.Relativity.Desktop.Client.Application.CredentialCheckResult.NotSet
					Try
						loginResult = _application.Login(_authOptions)
					Catch ex As Exception
						loginResult = Global.Relativity.Desktop.Client.Application.CredentialCheckResult.Fail
					End Try
					If loginResult = Global.Relativity.Desktop.Client.Application.CredentialCheckResult.AccessDisabled Then
						Console.WriteLine(Global.Relativity.Desktop.Client.Application.ACCESS_DISABLED_MESSAGE)
						Return
					ElseIf loginResult = Global.Relativity.Desktop.Client.Application.CredentialCheckResult.InvalidClientCredentials Then
						Throw New ClientCrendentialsException
					ElseIf loginResult = Global.Relativity.Desktop.Client.Application.CredentialCheckResult.FailToConnectToIdentityServer Then
						Throw New ConnectToIdentityServerException
					ElseIf Not loginResult = Global.Relativity.Desktop.Client.Application.CredentialCheckResult.Success Then
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
						Throw New InvalidOperationException("Load file is not supported for application imports")
					Case LoadMode.Export
						Await _import.RunExport(_importOptions.SelectedExportSettings)
				End Select

				Await _application.Logout()
			Catch ex As RdcBaseException
				Console.WriteLine("--------------------------")
				Console.WriteLine("ERROR: " & ex.Message)
				Console.WriteLine("")
				Console.WriteLine("Use Relativity.Desktop.Client.Application.exe -h for help")
				Console.WriteLine("--------------------------")
			Catch ex As RelativityNotSupportedException
				Console.WriteLine("--------------------------")
				Console.WriteLine("NOT SUPPORTED: " & ex.Message)
				Console.WriteLine("--------------------------")
			Catch ex As Exception
				Console.WriteLine("--------------------------")
				Console.WriteLine("FATAL ERROR:")
				Console.WriteLine(ex.ToString)
				Console.WriteLine("--------------------------")
			End Try

		End Function

		Private Sub SetupRelativityLogging()
			Const RdcLoggingApplication = "626BD889-2BFF-4407-9CE5-5CF3712E1BB7"
			Const RdcLoggingSystem = "Relativity.Desktop.Client"
			Const RdcLoggingSubSystem = "Relativity.DataExchange"

			Try
				Dim options As Relativity.Logging.LoggerOptions = New Relativity.Logging.LoggerOptions()
				options.Application = RdcLoggingApplication
				options.System = RdcLoggingSystem
				options.SubSystem = RdcLoggingSubSystem
				Dim configFileName As String = AppSettings.Instance.LogConfigXmlFileName
				If String.IsNullOrWhiteSpace(configFileName) Then
					configFileName = "LogConfig.xml"
				End If

				If (Not String.IsNullOrEmpty(configFileName) AndAlso (System.IO.Path.IsPathRooted(configFileName) AndAlso System.IO.File.Exists(configFileName))) Then
					options.ConfigurationFileLocation = configFileName
				Else 
					Dim assembly As System.Reflection.Assembly = System.Reflection.Assembly.GetEntryAssembly()
					Dim directory As String = System.IO.Directory.GetParent(assembly.Location).FullName
					Dim file = System.IO.Path.Combine(directory, configFileName)
					If (System.IO.File.Exists(file)) Then
						options.ConfigurationFileLocation = file
					End If
				End If

				' Storing the logger reference on the singleton ensures it will be used throughout (see RelativityLogFactory).
				_logger = Relativity.Logging.Factory.LogFactory.GetLogger(options)
			Catch ex As Exception
				Try
					Relativity.Logging.Tools.InternalLogger.WriteFromExternal(
						$"Failed to setup Relativity logging and reverting to a NullLogger. Exception: {ex.ToString()}",
						New Relativity.Logging.LoggerOptions With { .System = RdcLoggingSystem, .SubSystem = RdcLoggingSubSystem })
				Catch ex2 As Exception
					' Being overly cautious to ensure no fatal errors occur due to logging.
				End Try

				_logger = New Relativity.Logging.NullLogger()
			Finally
				Relativity.Logging.Log.Logger = _logger
			End Try
		End Sub

#Region " Utility "

		Private Sub CloseConsole()
			Dim ActiveWindowHandle As Integer = GetConsoleWindow
			FreeConsole()
			CloseWindow(ActiveWindowHandle)
		End Sub

		Private Sub GetHelpPage()
			Dim resourceManager As System.Resources.ResourceManager = New System.Resources.ResourceManager("StringConstants", Application.GetExecutingAssembly())
			Dim contents = resourceManager.GetString("HelpPage")
			Console.WriteLine(contents)
		End Sub

		Private Sub GetEncodingList()
			For Each encodingItem As EncodingItem In Constants.AllEncodings
				Console.WriteLine(encodingItem.CodePageId.ToString.PadLeft(6, " "c) & "  " & encodingItem.ToString)
			Next
		End Sub
#End Region

	End Module

End Namespace