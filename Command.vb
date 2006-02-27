Namespace kCura.CommandLine
	Public Class Command

		Private _directive As String
		Private _value As String

		Public Property Directive() As String
			Get
				Return _directive
			End Get
			Set(ByVal value As String)
				_directive = value
			End Set
		End Property

		Public Property Value() As String
			Get
				Return _value
			End Get
			Set(ByVal value As String)
				_value = value
			End Set
		End Property

	End Class
End Namespace
