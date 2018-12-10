Namespace kCura.WinEDDS
	<Serializable()> Public Class Pair
		Implements IComparable

		Public Value As String
		Public Display As String
		Public Overrides Function ToString() As String
			Return Me.Display
		End Function
		Public Sub New(ByVal v As String, ByVal d As String)
			Me.Value = v
			Me.Display = d
		End Sub
		Public Shadows Function equals(ByVal other As kCura.WinEDDS.Pair) As Boolean
			Return (Me.Display = other.Display And Me.Value = other.Value)
		End Function

		Public Function CompareTo(ByVal other As object) As Integer Implements IComparable.CompareTo
			If other Is Nothing Then Return 1
			Return String.Compare(Display, other.ToString(), StringComparison.InvariantCulture)
		End Function

	End Class
End Namespace