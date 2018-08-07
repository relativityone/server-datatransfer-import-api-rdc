Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Windows.Forms


Namespace Specialized
    Public Class SearchListSelector
        Inherits Form
        ReadOnly _masterDt As DataTable
        Private _currentSelection As Object
        Private _timer As Timer
        Private Const DelayedTextChangedTimeout As Integer = 600

        Protected Sub New()
            InitializeComponent()
        End Sub

        Public Sub New(dataTable As DataTable, formName As String)
            ' This call is required by the designer.
            InitializeComponent()
            ' Add any initialization after the InitializeComponent() call.
            Text = formName
            _masterDt = dataTable
            SetupListBox()
        End Sub

        Private Sub SetupListBox()
            selectionListBox.DataSource = _masterDt
            selectionListBox.DisplayMember = "Name"
            selectionListBox.ValueMember = "ArtifactID"
            _currentSelection = selectionListBox.SelectedValue
        End Sub

        Private Sub selectionSearchInput_TextChanged(sender As Object, e As EventArgs) _
            Handles selectionSearchInput.TextChanged
            If Not IsNothing(_timer) Then
                _timer.Stop()
            End If
            If IsNothing(_timer) Then
                _timer = New Timer()
                AddHandler _timer.Tick, AddressOf _timer_Tick
                _timer.Interval = DelayedTextChangedTimeout
            End If

            _timer.Start()
        End Sub

        Private Sub _timer_Tick(sender As Object, e As EventArgs)
            If Not IsNothing(_timer) Then
                _timer.Stop()
            End If
            Cursor = Cursors.WaitCursor
            selectionListBox.DataSource = FilterRowsFromDataTable(_masterDt, selectionSearchInput.Text)
            Cursor = Cursors.Default
        End Sub

        Protected Function FilterRowsFromDataTable(sourceDt As DataTable, substring As String) As DataTable
            If Not String.IsNullOrEmpty(substring) Then
                Dim currentDt As DataTable
                currentDt = sourceDt.Clone()
                substring = substring.ToLower()
                Dim items As IEnumerable(Of DataRow) =
                        sourceDt.Select.Where(
                            Function(x As DataRow) x.Item("Name").ToString().ToLower().Contains(substring))
                For Each dataRow As DataRow In items
                    currentDt.ImportRow(dataRow)
                Next
                Return currentDt
            End If
            Return sourceDt
        End Function

        Private Sub selectionListBox_DoubleClick(sender As Object, e As EventArgs) _
            Handles selectionListBox.MouseDoubleClick
            If selectionListBox.SelectedIndex > -1 Then
                DialogResult = DialogResult.OK
                Close()
            End If
        End Sub

        Private Sub selectionListBox_SelectedIndexChanged(sender As Object, e As EventArgs) _
            Handles selectionListBox.SelectedIndexChanged
            _currentSelection = selectionListBox.SelectedValue
        End Sub

        Public Function GetSelectedVal() As Object
            Return _currentSelection
        End Function
    End Class
End Namespace