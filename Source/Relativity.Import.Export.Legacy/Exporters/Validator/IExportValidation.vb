Namespace kCura.WinEDDS.Exporters.Validator
	Public Interface IExportValidation
		Function ValidateExport(exportFile As ExportFile, totalFiles As Int64) As Boolean
	End Interface
End NameSpace