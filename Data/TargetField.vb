Imports System.Collections.Generic
Imports Relativity.Applications.Serialization.Elements

Namespace kCura.EDDS.WinForm.Data

	Public Class TargetField
		Public Property MyName As String
		Public Property FieldGuids As New List(Of Guid)
		Public Property ArtifactID As Int32

		Private Shared _empty As New TargetField() With {.MyName = " - "}

		Public Shared ReadOnly Property Empty As TargetField
			Get
				Return _empty
			End Get
		End Property

		Public ReadOnly Property IsEmpty As Boolean
			Get
				Return Me Is Empty
			End Get
		End Property

	End Class

End Namespace