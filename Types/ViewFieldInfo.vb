Namespace kCura.WinEDDS
	Public Class ViewFieldInfo
		Inherits kCura.EDDS.Types.ViewFieldInfo
		Implements IComparable
		Public Sub New(ByVal row As System.Data.DataRow)
			MyBase.New(row)
		End Sub
		Public Sub New(ByVal vfi As kCura.EDDS.Types.ViewFieldInfo)
			MyBase.New(vfi)
		End Sub

		Public Overrides Function ToString() As String
			Return Me.DisplayName
		End Function

		Public Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo
			Return String.Compare(Me.DisplayName, obj.ToString)
		End Function
	End Class

End Namespace
