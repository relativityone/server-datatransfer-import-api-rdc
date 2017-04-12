Imports System.Threading.Tasks
Namespace kCura.WinEDDS.Exceptions
	Public Class LoginCanceledException
		Inherits TaskCanceledException
		
		Private Const _message As String = "A login was canceled before completion"

		Public Sub New(orginalException As Exception)
			MyBase.New(_message, orginalException)
		End Sub

	End Class

End Namespace

