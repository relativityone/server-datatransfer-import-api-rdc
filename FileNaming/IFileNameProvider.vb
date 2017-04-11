Imports kCura.WinEDDS.Exporters

Namespace kCura.WinEDDS

	Public Interface IFileNameProvider
		Function GetName(exportedObjectInfo As ObjectExportInfo) As String
	End Interface

End Namespace