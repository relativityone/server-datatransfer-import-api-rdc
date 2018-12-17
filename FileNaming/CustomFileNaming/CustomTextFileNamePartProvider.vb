Imports FileNaming.CustomFileNaming
Imports kCura.WinEDDS.Exporters

Namespace kCura.WinEDDS.FileNaming.CustomFileNaming
	Public Class CustomTextFileNamePartProvider
		Inherits FileNamePartProvider(Of CustomTextDescriptorPart)

		Public Overrides Function GetPartName(customText As CustomTextDescriptorPart, exportObject As ObjectExportInfo) As String
			Return customText.Value
		End Function
	End Class

End Namespace