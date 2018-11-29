﻿Imports System.Collections.Generic
Imports Relativity

Namespace kCura.WinEDDS.Exporters

	Public Class OriginalFileNameProvider
		Private Const _FILE_NAME_FIELD_COLUMN_NAME As String = "FileName"
		Private Const _FILE_NAME_FILE_TABLE_COLUMN_NAME  As String = "Filename"

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
				Dim fileNameObject As Object = record(_fieldLookupService.GetOrdinalIndex(_FILE_NAME_FIELD_COLUMN_NAME))
				Dim fileName As String =  If(fileNameObject, String.Empty).ToString()
				If IsFileNameValid(fileName)
					Return filename
				Else
					_warningWriter.Invoke("File Name field contains illegal characters or is empty. Using old original file name: " & nativeRow(_FILE_NAME_FILE_TABLE_COLUMN_NAME).ToString())
				End If
			End If

			Return nativeRow(_FILE_NAME_FILE_TABLE_COLUMN_NAME).ToString()
		End Function
		
		Public Shared Function ExtendFieldRequestByFileNameIfNecessary(exportableFields As ViewFieldInfo(), requestedFields As IList(Of Integer)) As Boolean
			If requestedFields Is Nothing OrElse exportableFields Is Nothing
				Return False
			End If
			Dim fileName As ViewFieldInfo = exportableFields.SingleOrDefault(AddressOf IsFileNameField)
			If fileName IsNot Nothing
				If Not requestedFields.Contains(fileName.AvfId)
					requestedFields.Add(fileName.AvfId)
				End If
				Return True
			End If

			Return False
		End Function

		Private Shared Function IsFileNameField(field As ViewFieldInfo) As Boolean
			Return field.AvfColumnName.Equals(_FILE_NAME_FIELD_COLUMN_NAME) AndAlso field.FieldType.Equals(FieldTypeHelper.FieldType.Varchar)
		End Function

		Private Shared Function IsFileNameValid(ByVal fileName as String) as Boolean
			Dim isCorrect As Boolean = fileName <> ""
			Return isCorrect 
		End Function
	End Class
End NameSpace