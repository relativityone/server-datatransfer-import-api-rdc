Public Enum DropOperation
	Reorder
	MoveToHere
	CopyToHere
	MoveFromHere
	CopyFromHere
End Enum

Public Class DroppedEventArgs
	Inherits EventArgs

	Public Property Operation As DropOperation
	Public Property Source As IDragDropSource
	Public Property Target As IDragDropSource

	Public Property DroppedItems As Object()
End Class
