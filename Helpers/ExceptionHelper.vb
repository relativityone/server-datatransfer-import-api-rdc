Imports System.Collections.Generic
Imports System.Security
Imports System.Threading

Namespace kCura.WinEDDS.Helpers

	''' <summary>
	''' The exception related helper methods.
	''' </summary>
	Public Class ExceptionHelper

		''' <summary>
		''' A fatal exception message that tells user to try again and contact an admin if the problem persists.
		''' </summary>
		''' <remarks>
		''' This generic message suffix was provided by the docs team.
		''' </remarks>
		Public Const TryAgainAdminFatalMessage As String = "Try again. If the problem persists please contact your system administrator for assistance."

		''' <summary>
		''' The list of all well-known fatal exception types.
		''' </summary>
		Public Shared ReadOnly FatalExceptionCandidates As List(Of Type) = New List(Of Type)(
			New Type() {
				GetType(AccessViolationException),
				GetType(ApplicationException),
				GetType(BadImageFormatException),
				GetType(DivideByZeroException),
				GetType(DllNotFoundException),
				GetType(EntryPointNotFoundException),
				GetType(InsufficientMemoryException),
				GetType(NullReferenceException),
				GetType(OutOfMemoryException),
				GetType(OverflowException),
				GetType(SecurityException),
				GetType(StackOverflowException),
				GetType(ThreadAbortException)
			})

		''' <summary>
		''' The thread synchronization object.
		''' </summary>
		Private Shared ReadOnly SyncRoot As Object = New Object

		''' <summary>
		''' Appends <see cref="TryAgainAdminFatalMessage"/> to the fatal exception message and returns a new message.
		''' </summary>
		''' <param name="message">
		''' The exception message to append.
		''' </param>
		Public Shared Function AppendTryAgainAdminFatalMessage(ByVal message As String) As String
			If Not String.IsNullOrEmpty(message) Then
				message = message.TrimEnd(" "C, "."C) + ". "
			End If

			Return $"{message}{TryAgainAdminFatalMessage}"
		End Function

		''' <summary>
		''' Determines whether the <paramref name="exception"/> is considered fatal.
		''' </summary>
		''' <param name="exception">
		''' The exception to check.
		''' </param>
		''' <returns>
		''' <see langword="true"/> indicates the exception is fatal; otherwise, <see langword="false"/>.
		''' </returns>
		''' <exception cref="ArgumentNullException">
		''' Thrown when <paramref name="exception"/> is <see langword="null"/>.
		''' </exception>
		Public Shared Function IsFatalException (ByVal exception As Exception) As Boolean
			If exception Is Nothing Then
				Throw New ArgumentNullException(NameOf(exception))
			End If

			SyncLock SyncRoot
				Dim exceptionType As Type = exception.GetType()
				Dim result As Boolean = FatalExceptionCandidates.Any(function(type) exceptionType = type)
				Return result
			End SyncLock
		End Function
	End Class	
End Namespace