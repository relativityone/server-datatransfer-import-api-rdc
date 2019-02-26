Namespace kCura.Windows.Process.Generic
	<Serializable()> Public Class ProcessEvent(Of T)
		Public Property DateTime As DateTime
		Public Property Type As ProcessEventTypeEnum
		Public Property RecordInfo As String
		Public Property Message As String
		Public Property Result As T

		Public Sub New()

		End Sub

		Public Sub New(ByVal typeValue As ProcessEventTypeEnum, ByVal res As T, ByVal getMessage As Func(Of T, String), ByVal getRecordInfo As Func(Of T, String))
			Me.DateTime = System.DateTime.Now
			Me.Type = typeValue
			Result = res
			Me.RecordInfo = getRecordInfo(Result)
			Me.Message = getMessage(Result)
		End Sub
	End Class
End Namespace