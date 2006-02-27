Namespace kCura.WinEDDS
	<Serializable()> Public Class ImageLoadFile
		<NonSerialized()> Public CaseInfo As kCura.EDDS.Types.CaseInfo
		Public DestinationFolderID As Int32
		Public FileName As String
		Public Overwrite As Boolean
		Public ControlKeyField As String
		<NonSerialized()> Public Credential As Net.NetworkCredential
		Public Sub New()
			MyBase.New()
			Overwrite = True
		End Sub
	End Class
End Namespace