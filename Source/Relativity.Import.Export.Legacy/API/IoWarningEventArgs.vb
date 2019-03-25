Namespace kCura.WinEDDS.Api
	Public Class IoWarningEventArgs
		Public Enum TypeEnum
			Message
			InstantRetryError
			WaitRetryError
		End Enum

		Private _waitTime As Int32
		Private _exception As System.Exception
		Private _currentLineNumber As Int64
		Private _message As String
		Private _type As TypeEnum

		Public Sub New(ByVal waitTime As Int32, ByVal ex As System.Exception, ByVal currentLineNumber As Int64)
			_waitTime = waitTime
			_exception = ex
			_currentLineNumber = currentLineNumber
			If waitTime > 0 Then
				_type = TypeEnum.WaitRetryError
			Else
				_type = TypeEnum.InstantRetryError
			End If
		End Sub
		Public Sub New(ByVal message As String, ByVal currentLineNumber As Int64)
			_message = message
			_currentLineNumber = currentLineNumber
			_type = TypeEnum.Message
		End Sub

		Public ReadOnly Property WaitTime() As Int32
			Get
				Return _waitTime
			End Get
		End Property
		Public ReadOnly Property Exception() As System.Exception
			Get
				Return _exception
			End Get
		End Property
		Public ReadOnly Property CurrentLineNumber() As Int64
			Get
				Return _currentLineNumber
			End Get
		End Property
		Public ReadOnly Property Message() As String
			Get
				Return _message
			End Get
		End Property

		Public ReadOnly Property Type() As TypeEnum
			Get
				Return _type
			End Get
		End Property

	End Class
End Namespace

