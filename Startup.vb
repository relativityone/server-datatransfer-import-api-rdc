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
		Private SelectedNativeLoadFile As kCura.WinEDDS.LoadFile
		Private SelectedImageLoadFile As kCura.WinEDDS.ImageLoadFile
		Private CurrentLoadMode As LoadMode
		Private HasSetLoadFileLocation As Boolean = False
		Private HasSetCaseInfo As Boolean = False
		Private HasSetUsername As Boolean = False
		Private HasSetPassword As Boolean = False
		Private HasSetSavedMapLocation As Boolean = False
		Private HasSetLoadMode As Boolean = False

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

		Private Sub RunInConsoleMode()
			Try
				_application = kCura.EDDS.WinForm.Application.Instance
				'_application.DoLogin(
				Dim commandList As kCura.CommandLine.CommandList = kCura.CommandLine.CommandLineParser.Parse
				For Each command As kCura.CommandLine.Command In commandList
					If command.Value Is Nothing OrElse command.Value = "" Then
						If command.Directive.ToLower.Replace("-", "").Replace("/", "") = "h" Then
							GetHelpPage()
							Exit Sub
						Else
							Throw New InvalidFlagException(command.Directive)
						End If
					End If
					Select Case command.Directive.ToLower.TrimStart("-/".ToCharArray)
						Case "h"
							GetHelpPage()
						Case "f"
							SetFileLocation(command)
						Case "c"
							SetCaseInfo(command)
						Case "u"
							SetUserName(command)
						Case "p"
							SetPassword(command)
						Case "k"
							SetSavedMapLocation(command)
						Case "m"
							SetLoadType(command)
					End Select
				Next
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
			importer.ImageLoadFile = SelectedImageLoadFile
			_application.SetWorkingDirectory(SelectedImageLoadFile.FileName)
			Dim executor As New kCura.EDDS.WinForm.CommandLineProcessRunner(importer.ProcessObserver, importer.ProcessController)
			_application.StartProcess(importer)
		End Sub

#End Region

#Region " Input Validation "

		Private Sub SetFileLocation(ByVal command As kCura.CommandLine.Command)

			HasSetLoadFileLocation = True
		End Sub

		Private Sub SetCaseInfo(ByVal command As kCura.CommandLine.Command)
			HasSetCaseInfo = True
		End Sub

		Private Sub SetUserName(ByVal command As kCura.CommandLine.Command)
			HasSetUsername = True
		End Sub

		Private Sub SetPassword(ByVal command As kCura.CommandLine.Command)
			HasSetPassword = True
		End Sub

		Private Sub SetSavedMapLocation(ByVal command As kCura.CommandLine.Command)
			HasSetSavedMapLocation = True
		End Sub

		Private Sub SetLoadType(ByVal command As kCura.CommandLine.Command)
			HasSetLoadMode = True
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

#End Region

	End Module

#Region " Exception "

	Public MustInherit Class RdcBaseException
		Inherits System.Exception
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub
	End Class

	Public Class InvalidFlagException
		Inherits RdcBaseException
		Public Sub New(ByVal flag As String)
			MyBase.New(String.Format("Flag {0} is invalid", flag))
		End Sub
	End Class

#End Region

End Namespace
