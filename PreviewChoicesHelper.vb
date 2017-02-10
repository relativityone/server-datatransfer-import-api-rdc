Namespace kCura.WinEDDS
	Public Class PreviewChoicesHelper
		Private _totalFolders As New System.Collections.Specialized.HybridDictionary

		Public Function GetFolderCount(ByVal al As ArrayList) As Int32
			_totalFolders.Clear()
			Dim fieldValue As String
			Dim item As Object
			Dim fields As Api.ArtifactField()
			Dim folderColumnIndex As Int32 = GetFolderColumnIndex(DirectCast(al(0), System.Array))

			If folderColumnIndex <> -1 Then
				For Each item In al
					If Not item Is Nothing Then
						fields = DirectCast(item, Api.ArtifactField())
						If folderColumnIndex <> -1 Then
							fieldValue = fields(folderColumnIndex).ValueAsString
							AddFoldersToTotalFolders(fieldValue)
						End If
					End If
				Next
				Return _totalFolders.Count
			End If
			Return -1
		End Function

		Private Function GetFolderColumnIndex(ByVal firstRow As Array) As Int32
			Dim folderColumnIndex As Int32 = -1
			Dim currentIndex As Int32 = 0
			For Each field As Api.ArtifactField In firstRow
				If field.ArtifactID = -2 And field.DisplayName = "Parent Folder Identifier" Then folderColumnIndex = currentIndex
				currentIndex += 1
			Next
			Return folderColumnIndex
		End Function

		Public Function GetCodeFieldColumnIndexes(ByVal firstRow As Array) As ArrayList
			Dim codeFieldColumnIndexes As New ArrayList
			Dim currentIndex As Int32 = 0
			For Each field As Api.ArtifactField In firstRow
				If field.Type = Relativity.FieldTypeHelper.FieldType.Code OrElse field.Type = Relativity.FieldTypeHelper.FieldType.MultiCode Then
					codeFieldColumnIndexes.Add(currentIndex)
				End If
				currentIndex += 1
			Next
			Return codeFieldColumnIndexes
		End Function

		Private Sub AddFoldersToTotalFolders(ByVal folderPath As String)
			If folderPath <> String.Empty AndAlso folderPath <> "\" Then
				If folderPath.LastIndexOf("\"c) < 1 Then
					If Not _totalFolders.Contains(folderPath) Then _totalFolders.Add(folderPath, String.Empty)
				Else
					If Not _totalFolders.Contains(folderPath) Then _totalFolders.Add(folderPath, String.Empty)
					AddFoldersToTotalFolders(folderPath.Substring(0, folderPath.LastIndexOf("\"c)))
				End If
			End If
		End Sub
	End Class
End Namespace

