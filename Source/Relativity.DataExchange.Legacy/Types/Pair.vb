Namespace kCura.WinEDDS
	<Serializable()> Public Class Pair
		Implements IComparable

		Public Sub New(value As String, display As String)
			Me.Value = value
			Me.Display = display
		End Sub

		Public Value As String
		Public Display As String

		Public Overrides Function ToString() As String
			Return Display
		End Function

		Public Shadows Function Equals(other As Pair) As Boolean
			Return Display = other.Display And Value = other.Value
		End Function

		Public Function CompareTo(other As object) As Integer Implements IComparable.CompareTo
			If other Is Nothing Then
				Return 1
			End If
			Dim otherPair As Pair = TryCast(other, Pair)
			If otherPair Is Nothing Then
				Throw New ArgumentException("Object is not a Pair")
			End If
			Return String.Compare(Display, otherPair.Display, StringComparison.InvariantCulture)
		End Function

	End Class
End Namespace