Namespace kCura.Windows.Process
	''' <summary>
	''' Possible event types for the <see cref="kCura.Windows.Process.ProcessEvent"></see> object.
	''' </summary>
	''' <remarks></remarks>
	Public Enum ProcessEventTypeEnum
		''' <summary>
		''' Information message type
		''' </summary>
		''' <remarks></remarks>
		Status = 0
		''' <summary>
		''' Warning message type
		''' </summary>
		''' <remarks></remarks>
		Warning = 1
		''' <summary>
		''' Error message type
		''' </summary>
		''' <remarks></remarks>
		[Error] = 2
	End Enum
End Namespace
