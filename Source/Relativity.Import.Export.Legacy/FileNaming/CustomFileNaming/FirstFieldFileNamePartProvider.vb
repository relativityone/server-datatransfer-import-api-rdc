Imports FileNaming.CustomFileNaming
Imports kCura.WinEDDS.Exporters

Namespace kCura.WinEDDS.FileNaming.CustomFileNaming
	Public Class FirstFieldFileNamePartProvider
		Inherits FileNamePartProvider(Of FirstFieldDescriptorPart)

		Public Overrides Function GetPartName(descriptorPart As FirstFieldDescriptorPart, exportObject As ObjectExportInfo) As String
			If descriptorPart Is Nothing Then
				Throw New ArgumentException($"Expected instance of {nameof(FirstFieldDescriptorPart)}")
			End If

			If ShouldNameAfterProductionBeginBates(descriptorPart, exportObject)
				Return exportObject.ProductionBeginBates
			End If
			Return exportObject.IdentifierValue
		End Function

		Private Shared Function ShouldNameAfterProductionBeginBates(descriptorPart As FirstFieldDescriptorPart, exportObject As ObjectExportInfo) As Boolean
			Return descriptorPart.IsProduction AndAlso Not String.IsNullOrEmpty(exportObject.ProductionBeginBates)
		End Function
	End Class

End Namespace