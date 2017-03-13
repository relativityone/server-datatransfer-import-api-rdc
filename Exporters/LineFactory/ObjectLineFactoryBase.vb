Imports System.Collections.Concurrent

Namespace kCura.WinEDDS.Exporters.LineFactory
	Public Class ObjectLineFactoryBase
		Inherits LineFactoryBase

		Public Overrides Sub WriteLine(ByVal stream As System.IO.StreamWriter, ByVal linesToWriteOpt As ConcurrentDictionary(Of String, String))

		End Sub
	End Class
End Namespace
