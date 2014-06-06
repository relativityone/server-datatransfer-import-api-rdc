Imports kCura.EDDS.WebAPI.BulkImportManagerBase
Imports NUnit.Framework
Imports kCura.Windows.Process
Imports kCura.WinEDDS.LoadFileFieldMap

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

#Region " GetMappedFields "
		<Test()> Public Sub GetMappedFields_AllObjectFieldsSelected_AllFieldsObjectFieldContainsArtifactId()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _controller, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True))

			Dim FieldMap As New LoadFileFieldMap
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field1", 1, Relativity.FieldTypeHelper.FieldType.Object, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field2", 2, Relativity.FieldTypeHelper.FieldType.Objects, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			bulkImporter.FieldMap = FieldMap

			Dim fields As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo() =
			 bulkImporter.GetMappedFields(1, New System.Collections.Generic.List(Of Integer)(New Integer() {1, 2}))

			Assert.AreEqual(ImportBehaviorChoice.ObjectFieldContainsArtifactId, fields(0).ImportBehavior)
			Assert.AreEqual(ImportBehaviorChoice.ObjectFieldContainsArtifactId, fields(1).ImportBehavior)
		End Sub

		<Test()> Public Sub GetMappedFields_NoObjectFieldsSelected_NoFieldsObjectFieldContainsArtifactId()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _controller, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True))

			Dim FieldMap As New LoadFileFieldMap
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field1", 1, Relativity.FieldTypeHelper.FieldType.Object, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field2", 2, Relativity.FieldTypeHelper.FieldType.Objects, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			bulkImporter.FieldMap = FieldMap

			Dim fields As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo() =
			 bulkImporter.GetMappedFields(1, New System.Collections.Generic.List(Of Integer)(New Integer() {11, 22}))

			Assert.AreEqual(Nothing, fields(0).ImportBehavior)
			Assert.AreEqual(Nothing, fields(1).ImportBehavior)
		End Sub

		<Test()> Public Sub GetMappedFields_NoSelection_NoFieldsObjectFieldContainsArtifactId()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _controller, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True))

			Dim FieldMap As New LoadFileFieldMap
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field1", 1, Relativity.FieldTypeHelper.FieldType.Object, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field2", 2, Relativity.FieldTypeHelper.FieldType.Objects, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			bulkImporter.FieldMap = FieldMap

			Dim fields As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo() =
			 bulkImporter.GetMappedFields(1, New System.Collections.Generic.List(Of Integer)())

			Assert.AreEqual(Nothing, fields(0).ImportBehavior)
			Assert.AreEqual(Nothing, fields(1).ImportBehavior)
		End Sub

		<Test()> Public Sub GetMappedFields_OneObjectFieldSelected_OneFieldsObjectFieldContainsArtifactId()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _controller, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True))

			Dim FieldMap As New LoadFileFieldMap
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field1", 1, Relativity.FieldTypeHelper.FieldType.Object, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field2", 2, Relativity.FieldTypeHelper.FieldType.Objects, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			bulkImporter.FieldMap = FieldMap

			Dim fields As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo() =
			 bulkImporter.GetMappedFields(1, New System.Collections.Generic.List(Of Integer)(New Integer() {1, 22}))

			Assert.AreEqual(ImportBehaviorChoice.ObjectFieldContainsArtifactId, fields(0).ImportBehavior)
			Assert.AreEqual(Nothing, fields(1).ImportBehavior)
		End Sub

		<Test()> Public Sub GetMappedFields_NoneObjectFieldsSelected_NoFieldsObjectFieldContainsArtifactId()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _controller, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True))

			Dim FieldMap As New LoadFileFieldMap
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field01", 1, Relativity.FieldTypeHelper.FieldType.Object, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field02", 2, Relativity.FieldTypeHelper.FieldType.Objects, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field03", 3, Relativity.FieldTypeHelper.FieldType.Boolean, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field04", 4, Relativity.FieldTypeHelper.FieldType.Code, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field05", 5, Relativity.FieldTypeHelper.FieldType.Currency, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field06", 6, Relativity.FieldTypeHelper.FieldType.Date, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field07", 7, Relativity.FieldTypeHelper.FieldType.Decimal, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field08", 8, Relativity.FieldTypeHelper.FieldType.Empty, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field09", 9, Relativity.FieldTypeHelper.FieldType.File, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field10", 10, Relativity.FieldTypeHelper.FieldType.Integer, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field11", 11, Relativity.FieldTypeHelper.FieldType.LayoutText, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field12", 12, Relativity.FieldTypeHelper.FieldType.MultiCode, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field13", 13, Relativity.FieldTypeHelper.FieldType.Text, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field14", 14, Relativity.FieldTypeHelper.FieldType.User, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field15", 15, Relativity.FieldTypeHelper.FieldType.Varchar, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			bulkImporter.FieldMap = FieldMap

			Dim fields As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo() =
			 bulkImporter.GetMappedFields(1, New System.Collections.Generic.List(Of Integer)(New Integer() {3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15}))

			Assert.AreEqual(Nothing, fields(0).ImportBehavior)
			Assert.AreEqual(Nothing, fields(1).ImportBehavior)
			Assert.AreEqual(Nothing, fields(2).ImportBehavior)
			Assert.AreEqual(Nothing, fields(3).ImportBehavior)
			Assert.AreEqual(Nothing, fields(4).ImportBehavior)
			Assert.AreEqual(Nothing, fields(5).ImportBehavior)
			Assert.AreEqual(Nothing, fields(6).ImportBehavior)
			Assert.AreEqual(Nothing, fields(7).ImportBehavior)
			Assert.AreEqual(Nothing, fields(8).ImportBehavior)
			Assert.AreEqual(Nothing, fields(9).ImportBehavior)
			Assert.AreEqual(Nothing, fields(10).ImportBehavior)
			Assert.AreEqual(Nothing, fields(11).ImportBehavior)
			Assert.AreEqual(Nothing, fields(12).ImportBehavior)
			Assert.AreEqual(Nothing, fields(13).ImportBehavior)
			Assert.AreEqual(Nothing, fields(14).ImportBehavior)
		End Sub

		<Test()> Public Sub GetMappedFields_TwoObjectFieldsSelected_TwoFieldsObjectFieldContainsArtifactId()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _controller, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True))

			Dim FieldMap As New LoadFileFieldMap
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field01", 1, Relativity.FieldTypeHelper.FieldType.Object, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field02", 2, Relativity.FieldTypeHelper.FieldType.Objects, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field03", 3, Relativity.FieldTypeHelper.FieldType.Boolean, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field04", 4, Relativity.FieldTypeHelper.FieldType.Code, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field05", 5, Relativity.FieldTypeHelper.FieldType.Currency, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field06", 6, Relativity.FieldTypeHelper.FieldType.Date, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field07", 7, Relativity.FieldTypeHelper.FieldType.Decimal, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field08", 8, Relativity.FieldTypeHelper.FieldType.Empty, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field09", 9, Relativity.FieldTypeHelper.FieldType.File, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field10", 10, Relativity.FieldTypeHelper.FieldType.Integer, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field11", 11, Relativity.FieldTypeHelper.FieldType.LayoutText, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field12", 12, Relativity.FieldTypeHelper.FieldType.MultiCode, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field13", 13, Relativity.FieldTypeHelper.FieldType.Text, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field14", 14, Relativity.FieldTypeHelper.FieldType.User, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field15", 15, Relativity.FieldTypeHelper.FieldType.Varchar, 1, 0, 0, 0, True, Nothing, EDDS.WebAPI.DocumentManagerBase.StorageLocationChoice.SQL), 0))
			bulkImporter.FieldMap = FieldMap

			Dim fields As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo() =
			 bulkImporter.GetMappedFields(1, New System.Collections.Generic.List(Of Integer)(New Integer() {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15}))

			Assert.AreEqual(ImportBehaviorChoice.ObjectFieldContainsArtifactId, fields(0).ImportBehavior)
			Assert.AreEqual(ImportBehaviorChoice.ObjectFieldContainsArtifactId, fields(1).ImportBehavior)
			Assert.AreEqual(Nothing, fields(2).ImportBehavior)
			Assert.AreEqual(Nothing, fields(3).ImportBehavior)
			Assert.AreEqual(Nothing, fields(4).ImportBehavior)
			Assert.AreEqual(Nothing, fields(5).ImportBehavior)
			Assert.AreEqual(Nothing, fields(6).ImportBehavior)
			Assert.AreEqual(Nothing, fields(7).ImportBehavior)
			Assert.AreEqual(Nothing, fields(8).ImportBehavior)
			Assert.AreEqual(Nothing, fields(9).ImportBehavior)
			Assert.AreEqual(Nothing, fields(10).ImportBehavior)
			Assert.AreEqual(Nothing, fields(11).ImportBehavior)
			Assert.AreEqual(Nothing, fields(12).ImportBehavior)
			Assert.AreEqual(Nothing, fields(13).ImportBehavior)
			Assert.AreEqual(Nothing, fields(14).ImportBehavior)
		End Sub
#End Region

#Region " GetMassImportOverlayType "

		<Test>
		<TestCaseSource("OverlayTypeSource")>
		Public Sub GetMassImportOverlayBehavior(ByVal inputOverlayType As LoadFile.FieldOverlayBehavior, expectedOverlayType As kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior)
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _controller, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True))
			Assert.AreEqual(bulkImporter.ConvertOverlayBehaviorEnum(inputOverlayType), expectedOverlayType)
		End Sub

		Public Shared Function OverlayTypeSource() As TestCaseData()
			Dim retval(3) As TestCaseData
			retval(0) = New TestCaseData(LoadFile.FieldOverlayBehavior.UseRelativityDefaults, kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior.UseRelativityDefaults)
			retval(1) = New TestCaseData(LoadFile.FieldOverlayBehavior.ReplaceAll, kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior.ReplaceAll)
			retval(2) = New TestCaseData(LoadFile.FieldOverlayBehavior.MergeAll, kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior.MergeAll)
			retval(3) = New TestCaseData(Nothing, kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior.UseRelativityDefaults)
			Return retval
		End Function

#End Region

#Region " CleanDestinationFolderPath "

		<Test>
		<TestCase("\", "")>
		<TestCase("\", "\\")>
		<TestCase("\ႝ", "\ႝ\")>
		<TestCase("\aaa\bbb\cc", "aaa\\bbb\\cc")>
		<TestCase("\aaa\bbb\cc", "aaa\\\\\\\\\\bbb\\cc")>
		<TestCase("\SourceCode\Mainline\EDDS\kCura.WinEDDS\Importers", ".\SourceCode\Mainline\EDDS\kCura.WinEDDS\Importers")>
		Public Sub CleanDestinationFolderPath(ByVal expected As String, ByVal input As String)
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _controller, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerSqlExceptions(True))
			Dim actual As String = bulkImporter.CleanFolderPath(input)
			Assert.AreEqual(expected, actual)
		End Sub

#End Region

	End Class
End Namespace