Imports NUnit.Framework
Imports kCura.Relativity.DataReaderClient.NUnit.kCura.Relativity.DataReaderClient.NUnit.Helpers
Imports System.Configuration
Imports System.Data.SqlClient

Namespace kCura.Relativity.DataReaderClient.NUnit.WriteTests
	<SetUpFixture()> _
	Public Class FixtureSetupTeardown
		Public Sub New()

		End Sub

		<SetUp()> _
		Public Sub FixtureSetup()
			Dim helper As New Helpers.SetupHelper()
			Try
				helper.CreateDefaultDirectories()

				Dim restoreDBSuccessful As Boolean = helper.RestoreDatabases()

				helper.FixUserMappings()

				helper.RestoreAllFiles()

				helper.SwitchRelativityServicesConnectionStringToTest()
				helper.SwitchProcuroConnectionStringToTest()
				helper.BuildProcuo()
				Dim procuroSucceeded As Boolean = helper.RunProcuro()
				If Not procuroSucceeded Then
					Throw New Exceptions.ProcuroException("Procuro Failed. Please refer to the results file " + _
					 ConfigurationManager.AppSettings("procuroResultsLocation").ToString() + " to identify the problem." + _
						"**** DEBUG INFO FOLLOWS **** NUnitDirectory = " + helper.ParentDirectoryName + _
						", EDDSDirectory = " + System.IO.Directory.GetParent(helper.ParentDirectoryName).FullName + _
						", procuroEXEPath = " + System.IO.Path.Combine(System.IO.Directory.GetParent(helper.ParentDirectoryName).FullName, _
						ConfigurationManager.AppSettings("procuroBuildLocation").ToString()))
				End If
			Catch x As SqlException
				Throw New Exceptions.DatabaseManagementException( _
				 "Restore of DB and/or Files failed during Fixture Setup. Please view the logs at C:\AutomatedTests\ImportAPI\Logs. " + _
				 x.Message)
			End Try

			Try
				helper.BackupTempDatabases(Helpers.SetupHelper._DBLIST, helper._DBLIST_FOR_TEMP_BACKUP)
			Catch x As SqlException
				Throw New ApplicationException( _
				 "Backup AfterProcuro of DB failed during Fixture Setup. Please view the logs at C:\AutomatedTests\ImportAPI\Logs. " + _
				 x.Message + x.StackTrace)
			End Try
		End Sub

		<TearDown()> _
		Public Sub FixtureTearDown()
			Dim helper As New Helpers.SetupHelper()
			' Note: Delete temp db backups
			For Each toDelete As String In System.IO.Directory.GetFiles(helper.GetTempDBBackupLocation())
				System.IO.File.Delete(toDelete)
			Next
			helper.SwitchProcuroConnectionStringToDev()
			helper.SwitchRelativityServicesConnectionStringToDev()
		End Sub
	End Class
End Namespace
