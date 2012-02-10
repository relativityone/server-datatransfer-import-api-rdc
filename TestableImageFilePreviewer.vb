Namespace kCura.WinEDDS.NUnit
	Public Class TestableImageFilePreviewer
		Inherits ImageFilePreviewer

		Public Sub New(ByVal args As ImageLoadFile, ByVal controller As kCura.Windows.Process.Controller, ByVal doRetryLogic As Boolean)
			MyBase.New(args, controller, doRetryLogic)
		End Sub

		Protected Overrides Sub InitializeUploaders(ByVal args As ImageLoadFile)
			'do nothing
		End Sub

	End Class
End Namespace