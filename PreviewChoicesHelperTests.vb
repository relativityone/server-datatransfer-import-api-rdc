Imports NUnit.Framework
Imports kCura.WinEDDS
Imports kCura.WinEDDS.Api

<TestFixture()>
Public Class PreviewChoicesHelperTests

	Private previewHelper As PreviewChoicesHelper
	Private artifactList As List(Of ArtifactField)
	Private al As ArrayList

	<SetUp()>
	Public Sub SetUp()
		previewHelper = New PreviewChoicesHelper
		artifactList = New List(Of ArtifactField)
		al = New ArrayList
	End Sub

	<Test()>
	Public Sub PreviewHelper_RetrieveFolderCount()
		PopulateArtifactList()

		Dim folderCount As Int32 = previewHelper.GetFolderCount(al)

		'Assert that the folder count correctly returns as 4
		Assert.AreEqual(4, folderCount)
	End Sub

	<Test()>
	Public Sub PreviewHelper_FindCodeFieldColumnIndexes()
		PopulateArtifactList()

		Dim fieldList As ArrayList = previewHelper.GetCodeFieldColumnIndexes(DirectCast(al(0), System.Array))

		'Assert that it identified that columns with artifactId 2 and 3 were a Code type
		Assert.AreEqual(2, fieldList.Count)
		Assert.AreEqual(2, fieldList(0))
		Assert.AreEqual(3, fieldList(1))
	End Sub

	Private Sub PopulateArtifactList()
		'Create an arraylist of 2 artifact field Arrays
		For j As Int32 = 0 To 1
			For i As Int32 = 0 To 4
				Dim docIdentifier As Int32 = i
				Dim fieldCat As Relativity.FieldCategory = Relativity.FieldCategory.Generic
				Dim fieldTypeId As Relativity.FieldTypeHelper.FieldType = Relativity.FieldTypeHelper.FieldType.Text
				'setting the first column as the identifier
				If (i = 0) Then
					fieldCat = Relativity.FieldCategory.Identifier
				ElseIf (i = 2 Or i = 3) Then 'Set these fields as choices
					fieldTypeId = Relativity.FieldTypeHelper.FieldType.Code
				ElseIf (i = 4) Then 'the 4th column will contain our folder path information
					fieldCat = Relativity.FieldCategory.ParentArtifact
					docIdentifier = -2
				End If
				Dim artifact As ArtifactField = New ArtifactField(String.Format("DisplayName{0}{1}", i, j), docIdentifier, fieldTypeId, fieldCat, -1, -1, -1, False)
				artifact.Value = i

				'Give our folder path information column the necessary DisplayName and a value that represents a folder path
				If (i = 4) Then
					artifact.Value = String.Format("\Folder\SubFolder\{0}", j)
					artifact.DisplayName = "Parent Folder Identifier"
				End If
				artifactList.Add(artifact)
			Next
			al.Add(artifactList.ToArray())
			artifactList.Clear()
		Next
	End Sub

End Class
