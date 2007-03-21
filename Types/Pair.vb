Namespace kCura.WinEDDS
	Public Class Pair
		Public Value As String
		Public Display As String
		Public Overrides Function ToString() As String
			Return Me.Display
		End Function
		Public Sub New(ByVal v As String, ByVal d As String)
			Me.Value = v
			Me.Display = d
		End Sub
	End Class
End Namespace