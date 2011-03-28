Imports System.Windows.Forms
Imports System.Drawing

' TODO : Change namespace of this control
Namespace kCura.Windows.Forms

	Public Class TwoListBox
		Inherits System.Windows.Forms.UserControl

		Public Event ClearHighlightedItems(ByVal sender As Object, ByVal e As HighlightItemEventArgs)
		Public Event HighlightItemByLocationAndIndex(ByVal sender As Object, ByVal e As HighlightItemEventArgs)
		Public Event ItemsShifted(ByVal sender As Object, ByVal e As EventArgs)

#Region " Windows Form Designer generated code "

		Public Sub New()
			MyBase.New()

			'This call is required by the Windows Form Designer.
			InitializeComponent()

			'Add any initialization after the InitializeComponent() call

		End Sub

		'UserControl overrides dispose to clean up the component list.
		Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
				If Not (components Is Nothing) Then
					components.Dispose()
				End If
			End If
			MyBase.Dispose(disposing)
		End Sub

		'Required by the Windows Form Designer
		Private components As System.ComponentModel.IContainer

		'NOTE: The following procedure is required by the Windows Form Designer
		'It can be modified using the Windows Form Designer.  
		'Do not modify it using the code editor.
		Friend WithEvents _rightListBox As kCura.Windows.Forms.ListBox
		Friend WithEvents _leftListBox As kCura.Windows.Forms.ListBox
		Friend WithEvents _moveAllFieldsLeft As System.Windows.Forms.Button
		Friend WithEvents _moveFieldLeft As System.Windows.Forms.Button
		Friend WithEvents _moveFieldRight As System.Windows.Forms.Button
		Friend WithEvents _moveAllFieldsRight As System.Windows.Forms.Button
		Friend WithEvents _moveRightSelectedItemDown As System.Windows.Forms.Button
		Friend WithEvents _moveRightSelectedItemUp As System.Windows.Forms.Button
		Friend WithEvents _moveLeftSelectedItemDown As System.Windows.Forms.Button
		Friend WithEvents _moveLeftSelectedItemUp As System.Windows.Forms.Button
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Me._rightListBox = New kCura.Windows.Forms.ListBox(Me.RelativityHighlightColor)
			Me._moveAllFieldsLeft = New System.Windows.Forms.Button
			Me._moveFieldLeft = New System.Windows.Forms.Button
			Me._moveFieldRight = New System.Windows.Forms.Button
			Me._moveAllFieldsRight = New System.Windows.Forms.Button
			Me._leftListBox = New kCura.Windows.Forms.ListBox(Me.RelativityHighlightColor)
			Me._moveRightSelectedItemDown = New System.Windows.Forms.Button
			Me._moveRightSelectedItemUp = New System.Windows.Forms.Button
			Me._moveLeftSelectedItemDown = New System.Windows.Forms.Button
			Me._moveLeftSelectedItemUp = New System.Windows.Forms.Button
			Me.SuspendLayout()
			'
			'_rightListBox
			'
			Me._rightListBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			 Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
			Me._rightListBox.HorizontalScrollbar = True
			Me._rightListBox.Location = New System.Drawing.Point(212, 0)
			Me._rightListBox.Name = "_rightListBox"
			Me._rightListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
			Me._rightListBox.Size = New System.Drawing.Size(144, 277)
			Me._rightListBox.TabIndex = 16
			'
			'_moveAllFieldsLeft
			'
			Me._moveAllFieldsLeft.Font = New System.Drawing.Font("Wingdings", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
			Me._moveAllFieldsLeft.Location = New System.Drawing.Point(172, 172)
			Me._moveAllFieldsLeft.Name = "_moveAllFieldsLeft"
			Me._moveAllFieldsLeft.Size = New System.Drawing.Size(36, 24)
			Me._moveAllFieldsLeft.TabIndex = 13
			Me._moveAllFieldsLeft.Text = "çç"
			'
			'_moveFieldLeft
			'
			Me._moveFieldLeft.Font = New System.Drawing.Font("Wingdings", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
			Me._moveFieldLeft.Location = New System.Drawing.Point(172, 140)
			Me._moveFieldLeft.Name = "_moveFieldLeft"
			Me._moveFieldLeft.Size = New System.Drawing.Size(36, 24)
			Me._moveFieldLeft.TabIndex = 12
			Me._moveFieldLeft.Text = "ß"
			'
			'_moveFieldRight
			'
			Me._moveFieldRight.Font = New System.Drawing.Font("Wingdings", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
			Me._moveFieldRight.Location = New System.Drawing.Point(172, 108)
			Me._moveFieldRight.Name = "_moveFieldRight"
			Me._moveFieldRight.Size = New System.Drawing.Size(36, 24)
			Me._moveFieldRight.TabIndex = 11
			Me._moveFieldRight.Text = "à"
			'
			'_moveAllFieldsRight
			'
			Me._moveAllFieldsRight.Font = New System.Drawing.Font("Wingdings", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
			Me._moveAllFieldsRight.Location = New System.Drawing.Point(172, 76)
			Me._moveAllFieldsRight.Name = "_moveAllFieldsRight"
			Me._moveAllFieldsRight.Size = New System.Drawing.Size(36, 24)
			Me._moveAllFieldsRight.TabIndex = 10
			Me._moveAllFieldsRight.Text = "èè"
			'
			'_leftListBox
			'
			Me._leftListBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			 Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
			Me._leftListBox.HorizontalScrollbar = True
			Me._leftListBox.Location = New System.Drawing.Point(24, 0)
			Me._leftListBox.Name = "_leftListBox"
			Me._leftListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
			Me._leftListBox.Size = New System.Drawing.Size(144, 277)
			Me._leftListBox.TabIndex = 9
			'
			'_moveRightSelectedItemDown
			'
			Me._moveRightSelectedItemDown.Font = New System.Drawing.Font("Wingdings", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
			Me._moveRightSelectedItemDown.Location = New System.Drawing.Point(360, 140)
			Me._moveRightSelectedItemDown.Name = "_moveRightSelectedItemDown"
			Me._moveRightSelectedItemDown.RightToLeft = System.Windows.Forms.RightToLeft.No
			Me._moveRightSelectedItemDown.Size = New System.Drawing.Size(20, 24)
			Me._moveRightSelectedItemDown.TabIndex = 15
			Me._moveRightSelectedItemDown.Text = "â"
			'
			'_moveRightSelectedItemUp
			'
			Me._moveRightSelectedItemUp.Font = New System.Drawing.Font("Wingdings", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
			Me._moveRightSelectedItemUp.Location = New System.Drawing.Point(360, 108)
			Me._moveRightSelectedItemUp.Name = "_moveRightSelectedItemUp"
			Me._moveRightSelectedItemUp.Size = New System.Drawing.Size(20, 24)
			Me._moveRightSelectedItemUp.TabIndex = 14
			Me._moveRightSelectedItemUp.Text = "á"
			'
			'_moveLeftSelectedItemDown
			'
			Me._moveLeftSelectedItemDown.Font = New System.Drawing.Font("Wingdings", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
			Me._moveLeftSelectedItemDown.Location = New System.Drawing.Point(0, 140)
			Me._moveLeftSelectedItemDown.Name = "_moveLeftSelectedItemDown"
			Me._moveLeftSelectedItemDown.RightToLeft = System.Windows.Forms.RightToLeft.No
			Me._moveLeftSelectedItemDown.Size = New System.Drawing.Size(20, 24)
			Me._moveLeftSelectedItemDown.TabIndex = 18
			Me._moveLeftSelectedItemDown.Text = "â"
			'
			'_moveLeftSelectedItemUp
			'
			Me._moveLeftSelectedItemUp.Font = New System.Drawing.Font("Wingdings", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
			Me._moveLeftSelectedItemUp.Location = New System.Drawing.Point(0, 108)
			Me._moveLeftSelectedItemUp.Name = "_moveLeftSelectedItemUp"
			Me._moveLeftSelectedItemUp.Size = New System.Drawing.Size(20, 24)
			Me._moveLeftSelectedItemUp.TabIndex = 17
			Me._moveLeftSelectedItemUp.Text = "á"
			'
			'TwoListBox
			'
			Me.Controls.Add(Me._moveLeftSelectedItemDown)
			Me.Controls.Add(Me._moveLeftSelectedItemUp)
			Me.Controls.Add(Me._rightListBox)
			Me.Controls.Add(Me._moveRightSelectedItemDown)
			Me.Controls.Add(Me._moveRightSelectedItemUp)
			Me.Controls.Add(Me._moveAllFieldsLeft)
			Me.Controls.Add(Me._moveFieldLeft)
			Me.Controls.Add(Me._moveFieldRight)
			Me.Controls.Add(Me._moveAllFieldsRight)
			Me.Controls.Add(Me._leftListBox)
			Me.Name = "TwoListBox"
			Me.Size = New System.Drawing.Size(380, 280)
			Me.ResumeLayout(False)

		End Sub

#End Region

#Region " Properties "

		Public Property AlternateRowColors() As Boolean

		Public Property OuterBox() As ListBoxLocation

		Public Property LeftOrderControlsVisible() As Boolean
			Get
				Return _moveLeftSelectedItemDown.Visible
			End Get
			Set(ByVal value As Boolean)
				_moveLeftSelectedItemDown.Visible = value
				_moveLeftSelectedItemUp.Visible = value
				If value = False Then
					Dim pos As Int32 = -24
					AdjustXPosition(_leftListBox, pos)
					AdjustXPosition(_moveAllFieldsLeft, pos)
					AdjustXPosition(_moveFieldLeft, pos)
					AdjustXPosition(_moveFieldRight, pos)
					AdjustXPosition(_moveAllFieldsRight, pos)
					AdjustXPosition(_rightListBox, pos)
					AdjustXPosition(_moveRightSelectedItemDown, pos)
					AdjustXPosition(_moveRightSelectedItemUp, pos)
				Else
					Dim pos As Int32 = 24
					If _leftListBox.Location.X <> 24 Then
						AdjustXPosition(_leftListBox, pos)
						AdjustXPosition(_moveAllFieldsLeft, pos)
						AdjustXPosition(_moveFieldLeft, pos)
						AdjustXPosition(_moveFieldRight, pos)
						AdjustXPosition(_moveAllFieldsRight, pos)
						AdjustXPosition(_rightListBox, pos)
						AdjustXPosition(_moveRightSelectedItemUp, pos)
						AdjustXPosition(_moveRightSelectedItemDown, pos)
					End If
				End If
			End Set
		End Property

		Public Property RightOrderControlVisible() As Boolean
			Get
				Return _moveRightSelectedItemDown.Visible
			End Get
			Set(ByVal value As Boolean)
				_moveRightSelectedItemDown.Visible = value
				_moveRightSelectedItemUp.Visible = value
			End Set
		End Property

		Private _buttonsCentered As Boolean
		Public Property KeepButtonsCentered() As Boolean
			Get
				Return _buttonsCentered
			End Get
			Set(ByVal value As Boolean)
				_buttonsCentered = value
				If _buttonsCentered Then
					CenterButtons()
				End If
			End Set
		End Property

		Public ReadOnly Property LeftListBoxItems() As System.Windows.Forms.ListBox.ObjectCollection
			Get
				Return _leftListBox.Items
			End Get
		End Property

		Public ReadOnly Property RightListBoxItems() As System.Windows.Forms.ListBox.ObjectCollection
			Get
				Return _rightListBox.Items
			End Get
		End Property

		Public ReadOnly Property RelativityHighlightColor() As System.Drawing.Color
			Get
				Return System.Drawing.Color.FromArgb(229, 142, 26)
			End Get
		End Property

#End Region

#Region " ListBox methods and event handlers "

		Private Sub ShiftSelectedItems(ByVal giver As kCura.Windows.Forms.ListBox, ByVal receiver As kCura.Windows.Forms.ListBox)
			If giver.SelectedItems.Count > 0 Then
				For i As Int32 = 0 To giver.SelectedItems.Count - 1
					receiver.Items.Add(giver.SelectedItems.Item(i))
				Next

				While giver.SelectedItems.Count > 0
					giver.Items.Remove(giver.SelectedItems.Item(0))
				End While
			End If

			Me.RaiseItemsShifted()
		End Sub

		Private Sub RaiseItemsShifted()
			Me.EnsureHorizontalScrollbars()
			RaiseEvent ItemsShifted(Me, New EventArgs)
		End Sub

		Public Sub EnsureHorizontalScrollbars()
			Me.EnsureHorizontalScrollbarForBox(_leftListBox)
			Me.EnsureHorizontalScrollbarForBox(_rightListBox)
		End Sub

		Private Sub EnsureHorizontalScrollbarForBox(ByVal box As kCura.Windows.Forms.ListBox)
			If _AlternateRowColors Then
				Dim g As System.Drawing.Graphics = Me.CreateGraphics
				box.HorizontalExtent = 0
				For i As Int32 = 0 To box.Items.Count - 1
					box.HorizontalExtent = System.Math.Max(CInt(box.HorizontalExtent), CInt(g.MeasureString(box.Items(i).ToString, box.Font, 0).Width + 20))
				Next

				g.Dispose()
			End If

			If box.Items.Count >= 0 Then
				box.HorizontalScrollOffset = 0
			End If
		End Sub

		Public Sub CenterButtons()
			Dim center As Int32 = CType(Me.Size.Height / 2, Int32)
			SetYPosition(_moveFieldRight, center - 32)
			SetYPosition(_moveAllFieldsRight, center - 64)
			SetYPosition(_moveFieldLeft, center)
			SetYPosition(_moveAllFieldsLeft, center + 32)
			SetYPosition(_moveRightSelectedItemDown, center)
			SetYPosition(_moveRightSelectedItemUp, center - 24)
			SetYPosition(_moveLeftSelectedItemDown, center)
			SetYPosition(_moveLeftSelectedItemUp, center - 24)
		End Sub

		Public Sub AdjustXPosition(ByVal control As System.Windows.Forms.Control, ByVal movement As Int32)
			control.Location = New System.Drawing.Point(control.Location.X + movement, control.Location.Y)
		End Sub

		Public Sub SetYPosition(ByVal control As System.Windows.Forms.Control, ByVal yPosition As Int32)
			control.Location = New System.Drawing.Point(control.Location.X, yPosition)
		End Sub

		Public Sub ClearAll()
			_leftListBox.Items.Clear()
			_leftListBox.HorizontalScrollOffset = 0

			_rightListBox.Items.Clear()
			_rightListBox.HorizontalScrollOffset = 0
		End Sub

		Private Sub TwoListBox_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
			_leftListBox.AlternateColors = _AlternateRowColors
			_rightListBox.AlternateColors = _AlternateRowColors
		End Sub

		Private Sub _leftListBox_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles _leftListBox.KeyUp
			If e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down Then
				_leftListBox.Invalidate()
			ElseIf e.KeyCode = Keys.Left OrElse e.KeyCode = Keys.Right Then
			End If
		End Sub

		Private Sub _rightListBox_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles _rightListBox.KeyUp
			If e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down Then
				_rightListBox.Invalidate()
			ElseIf e.KeyCode = Keys.Left OrElse e.KeyCode = Keys.Right Then
			End If
		End Sub

		Private Sub _leftListBox_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles _leftListBox.DoubleClick
			ShiftSelectedItems(_leftListBox, _rightListBox)
			EnsureHorizontalScrollbars()
		End Sub

		Private Sub _rightListBox_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles _rightListBox.DoubleClick
			ShiftSelectedItems(_rightListBox, _leftListBox)
			EnsureHorizontalScrollbars()
		End Sub

		Private Sub _leftListBox_Click(ByVal sender As Object, ByVal e As EventArgs) Handles _leftListBox.Click
			_leftListBox.Invalidate()
		End Sub

		Private Sub _rightListBox_Click(ByVal sender As Object, ByVal e As EventArgs) Handles _rightListBox.Click
			_rightListBox.Invalidate()
		End Sub

		Private Sub _leftListBox_Scrolled(ByVal sender As Object, ByVal e As ScrollEventArgs) Handles _leftListBox.Scrolled
			If e.ScrollOrientation = ScrollOrientation.HorizontalScroll Then
				_leftListBox.Invalidate()
			End If
		End Sub

		Private Sub _rightListBox_Scrolled(ByVal sender As Object, ByVal e As ScrollEventArgs) Handles _rightListBox.Scrolled
			If e.ScrollOrientation = ScrollOrientation.HorizontalScroll Then
				_rightListBox.Invalidate()
			End If
		End Sub

#End Region

#Region " Move methods and event handlers "

		Private Enum MoveDirection
			Up
			Down
		End Enum

		Private Sub MoveAllItems(ByVal giver As kCura.Windows.Forms.ListBox, ByVal receiver As System.Windows.Forms.ListBox)
			receiver.Items.AddRange(giver.Items)
			giver.Items.Clear()
			Me.RaiseItemsShifted()
		End Sub

		Private Sub MoveSelectedItem(ByVal box As System.Windows.Forms.ListBox, ByVal direction As MoveDirection)
			If box.Items.Count > 1 Then
				If box.SelectedItems.Count = 1 Then
					Dim bound As Int32
					Dim indexModifier As Int32
					Select Case direction
						Case MoveDirection.Down
							bound = box.Items.Count - 1
							indexModifier = 1
						Case MoveDirection.Up
							bound = 0
							indexModifier = -1
					End Select
					If Not box.SelectedIndex = bound Then
						Dim i As Int32 = box.Items.IndexOf(box.SelectedItem)
						Dim selectedItem As Object = box.SelectedItem
						box.Items.RemoveAt(i)
						box.Items.Insert(i + indexModifier, selectedItem)
						box.SelectedIndex = i + indexModifier
					End If
				End If
			End If
			Me.RaiseItemsShifted()
		End Sub

		Private Sub _moveAllFieldsIn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _moveAllFieldsRight.Click
			MoveAllItems(_leftListBox, _rightListBox)
		End Sub

		Private Sub _moveAllFieldsOut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _moveAllFieldsLeft.Click
			MoveAllItems(_rightListBox, _leftListBox)
		End Sub

		Private Sub _moveFieldIn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _moveFieldRight.Click
			ShiftSelectedItems(_leftListBox, _rightListBox)
		End Sub

		Private Sub _moveFieldOut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _moveFieldLeft.Click
			ShiftSelectedItems(_rightListBox, _leftListBox)
		End Sub

		Private Sub _moveLeftSelectedItemUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _moveLeftSelectedItemUp.Click
			MoveSelectedItem(_leftListBox, MoveDirection.Up)
		End Sub

		Private Sub _moveLeftSelectedItemDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _moveLeftSelectedItemDown.Click
			MoveSelectedItem(_leftListBox, MoveDirection.Down)
		End Sub

		Private Sub _moveRightSelectedItemUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _moveRightSelectedItemUp.Click
			MoveSelectedItem(_rightListBox, MoveDirection.Up)
		End Sub

		Private Sub _moveRightSelectedItemDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _moveRightSelectedItemDown.Click
			MoveSelectedItem(_rightListBox, MoveDirection.Down)
		End Sub

#End Region

#Region " Highlight methods and event handlers "

		Public Sub ClearItems(ByVal location As ListBoxLocation)
			Dim listbox As System.Windows.Forms.ListBox
			If location = ListBoxLocation.Left Then
				_leftListBox.HighlightIndex = -1
				listbox = _leftListBox
			Else
				_rightListBox.HighlightIndex = -1
				listbox = _rightListBox
			End If
			listbox.Refresh()
		End Sub

		Public Sub HighlightItembyIndex(ByVal index As Int32, ByVal location As ListBoxLocation)
			Dim listbox As kCura.Windows.Forms.ListBox = If(location = ListBoxLocation.Left, _leftListBox, _rightListBox)

			If location = ListBoxLocation.Left Then
				_leftListBox.HighlightIndex = index
				_leftListBox.Refresh()
			Else
				_rightListBox.HighlightIndex = index
				_rightListBox.Refresh()
			End If
		End Sub

		Private Sub HighlightMouseOverItem(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs, ByVal location As ListBoxLocation)
			Dim raiseEventLocation As ListBoxLocation = If(location = ListBoxLocation.Left, ListBoxLocation.Right, ListBoxLocation.Left)

			Dim listbox As kCura.Windows.Forms.ListBox = DirectCast(sender, kCura.Windows.Forms.ListBox)
			Dim g As System.Drawing.Graphics = System.Drawing.Graphics.FromHwnd(Me.Handle)
			Dim index As Int32 = listbox.IndexFromPoint(New System.Drawing.Point(e.X, e.Y))
			If index >= 0 Then	'mouse is over an item
				HighlightItembyIndex(index, location)
				RaiseEvent HighlightItemByLocationAndIndex(Me, New HighlightItemEventArgs With {.Location = raiseEventLocation, .Index = index})
			Else
				ClearItems(location)
				RaiseEvent ClearHighlightedItems(Me, New HighlightItemEventArgs With {.Location = raiseEventLocation})
			End If

			g.Dispose()
		End Sub

		Private Sub _rightListBox_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles _rightListBox.MouseLeave
			ClearItems(ListBoxLocation.Right)
			RaiseEvent ClearHighlightedItems(Me, New HighlightItemEventArgs With {.Location = ListBoxLocation.Left})
		End Sub

		Private Sub _leftListBox_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles _leftListBox.MouseLeave
			ClearItems(ListBoxLocation.Left)
			RaiseEvent ClearHighlightedItems(Me, New HighlightItemEventArgs With {.Location = ListBoxLocation.Right})
		End Sub

		Private Sub _leftListBox_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles _leftListBox.MouseMove
			If Not Me.OuterBox = ListBoxLocation.Left Then
				HighlightMouseOverItem(sender, e, ListBoxLocation.Left)
			End If
		End Sub

		Private Sub _rightListBox_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles _rightListBox.MouseMove
			If Not Me.OuterBox = ListBoxLocation.Right Then
				HighlightMouseOverItem(sender, e, ListBoxLocation.Right)
			End If
		End Sub

#End Region

	End Class

End Namespace