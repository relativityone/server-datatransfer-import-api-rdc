Namespace FileNaming.CustomFileNaming
	<Serializable>
	Public Class ExtendedDescriptorPart
		Public Sub New(separator As SeparatorDescriptorPart, valuePart As ValueDescriptorPart)
			Me.Separator = separator
			Me.ValuePart = valuePart
		End Sub
		
		Public ReadOnly Property Separator As SeparatorDescriptorPart
		Public ReadOnly Property ValuePart As ValueDescriptorPart
	End Class
End Namespace
