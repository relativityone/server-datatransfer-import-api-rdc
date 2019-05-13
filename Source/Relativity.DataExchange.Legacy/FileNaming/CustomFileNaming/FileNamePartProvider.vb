Imports FileNaming.CustomFileNaming
Imports kCura.WinEDDS.Exporters

Namespace kCura.WinEDDS.FileNaming.CustomFileNaming
	Public MustInherit Class FileNamePartProvider(Of T As DescriptorPart)
		Implements IFileNamePartProvider(Of T)

		Public MustOverride Function GetPartName(descriptorPart As T, exportObject As ObjectExportInfo) As String Implements IFileNamePartProvider(Of T).GetPartName

		Public Overridable Function GetPartName(descriptorPart As DescriptorPart, exportObject As ObjectExportInfo) As String Implements IFileNamePartProvider.GetPartName
			Return GetPartName(TryCast(descriptorPart, T), exportObject)
		End Function
	End Class
End Namespace
