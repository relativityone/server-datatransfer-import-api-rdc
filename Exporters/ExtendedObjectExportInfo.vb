Imports System.Collections.Generic
Imports kCura.WinEDDS.Exporters

Namespace kCura.WinEDDS.FileNaming.CustomFileNaming
	Public Class ExtendedObjectExportInfo
		Inherits ObjectExportInfo

		Private ReadOnly _fieldLookupService As IFieldLookupService

		Public Sub New(fieldLookupService As IFieldLookupService)
			_fieldLookupService = fieldLookupService
			SelectedNativeFileNameViewFields = New List(Of ViewFieldInfo)()
		End Sub

		Public Property SelectedNativeFileNameViewFields As List(Of ViewFieldInfo)

		Public Function GetFieldValue(fieldName As String) As Object
			Return Metadata(_fieldLookupService.GetOrdinalIndex(fieldName))
		End Function

	End Class
End Namespace
