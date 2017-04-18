
Namespace kCura.WinEDDS.Helpers


Public Class FieldValueHelper

	public Shared  Function ConvertToString(val As Object, field As ViewFieldInfo, multiRecordDelimiter As Char) As String
		
		If TypeOf val Is Byte() Then val = System.Text.Encoding.Unicode.GetString(DirectCast(val, Byte()))
		If field.FieldType = Relativity.FieldTypeHelper.FieldType.Date AndAlso field.Category <> Relativity.FieldCategory.MultiReflected Then
			If val Is System.DBNull.Value Then
				val = String.Empty
			ElseIf TypeOf val Is System.DateTime Then
				val = DirectCast(val, System.DateTime).ToString(field.FormatString)
			End If
		End If
		Dim fieldValue As String = kCura.Utility.NullableTypesHelper.ToEmptyStringOrValue(kCura.Utility.NullableTypesHelper.DBNullString(val))
		If field.IsMultiValueField Then
			fieldValue = GetMultivalueString(fieldValue, field, multiRecordDelimiter)
		ElseIf field.IsCodeOrMulticodeField Then
			fieldValue = GetCodeValueString(fieldValue, multiRecordDelimiter)
		End If
		return fieldValue
				
	End Function

	Public Shared Function GetMultivalueString(ByVal input As String, ByVal field As ViewFieldInfo, multiRecordDelimiter As Char) As String
			Dim retVal As String = input
			If input.Contains("<object>") Then
				Dim xr As New System.Xml.XmlTextReader(New System.IO.StringReader("<objects>" & input & "</objects>"))
				Dim firstTimeThrough As Boolean = True
				Dim sb As New System.Text.StringBuilder
				While xr.Read
					If xr.Name = "object" And xr.IsStartElement Then
						xr.Read()
						If firstTimeThrough Then
							firstTimeThrough = False
						Else
							sb.Append(multiRecordDelimiter)
						End If
						Dim cleanval As String = xr.Value.Trim
						Select Case field.FieldType
							Case Relativity.FieldTypeHelper.FieldType.Code, Relativity.FieldTypeHelper.FieldType.MultiCode
								cleanval = GetCodeValueString(cleanval, multiRecordDelimiter)
							Case Relativity.FieldTypeHelper.FieldType.Date
								cleanval = ToExportableDateString(cleanval, field.FormatString)
						End Select
						'If isCodeOrMulticodeField Then cleanval = Me.GetCodeValueString(cleanval)
						sb.Append(cleanval)
					End If
				End While
				xr.Close()
				retVal = sb.ToString
			End If
			Return retVal

	End Function

	Public Shared Function GetCodeValueString(ByVal input As String, multiRecordDelimiter As Char) As String
			input = System.Web.HttpUtility.HtmlDecode(input)
			input = input.Trim(New Char() {ChrW(11)}).Replace(ChrW(11), multiRecordDelimiter)
			Return input
	End Function

	Public Shared Function ToExportableDateString(ByVal val As Object, ByVal formatString As String) As String
			Dim datetime As String = kCura.Utility.NullableTypesHelper.DBNullString(val)
			Dim retval As String
			If datetime Is Nothing OrElse datetime.Trim = "" Then
				retval = ""
			Else
				retval = System.DateTime.Parse(datetime, System.Globalization.CultureInfo.InvariantCulture).ToString(formatString)
			End If
			Return retval
	End Function

End Class

End Namespace