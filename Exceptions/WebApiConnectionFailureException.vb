Imports System.Runtime.Serialization

Namespace kCura.WinEDDS.Exceptions
	Public Class WebApiConnectionFailureException
		Inherits System.Exception

		Private Const DEFAULT_MESSAGE As String = "Unkown failure while connecting to Relativity Web Api"

		Public Sub New ()
			MyBase.New(DEFAULT_MESSAGE)
		End Sub

		Public Sub New (message As String)
			MyBase.New(message)
		End Sub

		Public Sub New (message As String, innerException As Exception)
			MyBase.New(message, innerException)
		End Sub

		Protected Sub New (serializationInfo As SerializationInfo, streamingContext As StreamingContext)
			MyBase.New(serializationInfo, streamingContext)
		End Sub
	End Class
	
End Namespace


