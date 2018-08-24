Imports System.Drawing
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.CompilerServices
Imports Oli.Controls

Public Class DragDropListBox
	Inherits kCura.Windows.Forms.ListBox
	Implements IDragDropSource

	Private _selectionSave(0) As Integer
	Private _restoringSelection As Boolean = False
	Private _dragOriginBox As Rectangle = Rectangle.Empty
	Private _allowReorder As Boolean = True
	Private _isDragDropTarget As Boolean = True
	Private _visualCue As VisualCue

	Public Event AddFieldsEvent(items As Object())
	Public Event InsertFieldsEvent(position As Integer,item As Object)
	Public Event RemoveAtEvent As Action(Of Integer)

	Public Sub New()
		MyBase.New()
		AllowDrop = True
		_visualCue = New VisualCue(Me)
	End Sub

	Public Event Dropped As EventHandler(Of DroppedEventArgs)
	Private Sub SaveSelection()
		If Not _restoringSelection
			Dim sel As ListBox.SelectedIndexCollection = SelectedIndices
			If _selectionSave.Length <> sel.Count
				ReDim _selectionSave(sel.Count)
			End If
			SelectedIndices.CopyTo(_selectionSave, 0)
		End If
	End Sub

	Private Sub RestoreSelection(clickedItemIndex As Integer)
		If Control.ModifierKeys = Keys.Shift AndAlso Array.IndexOf(_selectionSave, clickedItemIndex) >= 0
			_restoringSelection = True
			For Each i As Integer In _selectionSave
				SetSelected(i, True)
			Next
			SetSelected(clickedItemIndex, true)
			_restoringSelection = False
		End If
	End Sub

	Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
		MyBase.OnMouseDown(e)
		Dim clickedItemIndex As Integer = IndexFromPoint(e.Location)

		If clickedItemIndex >= 0 AndAlso MouseButtons = MouseButtons.Left AndAlso (_isDragDropMoveSource OrElse _allowReorder) AndAlso (GetSelected(clickedItemIndex) OrElse Control.ModifierKeys = Keys.Shift)
		RestoreSelection(clickedItemIndex)

		Dim dragSize As Size = SystemInformation.DragSize
		_dragOriginBox = new Rectangle(new Point(CInt(e.X-(dragSize.Width / 2)), CInt(e.Y - (dragSize.Height / 2))), dragSize)
		End If

	End Sub

	Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
		MyBase.OnMouseUp(e)
		_dragOriginBox = Rectangle.Empty
	End Sub

	Protected Overrides Sub OnSelectedIndexChanged(e As EventArgs)
		MyBase.OnSelectedIndexChanged(e)
		SaveSelection()
	End Sub

	Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
		MyBase.OnMouseMove(e)
		If bSelectMode AndAlso e.Button <> Windows.Forms.MouseButtons.None Then
			Dim selectedindex As Integer= IndexFromPoint(e.Location)

			If selectedindex <> -1 Then
				SelectedItems.Add(Items(selectedindex))
			End If
		End If
		If _dragOriginBox <> Rectangle.Empty AndAlso Not _dragOriginBox.Contains(e.X, e.Y)
			DoDragDrop(new DataObject("IDragDropSource", Me), DragDropEffects.All)
			_dragOriginBox = Rectangle.Empty
		End If
	End Sub

	Public ReadOnly Property DragDropGroup As String Implements IDragDropSource.DragDropGroup
	Public ReadOnly Property IsDragDropMoveSource As Boolean Implements IDragDropSource.IsDragDropMoveSource

	Public Function GetSelectedItems() As Object() Implements IDragDropSource.GetSelectedItems
		Dim items As Object() = New Object(SelectedItems.Count - 1) {}
		SelectedItems.CopyTo(items, 0)
		Return items
	End Function

	Public Sub RemoveSelectedItems(ByRef itemIndexToAjust  As Integer) Implements IDragDropSource.RemoveSelectedItems
		For i As Integer = SelectedIndices.Count - 1 To 0
			Dim at As Integer = SelectedIndices(i)
			RaiseEvent RemoveAtEvent(at)
			'Items.RemoveAt(at)

			If at < itemIndexToAjust Then
				itemIndexToAjust -= 1
			End If
		Next
	End Sub


	Public Sub OnDropped(e As DroppedEventArgs) Implements IDragDropSource.OnDropped
		RaiseEvent Dropped(Me, e)
	End Sub


	Private Function GetDragDropEffect(ByVal drgevent As DragEventArgs) As DragDropEffects
		Dim effect As DragDropEffects = DragDropEffects.None


		Dim src As IDragDropSource = TryCast(drgevent.Data.GetData("IDragDropSource"), IDragDropSource)

		If src IsNot Nothing AndAlso _dragDropGroup = src.DragDropGroup Then

			If src.Equals(Me) Then

				If _allowReorder AndAlso Not Me.Sorted Then
					effect = DragDropEffects.Move
				End If
			ElseIf _isDragDropTarget Then

				If src.IsDragDropMoveSource Then
					effect = DragDropEffects.Move
				End If
			End If
		End If

		Return effect
	End Function

	Private Function DropIndex(ByVal yScreen As Integer) As Integer
		Dim y As Integer = PointToClient(New Point(0, yScreen)).Y

		If y < 0 Then
			y = 0
		ElseIf y > ClientRectangle.Bottom - 1 Then
			y = ClientRectangle.Bottom - 1
		End If

		Dim index As Integer = IndexFromPoint(0, y)

		If index = ListBox.NoMatches Then
			Return Items.Count
		End If

		Dim rect As Rectangle = GetItemRectangle(index)

		If y > rect.Top + rect.Height / 2 Then
			index += 1
		End If

		Dim lastFullyVisibleItemIndex As Integer = CType((TopIndex + ClientRectangle.Height / ItemHeight), Integer)

		If index > lastFullyVisibleItemIndex Then
			Return lastFullyVisibleItemIndex
		End If

		Return index
	End Function


	Protected Overrides Sub OnDragEnter(drgevent As DragEventArgs)
		MyBase.OnDragEnter(drgevent)
		drgevent.Effect = GetDragDropEffect(drgevent)
	End Sub

	Protected Overrides Sub OnDragLeave(e As EventArgs)
		MyBase.OnDragLeave(e)
		_visualCue.Clear()
	End Sub

	Protected Overrides Sub OnDragOver(drgevent As DragEventArgs)
		MyBase.OnDragOver(drgevent)
		drgevent.Effect = GetDragDropEffect(drgevent)

		If drgevent.Effect = DragDropEffects.None Then
			Return
		End If

		Dim dropIndex As Integer = Me.DropIndex(drgevent.Y)

		If dropIndex <> _visualCue.Index Then
			_visualCue.Clear()
			_visualCue.Draw(dropIndex)
		End If
	End Sub

Protected Overrides Sub OnDragDrop(ByVal drgevent As DragEventArgs)
    MyBase.OnDragDrop(drgevent)
    _visualCue.Clear()
    Dim src As IDragDropSource = TryCast(drgevent.Data.GetData("IDragDropSource"), IDragDropSource)
    Dim srcItems As Object() = src.GetSelectedItems()
    Dim sortedSave As Boolean = Sorted
    Sorted = False
    Dim row As Integer = DropIndex(drgevent.Y)
    Dim insertPoint As Integer = row

    If row >= Items.Count Then
		RaiseEvent AddFieldsEvent(srcItems)
       ' Items.AddRange(srcItems)
    Else

        For Each item As Object In srcItems
			RaiseEvent InsertFieldsEvent(Math.Min(System.Threading.Interlocked.Increment(row), row - 1), item)
            'Items.Insert(Math.Min(System.Threading.Interlocked.Increment(row), row - 1), item)
        Next
    End If

    Dim operation As DropOperation

    If drgevent.Effect = DragDropEffects.Move Then
        Dim adjustedInsertPoint As Integer = insertPoint
        src.RemoveSelectedItems(adjustedInsertPoint)

        If src.Equals(Me) Then
            insertPoint = adjustedInsertPoint
            operation = DropOperation.Reorder
        Else
            operation = DropOperation.MoveToHere
        End If
    Else
        operation = DropOperation.CopyToHere
    End If

    ClearSelected()

    If SelectionMode = SelectionMode.One Then
        SelectedIndex = insertPoint
    ElseIf SelectionMode <> SelectionMode.None Then

        For i As Integer = insertPoint To insertPoint + srcItems.Length - 1
            SetSelected(i, True)
        Next
    End If

    Sorted = sortedSave
    Dim e As DroppedEventArgs = New DroppedEventArgs() With {
        .Operation = operation,
        .Source = src,
        .Target = Me,
        .DroppedItems = srcItems
    }
    OnDropped(e)

    If operation <> DropOperation.Reorder Then
        e = New DroppedEventArgs() With {
            .Operation = If(operation = DropOperation.MoveToHere, DropOperation.MoveFromHere, DropOperation.CopyFromHere),
            .Source = src,
            .Target = Me,
            .DroppedItems = srcItems
        }
        src.OnDropped(e)
    End If
End Sub

	Private bSelectMode As Boolean = False

	Private Sub Form1_KeyUpOrDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown, Me.KeyUp
		bSelectMode = e.Control OrElse e.Shift
	End Sub

	Public Property IsDragDropTarget As Boolean
		Get
			Return _isDragDropTarget
		End Get
		Set(ByVal value As Boolean)
			_isDragDropTarget = value
			MyBase.AllowDrop = _isDragDropTarget OrElse _allowReorder
		End Set
	End Property

End Class
