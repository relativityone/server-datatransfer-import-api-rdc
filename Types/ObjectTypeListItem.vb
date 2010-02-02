Namespace kCura.WinEDDS
	Public Class ObjectTypeListItem

		Private _value As Int32
		Private _userCanAdd As Boolean
		Private _display As String

		Public ReadOnly Property Value() As Int32
			Get
				Return _value
			End Get
		End Property

		Public ReadOnly Property UserCanAdd() As Boolean
			Get
				Return _userCanAdd
			End Get
		End Property

		Public ReadOnly Property Display() As String
			Get
				Return _display
			End Get
		End Property

		Public Overrides Function ToString() As String
			Return _display
		End Function

		Public Sub New(ByVal v As Int32, ByVal d As String, ByVal userCanAdd As Boolean)
			_value = v
			_display = d
			_userCanAdd = userCanAdd
		End Sub

	End Class
End Namespace