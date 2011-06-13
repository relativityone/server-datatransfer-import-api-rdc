Imports System.Collections.Generic
Imports System.Linq
Imports Relativity.Applications.Serialization.Elements

Namespace kCura.EDDS.WinForm.Data

	Public Class AppObject
		Public Property ObjectName As String = String.Empty
		Public Property ObjectGuid As Guid
		Public Property AppFields As New List(Of AppField)
		Public Property QualifiesForMapping As Boolean = False

		Public Function FindExistingTargertField(ByVal artifactID As Int32) As TargetField
			For Each appField In AppFields
				For Each target In appField.MappingCandidates
					If target.ArtifactID = artifactID Then
						Return target
					End If
				Next
			Next
			Return Nothing
		End Function
	End Class

End Namespace