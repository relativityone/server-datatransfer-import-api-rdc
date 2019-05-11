﻿
Imports Relativity.DataExchange.Service

Namespace kCura.WinEDDS
	Public Class CoalescedTextViewField
		Inherits ViewFieldInfo
		Public Sub New(ByVal vfi As ViewFieldInfo, ByVal useCurrentFieldName As Boolean)
			MyBase.New(vfi)
			AvfColumnName = ServiceConstants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME
			Dim nameToUse As String = ServiceConstants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME
			If useCurrentFieldName Then nameToUse = vfi.DisplayName
			AvfHeaderName = nameToUse
			DisplayName = nameToUse
		End Sub
	End Class
End Namespace