
Imports System.Collections.Generic

Namespace kCura.WinEDDS.Exporters

	Public Interface ILoadFileHeaderFormatter
		Function GetHeader(ByVal columns As List(Of ViewFieldInfo)) As String
	End Interface

End Namespace


