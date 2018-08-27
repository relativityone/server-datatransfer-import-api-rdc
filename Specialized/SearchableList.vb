Imports System.Collections
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq
Imports System.Windows.Controls
Imports System.Windows.Forms
Imports System.Windows.Media

Namespace Specialized
	Public Class SearchableList
		Private _timer As Timer
		Private _dataSource As New List(Of Object)
		Private _isTextBoxPlaceholderUsed As Boolean = True
		Private _isUserTyping As Boolean = False
		Private _wasUsedPlaceholder As Boolean
		Private _lastSelectedItem As Object
		Private Const _DELAYED_TEXT_CHANGED_TIMEOUT_IN_MILLISECONDS As Integer = 600

		Public Event DoubleClickEvent(sender As Object, e As EventArgs)
		Public Event KeyPressEvent(sender As Object, e As KeyPressEventArgs)
		Public Event KeyUpEvent(sender As Object, e As KeyEventArgs)
		Public Event MeasureItemEvent(sender As Object, e As MeasureItemEventArgs)
		Public Event TextChangedEvent(sender As Object)
		Public Event MouseMoveEvent(sender As Object, e As MouseEventArgs)
		Public Event MouseLeaveEvent(sender As Object, e As EventArgs)

		Protected Overrides Sub OnCreateControl()
			MyBase.OnCreateControl()
			SetTextBoxPlaceholderText()
		End Sub

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
			If _wasUsedPlaceholder
				_wasUsedPlaceholder = False
				Return
			End If
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
			If _isTextBoxPlaceholderUsed AndAlso Not _isUserTyping Then Return
			Cursor = Cursors.WaitCursor
			FilterAndAssignDataSource()
			ResizeScrollBar()
			Cursor = Cursors.Default
			RaiseEvent TextChangedEvent(Me)
		End Sub

		Private Function Filter(sourceList As List(Of Object), filterText As String) As List(Of Object)
			If IsNothing(filterText) Then
				Return sourceList
			End If
			Dim filterTextLower As String = filterText.ToLower
			Dim items As IEnumerable(Of Object) = From c In sourceList Where c.ToString().ToLower().Contains(filterTextLower) Select c
			Dim listOfItems As List(Of Object) = items.ToList()
			Return listOfItems
		End Function

		Public Sub ForceRefresh()
			Dim visibleItem As Object = GetFirstVisibleItem()
			Dim item As Object = GetFirstNotSelectedItemAfterGiven(visibleItem)
			FilterAndAssignDataSource()
			If item IsNot Nothing
				_listBox.TopIndex = _listBox.Items.IndexOf(item)
			End If
			RemoveSelection()
		End Sub

		Private Sub FilterAndAssignDataSource()
			Dim listOfFields As List(Of Object) = Filter(_dataSource, If(_isTextBoxPlaceholderUsed, "", _textBox.Text))
			If (IsListSortable) Then
				listOfFields.Sort()
			End If
			_listBox.DataSource = listOfFields
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
			ForceRefresh()
		End Sub

		Public Sub SetSelection(item As Object)
			If Not IsNothing(item)
				_listBox.SelectedItem = item
			End If
		End Sub

		Private Function GetFirstVisibleItem() As Object

			If _listbox.Items.Count > 0
				Dim index As Integer = _listBox.TopIndex
				If index >=0
					Return _listbox.Items(index)
				End If
				Return 0
			End If
			Return Nothing
		End Function

		Private Function GetFirstNotSelectedItemAfterGiven(itemToDoSearchAfter As Object) As Object
			If itemToDoSearchAfter Is Nothing
				Return Nothing
			End If
			For i As Integer = _listbox.Items.IndexOf(itemToDoSearchAfter) To _listbox.Items.Count -1
				If i < 0
					Continue For
				End If
				If Not _listBox.GetSelected(i)
					Return _listbox.Items(i)
				End If
			Next
			Return Nothing
		End Function
#Region "Properties"
		Public Property IsListSortable As Boolean = True
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
		Private Sub _listBox_DoubleClick(sender As Object, e As EventArgs) Handles _listBox.DoubleClick
			RaiseEvent DoubleClickEvent(sender, e)
		End Sub

		Private Sub _listBox_Scrolled(ByVal sender As Object, ByVal e As ScrollEventArgs) Handles _listBox.Scrolled
			If e.ScrollOrientation = ScrollOrientation.HorizontalScroll Then
				Listbox.Invalidate()
			End If
		End Sub

		Private Sub _listBox_KeyPress(sender As Object, e As KeyPressEventArgs) Handles _listBox.KeyPress
			RaiseEvent KeyPressEvent(sender, e)
		End Sub

		Private Sub _listBox_KeyUp(sender As Object, e As KeyEventArgs) Handles _listBox.KeyUp
			RaiseEvent KeyUpEvent(sender, e)
		End Sub

		Private Sub _listBox_MeasureItem(sender As Object, e As MeasureItemEventArgs) Handles _listBox.MeasureItem
			RaiseEvent MeasureItemEvent(sender, e)
		End Sub

		Private Sub _textBox_Enter(sender As Object, e As EventArgs) Handles _textBox.Enter
			_isUserTyping = True
			If _isTextBoxPlaceholderUsed Then
				_wasUsedPlaceholder = True
				_isTextBoxPlaceholderUsed = False
				_textBox.Text = ""
				_textBox.ForeColor = System.Drawing.Color.Black
				_textBox.Font = New Font(_textBox.Font, FontStyle.Regular)
			End If
		End Sub

		Private Sub SetTextBoxPlaceholderText()
			_isTextBoxPlaceholderUsed = True
			_textBox.Text = "Filter"
			_textBox.ForeColor = System.Drawing.Color.Gray
			_textBox.Font = New Font(_textBox.Font, FontStyle.Italic)

		End Sub

		Private Sub _textBox_Leave(sender As Object, e As EventArgs) Handles _textBox.Leave
			_isUserTyping = False
			If _textBox.Text = "" Then
				SetTextBoxPlaceholderText()
			Else
				_isTextBoxPlaceholderUsed = False
			End If
		End Sub

		Private Sub _listBox_MouseMoveEvent(sender As Object, e As MouseEventArgs) Handles _listBox.MouseMove
			RaiseEvent MouseMoveEvent(sender, e)
		End Sub

		Private Sub _listBox_MouseLeaveEvent(sender As Object, e As EventArgs) Handles _listBox.MouseLeave
			RaiseEvent MouseLeaveEvent(sender, e)
		End Sub

		Private Sub _listBox_MouseUp(sender As Object, e As MouseEventArgs) Handles _listBox.MouseUp
			If Not My.Computer.Keyboard.CtrlKeyDown AndAlso Not My.Computer.Keyboard.ShiftKeyDown Then
				_listbox.ClearSelected()
				_listbox.SetSelected(_listBox.IndexFromPoint(e.Location), True)
				_lastSelectedItem = _listbox.Items(_listBox.IndexFromPoint(e.Location))
			ElseIf My.Computer.Keyboard.ShiftKeyDown Then

			End If
		End Sub


#Region "Selection methods"





#End Region
	End Class
End Namespace