Namespace kCura.WinEDDS
	Public Class DataTableCellValueBooleanPair
		Private _value As String
		Private _isError As Boolean
		Public Sub New(obj As Object)
			Reset(obj)
		End Sub
		Public Sub Reset(obj As Object)
			_value = obj.ToString()
			_isError = TypeOf obj Is Exceptions.ErrorMessage
		End Sub
		Public Overrides Function ToString() As String
			Return _value
		End Function
		Public ReadOnly Property IsError As Boolean
			Get
				Return _isError
			End Get
		End Property
	End Class
End Namespace

