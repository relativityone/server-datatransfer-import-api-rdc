Imports FileNaming.CustomFileNaming
Imports kCura.WinEDDS.Exporters

Namespace kCura.WinEDDS.FileNaming.CustomFileNaming
	Public Interface IFileNamePartProvider

		Function GetPartName(descriptorDescriptorPartBase As DescriptorPart, exportObject As ObjectExportInfo) As String

	End Interface

	Public Interface IFileNamePartProvider(Of In T)
		Inherits IFileNamePartProvider

		Function GetPartName(descriptorPart As T, exportObject As ObjectExportInfo) As String

	End Interface
End Namespace
