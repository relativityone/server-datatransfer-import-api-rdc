Imports System.Collections
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Collections.Generic
Imports Specialized

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
		Friend WithEvents _searchableListLeft As Specialized.SearchableList
		Friend WithEvents _searchableListRight As Specialized.SearchableList
		Friend WithEvents _moveLeftSelectedItemUp As System.Windows.Forms.Button

		Private Sub InitializeComponent()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TwoListBox))
		Me._moveAllFieldsLeft = New System.Windows.Forms.Button()
		Me._moveFieldLeft = New System.Windows.Forms.Button()
		Me._moveFieldRight = New System.Windows.Forms.Button()
		Me._moveAllFieldsRight = New System.Windows.Forms.Button()
		Me._moveRightSelectedItemDown = New System.Windows.Forms.Button()
		Me._moveRightSelectedItemUp = New System.Windows.Forms.Button()
		Me._moveLeftSelectedItemDown = New System.Windows.Forms.Button()
		Me._moveLeftSelectedItemUp = New System.Windows.Forms.Button()
		Me._searchableListRight = New Specialized.SearchableList()
		Me._searchableListLeft = New Specialized.SearchableList()
		Me._rightListBox = New kCura.Windows.Forms.ListBox()
		Me._leftListBox = New kCura.Windows.Forms.ListBox()
		Me.SuspendLayout
		'
		'_moveAllFieldsLeft
		'
		Me._moveAllFieldsLeft.Font = New System.Drawing.Font("Wingdings", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2,Byte))
		Me._moveAllFieldsLeft.Location = New System.Drawing.Point(172, 172)
		Me._moveAllFieldsLeft.Name = "_moveAllFieldsLeft"
		Me._moveAllFieldsLeft.Size = New System.Drawing.Size(36, 24)
		Me._moveAllFieldsLeft.TabIndex = 15
		Me._moveAllFieldsLeft.Text = "çç"
		'
		'_moveFieldLeft
		'
		Me._moveFieldLeft.Font = New System.Drawing.Font("Wingdings", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2,Byte))
		Me._moveFieldLeft.Location = New System.Drawing.Point(172, 140)
		Me._moveFieldLeft.Name = "_moveFieldLeft"
		Me._moveFieldLeft.Size = New System.Drawing.Size(36, 24)
		Me._moveFieldLeft.TabIndex = 14
		Me._moveFieldLeft.Text = "ß"
		'
		'_moveFieldRight
		'
		Me._moveFieldRight.Font = New System.Drawing.Font("Wingdings", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2,Byte))
		Me._moveFieldRight.Location = New System.Drawing.Point(172, 108)
		Me._moveFieldRight.Name = "_moveFieldRight"
		Me._moveFieldRight.Size = New System.Drawing.Size(36, 24)
		Me._moveFieldRight.TabIndex = 13
		Me._moveFieldRight.Text = "à"
		'
		'_moveAllFieldsRight
		'
		Me._moveAllFieldsRight.Font = New System.Drawing.Font("Wingdings", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2,Byte))
		Me._moveAllFieldsRight.Location = New System.Drawing.Point(172, 76)
		Me._moveAllFieldsRight.Name = "_moveAllFieldsRight"
		Me._moveAllFieldsRight.Size = New System.Drawing.Size(36, 24)
		Me._moveAllFieldsRight.TabIndex = 12
		Me._moveAllFieldsRight.Text = "èè"
		'
		'_moveRightSelectedItemDown
		'
		Me._moveRightSelectedItemDown.Font = New System.Drawing.Font("Wingdings", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2,Byte))
		Me._moveRightSelectedItemDown.Location = New System.Drawing.Point(360, 132)
		Me._moveRightSelectedItemDown.Name = "_moveRightSelectedItemDown"
		Me._moveRightSelectedItemDown.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me._moveRightSelectedItemDown.Size = New System.Drawing.Size(20, 24)
		Me._moveRightSelectedItemDown.TabIndex = 18
		Me._moveRightSelectedItemDown.Text = "â"
		'
		'_moveRightSelectedItemUp
		'
		Me._moveRightSelectedItemUp.Font = New System.Drawing.Font("Wingdings", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2,Byte))
		Me._moveRightSelectedItemUp.Location = New System.Drawing.Point(360, 108)
		Me._moveRightSelectedItemUp.Name = "_moveRightSelectedItemUp"
		Me._moveRightSelectedItemUp.Size = New System.Drawing.Size(20, 24)
		Me._moveRightSelectedItemUp.TabIndex = 17
		Me._moveRightSelectedItemUp.Text = "á"
		'
		'_moveLeftSelectedItemDown
		'
		Me._moveLeftSelectedItemDown.Font = New System.Drawing.Font("Wingdings", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2,Byte))
		Me._moveLeftSelectedItemDown.Location = New System.Drawing.Point(0, 140)
		Me._moveLeftSelectedItemDown.Name = "_moveLeftSelectedItemDown"
		Me._moveLeftSelectedItemDown.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me._moveLeftSelectedItemDown.Size = New System.Drawing.Size(20, 24)
		Me._moveLeftSelectedItemDown.TabIndex = 10
		Me._moveLeftSelectedItemDown.Text = "â"
		'
		'_moveLeftSelectedItemUp
		'
		Me._moveLeftSelectedItemUp.Font = New System.Drawing.Font("Wingdings", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2,Byte))
		Me._moveLeftSelectedItemUp.Location = New System.Drawing.Point(0, 108)
		Me._moveLeftSelectedItemUp.Name = "_moveLeftSelectedItemUp"
		Me._moveLeftSelectedItemUp.Size = New System.Drawing.Size(20, 24)
		Me._moveLeftSelectedItemUp.TabIndex = 9
		Me._moveLeftSelectedItemUp.Text = "á"
		'
		'_searchableListRight
		'
		Me._searchableListRight.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
		Me._searchableListRight.DataSource = CType(resources.GetObject("_searchableListRight.DataSource"),System.Collections.Generic.List(Of Object))
		Me._searchableListRight.IsListSortable = true
		Me._searchableListRight.Location = New System.Drawing.Point(212, 0)
		Me._searchableListRight.Margin = New System.Windows.Forms.Padding(4)
		Me._searchableListRight.Name = "_searchableListRight"
		Me._searchableListRight.Size = New System.Drawing.Size(144, 280)
		Me._searchableListRight.TabIndex = 20
		'
		'_searchableListLeft
		'
		Me._searchableListLeft.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
		Me._searchableListLeft.DataSource = CType(resources.GetObject("_searchableListLeft.DataSource"),System.Collections.Generic.List(Of Object))
		Me._searchableListLeft.IsListSortable = true
		Me._searchableListLeft.Location = New System.Drawing.Point(24, 0)
		Me._searchableListLeft.Margin = New System.Windows.Forms.Padding(4)
		Me._searchableListLeft.Name = "_searchableListLeft"
		Me._searchableListLeft.Size = New System.Drawing.Size(144, 280)
		Me._searchableListLeft.TabIndex = 19
		'
		'_rightListBox
		'
		Me._rightListBox.AlternateColors = false
		Me._rightListBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
		Me._rightListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
		Me._rightListBox.HighlightIndex = -1
		Me._rightListBox.HorizontalScrollbar = true
		Me._rightListBox.HorizontalScrollOffset = 0
		Me._rightListBox.IntegralHeight = false
		Me._rightListBox.Location = New System.Drawing.Point(212, 0)
		Me._rightListBox.Name = "_rightListBox"
		Me._rightListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
		Me._rightListBox.Size = New System.Drawing.Size(144, 280)
		Me._rightListBox.TabIndex = 16
		Me._rightListBox.VerticalScrollOffset = 0
		Me._rightListBox.Visible = false
		'
		'_leftListBox
		'
		Me._leftListBox.AlternateColors = false
		Me._leftListBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
		Me._leftListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
		Me._leftListBox.HighlightIndex = -1
		Me._leftListBox.HorizontalScrollbar = true
		Me._leftListBox.HorizontalScrollOffset = 0
		Me._leftListBox.IntegralHeight = false
		Me._leftListBox.Location = New System.Drawing.Point(24, 0)
		Me._leftListBox.Name = "_leftListBox"
		Me._leftListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
		Me._leftListBox.Size = New System.Drawing.Size(144, 280)
		Me._leftListBox.TabIndex = 11
		Me._leftListBox.VerticalScrollOffset = 0
		Me._leftListBox.Visible = false
		'
		'TwoListBox
		'
		Me.Controls.Add(Me._searchableListRight)
		Me.Controls.Add(Me._searchableListLeft)
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
		Me.ResumeLayout(false)

End Sub

#End Region

#Region "Resizing"

		'MeasureItem fixes issues in Large DPI mode
		Private Sub LeftListBox_MeasureItem(sender As Object, e As System.Windows.Forms.MeasureItemEventArgs) Handles _searchableListLeft.MeasureItemEvent
			MeasureItemImpl(_searchableListLeft, e)
		End Sub

		'MeasureItem fixes issues in Large DPI mode
		Private Sub RightListBox_MeasureItem(sender As Object, e As System.Windows.Forms.MeasureItemEventArgs) Handles _searchableListRight.MeasureItemEvent
			MeasureItemImpl(_searchableListRight, e)
		End Sub

		Private Sub MeasureItemImpl(parentListBox As SearchableList, e As System.Windows.Forms.MeasureItemEventArgs)
			Dim initialSize As Size = New Size(Me.Width, 1000)
			Dim str As String = parentListBox.CurrentItems(e.Index).ToString()
			Dim itemSize As SizeF = e.Graphics.MeasureString(str, _leftListBox.Font, initialSize)
			e.ItemHeight = CInt(itemSize.Height)
			e.ItemWidth = CInt(itemSize.Width)
		End Sub

		'These member variables are populated with data needed to resize the controls

		'Avoid adjusting the layout if the size hasn't changed
		Private _layoutControlSize As Size

		' The margin between controls plus the up-down button width
		' We need to know so we can adjust the control locations if the left buttons 
		' are changed to not-visible
		Private _layoutMarginPlusUpDownButtonWidth As Int32

		' Used to keep track of whether we need to calculate the layout values.  In addition to
		' initial population, they may need to be populated later due to autoscaling.  Autoscaling
		' will change the distance between concrols which we would not expect to change.  If this
		' happens, the _layout info which contains the relative location of controls needs to be 
		' updated.
		Private _layoutReferenceDistance As Int32 = 0

		Private _layoutRatioList As List(Of RelativeLayoutData)
		Private _layoutDifferenceList As List(Of RelativeLayoutData)

		Private Function CalcReferenceDistance() As Int32
			Return _searchableListRight.Left - _searchableListLeft.Right
		End Function

		Private Sub OnControl_Layout(ByVal sender As Object, ByVal e As System.Windows.Forms.LayoutEventArgs) Handles MyBase.Layout
			If _layoutReferenceDistance <> CalcReferenceDistance() Then
				InitializeLayout()
			Else
				AdjustLayout()
			End If
		End Sub

		Private Sub InitializeLayout()
			Dim margin As Int32 = _moveAllFieldsLeft.Left - _searchableListLeft.Right
			Dim upDownButtonWidth As Int32
			If (_moveRightSelectedItemUp.Visible) Then
				upDownButtonWidth = _moveRightSelectedItemUp.Width
			Else
				upDownButtonWidth = _moveLeftSelectedItemUp.Width
			End If
			_layoutMarginPlusUpDownButtonWidth = margin + upDownButtonWidth

			_layoutControlSize = Me.Size

			'Layout properties which are based on a ratio to another layout property. 
			If _layoutRatioList Is Nothing Then
				_layoutRatioList = New List(Of RelativeLayoutData)

				'When the width of the control increases by 2 pixels, each groupbox increases by 1 pixel.  The ratio is 1/2 = .5
				_layoutRatioList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForRatio.Width, _searchableListLeft, LayoutRelativePropertyTypeForRatio.Width, 0.5))
				_layoutRatioList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForRatio.Width, _searchableListRight, LayoutRelativePropertyTypeForRatio.Width, 0.5))

				'When the height of the control increases by 2 pixels, move the top of the arrow buttons by 1 pixel, 
				'so that they will continue to be vertically centered.  The ration is 1/2 = .5
				_layoutRatioList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForRatio.Height, _moveAllFieldsLeft, LayoutRelativePropertyTypeForRatio.Top, 0.5))
				_layoutRatioList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForRatio.Height, _moveAllFieldsRight, LayoutRelativePropertyTypeForRatio.Top, 0.5))
				_layoutRatioList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForRatio.Height, _moveFieldLeft, LayoutRelativePropertyTypeForRatio.Top, 0.5))
				_layoutRatioList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForRatio.Height, _moveFieldRight, LayoutRelativePropertyTypeForRatio.Top, 0.5))
				_layoutRatioList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForRatio.Height, _moveLeftSelectedItemUp, LayoutRelativePropertyTypeForRatio.Top, 0.5))
				_layoutRatioList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForRatio.Height, _moveLeftSelectedItemDown, LayoutRelativePropertyTypeForRatio.Top, 0.5))
				_layoutRatioList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForRatio.Height, _moveRightSelectedItemUp, LayoutRelativePropertyTypeForRatio.Top, 0.5))
				_layoutRatioList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForRatio.Height, _moveRightSelectedItemDown, LayoutRelativePropertyTypeForRatio.Top, 0.5))
			End If

			_layoutRatioList.ForEach(Sub(x)
										 x.InitalizeRatioValues()
									 End Sub)

			'Layout properties which are directly based on another layout property
			If _layoutDifferenceList Is Nothing Then
				_layoutDifferenceList = New List(Of RelativeLayoutData)

				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, _searchableListLeft, LayoutRelativePropertyTypeForDifference.Height))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, _searchableListRight, LayoutRelativePropertyTypeForDifference.Height))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, _searchableListLeft, LayoutRelativePropertyTypeForDifference.Height))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, _searchableListRight, LayoutRelativePropertyTypeForDifference.Height))

				_layoutDifferenceList.Add(New RelativeLayoutData(_searchableListLeft, LayoutBasePropertyTypeForDifference.Right, _moveAllFieldsLeft, LayoutRelativePropertyTypeForDifference.Left))
				_layoutDifferenceList.Add(New RelativeLayoutData(_searchableListLeft, LayoutBasePropertyTypeForDifference.Right, _moveAllFieldsRight, LayoutRelativePropertyTypeForDifference.Left))
				_layoutDifferenceList.Add(New RelativeLayoutData(_searchableListLeft, LayoutBasePropertyTypeForDifference.Right, _moveFieldLeft, LayoutRelativePropertyTypeForDifference.Left))
				_layoutDifferenceList.Add(New RelativeLayoutData(_searchableListLeft, LayoutBasePropertyTypeForDifference.Right, _moveFieldRight, LayoutRelativePropertyTypeForDifference.Left))

				_layoutDifferenceList.Add(New RelativeLayoutData(_moveFieldLeft, LayoutBasePropertyTypeForDifference.Right, _searchableListRight, LayoutRelativePropertyTypeForDifference.Left))

				_layoutDifferenceList.Add(New RelativeLayoutData(_searchableListRight, LayoutBasePropertyTypeForDifference.Right, _moveRightSelectedItemUp, LayoutRelativePropertyTypeForDifference.Left))
				_layoutDifferenceList.Add(New RelativeLayoutData(_searchableListRight, LayoutBasePropertyTypeForDifference.Right, _moveRightSelectedItemDown, LayoutRelativePropertyTypeForDifference.Left))

			End If

			_layoutDifferenceList.ForEach(Sub(x)
											  x.InitializeDifference()
										  End Sub)

			_layoutReferenceDistance = CalcReferenceDistance()
		End Sub

		Public Sub AdjustLayout()
			If Not _layoutControlSize.Equals(Me.Size) Then
				For Each x As RelativeLayoutData In _layoutRatioList
					x.AdjustRelativeControlBasedOnRatio()
				Next

				For Each x As RelativeLayoutData In _layoutDifferenceList
					x.AdjustRelativeControlBasedOnDifference()
				Next

				_layoutControlSize = Me.Size
			End If
		End Sub
#End Region

#Region " Properties "
		Public ReadOnly Property LeftSearchableList() As SearchableList
			Get
				Return _searchableListLeft
			End Get
		End Property
		Public ReadOnly Property RightSearchableList() As SearchableList
			Get
				Return _searchableListRight
			End Get
		End Property

		Public Property AlternateRowColors() As Boolean

		Public Property OuterBox() As ListBoxLocation

		Public Property LeftOrderControlsVisible() As Boolean
			Get
				Return _moveLeftSelectedItemDown.Visible
			End Get
			Set(ByVal value As Boolean)
				If _moveLeftSelectedItemDown.Visible <> value OrElse _moveLeftSelectedItemUp.Visible <> value Then
					Me.SuspendLayout()
					'System.Diagnostics.Debug.WriteLine("Visible = " + _moveLeftSelectedItemDown.Visible.ToString())
					'System.Diagnostics.Debug.WriteLine("Setting value to " + value.ToString())
					_moveLeftSelectedItemDown.Visible = value
					'System.Diagnostics.Debug.WriteLine("Visible = " + _moveLeftSelectedItemDown.Visible.ToString())
					_moveLeftSelectedItemUp.Visible = value

					'Visible
					If _moveLeftSelectedItemDown.Visible AndAlso _leftListBox.Left = 0 Then
						MoveControlsHorizontally(Me._layoutMarginPlusUpDownButtonWidth)
					ElseIf (Not _moveLeftSelectedItemDown.Visible) AndAlso _leftListBox.Left > 0 Then
						MoveControlsHorizontally(-_leftListBox.Left)
					End If
					Me.ResumeLayout()

					'Recalculate all the distances
					InitializeLayout()
				End If
			End Set
		End Property

		Private Sub MoveControlsHorizontally(distance As Int32)

			_searchableListLeft.Left = _searchableListLeft.Left + distance
			_searchableListRight.Left = _searchableListRight.Left + distance

			_moveAllFieldsLeft.Left = _moveAllFieldsLeft.Left + distance
			_moveAllFieldsRight.Left = _moveAllFieldsRight.Left + distance
			_moveFieldLeft.Left = _moveFieldLeft.Left + distance
			_moveFieldRight.Left = _moveFieldRight.Left + distance

			_moveRightSelectedItemDown.Left = _moveRightSelectedItemDown.Left + distance
			_moveRightSelectedItemUp.Left = _moveRightSelectedItemUp.Left + distance
		End Sub

		Public Property RightOrderControlVisible() As Boolean
			Get
				Return _moveRightSelectedItemDown.Visible
			End Get
			Set(ByVal value As Boolean)
				SetListBoxSortProperties(value)
				If _moveRightSelectedItemDown.Visible <> value OrElse _moveRightSelectedItemUp.Visible <> value Then
					_moveRightSelectedItemDown.Visible = value
					_moveRightSelectedItemUp.Visible = value
					InitializeLayout()
				End If
			End Set
		End Property

		Private Sub SetListBoxSortProperties(isLeftListBoxSortable As Boolean)
			_searchableListRight.IsListSortable = Not isLeftListBoxSortable
			_searchableListLeft.IsListSortable = isLeftListBoxSortable
		End Sub

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

		Public ReadOnly Property LeftSearchableListItems() As List(Of Object)
			Get
				Return _searchableListLeft.DataSource
			End Get
		End Property

		Public ReadOnly Property RightSearchableListItems() As List(Of Object)
			Get
				Return _searchableListRight.DataSource
			End Get
		End Property
#End Region

#Region " ListBox methods and event handlers "

		Private Sub SetEnabledOfRightMoveVerticalArrowButtons(value As Boolean)
			_moveRightSelectedItemDown.Enabled = value
			_moveRightSelectedItemUp.Enabled = value
		End Sub

		Private Sub SetEnabledOfLeftMoveVerticalArrowButtons(value As Boolean)
			_moveLeftSelectedItemDown.Enabled = value
			_moveLeftSelectedItemUp.Enabled = value
		End Sub
		Private Sub _searchableListLeft_TextChangedEvent(sender As Object) Handles _searchableListLeft.TextChangedEvent
			If LeftOrderControlsVisible
				If _searchableListLeft._textBox.Text <> ""
					SetEnabledOfLeftMoveVerticalArrowButtons(False)
					ElseIf _moveLeftSelectedItemUp.Enabled = False
					SetEnabledOfLeftMoveVerticalArrowButtons(True)
				End If
			End If
		End Sub

		Private Sub _searchableListRight_TextChangedEvent(sender As Object) Handles _searchableListRight.TextChangedEvent
			If Not LeftOrderControlsVisible
				If _searchableListRight._textBox.Text <> ""
					SetEnabledOfRightMoveVerticalArrowButtons(False)
				ElseIf _moveRightSelectedItemUp.Enabled = False
					SetEnabledOfRightMoveVerticalArrowButtons(True)
				End If
			End If
		End Sub

		Private Sub ShiftSelectedItems(ByVal giver As SearchableList, ByVal receiver As SearchableList)
			If giver.Listbox.SelectedItems.Count > 0 Then
				Dim selectedItemCollection As Windows.Forms.ListBox.SelectedObjectCollection = giver.Listbox.SelectedItems
				For i As Int32 = 0 To selectedItemCollection.Count - 1
					receiver.DataSource.Add(selectedItemCollection.Item(i))
					giver.DataSource.Remove(selectedItemCollection.Item(i))
				Next
				Me.RaiseItemsShifted()
			End If
		End Sub

		Private Sub RaiseItemsShifted()
			_searchableListLeft.ForceRefresh()
			_searchableListRight.ForceRefresh()
			_searchableListLeft.RemoveSelection()
			_searchableListRight.RemoveSelection()
			Me.EnsureHorizontalScrollbars()
			RaiseEvent ItemsShifted(Me, New EventArgs)
		End Sub

		Public Sub EnsureHorizontalScrollbars()
			Me.EnsureHorizontalScrollbarForBox(_searchableListLeft.Listbox)
			Me.EnsureHorizontalScrollbarForBox(_searchableListRight.Listbox)

		End Sub

		Private Sub EnsureHorizontalScrollbarForBox(ByVal box As kCura.Windows.Forms.ListBox)
			If _AlternateRowColors Then
				Using g As System.Drawing.Graphics = Me.CreateGraphics()
					box.HorizontalExtent = 0
					For i As Int32 = 0 To box.Items.Count - 1
						box.HorizontalExtent = System.Math.Max(CInt(box.HorizontalExtent), CInt(g.MeasureString(box.Items(i).ToString, box.Font, 0).Width + 20))
					Next

				End Using
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
			_searchableListLeft.ClearListBox()
			_searchableListLeft.Listbox.HorizontalScrollOffset = 0

			_searchableListRight.ClearListBox()
			_searchableListRight.Listbox.HorizontalScrollOffset = 0
		End Sub

		Private Sub TwoListBox_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
			_searchableListLeft.Listbox.AlternateColors = _AlternateRowColors
			_searchableListRight.Listbox.AlternateColors = _AlternateRowColors
		End Sub

		Private Sub _searchableListLeft_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles _searchableListLeft.KeyUpEvent
			If e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down OrElse e.KeyCode = Keys.Left OrElse e.KeyCode = Keys.Right Then
				_searchableListLeft.Listbox.Invalidate()
			End If
		End Sub

		Private Sub _searchableListRight_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles _searchableListRight.KeyUpEvent
			If e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down OrElse e.KeyCode = Keys.Left OrElse e.KeyCode = Keys.Right Then
				_searchableListRight.Listbox.Invalidate()
			End If
		End Sub

		Private Sub _searchableListLeft_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles _searchableListLeft.DoubleClickEvent
			ShiftSelectedItems(_searchableListLeft, __searchableListRight)
			EnsureHorizontalScrollbars()
		End Sub

		Private Sub _searchableListRight_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles _searchableListRight.DoubleClickEvent
			ShiftSelectedItems(__searchableListRight, _searchableListLeft)
			EnsureHorizontalScrollbars()
		End Sub

		Private Sub _leftListBox_Click(ByVal sender As Object, ByVal e As EventArgs) Handles _leftListBox.Click
			_leftListBox.Invalidate()
		End Sub

		Private Sub _rightListBox_Click(ByVal sender As Object, ByVal e As EventArgs) Handles _rightListBox.Click
			_rightListBox.Invalidate()
		End Sub

		Private Sub _searchableListLeft_KeyPress(sender As Object, e As KeyPressEventArgs) Handles _searchableListLeft.KeyPressEvent
			If e.KeyChar.Equals(Microsoft.VisualBasic.ControlChars.Cr) OrElse e.KeyChar.Equals(Microsoft.VisualBasic.ControlChars.Lf) Then
				ShiftSelectedItems(_searchableListLeft, __searchableListRight)
				EnsureHorizontalScrollbars()
			End If
		End Sub

		Private Sub _searchableListRight_KeyPress(sender As Object, e As KeyPressEventArgs) Handles _searchableListRight.KeyPressEvent
			If e.KeyChar.Equals(Microsoft.VisualBasic.ControlChars.Cr) OrElse e.KeyChar.Equals(Microsoft.VisualBasic.ControlChars.Lf) Then
				ShiftSelectedItems(__searchableListRight, _searchableListLeft)
				EnsureHorizontalScrollbars()
			End If
		End Sub

		Public Sub ForceRefresh()
			_searchableListLeft.ForceRefresh()
			_searchableListRight.ForceRefresh()
		End Sub

#End Region

#Region " Move methods and event handlers "

		Private Enum MoveDirection
			Up
			Down
		End Enum


		Private Sub MoveAllItems(ByVal giver As SearchableList, ByVal receiver As SearchableList)
			receiver.DataSource.AddRange(giver.CurrentItems)
			For Each currentItem As Object In giver.CurrentItems
				giver.DataSource.Remove(currentItem)
			Next
			Me.RaiseItemsShifted()
		End Sub

		Private Sub MoveSelectedItemAndRestoreSelection(ByVal box As SearchableList, ByVal direction As MoveDirection)
			Dim selectedItem As Object = box.Listbox.SelectedItem
			MoveSelectedItem(box, direction)
			box.SetSelection(selectedItem)
		End Sub

		Private Sub MoveSelectedItem(ByVal box As SearchableList, ByVal direction As MoveDirection)
			If box.DataSource.Count > 1 Then
				If box.Listbox.SelectedItems.Count = 1 Then
					Dim bound As Int32
					Dim indexModifier As Int32
					Select Case direction
						Case MoveDirection.Down
							bound = box.DataSource.Count - 1
							indexModifier = 1
						Case MoveDirection.Up
							bound = 0
							indexModifier = -1
					End Select
					If Not box.Listbox.SelectedIndex = bound Then
						Dim i As Int32 = box.DataSource.IndexOf(box.Listbox.SelectedItem)
						Dim selectedItem As Object = box.Listbox.SelectedItem
						box.DataSource.RemoveAt(i)
						box.DataSource.Insert(i + indexModifier, selectedItem)
						box.Listbox.SelectedIndex = i + indexModifier
					End If
				End If
			End If
			Me.RaiseItemsShifted()
		End Sub

		Private Sub _moveAllFieldsIn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _moveAllFieldsRight.Click
			MoveAllItems(_searchableListLeft, __searchableListRight)
		End Sub

		Private Sub _moveAllFieldsOut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _moveAllFieldsLeft.Click
			MoveAllItems(__searchableListRight, _searchableListLeft)
		End Sub

		Private Sub _moveFieldIn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _moveFieldRight.Click
			ShiftSelectedItems(_searchableListLeft, __searchableListRight)
		End Sub

		Private Sub _moveFieldOut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _moveFieldLeft.Click
			ShiftSelectedItems(__searchableListRight, _searchableListLeft)
		End Sub

		Private Sub _moveLeftSelectedItemUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _moveLeftSelectedItemUp.Click
			MoveSelectedItemAndRestoreSelection(_searchableListLeft, MoveDirection.Up)
		End Sub

		Private Sub _moveLeftSelectedItemDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _moveLeftSelectedItemDown.Click
			MoveSelectedItemAndRestoreSelection(_searchableListLeft, MoveDirection.Down)
		End Sub

		Private Sub _moveRightSelectedItemUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _moveRightSelectedItemUp.Click
			MoveSelectedItemAndRestoreSelection(_searchableListRight, MoveDirection.Up)
		End Sub

		Private Sub _moveRightSelectedItemDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _moveRightSelectedItemDown.Click
			MoveSelectedItemAndRestoreSelection(_searchableListRight, MoveDirection.Down)
		End Sub

#End Region

#Region " Highlight methods and event handlers "

		Public Sub ClearHighlight(ByVal location As ListBoxLocation)
			Dim listbox As kCura.Windows.Forms.ListBox = If(location = ListBoxLocation.Left, _searchableListLeft.Listbox, _searchableListRight.Listbox)
			listbox.HighlightIndex = -1
			listbox.Refresh()
		End Sub

		Public Sub ClearSelection(ByVal location As ListBoxLocation)
			Dim listbox As kCura.Windows.Forms.ListBox = If(location = ListBoxLocation.Left, _searchableListLeft.Listbox, _searchableListRight.Listbox)
			listbox.SelectedItem = Nothing
			listbox.SelectionMode = SelectionMode.None
			listbox.SelectionMode = SelectionMode.MultiExtended
			listbox.Refresh()
		End Sub

		Public Sub HighlightItembyIndex(ByVal index As Int32, ByVal location As ListBoxLocation)
			Dim listbox As kCura.Windows.Forms.ListBox = If(location = ListBoxLocation.Left, _searchableListLeft.Listbox, _searchableListRight.Listbox)
			listbox.HighlightIndex = index
			listbox.Refresh()
		End Sub

		Private Sub HighlightMouseOverItem(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs, ByVal location As ListBoxLocation)
			Dim raiseEventLocation As ListBoxLocation = If(location = ListBoxLocation.Left, ListBoxLocation.Right, ListBoxLocation.Left)

			Dim listbox As kCura.Windows.Forms.ListBox = DirectCast(sender, kCura.Windows.Forms.ListBox)
			Dim g As System.Drawing.Graphics = System.Drawing.Graphics.FromHwnd(Me.Handle)
			Dim index As Int32 = listbox.IndexFromPoint(New System.Drawing.Point(e.X, e.Y))
			If index >= 0 Then  'mouse is over an item
				HighlightItembyIndex(index, location)
				RaiseEvent HighlightItemByLocationAndIndex(Me, New HighlightItemEventArgs With {.Location = raiseEventLocation, .Index = index})
			Else
				ClearHighlight(location)
				RaiseEvent ClearHighlightedItems(Me, New HighlightItemEventArgs With {.Location = raiseEventLocation})
			End If

			g.Dispose()
		End Sub

		Private Sub _rightListBox_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles _rightListBox.MouseLeave
			ClearHighlight(ListBoxLocation.Right)
			RaiseEvent ClearHighlightedItems(Me, New HighlightItemEventArgs With {.Location = ListBoxLocation.Left})
		End Sub

		Private Sub _leftListBox_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles _leftListBox.MouseLeave
			ClearHighlight(ListBoxLocation.Left)
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