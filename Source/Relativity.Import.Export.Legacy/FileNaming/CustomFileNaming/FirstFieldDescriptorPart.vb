Namespace FileNaming.CustomFileNaming
	<Serializable>
	Public Class FirstFieldDescriptorPart
		Inherits FieldDescriptorPart

		Public Sub New(ByVal fieldId As Integer, Optional isProduction As Boolean = False)
			MyBase.New(fieldId)
			Me.IsProduction = isProduction
		End Sub
		Public ReadOnly Property IsProduction As Boolean
	End Class
End Namespace
