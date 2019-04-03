Namespace Relativity.Desktop.Client.CommandLine
	Public Class CommandLineParser

		Public Shared Function Parse() As Relativity.Desktop.Client.CommandLine.CommandList
			Dim commandList As New Relativity.Desktop.Client.CommandLine.CommandList
			Dim commandArgs() As String = System.Environment.GetCommandLineArgs()
			Dim x As Int32
			For x = 1 To commandArgs.Length - 1
				Dim command As New Relativity.Desktop.Client.CommandLine.Command
				If commandArgs(x).IndexOf(":") > -1 Then
					command.Directive = commandArgs(x).Split(":"c)(0)
					command.Value = commandArgs(x).Substring(commandArgs(x).IndexOf(":") + 1)
				Else
					command.Directive = commandArgs(x)
				End If
				commandList.Add(command)
			Next
			Return commandList
		End Function
	End Class
End Namespace