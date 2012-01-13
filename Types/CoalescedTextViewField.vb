Namespace kCura.WinEDDS
	Public Class CoalescedTextViewField
		Inherits ViewFieldInfo
		Public Sub New(ByVal vfi As ViewFieldInfo, ByVal useCurrentFieldName As Boolean)
			MyBase.New(vfi)
			_avfColumnName = Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME
			Dim nameToUse As String = Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME
			If useCurrentFieldName Then nameToUse = vfi.DisplayName
			_avfHeaderName = nameToUse
			_displayName = nameToUse
		End Sub
	End Class
End Namespace