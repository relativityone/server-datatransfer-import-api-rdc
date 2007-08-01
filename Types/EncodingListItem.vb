Namespace kCura.WinEDDS
	Public Class EncodingListItem
		Public Value As System.Text.Encoding
		Public Display As String
		Public Overrides Function ToString() As String
			Return Me.Display
		End Function
		Public Sub New(ByVal v As System.Text.Encoding, ByVal d As String)
			Me.Value = v
			Me.Display = d
		End Sub
	End Class
End Namespace