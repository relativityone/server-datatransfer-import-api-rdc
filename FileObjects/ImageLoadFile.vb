Namespace kCura.WinEDDS
	<Serializable()> Public Class ImageLoadFile
		<NonSerialized()> Public CaseInfo As kCura.EDDS.Types.CaseInfo
		Public DestinationFolderID As Int32
		Public FileName As String
		Public Overwrite As Boolean
		Public ControlKeyField As String
		Public ReplaceFullText As Boolean
		<NonSerialized()> Public Credential As Net.NetworkCredential
		<NonSerialized()> Public CookieContainer As System.Net.CookieContainer
		'<NonSerialized()> Public Identity As kCura.EDDS.EDDSIdentity

		Public Sub New()
			'Public Sub New(ByVal identity As kCura.EDDS.EDDSIdentity)
			MyBase.New()
			Overwrite = True
			'Me.Identity = identity
		End Sub
	End Class
End Namespace