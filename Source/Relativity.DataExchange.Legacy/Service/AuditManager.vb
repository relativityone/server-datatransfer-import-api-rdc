Imports Relativity.DataExchange

Namespace kCura.WinEDDS.Service
	Public Class AuditManager
		Inherits kCura.EDDS.WebAPI.AuditManagerBase.AuditManager
		Implements Export.IAuditManager
		Implements Replacement.IAuditManager

#Region "Constructors"

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()

			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}AuditManager.asmx", AppSettings.Instance.WebApiServiceUrl)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

#End Region

#Region " Shadow Methods "

		Public Shadows Function CreateAuditRecord(ByVal caseContextArtifactID As Int32, ByVal artifactID As Int32, ByVal action As Int32, ByVal details As String, ByVal origination As String) As Boolean
			Return RetryOnReLoginException(Function() MyBase.CreateAuditRecord(caseContextArtifactID, artifactID, action, details, origination))
		End Function

		Public Shadows Function AuditImageImport(ByVal appID As Int32, ByVal runId As String, ByVal isFatalError As Boolean, ByVal importStats As kCura.EDDS.WebAPI.AuditManagerBase.ImageImportStatistics) As Boolean Implements Replacement.IAuditManager.AuditImageImport
			Return RetryOnReLoginException(Function() MyBase.AuditImageImport(appID, runId, isFatalError, importStats))
		End Function

		Public Shadows Function AuditObjectImport(ByVal appID As Int32, ByVal runId As String, ByVal isFatalError As Boolean, ByVal importStats As kCura.EDDS.WebAPI.AuditManagerBase.ObjectImportStatistics) As Boolean Implements Replacement.IAuditManager.AuditObjectImport
			Return RetryOnReLoginException(Function() MyBase.AuditObjectImport(appID, runId, isFatalError, importStats))
		End Function

		Public Shadows Function AuditExport(ByVal appID As Int32, ByVal isFatalError As Boolean, ByVal exportStats As kCura.EDDS.WebAPI.AuditManagerBase.ExportStatistics) As Boolean Implements Export.IAuditManager.AuditExport, Replacement.IAuditManager.AuditExport
			Return RetryOnReLoginException(Function() MyBase.AuditExport(appID, isFatalError, exportStats))
		End Function

		Public Shadows Sub DeleteAuditToken(ByVal token As String) Implements Replacement.IAuditManager.DeleteAuditToken
			RetryOnReLoginException(Sub() MyBase.DeleteAuditToken(token))
		End Sub

#End Region

	End Class
End Namespace
