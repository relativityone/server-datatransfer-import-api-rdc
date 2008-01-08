Namespace kCura.WinEDDS.Service
	Public Class Settings
		''' -----------------------------------------------------------------------------
		''' <summary>
		'''		Default timeout wait time for Web Services in Milliseconds.
		'''		Set to 1 minute (2005-08-31).
		''' </summary>
		''' <remarks>
		''' </remarks>
		''' <history>
		''' 	[nkapuza]	8/31/2005	Created
		''' </history>
		''' -----------------------------------------------------------------------------
		Public Shared DefaultTimeOut As Int32 = 600000
		Public Shared AuthenticationToken As String = String.Empty
	End Class
End Namespace