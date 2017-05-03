Imports kCura.WinEDDS.Exporters

Namespace kCura.WinEDDS
	Public Class ProductionExportFileNameProvider
		Implements IFileNameProvider

		Private ReadOnly _exportSettings As ExportFile
		Private ReadOnly _nameTextAndNativesAfterBegBates As Boolean

		Public Sub New(exportSettings As ExportFile, nameTextAndNativesAfterBegBates As Boolean)
			_exportSettings = exportSettings
			_nameTextAndNativesAfterBegBates = nameTextAndNativesAfterBegBates
		End Sub

		Public Function GetName(exportedObjectInfo As ObjectExportInfo) As String Implements IFileNameProvider.GetName
			Return exportedObjectInfo.ProductionBeginBatesFileName(_exportSettings.AppendOriginalFileName, _nameTextAndNativesAfterBegBates)
		End Function

		Public Function GetTextName(exportedObjectInfo As ObjectExportInfo) As String Implements IFileNameProvider.GetTextName
			Dim productionTypeExport As Boolean = (_exportSettings.TypeOfExport = ExportFile.ExportType.Production)
			Return exportedObjectInfo.FullTextFileName(productionTypeExport AndAlso
													_exportSettings.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Identifier,
													False)
		End Function

	End Class
End Namespace

