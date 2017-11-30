Namespace kCura.WinEDDS.NUnit
	Public Class TestableImageFilePreviewer
		Inherits ImageFilePreviewer

		Public Sub New(ByVal controller As kCura.Windows.Process.Controller, ByVal doRetryLogic As Boolean)
			MyBase.New(controller, doRetryLogic)
		End Sub

		
	End Class
End Namespace