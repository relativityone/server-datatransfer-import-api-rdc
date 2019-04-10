Imports System.Runtime.Serialization

Namespace Relativity.Desktop.Client
	''' <summary>
	''' Represents an exceptional configuration state.
	''' </summary>
	<Serializable>
	Public Class ConfigurationException
		Inherits System.Exception

		''' <summary>
		''' Initializes a new instance of ConfigurationException with the specified message and inner exception.
		''' </summary>
		''' <param name="message">The message that explains the exception.</param>
		''' <param name="ex">The inner exception related to this exception.</param>
		Public Sub New(ByVal message As String, ByVal ex As System.Exception)
			MyBase.New(message, ex)
		End Sub

		''' <summary>
		''' Initializes a new instance of ConfigurationException with the specified message.
		''' </summary>
		''' <param name="message">The message that explains the exception.</param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' Initializes a new instance of ConfigurationException.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Initializes a new instance of ConfigurationException with serialized data.
		''' </summary>
		''' <param name="info">The SerializationInfo that represents the exception being thrown.</param>
		''' <param name="context">The StreamingContext the contains information about the source or destination.</param>
		Public Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
			MyBase.New(info, context)
		End Sub
	End Class
End Namespace