Imports System.Collections.Generic
Imports kCura.WinEDDS.Api

Namespace kCura.WinEDDS
	Public Class PreviewChoicesHelper
		Private _totalFolders As New System.Collections.Specialized.HybridDictionary

		''' <summary>
		''' Build out the DataTable to display the folder and choice counts given a generic list
		''' </summary>
		''' <param name="al"></param>
		''' <param name="previewCodeCount"></param>
		''' <returns></returns>
		Public Function BuildFoldersAndCodesDataSource(ByVal al As List(Of Object), ByVal previewCodeCount As System.Collections.Specialized.HybridDictionary) As DataTable
			Return BuildFoldersAndCodesDataSource(New ArrayList(al), previewCodeCount)
		End Function

		''' <summary>
		''' Build out the DataTable to display the folder and choice counts
		''' </summary>
		''' <param name="al"></param>
		''' <param name="previewCodeCount"></param>
		''' <returns></returns>
		Public Function BuildFoldersAndCodesDataSource(ByVal al As ArrayList, ByVal previewCodeCount As System.Collections.Specialized.HybridDictionary) As DataTable
			Dim folderCount As Int32 = GetFolderCount(al)
			Dim dt As New DataTable
			Dim codeFieldColumnIndexes As ArrayList = GetCodeFieldColumnIndexes(DirectCast(al(0), System.Array))

			'setup columns
			dt.Columns.Add("Field Name")
			dt.Columns.Add("Count")

			'setup folder row
			Dim folderRow As New System.Collections.ArrayList
			folderRow.Add("Folders")
			If folderCount <> -1 Then
				folderRow.Add(folderCount.ToString)
			Else
				folderRow.Add("0")
			End If
			dt.Rows.Add(folderRow.ToArray)

			'insert spacer
			Dim blankRow As New System.Collections.ArrayList
			blankRow.Add("")
			blankRow.Add("")
			dt.Rows.Add(blankRow.ToArray)

			'setup choice rows
			If codeFieldColumnIndexes.Count = 0 Then
				dt.Columns.Add("     ")
				dt.Rows.Add(New String() {"No choice fields have been mapped"})
			Else
				For Each key As String In previewCodeCount.Keys
					Dim row As New System.Collections.ArrayList
					row.Add(key.Split("_".ToCharArray, 2)(1))
					Dim currentFieldHashTable As System.Collections.Specialized.HybridDictionary = DirectCast(previewCodeCount(key), System.Collections.Specialized.HybridDictionary)
					row.Add(currentFieldHashTable.Count)
					dt.Rows.Add(row.ToArray)
					currentFieldHashTable.Clear()
				Next
			End If

			Return dt
		End Function

		''' <summary>
		''' Get the folder count based on the ArrayList returned from the LoadFilePreviewer
		''' </summary>
		''' <param name="al">ArrayList containing preview info</param>
		''' <returns>The folder count</returns>
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

		''' <summary>
		''' Get a list of the column indexes that contain a choice or multi choice type
		''' </summary>
		''' <param name="firstRow">First row in the ArrayList returned from the LoadFilePreviewer</param>
		''' <returns>An ArrayList containing the column indexes</returns>
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

