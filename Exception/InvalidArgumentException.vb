Namespace kCura.CommandLine.Exception
	Public Class InvalidArgumentException
		Inherits System.Exception

		Public Sub New(ByVal directive As String, ByVal value As String)
			MyBase.New(String.Format("Invalid command line argument: {0}:{1}", directive, value))
		End Sub

	End Class
End Namespace
