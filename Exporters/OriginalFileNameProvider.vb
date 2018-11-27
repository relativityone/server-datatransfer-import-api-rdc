Imports System.Collections.Generic

Namespace kCura.WinEDDS.Exporters

	Public Class OriginalFileNameProvider

		Public Sub New(isFileNameFieldPresent As Boolean, fieldLookupService As IFieldLookupService, warningWriter As Action(Of String))
		End Sub

		Public Property FieldLookupService As IFieldLookupService
	
		Public Function GetOriginalFileName(record() As Object, nativeRow As DataRowView) As String
			Return ""
		End Function

		Public Shared Function ExtendFieldRequestByFileNameIfNecessary(exportableFields As ViewFieldInfo(), requestedFields As IList(Of Integer)) As Boolean
			Return false
		End Function
	End Class
End NameSpace