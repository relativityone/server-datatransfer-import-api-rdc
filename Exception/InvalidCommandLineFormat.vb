Namespace kCura.CommandLine.Exception
	Public Class InvalidCommandLineFormat
		Inherits System.ApplicationException

		Public Sub New()
			MyBase.New()
		End Sub

		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		Public Sub New(ByVal message As String, ByVal innerException As System.Exception)
			MyBase.New(message, innerException)
		End Sub

		Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
			MyBase.New(info, context)
		End Sub
	End Class
End Namespace

