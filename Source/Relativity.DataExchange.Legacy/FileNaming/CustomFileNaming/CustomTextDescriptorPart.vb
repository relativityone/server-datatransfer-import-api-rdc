Namespace FileNaming.CustomFileNaming
	<Serializable>
	Public Class CustomTextDescriptorPart
		Inherits ValueDescriptorPart

		Public Sub New(ByVal text As String)
			Me.Value = text
		End Sub

		Public ReadOnly Property Value As String
	End Class
End Namespace
