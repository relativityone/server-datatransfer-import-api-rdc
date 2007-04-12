Namespace kCura.WinEDDS
	<Serializable()> Public Class ImageLoadFile
		<NonSerialized()> Public CaseInfo As kCura.EDDS.Types.CaseInfo
		Public DestinationFolderID As Int32
		Public FileName As String
		Public Overwrite As String
		Public ControlKeyField As String
		Public ReplaceFullText As Boolean
		Public ForProduction As Boolean
		Public ProductionTable As System.Data.DataTable
		Public ProductionArtifactID As Int32
		<NonSerialized()> Public Credential As Net.NetworkCredential
		<NonSerialized()> Public CookieContainer As System.Net.CookieContainer
		'<NonSerialized()> Public Identity As kCura.EDDS.EDDSIdentity

		Public Sub New()
			'Public Sub New(ByVal identity As kCura.EDDS.EDDSIdentity)
			MyBase.New()
			Overwrite = "None"
			ProductionArtifactID = 0
			'Me.Identity = identity
		End Sub
	End Class
End Namespace