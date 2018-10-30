Imports FileNaming.CustomFileNaming
Imports kCura.WinEDDS.Exporters

Namespace kCura.WinEDDS.FileNaming.CustomFileNaming
	Public Class SeparatorFileNamePartProvider
		Inherits FileNamePartProvider(Of SeparatorDescriptorPart)

		Public Overrides Function GetPartName(descriptorPart As SeparatorDescriptorPart, exportObject As ObjectExportInfo) As String
			Return descriptorPart.Value
		End Function

	End Class
End Namespace
