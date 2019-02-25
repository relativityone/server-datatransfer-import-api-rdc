Imports System.Runtime.CompilerServices

Namespace Relativity.Desktop.Client.Legacy.Controls
	Module ListBoxExtensions

	    <Extension>   
	    Public Sub SelectItemWhichNameStartsWith(listBox As ListBox, firstLetter As Char)  
	        Dim loweredFirstLetter As Char = Char.ToLower(firstLetter)
	        Dim itemsStartingWithLetter As IList(Of Object) = listBox.Items.OfType(Of Object).Where(
	            Function(item) item.ToString().ToLower().StartsWith(loweredFirstLetter)
	            ).ToList()
	        Dim itemToSelect As Object = GetFirstItemNextToCurrentlySelectedOne(
	            itemsStartingWithLetter, 
	            listBox.SelectedItem
	        )

	        If itemToSelect IsNot Nothing Then
	            listBox.ClearSelected()
	            listBox.SelectedItem = itemToSelect
	        End If 
	    End Sub  

	    Private Function GetFirstItemNextToCurrentlySelectedOne(items As IList(Of Object), currentSelectedItem As Object) As Object
	        If Not items.Any() Then
	            Return Nothing
	        End If
	            
	        Dim currentSelectedItemIndex As Integer = items.IndexOf(currentSelectedItem)
	        Dim itemToSelectIndex As Integer = (currentSelectedItemIndex + 1) Mod items.Count 
	        Return items.Item(itemToSelectIndex)
	    End Function

	End Module
End Namespace