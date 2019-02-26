Namespace kCura.Windows.Process
	''' <summary>
	''' A class for holding general result information. This result gets passed to the <see cref="kCura.Windows.Process.ProcessObserver"></see> which packages it into an event and raises it.
	''' </summary>
	Public Class DefaultResult
		''' <summary>
		''' A message from the running process.
		''' </summary>
		''' <value>A string representing the message.</value>
		''' <returns>A string representing the message.</returns>
		Public Property Message As String
		''' <summary>
		''' More detailed information from the running process (i think).
		''' </summary>
		''' <value>A string representing the detailed information.</value>
		''' <returns>A string representing the detailed information.</returns>
		Public Property RecordInfo As String
	End Class
End Namespace
