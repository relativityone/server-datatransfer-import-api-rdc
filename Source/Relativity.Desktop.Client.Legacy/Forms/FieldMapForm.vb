Imports System.Linq
Imports kCura.Utility.Extensions
Public Class FieldMapForm

	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles _exportButton.Click
		ExportToFileDialog.ShowDialog()
	End Sub

	Private Sub FieldMapForm_ClientSizeChanged(sender As Object, e As EventArgs) Handles Me.ClientSizeChanged
		FieldMapGrid.AutoResizeColumns()
	End Sub

	Private Sub FieldMapForm_Load(sender As Object, e As EventArgs) Handles Me.Load
		FieldMapGrid.AutoResizeColumns()
	End Sub

	Private Sub CloseButton_Click(sender As Object, e As EventArgs) Handles CloseButton.Click
		Me.Close()
	End Sub

	Private Sub ExportToFileDialog_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ExportToFileDialog.FileOk
		Using sw As New System.IO.StreamWriter(ExportToFileDialog.FileName, False, System.Text.Encoding.UTF8)
			Dim dataSource As DataTable = TryCast(FieldMapGrid.DataSource, System.Data.DataTable)
			DataTableToCsv(dataSource, sw)
		End Using
	End Sub

	Private Shared Sub DataTableToCsv(input As DataTable, destination As System.IO.TextWriter)
		Dim columns As DataColumn() = input.Columns.Cast(Of DataColumn)().ToArray()
		destination.WriteLine(columns.Select(Function(c) c.ColumnName).ToCsv(Function(name) ToCsvContents(name)))
		For Each row As DataRow In input.Rows
			destination.WriteLine(columns.Select(Function(c) row(c.Ordinal).ToString()).ToCsv(Function(cellContents) ToCsvContents(cellContents)))
		Next
	End Sub

	Private Shared Function ToCsvContents(input As String) As String
		Return String.Format("""{0}""", input.Replace("""", """"""))
	End Function
End Class