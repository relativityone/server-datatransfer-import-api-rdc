Namespace FileNaming.CustomFileNaming
	<Serializable>
	Public Class FirstFieldDescriptorPart
		Inherits ValueDescriptorPart

		Public Sub New(ByVal fieldId As Integer, isProduction As Boolean)
			Me.Value = fieldId
			Me.isProduction = isProduction
		End Sub

		Public ReadOnly Property Value As Integer
		Public ReadOnly Property isProduction As Boolean
	End Class
End Namespace
