Namespace kCura.WinEDDS
	Public Class CoalescedTextViewField
		Inherits ViewFieldInfo
		Public Sub New(ByVal vfi As ViewFieldInfo)
			MyBase.New(vfi)
			_avfColumnName = Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME
			_avfHeaderName = Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME
			_displayName = Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME
		End Sub
	End Class
End Namespace