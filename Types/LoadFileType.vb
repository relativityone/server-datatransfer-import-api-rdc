Namespace kCura.WinEDDS
	Public Class LoadFileType
		Public Enum FileFormat
			IPRO
			Concordance
		End Enum

		Public Shared Function GetLoadFileTypes() As System.Data.DataTable
			Dim dt As New System.Data.DataTable
			dt.Columns.Add("DisplayName")
			dt.Columns.Add("Value", GetType(Int32))
			dt.Rows.Add(New Object() {"Select...", 0})
			dt.Rows.Add(New Object() {FileFormat.Concordance, 1})
			dt.Rows.Add(New Object() {FileFormat.IPRO, 2})
			Return dt
		End Function
	End Class
End Namespace