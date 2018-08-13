Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows.Forms
Imports Relativity

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
			_listBox.DataSource = Filter(_dataSource, _textBox.Text)
			ResizeScrollBar()
			Cursor = Cursors.Default
		End Sub

		Private Function Filter(dataSource As List(Of Object), filterText As String) As List(Of Object)
			If IsNothing(filterText) Then
				Return dataSource
			End If
			Dim filterTextLower As String = filterText.ToLower
			Dim items As IEnumerable(Of Object) = From c In dataSource Where c.ToString().ToLower().Contains(filterTextLower) Select c
			Return items.ToList()
		End Function
		Public Sub Initialize(list As ArrayList)
			_dataSource = list.Cast(Of Object)().ToList()
			_listBox.DataSource = _dataSource
			ResizeScrollBar()
		End Sub

		Private Sub RedrawListBox()
			_listBox.Refresh()
		End Sub
		Private Sub _listBox_Scrolled(sender As Object, e As ScrollEventArgs) Handles _listBox.Scrolled
			RedrawListBox()
		End Sub

		Private Sub _listBox_KeyDown(sender As Object, e As KeyEventArgs) Handles _listBox.KeyDown
			If e.KeyCode = Keys.Left OrElse e.KeyCode = Keys.Right Then
				RedrawListBox()
			End If
		End Sub
	End Class
End Namespace