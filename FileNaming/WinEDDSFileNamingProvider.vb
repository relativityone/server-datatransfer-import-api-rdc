Imports System.Collections.Generic
Imports kCura.WinEDDS.Exporters

Namespace kCura.WinEDDS
	Public Class WinEDDSFileNamingProvider
		Implements IFileNameProvider

		Private ReadOnly _exportSettings As ExportFile
		Private ReadOnly _fileNameProviders As IDictionary(Of ExportNativeWithFilenameFrom, IFileNameProvider)

		Sub New(exportSettings As ExportFile, fileNameProviders As IDictionary(Of ExportNativeWithFilenameFrom, IFileNameProvider))
			_exportSettings = exportSettings
			_fileNameProviders = fileNameProviders
		End Sub

		Public Function GetName(exportedObjectInfo As ObjectExportInfo) As String Implements IFileNameProvider.GetName
			If (_fileNameProviders.ContainsKey(_exportSettings.ExportNativesToFileNamedFrom))
				Return _fileNameProviders(_exportSettings.ExportNativesToFileNamedFrom).GetName(exportedObjectInfo)
			Else 
				Return Nothing
			End If
		End Function
	End Class
End Namespace
