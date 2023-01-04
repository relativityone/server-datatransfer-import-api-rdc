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
			If exportedObjectInfo Is Nothing Then
				Throw New ArgumentNullException(NameOf(exportedObjectInfo))
			End If
			Return exportedObjectInfo.ProductionBeginBatesFileName(_exportSettings.AppendOriginalFileName, _nameTextAndNativesAfterBegBates)
		End Function

		Public Function GetTextName(exportedObjectInfo As ObjectExportInfo) As String Implements IFileNameProvider.GetTextName
			If exportedObjectInfo Is Nothing Then
				Throw New ArgumentNullException(NameOf(exportedObjectInfo))
			End If
			Return exportedObjectInfo.FullTextFileName(
				nameFilesAfterIdentifier := AreFilesNamedAfterIdentifier(),
				tryProductionBegBates := False, 
				appendOriginalFilename := _exportSettings.AppendOriginalFileName)
		End Function

		Public Function GetPdfName(exportedObjectInfo As ObjectExportInfo) As String Implements IFileNameProvider.GetPdfName
			Throw New InvalidOperationException("Production export doesn't support searchable PDFs")
		End Function

		Private Function AreFilesNamedAfterIdentifier() As Boolean
			Dim productionTypeExport As Boolean = _exportSettings.TypeOfExport = ExportFile.ExportType.Production
			Return productionTypeExport AndAlso _exportSettings.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Identifier
		End Function

	End Class
End Namespace

