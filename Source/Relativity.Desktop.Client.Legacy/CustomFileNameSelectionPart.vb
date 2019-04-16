Namespace Relativity.Desktop.Client
	Public Class CustomFileNameSelectionPart
		Public Sub New(separator As String, fieldID As Integer)
			Me.New(
				separator,
				fieldID,
				customText:=Nothing,
				isProduction:=False
			)
		End Sub

		Public Sub New(separator As String, customText As String)
			Me.New(
				separator,
				fieldID:=-1,
				customText:=customText,
				isProduction:=False
			)
		End Sub

		Public Sub New(fieldID As Integer, isProduction As Boolean)
			Me.New(
				separator:=Nothing,
				fieldID:=fieldID,
				customText:=Nothing,
				isProduction:=isProduction
			)
		End Sub

		Private Sub New(
			separator As String,
			fieldID As Integer,
			customText As String,
			isProduction As Boolean
		)
			Me.Separator = separator
			Me.FieldID = fieldID
			Me.CustomText = customText
			Me.IsProductionBegBates = isProduction
		End Sub

		Public Property Separator As String
		Public Property FieldID As Integer
		Public Property CustomText As String

		Public Property IsProductionBegBates As Boolean

		Public Function HasCustomText() As Boolean
			Return FieldID = -1
		End Function

	End Class
End Namespace