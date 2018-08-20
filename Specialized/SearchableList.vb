Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows.Forms

Namespace Specialized
	Public Class SearchableList
		Private _timer As Timer
		Private _dataSource As New List(Of Object)
		Private Const _DELAYED_TEXT_CHANGED_TIMEOUT_IN_MILLISECONDS As Integer = 600

		Private Function GetMaxTextLength() As Integer
			Dim maxLength As Integer = 0
			For Each element As Object In _dataSource
				Dim currentLength As Integer = TextRenderer.MeasureText(element.ToString(), _listBox.Font).Width
				If currentLength > maxLength Then
					maxLength = currentLength
				End If
			Next
			Return maxLength
		End Function

		Private Sub ResizeScrollBar()
			_listBox.HorizontalExtent = GetMaxTextLength()
		End Sub

		Private Sub selectionSearchInput_TextChanged(sender As Object, e As EventArgs) _
			Handles _textBox.TextChanged
			If IsNothing(_timer) Then
				_timer = New Timer()
				AddHandler _timer.Tick, AddressOf Timer_Tick
				_timer.Interval = _DELAYED_TEXT_CHANGED_TIMEOUT_IN_MILLISECONDS
			Else
				_timer.Stop()

			End If

			_timer.Start()
		End Sub

		Private Sub Timer_Tick(sender As Object, e As EventArgs)
			If Not IsNothing(_timer) Then
				_timer.Stop()
			End If
			Cursor = Cursors.WaitCursor
			FilterAndAssignDataSource()
			ResizeScrollBar()
			Cursor = Cursors.Default
		End Sub

		Private Function Filter(dataSource As List(Of Object), filterText As String) As List(Of Object)
			If IsNothing(filterText) Then
				Return dataSource
			End If
			Dim filterTextLower As String = filterText.ToLower
			Dim items As IEnumerable(Of Object) = From c In dataSource Where c.ToString().ToLower().Contains(filterTextLower) Select c
			Dim listOfItems As List(Of Object) = items.ToList()
			listOfItems.Sort()
			Return listOfItems
		End Function

		Public Sub ForceRefresh()
			FilterAndAssignDataSource()
		End Sub

		Private Sub FilterAndAssignDataSource()
			_listBox.DataSource = Filter(_dataSource, _textBox.Text)
		End Sub

		Public Sub RemoveSelection()
			_listBox.SelectedIndex = -1
		End Sub

		Public Sub ClearListBox()
			_dataSource = New List(Of Object)
			ForceRefresh()
		End Sub

		Public Sub AddField(fieldInfo As Object)
			_dataSource.Add(fieldInfo)
			ForceRefresh()
		End Sub

		Public Sub AddFields(fields As Object())
			_dataSource.AddRange(fields)
			ForceRefresh()
		End Sub

		Public Sub RemoveField(field As Object)
			_dataSource.Remove(field)
		End Sub

#Region "Properties"

		Public ReadOnly Property Listbox() As kCura.Windows.Forms.ListBox
			Get
				Return _listBox
			End Get
		End Property

		Public Property DataSource As List(Of Object)
			Get
				Return _dataSource
			End Get
			Set
				_dataSource = Value
			End Set
		End Property

		Public ReadOnly Property CurrentItems() As List(Of Object)
			Get
				Return _listBox.Items.Cast(Of Object).ToList()
			End Get
		End Property

#End Region

	End Class
End Namespace