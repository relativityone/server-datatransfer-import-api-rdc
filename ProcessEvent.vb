Namespace kCura.Windows.Process
	<Serializable()> Public Class ProcessEvent
		Public DateTime As DateTime
		Public Type As ProcessEventTypeEnum
		Public RecordInfo As String
		Public Message As String

		Public Sub New()

		End Sub

		Public Sub New(ByVal typeValue As ProcessEventTypeEnum, ByVal recordInfoValue As String, ByVal messageValue As String)
			Me.DateTime = System.DateTime.Now
			Me.Type = typeValue
			Me.RecordInfo = recordInfoValue
			Me.Message = messageValue
		End Sub
	End Class

	Public Enum ProcessEventTypeEnum
		Status = 0
		Warning = 1
		[Error] = 2
	End Enum

End Namespace