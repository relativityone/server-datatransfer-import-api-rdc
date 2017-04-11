Imports kCura.WinEDDS.Exporters

Namespace kCura.WinEDDS
	Public Class ProductionExportFileNameProvider
		Implements IFileNameProvider

		Private ReadOnly _exportedObjectInfo As ObjectExportInfo
		Private ReadOnly _exportSettings As ExportFile
		Private ReadOnly _nameTextAndNativesAfterBegBates As Boolean
		
		Public Sub New(exportedObjectInfo As ObjectExportInfo, exportSettings As ExportFile, nameTextAndNativesAfterBegBates As Boolean)
			_exportedObjectInfo = exportedObjectInfo
			_exportSettings = exportSettings
			_nameTextAndNativesAfterBegBates = nameTextAndNativesAfterBegBates
		End Sub

		Public Function GetName() As String Implements IFileNameProvider.GetName
			Return _exportedObjectInfo.ProductionBeginBatesFileName(_exportSettings.AppendOriginalFileName, _nameTextAndNativesAfterBegBates)
		End Function
	End Class
End Namespace

