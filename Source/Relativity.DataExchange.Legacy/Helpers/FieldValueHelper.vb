Imports System.Collections.Generic
Imports System.IO
Imports System.Xml
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Service

Namespace kCura.WinEDDS.Helpers

Public Module FieldValueHelper

	Public Function ConvertToString(val As Object, field As ViewFieldInfo, multiRecordDelimiter As Char) As String
		
		If TypeOf val Is Byte() Then val = System.Text.Encoding.Unicode.GetString(DirectCast(val, Byte()))
		If field.FieldType = FieldType.Date AndAlso field.Category <> FieldCategory.MultiReflected Then
			If val Is System.DBNull.Value Then
				val = String.Empty
			ElseIf TypeOf val Is System.DateTime Then
				val = DirectCast(val, System.DateTime).ToString(field.FormatString)
			End If
		End If
		Dim fieldValue As String = NullableTypesHelper.ToEmptyStringOrValue(NullableTypesHelper.DBNullString(val))
		If field.IsMultiValueField Then
			fieldValue = GetMultivalueString(fieldValue, field.FieldType, field.FormatString, multiRecordDelimiter)
		ElseIf field.IsCodeOrMulticodeField Then
			fieldValue = GetCodeValueString(fieldValue, multiRecordDelimiter)
		End If
		return fieldValue
				
	End Function

    Public Function GetMultiValueString(ByVal input As String, ByVal fieldType As FieldType, ByVal fieldFormatString As String, multiRecordDelimiter As Char) As String
        Dim retVal As String = input
		Dim xml As String = "<objects>" & input & "</objects>"
		Dim values As New List(Of String)

			Using stringReader as StringReader =  New StringReader(xml)
			Using xmlTextReader as XmlTextReader =  New XmlTextReader(stringReader)

				While (xmlTextReader.Read())
				    If xmlTextReader.IsEmptyElement Or xmlTextReader.NodeType = XmlNodeType.Text

				        Dim value As String = xmlTextReader.Value.Trim
				        Select Case fieldType
				            Case FieldType.Code, FieldType.MultiCode
				                value = GetCodeValueString(value, multiRecordDelimiter)
				            Case FieldType.Date
				                value = ToExportableDateString(value, fieldFormatString)
				        End Select
						values.Add(value)
				    End If
				End While
			End Using
			End Using

        retVal = String.Join(multiRecordDelimiter, values)
        Return retVal

    End Function

	Public Function GetCodeValueString(ByVal input As String, multiRecordDelimiter As Char) As String
			input = System.Web.HttpUtility.HtmlDecode(input)
			input = input.Trim(New Char() {ChrW(11)}).Replace(ChrW(11), multiRecordDelimiter)
			Return input
	End Function

	Public Function ToExportableDateString(ByVal val As Object, ByVal formatString As String) As String
			Dim datetime As String = NullableTypesHelper.DBNullString(val)
			Dim retval As String
			If datetime Is Nothing OrElse datetime.Trim = "" Then
				retval = ""
			Else
				retval = System.DateTime.Parse(datetime, System.Globalization.CultureInfo.InvariantCulture).ToString(formatString)
			End If
			Return retval
	End Function

End Module
End Namespace