Namespace kCura.EDDS.WinForm
	Public Class CustomFileNameSelectionPart

		Public Sub New(fieldID As Integer)
			Me.New(Nothing, fieldID)
		End Sub

		Public Sub New(separator As String, fieldID As Integer)
			Me.Separator = separator
			Me.FieldID = fieldID
			Me.CustomText = Nothing
		End Sub

		Public Sub New(separator As String, customText As String)
			Me.Separator = separator
			Me.FieldID = -1
			Me.CustomText = customText
		End Sub

		Public Property Separator As String
		Public Property FieldID As Integer
		Public Property CustomText As String

	End Class
End Namespace