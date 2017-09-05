Imports System.Collections.Concurrent

Namespace kCura.WinEDDS.Exporters.LineFactory
	Public MustInherit Class LineFactoryBase
		Public MustOverride Sub WriteLine(ByVal stream As System.IO.StreamWriter, ByVal linesToWriteOpt As ConcurrentDictionary(Of String, String))

		Protected Sub New()
			'Satifies Rule: Abstract types should not have constructors
		End Sub
	End Class
End Namespace
