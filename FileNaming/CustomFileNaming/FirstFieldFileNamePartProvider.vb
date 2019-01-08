Imports FileNaming.CustomFileNaming
Imports kCura.WinEDDS.Exporters

Namespace kCura.WinEDDS.FileNaming.CustomFileNaming
	Public Class FirstFieldFileNamePartProvider
		Inherits FileNamePartProvider(Of FirstFieldDescriptorPart)

		Public Overrides Function GetPartName(descriptorPart As FirstFieldDescriptorPart, exportObject As ObjectExportInfo) As String
			If descriptorPart.isProduction AndAlso exportObject.ProductionBeginBates IsNot Nothing AndAlso exportObject.ProductionBeginBates <> ""
				Return exportObject.ProductionBeginBates
			End If
			Return exportObject.IdentifierValue
		End Function
	End Class

End Namespace