Imports System.Collections
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq
Imports System.Windows.Controls
Imports System.Windows.Forms
Imports System.Windows.Media
Imports kCura.Windows.Forms

Namespace kCura.Windows.Forms.Specialized
	Public Class SearchableList
		Private _timer As Timer
		Private _dataSource As New List(Of Object)
		Private _isTextBoxPlaceholderUsed As Boolean = True
		Private _wasUsedPlaceholder As Boolean
		Private _previousText As String
		Private _lastSelectedItemIndex As Integer
		Private Const _DELAYED_TEXT_CHANGED_TIMEOUT_IN_MILLISECONDS As Integer = 600

		Public Event DoubleClickEvent(sender As Object, e As EventArgs)
		Public Event KeyPressEvent(sender As Object, e As KeyPressEventArgs)
		Public Event KeyUpEvent(sender As Object, e As KeyEventArgs)
		Public Event MeasureItemEvent(sender As Object, e As MeasureItemEventArgs)
		Public Event TextChangedEvent(sender As Object, isPlaceHOlderUsed As Boolean)
		Public Event MouseMoveEvent(sender As Object, e As MouseEventArgs)
		Public Event MouseLeaveEvent(sender As Object, e As EventArgs)
		Public Event RaiseItemsShifted As Action(Of SearchableList)

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
			If _previousText = "" AndAlso _isTextBoxPlaceholderUsed Then Return
			If Not _textBox.Text.Equals(_previousText) Then
				_previousText = _textBox.Text
				Cursor = Cursors.WaitCursor
				FilterAndAssignDataSource()
				ResizeScrollBar()
				Cursor = Cursors.Default

				Dim isTextBoxEmptyOrPlaceholder As Boolean =  _textBox.Text = "" OrElse _isTextBoxPlaceholderUsed
				RaiseEvent TextChangedEvent(Me,isTextBoxEmptyOrPlaceholder )
			End If
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
			_lastSelectedItemIndex = Nothing
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
				Return Nothing
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

		Public Sub MoveSelectedItemAndRestoreSelection(ByVal direction As MoveDirection)
			Dim selectedItem As Object = Listbox.SelectedItem
			MoveSelectedItem(direction)
			SetSelection(selectedItem)
		End Sub

		Public Sub MoveSelectedItem(ByVal direction As MoveDirection)
			If DataSource.Count > 1 Then
				If Listbox.SelectedItems.Count = 1 Then
					Dim bound As Int32
					Dim indexModifier As Int32
					Select Case direction
						Case MoveDirection.Down
							bound = DataSource.Count - 1
							indexModifier = 1
						Case MoveDirection.Up
							bound = 0
							indexModifier = -1
					End Select
					If Not Listbox.SelectedIndex = bound Then
						Dim i As Int32 = DataSource.IndexOf(Listbox.SelectedItem)
						Dim selectedItem As Object = Listbox.SelectedItem
						DataSource.RemoveAt(i)
						DataSource.Insert(i + indexModifier, selectedItem)
						Listbox.SelectedIndex = i + indexModifier
					End If
				End If
			End If
			ForceRefresh()
		End Sub

		Public Sub MoveAllItems(ByVal receiver As SearchableList)
			receiver.DataSource.AddRange(CurrentItems)
			For Each currentItem As Object In CurrentItems
				DataSource.Remove(currentItem)
			Next
			RaiseEvent RaiseItemsShifted(Me)
		End Sub


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
		Private Sub SelectAllItemsInRange(first As Integer, last As Integer)
			For i As Integer = Math.Min(first, last) To Math.Max(first, last)
				_listBox.SetSelected(i, True)
			Next
		End Sub
		Private Sub _listBox_MouseUp(sender As Object, e As MouseEventArgs) Handles _listBox.MouseUp
			If ClientRectangle.Contains(e.Location) Then
				Dim index As Integer = _listBox.IndexFromPoint(e.Location)
				If index >= 0 Then
					If My.Computer.Keyboard.ShiftKeyDown Then
						If Not IsNothing(_lastSelectedItemIndex)
							SelectAllItemsInRange(_lastSelectedItemIndex, index)
						Else
							SelectAllItemsInRange(0, index)
						End If
					ElseIf My.Computer.Keyboard.CtrlKeyDown
						_lastSelectedItemIndex = index
					Else
						_listbox.ClearSelected()
						_listbox.SetSelected(index, True)
						_lastSelectedItemIndex = index
					End If
				End If
			End If
		End Sub
	End Class
End Namespace