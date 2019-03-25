Namespace FileNaming.CustomFileNaming
	<Serializable>
	Public Class FieldDescriptorPart
		Inherits ValueDescriptorPart

		Public Sub New(ByVal fieldId As Integer)
			Me.Value = fieldId
		End Sub

		Public ReadOnly Property Value As Integer
	End Class
End Namespace
