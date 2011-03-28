Namespace kCura.Windows.Forms
	''' <summary>
	''' An EventArgs class used for highlight item events in ListBox controls.
	''' </summary>
	Public Class HighlightItemEventArgs
		Inherits EventArgs

		''' <summary>
		''' Indicates the position of the listbox with the highlighted item
		''' </summary>
		''' <value></value>
		''' <returns>A <see cref="kCura.Windows.Forms.ListBoxLocation"></see> enum.</returns>
		Public Property Location As ListBoxLocation

		''' <summary>
		''' Indicates the item that was highlighted
		''' </summary>
		''' <value></value>
		''' <returns>An integer representing the index of the item.</returns>
		Public Property Index As Int32
	End Class
End Namespace