Namespace Relativity.Desktop.Client
	Public Class CommandLineParser

		Public Shared Function Parse() As CommandList
			Dim commandList As New CommandList
			Dim commandArgs() As String = System.Environment.GetCommandLineArgs()
			Dim x As Int32
			For x = 1 To commandArgs.Length - 1
				Dim command As New Command
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