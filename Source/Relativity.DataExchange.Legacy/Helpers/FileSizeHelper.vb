Public Class FileSizeHelper
	Public Shared Function ConvertBytesNumberToDisplayString(bytesNumber As Long) As String
		If bytesNumber >= 1048576 Then
			Return $"{(bytesNumber / 1048576).ToString("N0")} MB"
		ElseIf bytesNumber < 1048576 AndAlso bytesNumber >= 102400 Then
			Return $"{(bytesNumber / 1024).ToString("N0")} KB"
		Else
			Return $"{bytesNumber.ToString("N0")} B"
		End If
	End Function
End Class
