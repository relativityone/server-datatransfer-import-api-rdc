Imports System.Collections.Concurrent
Imports System.Collections.Generic

Namespace kCura.WinEDDS.Exporters.LineFactory
	Public Class ObjectLineFactoryBase
		Inherits LineFactoryBase

		Public Overrides Sub WriteLine(ByVal stream As System.IO.StreamWriter, ByVal linesToWriteOpt As List(Of KeyValuePair(Of String, String)))

		End Sub
	End Class
End Namespace
