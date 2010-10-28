Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Text
Imports System.IO

Namespace kCura.Relativity.DataReaderClient.NUnit.Helpers
	Public Class SetupHelper

#Region "Setup/Tear down helpers"
		''' <summary>
		''' Set up a Test Case
		''' </summary>
		Public Sub SetupTestWithRestore()
			Dim restoreFromBackupAfterProcuro As Boolean = True
			If RestoreDatabases(restoreFromBackupAfterProcuro) Then
				'System.Data.SqlClient.SqlConnection.ClearAllPools()
			Else
				Throw New Exceptions.DatabaseManagementException("Database restore failed.")
			End If
		End Sub

		''' <summary>
		''' Set up a Test Case
		''' </summary>
		Public Sub SetupTestWithoutRestore()
		End Sub

		''' <summary>
		''' Tear down the test case
		''' </summary>
		''' <remarks></remarks>
		Public Sub TearDownTest()
		End Sub
#End Region
#Region "Constants - Relativity Users"
#End Region
#Region "Constants - Artifact IDs"
		'Public Const _CASE_ID_CRUD As Int32 = 1016204
		'Public Const _CASE_ID_QUERY_BATCH_SCRIPT As Int32 = 1016506
		Public Const _CASE_ID_IMPORT_API_SOURCE As Int32 = 1016621
		Public Const _CASE_ID_IMPORT_API_DESTINATION As Int32 = 1016623
		Public Const _DOCUMENT_ID As Int32 = 1035472
		Public Const _DOCUMENT_ID2 As Int32 = 1035604
		Public Const _DOCUMENT_ID3 As Int32 = 1035605
		Public Const _DOCUMENT_ID4 As Int32 = 1035606
		Public Const _DOCUMENT_ID5 As Int32 = 1035607
		Public Const _FOLDER_ID As Int32 = 1035430
#End Region

#Region "Folder Constants"
		Private Const _CAINDEX_BACKUP_DIRECTORY_NAME As String = "Backups\CAIndexBackup"
		Private Const _DTSEARCHINDEX_BACKUP_DIRECTORY_NAME As String = "Backups\dtSearchBackup"
		Private Const _FILEREPO_BACKUP_DIRECTORY_NAME As String = "Backups\FileRepoBackup"
		Private Const _DBBACKUP_DIRECTORY_NAME As String = "Backups\DatabaseBackup"
		Private Const _LOG_DIRECTORY_NAME As String = "C:\AutomatedTests\ImportAPI\Logs"
		Private ReadOnly _DBBACKUP_DIRECTORY_FULL_PATH As String = kCura.Utility.URI.ParentDirectoryName + "\Backups\DatabaseBackup"
		'"C:\SourceCode\Mainline\EDDS\kCura.Relativity.Client.NUnit\Backups\DatabaseBackup"

		Private ReadOnly _TEMPDBBACKUP_DIRECTORY_FULL_PATH As String = "C:\AutomatedTests\ImportAPI\TempDatabaseBackups"

		Private ReadOnly _CAINDEX_BACKUP_FULL_PATH As String = Path.Combine(ParentDirectoryName, _CAINDEX_BACKUP_DIRECTORY_NAME)
		Private ReadOnly _DTSEARCHINDEX_BACKUP_FULL_PATH As String = Path.Combine(ParentDirectoryName, _DTSEARCHINDEX_BACKUP_DIRECTORY_NAME)
		Private ReadOnly _FILEREPO_BACKUP_FULL_PATH As String = Path.Combine(ParentDirectoryName, _FILEREPO_BACKUP_DIRECTORY_NAME)
#End Region
#Region "Other Members"
		'Private Shared ReadOnly _CASEDBNAME_CRUD As String = "EDDS" + _CASE_ID_CRUD.ToString()
		'Private Shared ReadOnly _CASEDBNAME_QUERY_BATCH_SCRIPT As String = "EDDS" + _CASE_ID_QUERY_BATCH_SCRIPT.ToString()

		Private Shared ReadOnly _CASEDBNAME_ImportAPI_Source As String = "EDDS" + _CASE_ID_IMPORT_API_SOURCE.ToString()
		Private Shared ReadOnly _CASEDBNAME_ImportAPI_Destination As String = "EDDS" + _CASE_ID_IMPORT_API_DESTINATION.ToString()

		Public Shared ReadOnly _DBLIST() As String = {_CASEDBNAME_ImportAPI_Source, _CASEDBNAME_ImportAPI_Destination, "EDDS", "EDDSResource"}
		Public ReadOnly _DBLIST_FOR_TEMP_BACKUP() As String = {_CASEDBNAME_ImportAPI_Source + "_AFTERPROCURO", _CASEDBNAME_ImportAPI_Destination + "_AFTERPROCURO", "EDDS_AFTERPROCURO", "EDDSResource_AFTERPROCURO"}
		'Public Shared ReadOnly _DBLIST() As String = {_CASEDBNAME_CRUD, _CASEDBNAME_QUERY_BATCH_SCRIPT, "EDDS", "EDDSResource"}
		'Public ReadOnly _DBLIST_FOR_TEMP_BACKUP() As String = {_CASEDBNAME_CRUD + "_AFTERPROCURO", _CASEDBNAME_QUERY_BATCH_SCRIPT + "_AFTERPROCURO", "EDDS_AFTERPROCURO", "EDDSResource_AFTERPROCURO"}

		Private Shared _connStrForSetup As String = String.Empty
		Public _restoreSucceeded As Boolean = False
		Public WithEvents conn As SqlConnection
#End Region
#Region "Untouchable Members"
		Private Shared _fileRepoLocation As String = Nothing
		Private Shared _dtSearchIndexLocation As String = Nothing
		Private Shared _caIndexLocation As String = "C:\ContentAnalyst\indexes"
		Private Shared _ldfDirectory As String = Nothing
		Private Shared _mdfDirectory As String = Nothing
#End Region

#Region "Private ReadOnly Properties"
		Public ReadOnly Property ParentDirectoryName() As String
			Get
				Return Global.kCura.Utility.URI.ParentDirectoryName()
			End Get
		End Property

		Private Shared ReadOnly Property LDFPath() As String
			Get
				If String.IsNullOrEmpty(_ldfDirectory) Then
					_ldfDirectory = ConfigurationManager.AppSettings("defaultLDFDirectory").ToString()
					If Not Directory.Exists(_ldfDirectory) Then
						Throw New ApplicationException(String.Format("Default LDF Directory is not present. Please verify that {0} exists and is shared.", _
						 _ldfDirectory))
					End If
				End If
				Return _ldfDirectory
			End Get
		End Property

		Private Shared ReadOnly Property MDFPath() As String
			Get
				If String.IsNullOrEmpty(_mdfDirectory) Then
					_mdfDirectory = ConfigurationManager.AppSettings("defaultMDFDirectory").ToString()
					If Not Directory.Exists(_mdfDirectory) Then
						Throw New ApplicationException(String.Format("Default MDF Directory is not present. Please verify that {0} exists and is shared.", _
						 _mdfDirectory))
					End If

				End If
				Return _mdfDirectory
			End Get
		End Property

		Private Shared ReadOnly Property FileRepositoryLocationForRelativity() As String
			Get
				If String.IsNullOrEmpty(_fileRepoLocation) Then
					_fileRepoLocation = ConfigurationManager.AppSettings("defaultFileRepositoryLocation").ToString()
					If Not Directory.Exists(_fileRepoLocation) Then
						Throw New ApplicationException(String.Format("Default File Repository Directory is not present. Please verify that {0} exists and is shared.", _
						 _fileRepoLocation))
					End If

				End If
				Return _fileRepoLocation
			End Get
		End Property

		Private Shared ReadOnly Property DTSearchIndexLocationForRelativity() As String
			Get
				If String.IsNullOrEmpty(_dtSearchIndexLocation) Then
					_dtSearchIndexLocation = ConfigurationManager.AppSettings("defaultDTSearchIndexDirectory").ToString()
					If Not Directory.Exists(_dtSearchIndexLocation) Then
						Throw New ApplicationException(String.Format("Default dtSearch Indexes Directory is not present. Please verify that {0} exists and is shared.", _
						 _dtSearchIndexLocation))
					End If
				End If
				Return _dtSearchIndexLocation
			End Get
		End Property

		Private Shared ReadOnly Property CAIndexLocationForRelativity() As String
			Get
				If String.IsNullOrEmpty(_caIndexLocation) Then
					_caIndexLocation = ConfigurationManager.AppSettings("defaultCAIndexDirectory").ToString()
				End If
				Return _caIndexLocation
			End Get
		End Property
#End Region

#Region "Helpers"

		Public Shared Function GetConnectionString(ByVal forSetup As Boolean) As String
			If (forSetup) Then
				If (String.IsNullOrEmpty(_connStrForSetup)) Then
					_connStrForSetup = ConfigurationManager.AppSettings("connectionStringForSetup").ToString()
				End If
				Return _connStrForSetup
			Else
				Return ConfigurationManager.AppSettings("connectionString").ToString()
			End If
		End Function

		'Public Function GetCaseDBName() As String
		'	Return _CASEDBNAME_CRUD
		'End Function

		Public Function GetDBBackupLocation() As String
			Return _DBBACKUP_DIRECTORY_FULL_PATH
		End Function

		Public Function GetTempDBBackupLocation() As String
			Return _TEMPDBBACKUP_DIRECTORY_FULL_PATH
		End Function

		Public Function GetFileRepoBackupLocation() As String
			Return _FILEREPO_BACKUP_FULL_PATH
		End Function

		Public Function GetDtSearchIndexBackupLocation() As String
			Return _DTSEARCHINDEX_BACKUP_FULL_PATH
		End Function

		Public Function GetCAIndexBackupLocation() As String
			Return _CAINDEX_BACKUP_FULL_PATH
		End Function
#End Region	'Helpers
		Public Sub ReplaceAutenticationDataForCurrentUser()
			Dim replaceSql As New StringBuilder()
			replaceSql.AppendLine("use [EDDS]")
			replaceSql.AppendLine("UPDATE [EDDS].[EDDSDBO].[User]")
			replaceSql.Append(" SET [AuthenticationData] = '")
			replaceSql.AppendLine(System.Security.Principal.WindowsIdentity.GetCurrent().Name)
			replaceSql.AppendLine("' WHERE EmailAddress = 'relativity.admin@kcura.com'")

			Dim forSetup As Boolean = True
			Helpers.QueryExecutionHelper.RunSqlNonQuery(replaceSql.ToString(), forSetup, New SqlClient.SqlConnection(GetConnectionString(forSetup)), _LOG_DIRECTORY_NAME)
		End Sub

		' TODO: Rewrite this according to guidelines specified in the RestoreFiles() method notes
		'Private Sub RestoreFileRepository(ByVal fileRepositoryLocation As String)
		'	Dim allFiles() As String
		'	Dim overwrite As Boolean = True
		'	allFiles = System.IO.Directory.GetFiles(GetFileRepoBackupLocation())
		'	For Each file As String In allFiles
		'		System.IO.File.Copy(file, fileRepositoryLocation, overwrite)
		'	Next
		'End Sub

		' TODO: Rewrite this according to guidelines specified in the RestoreFiles() method notes
		'Private Sub RestoreDTSearchIndexes(ByVal dtSearchIndexLocation As String)
		'	Dim allIndexes() As String
		'	Dim overwrite As Boolean = True
		'	allIndexes = System.IO.Directory.GetFiles(GetDtSearchIndexBackupLocation())
		'	For Each file As String In allIndexes
		'		System.IO.File.Copy(file, dtSearchIndexLocation, overwrite)
		'	Next
		'End Sub

		' TODO: Rewrite this according to guidelines specified in the RestoreFiles() method notes
		'Private Sub RestoreCAIndexes(ByVal caIndexLocation As String)
		'	Dim allIndexes() As String
		'	Dim overwrite As Boolean = True
		'	allIndexes = System.IO.Directory.GetFiles(GetCAIndexBackupLocation())
		'	For Each file As String In allIndexes
		'		System.IO.File.Copy(file, caIndexLocation, overwrite)
		'	Next
		'End Sub

		Public Sub RestoreAllFiles()
			' Note: Restore files feature will be implemented in the near future by taking the following steps:
			' * Use the File table in the Case database to identify the actual locations of files
			' * Use the dtSearchIndex table in the Case database to identify actual dtSearchIndex locations
			' * Use the ContentAnalystIndex table in the Case database to identify actual CA Index locations
			' *
			' * Restore backed up files to appropriate locations [for efficiency, use the same location for all files of a single type (dt, ca, file)

			'RestoreFileRepository(FileRepositoryLocationForRelativity)
			'RestoreDTSearchIndexes(DTSearchIndexLocationForRelativity)
			'RestoreCAIndexes(CAIndexLocationForRelativity)
		End Sub

		''' <summary>
		''' Backs up database files
		''' </summary>
		''' <remarks></remarks>
		Public Sub BackupDatabases()
			Dim fileName As String

			For Each dbName As String In _DBLIST
				fileName = Path.Combine(_DBBACKUP_DIRECTORY_FULL_PATH, dbName)
				Dim messagesToWrite As New StringBuilder()

				messagesToWrite.AppendLine("============================================================================================" + vbCrLf)
				messagesToWrite.AppendLine("                    BEGIN BACKUP OF DATABASE [" + dbName + "]" + vbCrLf)
				messagesToWrite.AppendLine("============================================================================================" + vbCrLf)

				BackupSingleDatabase(dbName, fileName)

				messagesToWrite.AppendLine("============================================================================================" + vbCrLf)
				messagesToWrite.AppendLine("                    END OF  [" + dbName + "] DATABASE BACKUP" + vbCrLf)
				messagesToWrite.AppendLine("============================================================================================" + vbCrLf)
			Next

		End Sub

		''' <summary>
		''' Backs up database files
		''' </summary>
		''' <remarks></remarks>
		Public Sub BackupTempDatabases(ByVal listOfDatabases() As String, ByVal listOfTempDatabases() As String)
			Dim fileName As String

			For i As Int32 = 0 To listOfDatabases.Count - 1
				fileName = Path.Combine(GetTempDBBackupLocation(), listOfTempDatabases(i) + ".bak")
				If Not Directory.Exists(GetTempDBBackupLocation()) Then
					Directory.CreateDirectory(GetTempDBBackupLocation())
				End If
				BackupSingleDatabase(listOfDatabases(i), fileName)
			Next

		End Sub

		Private Function DatabaseExists(ByVal databaseName As String) As Boolean
			Dim sqlStatement As String = String.Format("SELECT DB_ID('{0}')", databaseName)
			Dim successful As Boolean = True
			Dim forSetup As Boolean = True
			If QueryExecutionHelper.RunSqlQueryScalarResult(sqlStatement, forSetup, New SqlClient.SqlConnection(GetConnectionString(forSetup)), _LOG_DIRECTORY_NAME) Is Nothing Then
				Return Not successful
			Else
				Return successful
			End If
		End Function

		Public Function BackupSingleDatabase(ByVal dbName As String, ByVal fileName As String) As Boolean
			Dim backupSql As New StringBuilder()
			Dim messagesToWrite As New StringBuilder()

			backupSql.AppendLine(String.Format("BACKUP DATABASE [{0}] TO DISK = N'{1}'", dbName, fileName))
			backupSql.AppendLine(String.Format(" WITH NOFORMAT, INIT, NAME = N'{0}',", dbName))
			backupSql.AppendLine(String.Format(" SKIP, NOREWIND, NOUNLOAD, STATS = 10"))
			backupSql.AppendLine(String.Format(" DECLARE @backupSetId AS INT"))
			backupSql.AppendLine(String.Format(" SELECT @backupSetId = position FROM msdb..backupset WHERE  database_name = N'{0}' AND", dbName))
			backupSql.AppendLine(String.Format(" backup_set_id=(SELECT MAX(backup_set_id) FROM msdb..backupset WHERE database_name=N'{0}' )", dbName))
			backupSql.AppendLine(String.Format(" IF @backupSetId IS NULL"))
			backupSql.AppendLine(String.Format(" BEGIN raiserror(N'Verify failed. Backup information for database ''{0}'' not found.', 16, 1) END", dbName))
			backupSql.AppendLine(String.Format(" RESTORE VERIFYONLY FROM  DISK = N'{0}' WITH  FILE = @backupSetId,  NOUNLOAD,  NOREWIND", fileName))

			Dim forSetup As Boolean = True
			Dim connStr As String = GetConnectionString(forSetup)
			Dim combinedSql As String = SqlForKillAllOpenConnectionsToDatabase(dbName, connStr) + vbNewLine + backupSql.ToString()

			Helpers.QueryExecutionHelper.RunSqlNonQuery(combinedSql, forSetup, New SqlClient.SqlConnection(connStr), _LOG_DIRECTORY_NAME)

			'Note: write logs
			Helpers.FileHelper.SaveTextToFile(messagesToWrite.ToString(), _LOG_DIRECTORY_NAME)

			Return True
		End Function

		Public Shared Function ExtractDBName(ByVal filePath As String) As String
			Dim databaseName As String = filePath.Substring(0, filePath.Length - 4).ToUpper
			Return databaseName
		End Function

		''' <summary>
		''' Restores backup database files and verifies that the restore succeeded
		''' </summary>
		''' <remarks></remarks>
		Public Function RestoreDatabases() As Boolean
			Dim forSetup As Boolean = True
			Dim messagesToWrite As New StringBuilder()
			Dim successful As Boolean
			Dim files As String() = System.IO.Directory.GetFiles(GetDBBackupLocation(), "*.bak")
			For Each file As String In files
				successful = False

				Dim filePath As New System.IO.FileInfo(file)
				Dim extractedDBName As String = ExtractDBName(filePath.Name)
				messagesToWrite.AppendLine("============================================================================================" + vbCrLf)
				messagesToWrite.AppendLine("                    BEGIN RESTORE OF DATABASE [" + extractedDBName + "]" + vbCrLf)
				messagesToWrite.AppendLine("============================================================================================" + vbCrLf)

				RestoreSingleDatabase(extractedDBName, file)

				messagesToWrite.AppendLine("============================================================================================" + vbCrLf)
				messagesToWrite.AppendLine("                    END OF  [" + extractedDBName + "] DATABASE RESTORE" + vbCrLf)
				messagesToWrite.AppendLine("============================================================================================" + vbCrLf)
				successful = True
			Next
			'Note: write logs
			Helpers.FileHelper.SaveTextToFile(messagesToWrite.ToString(), _LOG_DIRECTORY_NAME)
			Return successful
		End Function

		Private Function SqlForKillAllOpenConnectionsToDatabase(ByVal dbName As String, ByVal connStringForSetup As String) As String
			Dim sql As New StringBuilder()
			sql.AppendLine("DECLARE @DatabaseName nvarchar(50)")
			sql.AppendLine("DECLARE @SPId int")
			sql.AppendLine(String.Format("SET @DatabaseName = N'{0}'", dbName))
			sql.AppendLine("DECLARE my_cursor CURSOR FAST_FORWARD FOR")
			sql.AppendLine("SELECT SPId FROM MASTER..SysProcesses ")
			sql.AppendLine("WHERE DBId = DB_ID(@DatabaseName) AND SPId > 50 AND SPId <> @@SPId")
			sql.AppendLine("OPEN my_cursor")
			sql.AppendLine("FETCH NEXT FROM my_cursor INTO @SPId")
			sql.AppendLine("WHILE @@FETCH_STATUS = 0")
			sql.AppendLine("BEGIN")
			sql.AppendLine("    DECLARE @killCommand CHAR(8)")
			sql.AppendLine("    SELECT @killCommand = 'KILL ' + convert(char(8) ,@SPId)")
			sql.AppendLine("    EXEC (@killCommand)")
			sql.AppendLine("    FETCH NEXT FROM my_cursor INTO @SPId")
			sql.AppendLine("End")
			sql.AppendLine("CLOSE my_cursor")
			sql.AppendLine("DEALLOCATE my_cursor")

			Return sql.ToString()
		End Function

		Private Sub KillAllOpenConnectionsToDatabase(ByVal dbName As String, ByVal connStringForSetup As String)
			Dim sql As New StringBuilder()
			sql.AppendLine("DECLARE @DatabaseName nvarchar(50)")
			sql.AppendLine("DECLARE @SPId int")
			sql.AppendLine(String.Format("SET @DatabaseName = N'{0}'", dbName))
			sql.AppendLine("DECLARE my_cursor CURSOR FAST_FORWARD FOR")
			sql.AppendLine("SELECT SPId FROM MASTER..SysProcesses WHERE DBId = DB_ID(@DatabaseName) AND SPId >= 50 AND SPId <> @@SPId")
			sql.AppendLine("OPEN my_cursor")
			sql.AppendLine("FETCH NEXT FROM my_cursor INTO @SPId")
			sql.AppendLine("WHILE @@FETCH_STATUS = 0")
			sql.AppendLine("BEGIN")
			sql.AppendLine("    DECLARE @killCommand CHAR(8)")
			sql.AppendLine("    SELECT @killCommand = 'KILL ' + convert(char(8) ,@SPId)")
			sql.AppendLine("    EXEC (@killCommand)")
			sql.AppendLine("    FETCH NEXT FROM my_cursor INTO @SPId")
			sql.AppendLine("End")
			sql.AppendLine("CLOSE my_cursor")
			sql.AppendLine("DEALLOCATE my_cursor")

			Dim forSetup As Boolean = True
			Helpers.QueryExecutionHelper.RunSqlNonQuery(sql.ToString(), forSetup, New SqlClient.SqlConnection(connStringForSetup), _LOG_DIRECTORY_NAME)
		End Sub

		Public Function SqlForSetSingleUserForDatabase(ByVal dbName As String, ByVal connStringForSetup As String) As String
			Dim setSingleUserSql As New StringBuilder()
			setSingleUserSql.AppendLine(String.Format("ALTER DATABASE [{0}]", dbName))
			setSingleUserSql.AppendLine(String.Format(" SET SINGLE_USER WITH ROLLBACK IMMEDIATE"))
			Return setSingleUserSql.ToString()
		End Function

		Public Sub SetSingleUserForDatabase(ByVal dbName As String, ByVal connStringForSetup As String)
			Dim setSingleUserSql As New StringBuilder()
			setSingleUserSql.AppendLine(String.Format("ALTER DATABASE [{0}]", dbName))
			setSingleUserSql.AppendLine(String.Format(" SET SINGLE_USER WITH ROLLBACK IMMEDIATE"))
			Dim forSetup As Boolean = True
			Helpers.QueryExecutionHelper.RunSqlNonQuery(setSingleUserSql.ToString(), forSetup, New SqlClient.SqlConnection(connStringForSetup), _LOG_DIRECTORY_NAME)
		End Sub

		Public Function SqlForSetMultiUserForDatabase(ByVal dbName As String, ByVal connStringForSetup As String) As String
			Dim setMultiUserSql As New StringBuilder()
			setMultiUserSql.AppendLine(String.Format("ALTER DATABASE [{0}]", dbName))
			setMultiUserSql.AppendLine(String.Format(" SET MULTI_USER WITH NO_WAIT"))
			Return setMultiUserSql.ToString()
		End Function

		Public Sub SetMultiUserForDatabase(ByVal dbName As String, ByVal connStringForSetup As String)
			Dim setMultiUserSql As New StringBuilder()
			setMultiUserSql.AppendLine(String.Format("ALTER DATABASE [{0}]", dbName))
			setMultiUserSql.AppendLine(String.Format(" SET MULTI_USER WITH NO_WAIT"))
			Dim forSetup As Boolean = True
			Helpers.QueryExecutionHelper.RunSqlNonQuery(setMultiUserSql.ToString(), forSetup, New SqlClient.SqlConnection(connStringForSetup), _LOG_DIRECTORY_NAME)
		End Sub

		Private Sub RestoreSingleDatabase(ByVal dbName As String, ByVal dbFileName As String)
			Dim restoreSql As New StringBuilder()
			restoreSql.AppendLine(String.Format("RESTORE DATABASE [{0}] ", dbName))
			restoreSql.AppendLine(String.Format(" FROM DISK = N'{0}' WITH FILE = 1, RECOVERY,", dbFileName))
			restoreSql.AppendLine(String.Format(" MOVE N'{0}' TO N'{1}', ", _
			 dbName, Path.Combine(MDFPath, dbName) + ".IntegratedTests.mdf"))
			restoreSql.AppendLine(String.Format(" MOVE N'{0}' TO N'{1}', ", _
			 dbName + "_log", Path.Combine(LDFPath, dbName) + ".IntegratedTests.ldf"))
			restoreSql.AppendLine(String.Format(" NOUNLOAD, REPLACE, STATS = 1"))

			Dim forSetup As Boolean = True
			Dim connStr As String = GetConnectionString(forSetup)

			Dim combinedSql As String = SqlForKillAllOpenConnectionsToDatabase(dbName, connStr) + vbNewLine + restoreSql.ToString()

			Helpers.QueryExecutionHelper.RunSqlNonQuery(combinedSql, forSetup, New SqlClient.SqlConnection(GetConnectionString(forSetup)), _LOG_DIRECTORY_NAME)
		End Sub

		''' <summary>
		''' Restores backup database files and verifies that the restore succeeded
		''' </summary>
		''' <remarks></remarks>
		Public Function RestoreDatabases(ByVal restoreFromTempBackup As Boolean) As Boolean
			Dim messagesToWrite As New StringBuilder()
			Dim successful As Boolean
			Dim forSetup As Boolean = True

			If Not restoreFromTempBackup Then
				Return RestoreDatabases()
			Else
				For i As Int32 = 0 To _DBLIST_FOR_TEMP_BACKUP.Count - 1
					successful = False

					Dim filePath As New System.IO.FileInfo(Path.Combine(GetTempDBBackupLocation(), _DBLIST_FOR_TEMP_BACKUP(i) + ".bak"))
					Dim extractedDBName As String = _DBLIST(i)
					messagesToWrite.AppendLine("============================================================================================" + vbCrLf)
					messagesToWrite.AppendLine("                    BEGIN RESTORE OF DATABASE [" + extractedDBName + "]" + vbCrLf)
					messagesToWrite.AppendLine("============================================================================================" + vbCrLf)

					RestoreSingleDatabase(extractedDBName, filePath.FullName)

					messagesToWrite.AppendLine("============================================================================================" + vbCrLf)
					messagesToWrite.AppendLine("                    END OF  [" + extractedDBName + "] DATABASE RESTORE" + vbCrLf)
					messagesToWrite.AppendLine("============================================================================================" + vbCrLf)
					successful = True
				Next
				FixUserMappings()

				'Note: write logs
				Helpers.FileHelper.SaveTextToFile(messagesToWrite.ToString(), _LOG_DIRECTORY_NAME)
				Return successful
			End If
		End Function

		''' <summary>
		''' Event handler that grabs all the messages from the SQL message window
		''' </summary>
		''' <param name="sender"></param>
		''' <param name="e"></param>
		''' <remarks></remarks>
		Private Sub LogSqlInfoMessages(ByVal sender As Object, ByVal e As System.Data.SqlClient.SqlInfoMessageEventArgs) Handles conn.InfoMessage
			Dim messages As New StringBuilder()
			For Each err As System.Data.SqlClient.SqlError In e.Errors
				messages.AppendLine(err.Message)
				If err.Message.ToLower().Contains("database restore successfully processed") Then
					_restoreSucceeded = True
				End If
			Next
			Helpers.FileHelper.SaveTextToFile(messages.ToString(), _LOG_DIRECTORY_NAME)
		End Sub

		Public Sub FixUserMappings()
			Dim fixMappingSqlTemplate As String = "USE {0} EXEC sp_change_users_login 'update_one', 'eddsdbo', 'eddsdbo'"

			For Each db As String In _DBLIST
				Dim forSetup As Boolean = True
				Helpers.QueryExecutionHelper.RunSqlNonQuery(String.Format(fixMappingSqlTemplate, db), forSetup, New SqlClient.SqlConnection(GetConnectionString(forSetup)), _LOG_DIRECTORY_NAME)
			Next
		End Sub

		Public Sub BuildProcuo()
			Dim compiler As String = ConfigurationManager.AppSettings("compilerLocation").ToString()
			Dim NUnitDirectory As String = ParentDirectoryName
			Dim EDDSDirectory As String = Directory.GetParent(NUnitDirectory).FullName
			Dim procuroSolution As String = Path.Combine(EDDSDirectory, ConfigurationManager.AppSettings("procuroSolutionLocation").ToString())

			Dim runProcess As New System.Diagnostics.Process
			runProcess = New System.Diagnostics.Process
			runProcess.StartInfo.FileName = compiler
			runProcess.StartInfo.Arguments = """" + procuroSolution + """" + " -rebuild"
			runProcess.Start()
			runProcess.WaitForExit()
		End Sub

		Public Function RunProcuro() As Boolean
			Dim NUnitDirectory As String = ParentDirectoryName
			Dim EDDSDirectory As String = Directory.GetParent(NUnitDirectory).FullName
			Dim procuroEXEPath As String = Path.Combine(EDDSDirectory, ConfigurationManager.AppSettings("procuroBuildLocation").ToString())

			Dim procuroProcess As New System.Diagnostics.Process
			procuroProcess.StartInfo.FileName = procuroEXEPath
			procuroProcess.StartInfo.Arguments = String.Format("-nogui -x:{0} -user:{1} -pass:{2}", _
			 ConfigurationManager.AppSettings("procuroResultsLocation").ToString(), _
			 "relativity.admin@kcura.com", _
			 "Test1234!" _
			 )
			procuroProcess.Start()
			procuroProcess.WaitForExit()
			Return VerifyProcuroResults()
		End Function

		Private Function VerifyProcuroResults() As Boolean
			Dim resultsFileLocation As String = ConfigurationManager.AppSettings("procuroResultsLocation").ToString()

			Dim allText() As String = File.ReadAllLines(resultsFileLocation)
			Console.WriteLine(allText.ToString())

			Dim upgradeResultsElement As XElement = XElement.Load(resultsFileLocation).Element("UpgradeResults")
			Dim resultsGood As String = upgradeResultsElement.Attribute("success").Value
			If resultsGood.ToLower().Equals("true") Then
				Return True
			Else
				Return False
			End If
		End Function

		Public Sub SwitchProcuroConnectionStringToTest()
			Dim NUnitDirectory As String = ParentDirectoryName
			Dim EDDSDirectory As String = Directory.GetParent(NUnitDirectory).FullName
			Dim pathToProcuro As String = Path.Combine(EDDSDirectory, "kCura.EDDS.Procuro")

			Dim overwrite As Boolean = True
			File.Copy(Path.Combine(pathToProcuro, "App.IntegratedTests.Config"), Path.Combine(pathToProcuro, "App.config"), overwrite)

		End Sub

		Public Sub SwitchProcuroConnectionStringToDev()
			Dim NUnitDirectory As String = ParentDirectoryName
			Dim EDDSDirectory As String = Directory.GetParent(NUnitDirectory).FullName
			Dim pathToProcuro As String = Path.Combine(EDDSDirectory, "kCura.EDDS.Procuro")

			Dim overwrite As Boolean = True
			File.Copy(Path.Combine(pathToProcuro, "App.Dev.config"), Path.Combine(pathToProcuro, "App.config"), overwrite)

		End Sub

		Public Sub SwitchRelativityServicesConnectionStringToTest()
			Dim NUnitDirectory As String = ParentDirectoryName
			Dim EDDSDirectory As String = Directory.GetParent(NUnitDirectory).FullName
			Dim pathToRelativityServices As String = Path.Combine(EDDSDirectory, "kCura.Relativity")

			Dim overwrite As Boolean = True
			File.Copy(Path.Combine(pathToRelativityServices, "Web.IntegratedTests.config"), Path.Combine(pathToRelativityServices, "Web.config"), overwrite)

		End Sub

		Public Sub SwitchRelativityServicesConnectionStringToDev()
			Dim NUnitDirectory As String = ParentDirectoryName
			Dim EDDSDirectory As String = Directory.GetParent(NUnitDirectory).FullName
			Dim pathToRelativityServices As String = Path.Combine(EDDSDirectory, "kCura.Relativity")

			Dim overwrite As Boolean = True
			File.Copy(Path.Combine(pathToRelativityServices, "Web.Dev.config"), Path.Combine(pathToRelativityServices, "Web.config"), overwrite)

		End Sub

		Public Sub CreateDefaultDirectories()
			Dim dirs As New List(Of String)()
			dirs.Add(ConfigurationManager.AppSettings("defaultLDFDirectory").ToString())
			dirs.Add(ConfigurationManager.AppSettings("defaultMDFDirectory").ToString())
			dirs.Add(ConfigurationManager.AppSettings("defaultFileRepositoryLocation").ToString())
			dirs.Add(ConfigurationManager.AppSettings("defaultDTSearchIndexDirectory").ToString())
			dirs.Add(ConfigurationManager.AppSettings("defaultCAIndexDirectory").ToString())

			For Each d In dirs
				If Not Directory.Exists(d) Then
					Directory.CreateDirectory(d)
				End If
			Next

		End Sub

	End Class
End Namespace

