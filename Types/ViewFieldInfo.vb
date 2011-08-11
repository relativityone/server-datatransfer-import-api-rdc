Namespace kCura.WinEDDS
	<Serializable()> Public Class ViewFieldInfo
		Inherits Relativity.ViewFieldInfo
		Implements IComparable

		Public Sub New(ByVal row As System.Data.DataRow)
			MyBase.New(row)
		End Sub
		Public Sub New(ByVal vfi As Relativity.ViewFieldInfo)
			MyBase.New(vfi)
		End Sub

		Public Overrides Function ToString() As String
			Return Me.DisplayName
		End Function

		Public Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo
			Return String.Compare(Me.DisplayName, obj.ToString)
		End Function

		Public Shadows Function Equals(ByVal other As ViewFieldInfo) As Boolean
			If Me.AvfId = other.AvfId AndAlso Me.AvfColumnName = other.AvfColumnName Then Return True
			Return False
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return 45 * Me.AvfId
		End Function
	End Class

End Namespace
