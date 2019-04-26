Imports kCura.WinEDDS
Imports Relativity.Import.Export

Public Class VersionCompatibilityChecker

	Private ReadOnly _appSettings As IAppSettings
	Private ReadOnly _importExportCompatibilityCheck As IImportExportCompatibilityCheck
	Private Const DefaultApplicationName As String = "Import API"


	Public Sub New(appSettings As IAppSettings, importExportCompatibilityCheck As IImportExportCompatibilityCheck)
		_appSettings = appSettings
		_importExportCompatibilityCheck = importExportCompatibilityCheck
	End Sub

	Public Sub Verify()

		If (Not _appSettings.EnforceVersionCompatibilityCheck) Then
			Return
		End If

		Dim applicationName As String = _appSettings.ApplicationName

		If String.IsNullOrEmpty(applicationName) Then
			applicationName = "Relativity Desktop Client"
		Else
			applicationName = DefaultApplicationName
		End If

		Dim result As VersionCompatibilityCheckResult = _importExportCompatibilityCheck.ValidateCompatibility()

		If Not result.CompatibilityResult Then
			Dim versionMsg As String

			If result.WebApiVersion <> Nothing Then
				versionMsg = String.Format($"WebApi {result.WebApiVersion} version")
			Else
				versionMsg = String.Format($"Relativity {result.RelativityVersion} version")
			End If

			Dim message As String = $"Your version of {applicationName} ({ImportExportApiClientVersion.Version _
					}) is not comaptible with {versionMsg}."

			Throw New RelativityVersionMismatchException(message, versionMsg, ImportExportApiClientVersion.Version.ToString())
		End If
	End Sub

End Class
