Namespace kCura.WinEDDS.Exporters
	Public Interface ILoadFileCellFormatter
		Function TransformToCell(ByVal contents As String) As String
		Function CreateNativeCell(ByVal location As String, ByVal artifact As Exporters.ObjectExportInfo) As String
		Function CreateImageCell(ByVal artifact As Exporters.ObjectExportInfo) As String

		ReadOnly Property RowPrefix() As String
		ReadOnly Property RowSuffix() As String
	End Interface





	
End Namespace

