Imports System.Collections.Generic
Imports Relativity.Applications.Serialization.Elements

Namespace kCura.EDDS.WinForm.Data

	Public Class AppField
		Public Property MyName As String
		Public Property FieldGuids As New List(Of Guid)
		Public Property MappingCandidates As New List(Of TargetField)
		Public Property MappedTargetField As TargetField = TargetField.Empty
	End Class

End Namespace