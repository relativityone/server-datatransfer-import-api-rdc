Namespace kCura.WinEDDS.Exceptions
	Public Class InvalidLoginException
		Inherits System.Exception

		Public Sub New()
			MyBase.New("Invalid login. Try again?")
		End Sub
	End Class
End Namespace