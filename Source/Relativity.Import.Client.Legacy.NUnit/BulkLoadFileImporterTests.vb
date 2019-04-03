' -----------------------------------------------------------------------------------------------------
' <copyright file="BulkLoadFileImporterTests.cs" company="Relativity ODA LLC">
'   © Relativity All Rights Reserved.
' </copyright>
' -----------------------------------------------------------------------------------------------------

Imports System.Threading

Imports kCura.EDDS.WebAPI.BulkImportManagerBase
Imports kCura.WinEDDS
Imports kCura.WinEDDS.LoadFileFieldMap
Imports Moq

Imports NUnit.Framework
Imports Relativity.Import.Export
Imports Relativity.Import.Export.Io
Imports Relativity.Import.Export.Process

Imports Relativity.Logging

Namespace Relativity.Import.Client.NUnit

	<TestFixture>
	Public Class BulkLoadFileImporterTests

		Dim _args As LoadFile
		Dim _guid As System.Guid
		Dim _context As ProcessContext
		Dim _ioReporter As IIoReporter
		Dim _logger As Logging.ILog
		Dim _keyPathExistsAlready As Boolean
		Dim _keyValExistsAlready As Boolean
		Dim tokenSource As CancellationTokenSource

		<SetUp>
		Public Sub SetUp()
			kCura.WinEDDS.Config.ProgrammaticServiceURL = "https://r1.kcura.com/RelativityWebAPI/"
			_args = New LoadFile()
			_args.CaseInfo = New Relativity.Import.Export.Services.CaseInfo()
			_args.CaseInfo.RootArtifactID = 1
			_guid = New Guid("E09E18F3-D0C8-4CFC-96D1-FBB350FAB3E1")
			Dim mockProcessEventWriter = New Mock(Of IProcessEventWriter)()
			Dim mockProcessErrorWriter = New Mock(Of IProcessErrorWriter)()
			Dim mockAppSettings = New Mock(Of IAppSettings)()
			_logger = New NullLogger()
			_context = New ProcessContext(mockProcessEventWriter.Object, mockProcessErrorWriter.Object, mockAppSettings.Object, _logger)
			tokenSource = New CancellationTokenSource()
			Dim ioReporterContext As New IoReporterContext()
			_ioReporter = New IoReporter(ioReporterContext, _logger, tokenSource.Token)
			_keyPathExistsAlready = RegKeyHelper.SubKeyPathExists(RegKeyHelper.RelativityKeyPath)
			_keyValExistsAlready = False
			If _keyPathExistsAlready = True Then
				_keyValExistsAlready = RegKeyHelper.SubKeyExists(RegKeyHelper.RelativityKeyPath, RegKeyHelper.RelativityServiceURLKey)
			End If

			If _keyValExistsAlready = False Then
				RegKeyHelper.CreateKeyWithValueOnPath(Not _keyPathExistsAlready, RegKeyHelper.RelativityKeyPath, RegKeyHelper.RelativityServiceURLKey, RegKeyHelper.RelativityDefaultServiceURL)
			End If
		End Sub

		<TearDown>
		Public Sub TakeDown()
			_args = Nothing
			_guid = Nothing
			_context = Nothing

			If _keyValExistsAlready = False Then
				RegKeyHelper.RemoveKeyPath(RegKeyHelper.RelativityKeyPath)
			End If
		End Sub

		<Test>
		Public Sub BulkImportLoadFile_NoException_NoRetries_CallsLowerBatchSize_False()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _context, _ioReporter, _logger, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(False), tokenSource)
			bulkImporter.TryBulkImport(New kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo())
			Assert.AreEqual(500, bulkImporter.BatchSize)
			Assert.AreEqual(0, bulkImporter.PauseCalled)
		End Sub

		<Test>
		Public Sub BulkImportLoadFile_CatchSystemException_Retries_CallsLowerBatchSize_False()
			Dim manager As New MockBulkImportManagerWebExceptions(True)
			manager.ErrorMessage = New System.Exception("bombed out")
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _context, _ioReporter, _logger, 0, False, False, _guid, True, "S", True, manager, tokenSource)
			Try
				bulkImporter.TryBulkImport(New kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo())
			Catch ex As Exception
				Assert.AreEqual(GetType(System.Exception), ex.GetType)
				Assert.AreEqual("bombed out", ex.Message)
			End Try
			Assert.AreEqual(500, bulkImporter.BatchSize)
			Assert.AreEqual(2, bulkImporter.PauseCalled)
		End Sub

		<Test>
		Public Sub BulkImportLoadFile_Lower_500BatchSize_to300()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _context, _ioReporter, _logger, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True), tokenSource)
			bulkImporter.BatchSize = 300
			Assert.AreEqual(300, bulkImporter.BatchSize)
		End Sub

		<Test>
		Public Sub BulkImportLoadFile_Lower_500BatchSize_ToMinimum300()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _context, _ioReporter, _logger, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True), tokenSource)
			bulkImporter.MinimumBatch = 300
			bulkImporter.BatchSize = 300
			Assert.AreEqual(300, bulkImporter.BatchSize)
		End Sub

		<Test>
		Public Sub BulkImportLoadFile_Lower_500BatchSize_To200_PastMinimum300()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _context, _ioReporter, _logger, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True), tokenSource)
			bulkImporter.MinimumBatch = 300
			bulkImporter.BatchSize = 200
			Assert.AreEqual(300, bulkImporter.BatchSize)
		End Sub

		<Test>
		Public Sub GetMappedFields_AllObjectFieldsSelected_AllFieldsObjectFieldContainsArtifactId()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _context, _ioReporter, _logger, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True), tokenSource)

			Dim FieldMap As New LoadFileFieldMap
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field1", 1, FieldTypeHelper.FieldType.Object, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field2", 2, FieldTypeHelper.FieldType.Objects, 1, 0, 0, 0, True, Nothing, False), 0))
			bulkImporter.FieldMap = FieldMap

			Dim fields As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo() =
			 bulkImporter.GetMappedFields(1, New System.Collections.Generic.List(Of Integer)(New Integer() {1, 2}))

			Assert.AreEqual(ImportBehaviorChoice.ObjectFieldContainsArtifactId, fields(0).ImportBehavior)
			Assert.AreEqual(ImportBehaviorChoice.ObjectFieldContainsArtifactId, fields(1).ImportBehavior)
		End Sub

		<Test>
		Public Sub GetMappedFields_NoObjectFieldsSelected_NoFieldsObjectFieldContainsArtifactId()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _context, _ioReporter, _logger, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True), tokenSource)

			Dim FieldMap As New LoadFileFieldMap
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field1", 1, FieldTypeHelper.FieldType.Object, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field2", 2, FieldTypeHelper.FieldType.Objects, 1, 0, 0, 0, True, Nothing, False), 0))
			bulkImporter.FieldMap = FieldMap

			Dim fields As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo() =
			 bulkImporter.GetMappedFields(1, New System.Collections.Generic.List(Of Integer)(New Integer() {11, 22}))

			Assert.AreEqual(Nothing, fields(0).ImportBehavior)
			Assert.AreEqual(Nothing, fields(1).ImportBehavior)
		End Sub

		<Test>
		Public Sub GetMappedFields_NoSelection_NoFieldsObjectFieldContainsArtifactId()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _context, _ioReporter, _logger, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True), tokenSource)

			Dim FieldMap As New LoadFileFieldMap
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field1", 1, FieldTypeHelper.FieldType.Object, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field2", 2, FieldTypeHelper.FieldType.Objects, 1, 0, 0, 0, True, Nothing, False), 0))
			bulkImporter.FieldMap = FieldMap

			Dim fields As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo() =
			 bulkImporter.GetMappedFields(1, New System.Collections.Generic.List(Of Integer)())

			Assert.AreEqual(Nothing, fields(0).ImportBehavior)
			Assert.AreEqual(Nothing, fields(1).ImportBehavior)
		End Sub

		<Test>
		Public Sub GetMappedFields_OneObjectFieldSelected_OneFieldsObjectFieldContainsArtifactId()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _context, _ioReporter, _logger, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True), tokenSource)

			Dim FieldMap As New LoadFileFieldMap
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field1", 1, FieldTypeHelper.FieldType.Object, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field2", 2, FieldTypeHelper.FieldType.Objects, 1, 0, 0, 0, True, Nothing, False), 0))
			bulkImporter.FieldMap = FieldMap

			Dim fields As kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo() =
			 bulkImporter.GetMappedFields(1, New System.Collections.Generic.List(Of Integer)(New Integer() {1, 22}))

			Assert.AreEqual(ImportBehaviorChoice.ObjectFieldContainsArtifactId, fields(0).ImportBehavior)
			Assert.AreEqual(Nothing, fields(1).ImportBehavior)
		End Sub

		<Test>
		Public Sub GetMappedFields_NoneObjectFieldsSelected_NoFieldsObjectFieldContainsArtifactId()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _context, _ioReporter, _logger, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True), tokenSource)

			Dim FieldMap As New LoadFileFieldMap
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field01", 1, FieldTypeHelper.FieldType.Object, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field02", 2, FieldTypeHelper.FieldType.Objects, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field03", 3, FieldTypeHelper.FieldType.Boolean, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field04", 4, FieldTypeHelper.FieldType.Code, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field05", 5, FieldTypeHelper.FieldType.Currency, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field06", 6, FieldTypeHelper.FieldType.Date, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field07", 7, FieldTypeHelper.FieldType.Decimal, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field08", 8, FieldTypeHelper.FieldType.Empty, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field09", 9, FieldTypeHelper.FieldType.File, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field10", 10, FieldTypeHelper.FieldType.Integer, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field11", 11, FieldTypeHelper.FieldType.LayoutText, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field12", 12, FieldTypeHelper.FieldType.MultiCode, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field13", 13, FieldTypeHelper.FieldType.Text, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field14", 14, FieldTypeHelper.FieldType.User, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field15", 15, FieldTypeHelper.FieldType.Varchar, 1, 0, 0, 0, True, Nothing, False), 0))
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

		<Test>
		Public Sub GetMappedFields_TwoObjectFieldsSelected_TwoFieldsObjectFieldContainsArtifactId()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _context, _ioReporter, _logger, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True), tokenSource)

			Dim FieldMap As New LoadFileFieldMap
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field01", 1, FieldTypeHelper.FieldType.Object, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field02", 2, FieldTypeHelper.FieldType.Objects, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field03", 3, FieldTypeHelper.FieldType.Boolean, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field04", 4, FieldTypeHelper.FieldType.Code, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field05", 5, FieldTypeHelper.FieldType.Currency, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field06", 6, FieldTypeHelper.FieldType.Date, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field07", 7, FieldTypeHelper.FieldType.Decimal, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field08", 8, FieldTypeHelper.FieldType.Empty, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field09", 9, FieldTypeHelper.FieldType.File, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field10", 10, FieldTypeHelper.FieldType.Integer, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field11", 11, FieldTypeHelper.FieldType.LayoutText, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field12", 12, FieldTypeHelper.FieldType.MultiCode, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field13", 13, FieldTypeHelper.FieldType.Text, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field14", 14, FieldTypeHelper.FieldType.User, 1, 0, 0, 0, True, Nothing, False), 0))
			FieldMap.Add(New LoadFileFieldMapItem(New DocumentField("Field15", 15, FieldTypeHelper.FieldType.Varchar, 1, 0, 0, 0, True, Nothing, False), 0))
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

		<Test>
		<TestCaseSource("OverlayTypeSource")>
		Public Sub GetMassImportOverlayBehavior(ByVal inputOverlayType As LoadFile.FieldOverlayBehavior, expectedOverlayType As kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior)
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _context, _ioReporter, _logger, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True), tokenSource)
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

		<Test>
		<TestCase("\", "")>
		<TestCase("\", "\\")>
		<TestCase("\ႝ", "\ႝ\")>
		<TestCase("\aaa\bbb\cc", "aaa\\bbb\\cc")>
		<TestCase("\aaa\bbb\cc", "aaa\\\\\\\\\\bbb\\cc")>
		<TestCase("\SourceCode\Mainline\EDDS\kCura.WinEDDS\Importers", ".\SourceCode\Mainline\EDDS\kCura.WinEDDS\Importers")>
		Public Sub CleanDestinationFolderPath(ByVal expected As String, ByVal input As String)
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _context, _ioReporter, _logger, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerSqlExceptions(True), tokenSource)
			Dim actual As String = bulkImporter.CleanFolderPath(input)
			Assert.AreEqual(expected, actual)
		End Sub

		<Test>
		Public Sub GetMaxExtractedTextLength_Return_Correct_Value_Nothing()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _context, _ioReporter, _logger, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerSqlExceptions(True), tokenSource)
			Dim encoding As System.Text.Encoding = Nothing
			Assert.AreEqual(1073741824, bulkImporter.GetMaxExtractedTextLength(encoding))
		End Sub

		<Test>
		Public Sub GetMaxExtractedTextLength_Return_Correct_Value_UTF8()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _context, _ioReporter, _logger, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerSqlExceptions(True), tokenSource)
			Dim encoding As System.Text.Encoding = System.Text.Encoding.UTF8
			Assert.AreEqual(1073741824, bulkImporter.GetMaxExtractedTextLength(encoding))
		End Sub

		<Test>
		Public Sub GetMaxExtractedTextLength_Return_Correct_Value_Other()
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _context, _ioReporter, _logger, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerSqlExceptions(True), tokenSource)
			Dim encoding As System.Text.Encoding = System.Text.Encoding.UTF32
			Assert.AreEqual(2147483647, bulkImporter.GetMaxExtractedTextLength(encoding))
		End Sub

		<Test>
		Public Sub WaitForRetry_TestRetryCountOnFail()
			Dim retryMax As Int32 = 10
			Dim attemptCount As Int32 = 0
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _context, _ioReporter, _logger, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True), tokenSource)
			Dim retryFunction As Func(Of Boolean) = Function()
														attemptCount += 1
														Return False
													End Function

			bulkImporter.WaitForRetry(retryFunction, "", "", "", retryMax, 0)

			Assert.AreEqual(retryMax + 1, attemptCount)
		End Sub

		<Test>
		Public Sub WaitForRetry_TestRetryNotExecuted()
			Dim attemptCount As Int32 = 0
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _context, _ioReporter, _logger, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True), tokenSource)
			Dim retryFunction As Func(Of Boolean) = Function()
														attemptCount += 1
														Return True
													End Function

			bulkImporter.WaitForRetry(retryFunction, "", "", "", 10, 0)

			Assert.AreEqual(1, attemptCount)
		End Sub

		<Test>
		Public Sub WaitForRetry_TestEventuallySucceeds()
			Dim attemptCount As Int32 = 0
			Dim succeedCount As Int32 = 6
			Dim bulkImporter As MockBulkLoadFileImporter = New MockBulkLoadFileImporter(_args, _context, _ioReporter, _logger, 0, False, False, _guid, True, "S", True, New MockBulkImportManagerWebExceptions(True), tokenSource)
			Dim retryFunction As Func(Of Boolean) = Function()
														attemptCount += 1
														If attemptCount = succeedCount Then
															Return True
														End If
														Return False
													End Function

			bulkImporter.WaitForRetry(retryFunction, "", "", "", 10, 0)

			Assert.AreEqual(succeedCount, attemptCount)
		End Sub
	End Class
End Namespace