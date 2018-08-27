Public Interface IDragDropSource
	ReadOnly Property DragDropGroup As String
	ReadOnly Property IsDragDropMoveSource As Boolean
	Function GetSelectedItems() As Object()
	Sub RemoveSelectedItems(ByRef rowIndexToAjust As Integer)
	Sub OnDropped(ByVal e As DroppedEventArgs)
	Sub RemoveItems(srcItems As Object())
End Interface