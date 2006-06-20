Namespace kCura.WinEDDS
	Public Class ValueThrower
		Public Event OnEvent(ByVal value As Object)
		Public Sub ThrowValue(ByVal value As Object)
			RaiseEvent OnEvent(value)
		End Sub
	End Class
End Namespace