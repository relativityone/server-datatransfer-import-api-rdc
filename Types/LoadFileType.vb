Namespace kCura.WinEDDS
	Public Class LoadFileType
		Public Enum FileFormat
			Concordance
			IPRO
			IPRO_FullText
		End Enum

		Public Shared Function GetLoadFileTypes() As System.Data.DataTable
			Dim dt As New System.Data.DataTable
			dt.Columns.Add("DisplayName")
			dt.Columns.Add("Value", GetType(Int32))
			dt.Rows.Add(New Object() {"Select...", -1})
			dt.Rows.Add(New Object() {"Concordance", 0})
			dt.Rows.Add(New Object() {"IPRO", 1})
			dt.Rows.Add(New Object() {"IPRO (FullText)", 2})
			Return dt
		End Function
	End Class
End Namespace