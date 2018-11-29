Imports System.Collections.Generic
Imports System.IO

Namespace kCura.WinEDDS.Exporters

	Public Class OriginalFileNameProvider
		Private ReadOnly _isFileNameFieldPresent As Boolean
		Private ReadOnly _fieldLookupService As IFieldLookupService
		Private ReadOnly _warningWriter As Action(Of String)


		Public Sub New(isFileNameFieldPresent As Boolean, fieldLookupService As IFieldLookupService, warningWriter As Action(Of String))
			_isFileNameFieldPresent = isFileNameFieldPresent
			_fieldLookupService = fieldLookupService
			_warningWriter = warningWriter
		End Sub

		Public Function GetOriginalFileName(record() As Object, nativeRow As DataRowView) As String
			If _isFileNameFieldPresent
				Dim fileNameObject As Object = record(_fieldLookupService.GetOrdinalIndex("FileName"))
				Dim fileName As String =  If(fileNameObject, String.Empty).ToString()
				If IsFileNameValid(fileName)
					Return filename
				Else
					_warningWriter.Invoke("File Name field contains illegal characters or is empty. Using old original file name: " & nativeRow("Filename").ToString())
				End If
			End If

			Return nativeRow("Filename").ToString()
		End Function
		
		Public Shared Function ExtendFieldRequestByFileNameIfNecessary(exportableFields As ViewFieldInfo(), requestedFields As IList(Of Integer)) As Boolean
			If requestedFields Is Nothing OrElse exportableFields Is Nothing
				Return False
			End If
			Dim fileName As ViewFieldInfo = exportableFields.SingleOrDefault(Function(info) info.DisplayName.Equals("File Name"))
			If fileName IsNot Nothing
				If Not requestedFields.Contains(fileName.AvfId)
					requestedFields.Add(fileName.AvfId)
				End If
				Return True
			End If

			Return False
		End Function

		Private Shared Function IsFileNameValid(ByVal fileName as String) as Boolean
			Dim isCorrect As Boolean = fileName <> ""
			Return isCorrect 
		End Function
	End Class
End NameSpace