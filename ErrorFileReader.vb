Namespace kCura.Windows.Process
	Public Class ErrorFileReader
		Inherits kCura.Utility.DelimitedFileImporter

		Public Overrides Function ReadFile(ByVal path As String) As Object
			Me.Reader = New System.IO.StreamReader(path)
			Dim retval As New System.Data.DataTable
			retval.Columns.Add("Key")
			retval.Columns.Add("Status")
			retval.Columns.Add("Description")
			retval.Columns.Add("Timestamp")
			Dim tmp As String()
			Dim i As Int32 = 0
			While Not Me.HasReachedEOF AndAlso i < 1000
				retval.Rows.Add(Me.GetLine)
				i += 1
			End While
			Me.Close()
			Dim os As Object() = {retval, i >= 1000}
			Return os
		End Function

		Public Sub New(ByVal doRetryLogic As Boolean)
			MyBase.New(","c, """"c, ChrW(20), doRetryLogic)
		End Sub
	End Class
End Namespace
