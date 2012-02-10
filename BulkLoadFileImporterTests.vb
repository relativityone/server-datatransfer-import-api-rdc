Imports kCura.EDDS.WebAPI.BulkImportManagerBase
Imports NUnit.Framework
Imports kCura.Windows.Process

Namespace kCura.WinEDDS.NUnit
	<TestFixture()> Public Class BulkLoadFileImporterTests

#Region " Members "
		Dim _args As LoadFile
		Dim _guid As System.Guid
		Dim _controller As Controller
		Dim _keyPathExistsAlready As Boolean
		Dim _keyValExistsAlready As Boolean
#End Region

#Region " Setup "

		<SetUp()> Public Sub SetUp()
			_args = New LoadFile()
			_args.CaseInfo = New Relativity.CaseInfo()
			_args.CaseInfo.RootArtifactID = 1
			_guid = New Guid("E09E18F3-D0C8-4CFC-96D1-FBB350FAB3E1")
			_controller = New Controller()

			_keyPathExistsAlready = RegKeyHelper.SubKeyPathExists(RegKeyHelper.RelativityKeyPath)
			_keyValExistsAlready = False
			If _keyPathExistsAlready = True Then
				_keyValExistsAlready = RegKeyHelper.SubKeyExists(RegKeyHelper.RelativityKeyPath, RegKeyHelper.RelativityServiceURLKey)
			End If

			If _keyValExistsAlready = False Then
				RegKeyHelper.CreateKeyWithValueOnPath(Not _keyPathExistsAlready, RegKeyHelper.RelativityKeyPath, RegKeyHelper.RelativityServiceURLKey, RegKeyHelper.RelativityDefaultServiceURL)
			End If
		End Sub

		<TearDown()> Public Sub TakeDown()
			_args = Nothing
			_guid = Nothing
			_controller = Nothing

			If _keyValExistsAlready = False Then
				RegKeyHelper.RemoveKeyPath(RegKeyHelper.RelativityKeyPath)
			End If
		End Sub
#End Region

#Region " Basic Retry + Batch Size  Tests"

		<Test(), Ignore()> Public Sub BulkImportLoadFile_CatchWebException_Retries_CallsLowerBatchSize_True()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _controller, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True))
			Try
				bulkImporter.TryBulkImport(New ObjectLoadInfo())
			Catch ex As Exception
				Assert.AreEqual(GetType(System.Net.WebException), ex.GetType)
			End Try
			Assert.AreEqual(500 - 200, bulkImporter.BatchSize)
			Assert.AreEqual(2, bulkImporter.PauseCalled)
		End Sub

		<Test()> Public Sub BulkImportLoadFile_NoException_NoRetries_CallsLowerBatchSize_False()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _controller, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(False))
			bulkImporter.TryBulkImport(New ObjectLoadInfo())
			Assert.AreEqual(500, bulkImporter.BatchSize)
			Assert.AreEqual(0, bulkImporter.PauseCalled)
		End Sub

		<Test()> Public Sub BulkImportLoadFile_CatchSystemException_Retries_CallsLowerBatchSize_False()
			Dim manager As New MockBulkImportManagerWebExceptions(True)
			manager.ErrorMessage = New System.Exception("bombed out")
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _controller, 0, False, False, _guid, True, "S", True, manager)
			Try
				bulkImporter.TryBulkImport(New ObjectLoadInfo())
			Catch ex As Exception
				Assert.AreEqual(GetType(System.Exception), ex.GetType)
				Assert.AreEqual("bombed out", ex.Message)
			End Try
			Assert.AreEqual(500, bulkImporter.BatchSize)
			Assert.AreEqual(2, bulkImporter.PauseCalled)
		End Sub


		<Test(), Ignore()> Public Sub BulkImportLoadFile_CatchSQLException_Retries_CallsLowerBatchSize_True()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _controller, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerSqlExceptions(True))
			Try
				bulkImporter.TryBulkImport(New ObjectLoadInfo())
			Catch ex As Exception
				Assert.AreEqual(GetType(kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlTimeoutException), ex.GetType)
			End Try
			Assert.AreEqual(500 - 200, bulkImporter.BatchSize)
			Assert.AreEqual(2, bulkImporter.PauseCalled)
		End Sub

#End Region

#Region " Retry-Until-It-Works Tests"
		<Test(), Ignore()> Public Sub BulkImportLoadFile_CatchSQLException_500RetryTo400_ThenWork()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _controller, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerSqlExceptions(400))
			bulkImporter.TryBulkImport(New ObjectLoadInfo())

			Assert.AreEqual(400, bulkImporter.BatchSize)
			Assert.AreEqual(1, bulkImporter.PauseCalled)
			CollectionAssert.AreEquivalent({500, 400}, bulkImporter.BatchSizeHistoryList)
		End Sub

		<Test(), Ignore()> Public Sub BulkImportLoadFile_CatchSQLException_500RetryToMinimum_ThenWork()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _controller, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerSqlExceptions(300))
			bulkImporter.MinimumBatch = 300
			bulkImporter.TryBulkImport(New ObjectLoadInfo())
			Assert.AreEqual(300, bulkImporter.BatchSize)
			Assert.AreEqual(2, bulkImporter.PauseCalled)
			CollectionAssert.AreEquivalent({500, 400, 300}, bulkImporter.BatchSizeHistoryList)
		End Sub

		<Test(), Ignore()> Public Sub BulkImportLoadFile_CatchSQLException_500Retry_HitMinimum_ThrowException()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _controller, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerSqlExceptions(200))
			bulkImporter.MinimumBatch = 350
			Try
				bulkImporter.TryBulkImport(New ObjectLoadInfo())
			Catch ex As Exception
				Assert.AreEqual(GetType(kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlTimeoutException), ex.GetType)
			End Try
			Assert.AreEqual(350, bulkImporter.BatchSize)
			Assert.AreEqual(2, bulkImporter.PauseCalled)
			CollectionAssert.AreEquivalent({500, 400, 350}, bulkImporter.BatchSizeHistoryList)
		End Sub

#End Region

#Region " Batch Size Adjustment "
		<Test()> Public Sub BulkImportLoadFile_Lower_500BatchSize_to300()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _controller, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True))
			bulkImporter.BatchSize = 300
			Assert.AreEqual(300, bulkImporter.BatchSize)
		End Sub
		<Test()> Public Sub BulkImportLoadFile_Lower_500BatchSize_ToMinimum300()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _controller, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True))
			bulkImporter.MinimumBatch = 300
			bulkImporter.BatchSize = 300
			Assert.AreEqual(300, bulkImporter.BatchSize)
		End Sub

		<Test()> Public Sub BulkImportLoadFile_Lower_500BatchSize_To200_PastMinimum300()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _controller, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True))
			bulkImporter.MinimumBatch = 300
			bulkImporter.BatchSize = 200
			Assert.AreEqual(300, bulkImporter.BatchSize)
		End Sub

#End Region

	End Class
End NameSpace