Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Linq
Imports System.Windows.Forms

Namespace Specialized


    Public Class SearchListSelector
        Inherits System.Windows.Forms.Form
        ReadOnly _masterDt As DataTable
        Private _currentSelection As Object
        ReadOnly _ci As CultureInfo = New CultureInfo(CultureInfo.CurrentCulture.ToString())
        Private Sub SetupListBox()
            selectionListBox.DataSource = _masterDt
            selectionListBox.DisplayMember = "Name"
            selectionListBox.ValueMember = "ArtifactID"
            _currentSelection = selectionListBox.SelectedValue
        End Sub
        Public Sub New(dataTable As DataTable, formName As String)

            ' This call is required by the designer.
            InitializeComponent()
            ' Add any initialization after the InitializeComponent() call.
            _masterDt = dataTable
            Me.Name = formName
            SetupListBox()
        End Sub

        Private Function FilterRowsFromDataTable(sourceDt As DataTable, substring As String) As DataTable
            If Not String.IsNullOrEmpty(substring) Then
                Static currentDt As New DataTable
                currentDt.Clear()
                currentDt = sourceDt.Clone()
                Dim items As IEnumerable(Of DataRow) = sourceDt.Select.Where(Function(x As DataRow) x.Item("Name").ToString().StartsWith(substring, True, _ci))
                For Each dataRow As DataRow In items
                    currentDt.ImportRow(dataRow)
                Next
                Return currentDt
            End If
            Return sourceDt
        End Function
        Private Sub selectionSearchInput_TextChanged(sender As Object, e As EventArgs) Handles selectionSearchInput.TextChanged
            selectionListBox.DataSource = FilterRowsFromDataTable(_masterDt, selectionSearchInput.Text)
        End Sub
        Private Sub selectionListBox_DoubleClick(sender As Object, e As EventArgs) Handles selectionListBox.MouseDoubleClick
            If selectionListBox.SelectedIndex > -1 Then
                DialogResult = DialogResult.OK
                Close()
            End If
        End Sub
        Private Sub selectionListBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles selectionListBox.SelectedIndexChanged
            _currentSelection = selectionListBox.SelectedValue
        End Sub

        Public Function GetSelectedVal() As Object
            Return _currentSelection
        End Function
    End Class
End Namespace