Namespace FileNaming.CustomFileNaming
	<Serializable>
	Public Class SeparatorDescriptorPart
		Inherits DescriptorPart

		Public Sub New(ByVal value As String)
			Me.Value = value
		End Sub

		Public ReadOnly Property Value As String
	End Class
End Namespace
