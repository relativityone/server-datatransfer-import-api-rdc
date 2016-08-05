Imports kCura.EDDS.WebAPI.BulkImportManagerBase
Imports NUnit.Framework
Imports kCura.Windows.Process

Namespace kCura.WinEDDS.NUnit
	<TestFixture()> Public Class BulkImageFileImporterTests

#Region " Members "
		Dim _args As ImageLoadFile
		Dim _guid As System.Guid
		Dim _controller As Controller
		Dim _overwrite As kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType
		Dim _keyPathExistsAlready As Boolean
		Dim _keyValExistsAlready As Boolean
#End Region

#Region " Setup "

		<SetUp()> Public Sub SetUp()
			_args = New ImageLoadFile()
			_args.CaseInfo = New Relativity.CaseInfo()
			_args.CaseInfo.RootArtifactID = 1
			_guid = New Guid("E09E18F3-D0C8-4CFC-96D1-FBB350FAB3E1")
			_controller = New Controller()
			_overwrite = OverwriteType.Both

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
			If _keyValExistsAlready = False Then
				RegKeyHelper.RemoveKeyPath(RegKeyHelper.RelativityKeyPath)
			End If
		End Sub
#End Region

#Region " Basic Retry + Batch Size  Tests"

		<Test(), Ignore()> Public Sub BulkImportImageFile_CatchWebException_Retries_CallsLowerBatchSize_True()
			Dim bulkImporter As MockBulkImageFileImporter = New MockBulkImageFileImporter(_args, _controller, _guid, False, False, New MockBulkImportManagerWebExceptions(True))
			Try
				bulkImporter.TryBulkImport(_overwrite)
			Catch ex As Exception
				Assert.AreEqual(GetType(System.Net.WebException), ex.GetType)
			End Try
			Assert.AreEqual(500 - 200, bulkImporter.BatchSize)
			Assert.AreEqual(2, bulkImporter.PauseCalled)
		End Sub

		<Test()> Public Sub BulkImportImageFile_NoException_NoRetries_CallsLowerBatchSize_False()
			Dim bulkImporter As MockBulkImageFileImporter = New MockBulkImageFileImporter(_args, _controller, _guid, False, False, New MockBulkImportManagerWebExceptions(False))
			bulkImporter.TryBulkImport(_overwrite)
			Assert.AreEqual(500, bulkImporter.BatchSize)
			Assert.AreEqual(0, bulkImporter.PauseCalled)
		End Sub

		<Test(), Ignore()> Public Sub BulkImportImageFile_CatchSystemException_Retries_CallsLowerBatchSize_False()
			Dim manager As New MockBulkImportManagerWebExceptions(True)
			manager.ErrorMessage = New System.Exception("bombed out")
			Dim bulkImporter As MockBulkImageFileImporter = New MockBulkImageFileImporter(_args, _controller, _guid, False, False, manager)
			Try
				bulkImporter.TryBulkImport(_overwrite)
			Catch ex As Exception
				Assert.AreEqual(GetType(System.Exception), ex.GetType)
				Assert.AreEqual("bombed out", ex.Message)
			End Try
			Assert.AreEqual(500, bulkImporter.BatchSize)
			Assert.AreEqual(2, bulkImporter.PauseCalled)
		End Sub

		<Test(), Ignore()> Public Sub BulkImportImageFile_CatchSQLException_Retries_CallsLowerBatchSize_True()
			Dim bulkImporter As MockBulkImageFileImporter = New MockBulkImageFileImporter(_args, _controller, _guid, False, False, New MockBulkImportManagerSqlExceptions(True))
			Try
				bulkImporter.TryBulkImport(_overwrite)
			Catch ex As Exception
				Assert.AreEqual(GetType(kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlTimeoutException), ex.GetType)
			End Try
			Assert.AreEqual(500 - 200, bulkImporter.BatchSize)
			Assert.AreEqual(2, bulkImporter.PauseCalled)
		End Sub

#End Region

#Region " Retry-Until-It-Works Tests"
		<Test(), Ignore()> Public Sub BulkImportImageFile_CatchSQLException_500RetryTo400_ThenWork()
			Dim bulkImporter As MockBulkImageFileImporter = New MockBulkImageFileImporter(_args, _controller, _guid, False, False, New MockBulkImportManagerSqlExceptions(400))
			bulkImporter.TryBulkImport(_overwrite)

			Assert.AreEqual(400, bulkImporter.BatchSize)
			Assert.AreEqual(1, bulkImporter.PauseCalled)
			CollectionAssert.AreEquivalent({500, 400}, bulkImporter.BatchSizeHistoryList)
		End Sub

		<Test(), Ignore()> Public Sub BulkImportImageFile_CatchSQLException_500RetryToMinimum_ThenWork()
			Dim bulkImporter As MockBulkImageFileImporter = New MockBulkImageFileImporter(_args, _controller, _guid, False, False, New MockBulkImportManagerSqlExceptions(300))
			bulkImporter.MinimumBatch = 300
			bulkImporter.TryBulkImport(_overwrite)
			Assert.AreEqual(300, bulkImporter.BatchSize)
			Assert.AreEqual(2, bulkImporter.PauseCalled)
			CollectionAssert.AreEquivalent({500, 400, 300}, bulkImporter.BatchSizeHistoryList)
		End Sub

		<Test(), Ignore()> Public Sub BulkImportImageFile_CatchSQLException_500Retry_HitMinimum_ThrowException()
			Dim bulkImporter As MockBulkImageFileImporter = New MockBulkImageFileImporter(_args, _controller, _guid, False, False, New MockBulkImportManagerSqlExceptions(200))
			bulkImporter.MinimumBatch = 350
			Try
				bulkImporter.TryBulkImport(_overwrite)
			Catch ex As Exception
				Assert.AreEqual(GetType(kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlTimeoutException), ex.GetType)
			End Try
			Assert.AreEqual(350, bulkImporter.BatchSize)
			Assert.AreEqual(2, bulkImporter.PauseCalled)
			CollectionAssert.AreEquivalent({500, 400, 350}, bulkImporter.BatchSizeHistoryList)
		End Sub

#End Region

#Region " Batch Size Adjustment "
		<Test()> Public Sub BulkImportImageFile_Lower_500BatchSize_to300()
			Dim bulkImporter As MockBulkImageFileImporter = New MockBulkImageFileImporter(_args, _controller, _guid, False, False, New MockBulkImportManagerWebExceptions(True))
			bulkImporter.BatchSize = 300
			Assert.AreEqual(300, bulkImporter.BatchSize)
		End Sub

		<Test()> Public Sub BulkImportImageFile_Lower_500BatchSize_ToMinimum300()
			Dim bulkImporter As MockBulkImageFileImporter = New MockBulkImageFileImporter(_args, _controller, _guid, False, False, New MockBulkImportManagerWebExceptions(True))
			bulkImporter.MinimumBatch = 300
			bulkImporter.BatchSize = 300
			Assert.AreEqual(300, bulkImporter.BatchSize)
		End Sub

		<Test()> Public Sub BulkImportImageFile_Lower_500BatchSize_To200_PastMinimum300()
			Dim bulkImporter As MockBulkImageFileImporter = New MockBulkImageFileImporter(_args, _controller, _guid, False, False, New MockBulkImportManagerWebExceptions(True))
			bulkImporter.MinimumBatch = 300
			bulkImporter.BatchSize = 200
			Assert.AreEqual(300, bulkImporter.BatchSize)
		End Sub

#End Region

#Region "LowerBatchSizeAndRetry"
		<Test()>
		Public Sub LowerBatchSizeAndRetry_Initial100Docs_Batch78_NewFileHas80()
			Dim originalBatchSizeWith100Images As String = <string>1,0,0,0,1,AS000001,AS000001,09ea36a7-6e04-45e5-8a89-79545589cb6f,AS000001.tif,0,0,35852,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\09ea36a7-6e04-45e5-8a89-79545589cb6f,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000001.tif,þþKþþ
1,0,0,0,2,AS000002,AS000002,684dabe5-1625-49fa-a5b3-7bf5c5106c71,AS000002.tif,0,0,256498,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\684dabe5-1625-49fa-a5b3-7bf5c5106c71,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002.tif,þþKþþ
0,0,0,0,3,AS000002,AS000002_001,60254207-6cb3-49fd-8db8-ae278c00a1b7,AS000002_001.tif,1,0,87746,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\60254207-6cb3-49fd-8db8-ae278c00a1b7,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_001.tif,þþKþþ
0,0,0,0,4,AS000002,AS000002_002,c1840300-999f-4a78-ad71-2aba69cfdbb3,AS000002_002.tif,2,0,84251,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\c1840300-999f-4a78-ad71-2aba69cfdbb3,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_002.tif,þþKþþ
0,0,0,0,5,AS000002,AS000002_003,fbf4dc1d-ac8b-4616-9831-8131e8bd67ce,AS000002_003.tif,3,0,85756,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\fbf4dc1d-ac8b-4616-9831-8131e8bd67ce,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_003.tif,þþKþþ
0,0,0,0,6,AS000002,AS000002_004,1388ce19-2b7c-4d84-9e0f-c854c1736e9f,AS000002_004.tif,4,0,88604,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\1388ce19-2b7c-4d84-9e0f-c854c1736e9f,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_004.tif,þþKþþ
0,0,0,0,7,AS000002,AS000002_005,567847ff-0265-4a5b-8bd0-70e291e27107,AS000002_005.tif,5,0,89120,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\567847ff-0265-4a5b-8bd0-70e291e27107,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_005.tif,þþKþþ
0,0,0,0,8,AS000002,AS000002_006,602c01b1-ddca-4d07-9aea-bcd135e929ae,AS000002_006.tif,6,0,90408,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\602c01b1-ddca-4d07-9aea-bcd135e929ae,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_006.tif,þþKþþ
0,0,0,0,9,AS000002,AS000002_007,d6b7144f-5b7f-4ca5-bb4b-7fdd655bd91d,AS000002_007.tif,7,0,85319,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\d6b7144f-5b7f-4ca5-bb4b-7fdd655bd91d,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_007.tif,þþKþþ
0,0,0,0,10,AS000002,AS000002_008,d963c346-e072-4069-95c9-4139476f0e81,AS000002_008.tif,8,0,88129,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\d963c346-e072-4069-95c9-4139476f0e81,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_008.tif,þþKþþ
0,0,0,0,11,AS000002,AS000002_009,338a1d90-e2c7-42ad-bcb8-c766a5fe9f81,AS000002_009.tif,9,0,88843,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\338a1d90-e2c7-42ad-bcb8-c766a5fe9f81,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_009.tif,þþKþþ
0,0,0,0,12,AS000002,AS000002_010,4891011a-dee1-4451-b6ce-597278b9e0fd,AS000002_010.tif,10,0,89422,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\4891011a-dee1-4451-b6ce-597278b9e0fd,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_010.tif,þþKþþ
0,0,0,0,13,AS000002,AS000002_011,4d54ad62-9889-4013-9972-47cd27b66901,AS000002_011.tif,11,0,85894,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\4d54ad62-9889-4013-9972-47cd27b66901,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_011.tif,þþKþþ
0,0,0,0,14,AS000002,AS000002_012,4e1f27f2-8d6c-43c4-a585-0f5eccc2c85e,AS000002_012.tif,12,0,87147,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\4e1f27f2-8d6c-43c4-a585-0f5eccc2c85e,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_012.tif,þþKþþ
0,0,0,0,15,AS000002,AS000002_013,564e2e34-4aa4-4d34-9b34-1860422ca2dc,AS000002_013.tif,13,0,88158,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\564e2e34-4aa4-4d34-9b34-1860422ca2dc,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_013.tif,þþKþþ
0,0,0,0,16,AS000002,AS000002_014,5a0b726c-2c7c-4fa9-9a84-5180319c7d59,AS000002_014.tif,14,0,23346,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\5a0b726c-2c7c-4fa9-9a84-5180319c7d59,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_014.tif,þþKþþ
0,0,0,0,17,AS000002,AS000002_015,b771cf85-751c-4d00-8b59-b3cbf8ac660e,AS000002_015.tif,15,0,167358,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\b771cf85-751c-4d00-8b59-b3cbf8ac660e,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_015.tif,þþKþþ
0,0,0,0,18,AS000002,AS000002_016,f0027451-4ea5-4cc9-aa38-922ce258c875,AS000002_016.tif,16,0,26464,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\f0027451-4ea5-4cc9-aa38-922ce258c875,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_016.tif,þþKþþ
0,0,0,0,19,AS000002,AS000002_017,cdd54394-a97f-4cbe-b57f-a58c9d8da9a9,AS000002_017.tif,17,0,26710,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\cdd54394-a97f-4cbe-b57f-a58c9d8da9a9,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_017.tif,þþKþþ
0,0,0,0,20,AS000002,AS000002_018,a50cb03c-9e83-455b-a523-16951dbb1ddb,AS000002_018.tif,18,0,28295,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\a50cb03c-9e83-455b-a523-16951dbb1ddb,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_018.tif,þþKþþ
0,0,0,0,21,AS000002,AS000002_019,93d89b1e-abe1-4ccd-9368-9c0eff5f5e44,AS000002_019.tif,19,0,28593,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\93d89b1e-abe1-4ccd-9368-9c0eff5f5e44,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_019.tif,þþKþþ
0,0,0,0,22,AS000002,AS000002_020,a910184b-1cfc-426c-ac56-c6e53348ff1b,AS000002_020.tif,20,0,28740,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\a910184b-1cfc-426c-ac56-c6e53348ff1b,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_020.tif,þþKþþ
0,0,0,0,23,AS000002,AS000002_021,f188f158-4564-4433-bf9b-9e47493d9ba6,AS000002_021.tif,21,0,29386,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\f188f158-4564-4433-bf9b-9e47493d9ba6,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_021.tif,þþKþþ
0,0,0,0,24,AS000002,AS000002_022,f60cd03c-e142-437a-a596-26282879660f,AS000002_022.tif,22,0,29364,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\f60cd03c-e142-437a-a596-26282879660f,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_022.tif,þþKþþ
0,0,0,0,25,AS000002,AS000002_023,cd615a44-7b91-4d13-9b26-7fab4d2ccf00,AS000002_023.tif,23,0,29678,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\cd615a44-7b91-4d13-9b26-7fab4d2ccf00,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_023.tif,þþKþþ
0,0,0,0,26,AS000002,AS000002_024,e4a3ffb5-1221-48e3-b833-6c8eb310bee5,AS000002_024.tif,24,0,31349,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\e4a3ffb5-1221-48e3-b833-6c8eb310bee5,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_024.tif,þþKþþ
0,0,0,0,27,AS000002,AS000002_025,8765f161-b000-40ad-a3e9-424fd54144d3,AS000002_025.tif,25,0,30698,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\8765f161-b000-40ad-a3e9-424fd54144d3,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_025.tif,þþKþþ
0,0,0,0,28,AS000002,AS000002_026,c627719a-7a19-45c4-b103-8f7cd546f58e,AS000002_026.tif,26,0,30142,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\c627719a-7a19-45c4-b103-8f7cd546f58e,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_026.tif,þþKþþ
0,0,0,0,29,AS000002,AS000002_027,6b60ffbc-865e-4980-a44f-5c6a7e04478c,AS000002_027.tif,27,0,28973,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\6b60ffbc-865e-4980-a44f-5c6a7e04478c,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_027.tif,þþKþþ
0,0,0,0,30,AS000002,AS000002_028,7b49d6ca-ce35-4ab3-8448-dbecb559693d,AS000002_028.tif,28,0,29180,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\7b49d6ca-ce35-4ab3-8448-dbecb559693d,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_028.tif,þþKþþ
0,0,0,0,31,AS000002,AS000002_029,48c2aba0-bcb2-428e-aa33-71cce0f88ca3,AS000002_029.tif,29,0,7538,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\48c2aba0-bcb2-428e-aa33-71cce0f88ca3,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_029.tif,þþKþþ
1,0,0,0,32,AS000003,AS000003,8c937e0c-33ba-4203-97d2-8d9d9682d147,AS000003.tif,0,0,63186,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\8c937e0c-33ba-4203-97d2-8d9d9682d147,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000003.tif,þþKþþ
0,0,0,0,33,AS000003,AS000003_01,4c8c7e2b-8f2b-477e-ba74-71482e18a548,AS000003_01.tif,1,0,537377,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\4c8c7e2b-8f2b-477e-ba74-71482e18a548,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000003_01.tif,þþKþþ
0,0,0,0,34,AS000003,AS000003_02,80867237-a72e-4c2a-8aae-c1b2ae8c549e,AS000003_02.tif,2,0,138412,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\80867237-a72e-4c2a-8aae-c1b2ae8c549e,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000003_02.tif,þþKþþ
0,0,0,0,35,AS000003,AS000003_03,d8ffbeca-5dcc-48ce-8e06-46ff1bd65541,AS000003_03.tif,3,0,87122,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\d8ffbeca-5dcc-48ce-8e06-46ff1bd65541,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000003_03.tif,þþKþþ
1,0,0,0,36,AS000004,AS000004,fae7699b-4121-4758-a9d1-b078db7acbf6,AS000004.tif,0,0,20277,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\fae7699b-4121-4758-a9d1-b078db7acbf6,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000004.tif,þþKþþ
0,0,0,0,37,AS000004,AS000004_01,e4f11a66-98ec-4ce1-8387-d233424fe18c,AS000004_01.tif,1,0,22472,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\e4f11a66-98ec-4ce1-8387-d233424fe18c,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000004_01.tif,þþKþþ
1,0,0,0,38,AS000005,AS000005,945d9abc-a535-42b0-b21c-570725cac188,AS000005.tif,0,0,35421,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\945d9abc-a535-42b0-b21c-570725cac188,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000005.tif,þþKþþ
0,0,0,0,39,AS000005,AS000005_01,e4ce8922-e0d8-4398-82a7-30bcc97f41b0,AS000005_01.tif,1,0,15036,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\e4ce8922-e0d8-4398-82a7-30bcc97f41b0,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000005_01.tif,þþKþþ
0,0,0,0,40,AS000005,AS000005_02,3bb60589-a25c-43cd-9d50-be63e231095c,AS000005_02.tif,2,0,55065,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\3bb60589-a25c-43cd-9d50-be63e231095c,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000005_02.tif,þþKþþ
0,0,0,0,41,AS000005,AS000005_03,f4977a8f-3c00-4ed4-8370-2ffeba4ea03d,AS000005_03.tif,3,0,8124,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\f4977a8f-3c00-4ed4-8370-2ffeba4ea03d,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000005_03.tif,þþKþþ
1,0,0,0,42,EN000001,EN000001,7c91e888-1fe8-4a7e-b904-4ff48f181173,EN000001.tif,0,0,32539,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\7c91e888-1fe8-4a7e-b904-4ff48f181173,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000001.tif,þþKþþ
0,0,0,0,43,EN000001,EN000001_01,4f26e3f3-4993-431a-a8dd-4ab8d08d1d60,EN000001_01.tif,1,0,45266,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\4f26e3f3-4993-431a-a8dd-4ab8d08d1d60,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000001_01.tif,þþKþþ
0,0,0,0,44,EN000001,EN000001_02,75e265ad-6580-416e-a3a2-6579d388676e,EN000001_02.tif,2,0,55522,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\75e265ad-6580-416e-a3a2-6579d388676e,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000001_02.tif,þþKþþ
0,0,0,0,45,EN000001,EN000001_03,0431dc32-0633-4cb5-ad14-566f199f0476,EN000001_03.tif,3,0,56934,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\0431dc32-0633-4cb5-ad14-566f199f0476,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000001_03.tif,þþKþþ
0,0,0,0,46,EN000001,EN000001_04,c7954bab-c978-483d-8392-7af3c4699fb0,EN000001_04.tif,4,0,52800,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\c7954bab-c978-483d-8392-7af3c4699fb0,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000001_04.tif,þþKþþ
0,0,0,0,47,EN000001,EN000001_05,939aa5c3-54a5-4976-b950-8ce76cfe7bdb,EN000001_05.tif,5,0,35386,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\939aa5c3-54a5-4976-b950-8ce76cfe7bdb,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000001_05.tif,þþKþþ
1,0,0,0,48,EN000002,EN000002,6534c151-f9e1-43ae-a5be-726f1d4a4998,EN000002.tif,0,0,39912,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\6534c151-f9e1-43ae-a5be-726f1d4a4998,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000002.tif,þþKþþ
0,0,0,0,49,EN000002,EN000002_01,c3098e1a-42eb-4f12-afbb-c83ace90e42b,EN000002_01.tif,1,0,43989,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\c3098e1a-42eb-4f12-afbb-c83ace90e42b,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000002_01.tif,þþKþþ
0,0,0,0,50,EN000002,EN000002_02,e26630a7-379d-455d-9e1f-7faad14b282a,EN000002_02.tif,2,0,41384,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\e26630a7-379d-455d-9e1f-7faad14b282a,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000002_02.tif,þþKþþ
0,0,0,0,51,EN000002,EN000002_03,d21f20a3-9f9f-48a6-ae46-c9e7e3fa26b9,EN000002_03.tif,3,0,37265,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\d21f20a3-9f9f-48a6-ae46-c9e7e3fa26b9,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000002_03.tif,þþKþþ
1,0,0,0,52,EN000003,EN000003,2ab6498f-552b-4258-b33a-a2a230d35256,EN000003.tif,0,0,43106,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\2ab6498f-552b-4258-b33a-a2a230d35256,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000003.tif,þþKþþ
0,0,0,0,53,EN000003,EN000003_01,31d0c30a-c2ff-458d-aeb3-28c4161f8e05,EN000003_01.tif,1,0,58084,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\31d0c30a-c2ff-458d-aeb3-28c4161f8e05,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000003_01.tif,þþKþþ
0,0,0,0,54,EN000003,EN000003_02,19186002-74ac-413a-a949-bf7e05f0e3c6,EN000003_02.tif,2,0,6022,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\19186002-74ac-413a-a949-bf7e05f0e3c6,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000003_02.tif,þþKþþ
1,0,0,0,55,EN000004,EN000004,694fe524-938a-4ee8-9a31-19a57e0e1c58,EN000004.tif,0,0,43114,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\694fe524-938a-4ee8-9a31-19a57e0e1c58,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000004.tif,þþKþþ
0,0,0,0,56,EN000004,EN000004_01,fb59d6fa-cd3e-4a0c-a269-3679f9dcca0e,EN000004_01.tif,1,0,58084,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\fb59d6fa-cd3e-4a0c-a269-3679f9dcca0e,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000004_01.tif,þþKþþ
0,0,0,0,57,EN000004,EN000004_02,81ca5ff3-219b-49a1-99b3-331cb452ec5e,EN000004_02.tif,2,0,6022,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\81ca5ff3-219b-49a1-99b3-331cb452ec5e,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000004_02.tif,þþKþþ
1,0,0,0,58,EN000005,EN000005,1de2940b-9f61-4ccf-9d6b-2e0a04109689,EN000005.tif,0,0,5331,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\1de2940b-9f61-4ccf-9d6b-2e0a04109689,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000005.tif,þþKþþ
1,0,0,0,59,EN000006,EN000006,6d139a06-2ae3-4788-9e65-6f122f27ad4c,EN000006.tif,0,0,4765,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\6d139a06-2ae3-4788-9e65-6f122f27ad4c,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000006.tif,þþKþþ
1,0,0,0,60,EN000007,EN000007,b4e5480a-8538-4534-84f4-6804d963d15b,EN000007.tif,0,0,23585,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\b4e5480a-8538-4534-84f4-6804d963d15b,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000007.tif,þþKþþ
1,0,0,0,61,EN000008,EN000008,98045c12-8e56-4e23-b00d-36cd9f558d42,EN000008.tif,0,0,34341,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\98045c12-8e56-4e23-b00d-36cd9f558d42,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000008.tif,þþKþþ
1,0,0,0,62,EN000009,EN000009,800a9721-3445-43d0-8ac8-fe6aad7fcb51,EN000009.tif,0,0,18595,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\800a9721-3445-43d0-8ac8-fe6aad7fcb51,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000009.tif,þþKþþ
1,0,0,0,63,EN000010,EN000010,058cbcb1-bc4e-4c0d-b5ce-33bfee08775f,EN000010.tif,0,0,5980,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\058cbcb1-bc4e-4c0d-b5ce-33bfee08775f,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000010.tif,þþKþþ
1,0,0,0,64,EN000011,EN000011,c6b67844-9c67-430d-9418-d5a5958cbc81,EN000011.tif,0,0,4954,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\c6b67844-9c67-430d-9418-d5a5958cbc81,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000011.tif,þþKþþ
1,0,0,0,65,EN000012,EN000012,5dbf3691-194a-45e9-bc6a-994638347459,EN000012.tif,0,0,6644,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\5dbf3691-194a-45e9-bc6a-994638347459,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000012.tif,þþKþþ
1,0,0,0,66,EN000013,EN000013,681c6b76-2942-4ecc-8ba6-fb4c1dbdbb1e,EN000013.tif,0,0,11482,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\681c6b76-2942-4ecc-8ba6-fb4c1dbdbb1e,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000013.tif,þþKþþ
1,0,0,0,67,EN000014,EN000014,d9162157-22d4-49e8-8268-ed3699f58ac0,EN000014.tif,0,0,45830,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\d9162157-22d4-49e8-8268-ed3699f58ac0,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000014.tif,þþKþþ
0,0,0,0,68,EN000014,EN000014_01,ba2095ce-b07d-4843-b82f-1daebb79ed6a,EN000014_01.tif,1,0,1474,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\ba2095ce-b07d-4843-b82f-1daebb79ed6a,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000014_01.tif,þþKþþ
1,0,0,0,69,EN000015,EN000015,4c2f860e-8f74-4c61-897e-90ec62578652,EN000015.tif,0,0,44286,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\4c2f860e-8f74-4c61-897e-90ec62578652,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000015.tif,þþKþþ
1,0,0,0,70,EN000016,EN000016,c52da0b7-cbb2-4e57-bcbf-a4d168d144bc,EN000016.tif,0,0,46507,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\c52da0b7-cbb2-4e57-bcbf-a4d168d144bc,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000016.tif,þþKþþ
0,0,0,0,71,EN000016,EN000016_01,13f002cf-553e-4faf-b678-030d23a9a85c,EN000016_01.tif,1,0,70123,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\13f002cf-553e-4faf-b678-030d23a9a85c,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000016_01.tif,þþKþþ
0,0,0,0,72,EN000016,EN000016_02,7854750b-8f14-4490-9149-8dcfc05893cf,EN000016_02.tif,2,0,51746,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\7854750b-8f14-4490-9149-8dcfc05893cf,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000016_02.tif,þþKþþ
1,0,0,0,73,EN000017,EN000017,de2c471a-2eb3-442f-829b-e3481eeee46f,EN000017.tif,0,0,52518,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\de2c471a-2eb3-442f-829b-e3481eeee46f,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000017.tif,þþKþþ
0,0,0,0,74,EN000017,EN000017_01,93dd5cb2-1c1e-4be5-a15a-7029f4094367,EN000017_01.tif,1,0,79364,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\93dd5cb2-1c1e-4be5-a15a-7029f4094367,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000017_01.tif,þþKþþ
0,0,0,0,75,EN000017,EN000017_02,df396946-c0f6-4c44-9622-3aeef8b3e4df,EN000017_02.tif,2,0,58376,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\df396946-c0f6-4c44-9622-3aeef8b3e4df,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000017_02.tif,þþKþþ
1,0,0,0,76,EN000018,EN000018,2658007a-fd1d-47ce-928b-17239a229511,EN000018.tif,0,0,20921,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\2658007a-fd1d-47ce-928b-17239a229511,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000018.tif,þþKþþ
0,0,0,0,77,EN000018,EN000018_01,e8d2a1e4-091f-4d20-8307-c1193f66cd6c,EN000018_01.tif,1,0,26544,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\e8d2a1e4-091f-4d20-8307-c1193f66cd6c,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000018_01.tif,þþKþþ
0,0,0,0,78,EN000018,EN000018_02,fed5df43-4715-4ef1-b83e-e565ef187c82,EN000018_02.tif,2,0,26168,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\fed5df43-4715-4ef1-b83e-e565ef187c82,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000018_02.tif,þþKþþ
0,0,0,0,79,EN000018,EN000018_03,4a5503a6-838c-477b-be28-134206a7c0cd,EN000018_03.tif,3,0,22676,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\4a5503a6-838c-477b-be28-134206a7c0cd,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000018_03.tif,þþKþþ
0,0,0,0,80,EN000018,EN000018_04,3986ff37-cba2-4054-9b1b-7827a8de4c72,EN000018_04.tif,4,0,18039,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\3986ff37-cba2-4054-9b1b-7827a8de4c72,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000018_04.tif,þþKþþ
1,0,0,0,81,EN000019,EN000019,32401c1b-e238-44f3-9630-d95b450c3f41,EN000019.tif,0,0,5272,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\32401c1b-e238-44f3-9630-d95b450c3f41,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000019.tif,þþKþþ
1,0,0,0,82,EN000020,EN000020,bd39ac2e-1634-4db8-b4f7-e864447b1153,EN000020.tif,0,0,14956,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\bd39ac2e-1634-4db8-b4f7-e864447b1153,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000020.tif,þþKþþ
1,0,0,0,83,EN000021,EN000021,a7163280-81ae-4f92-b7c2-da16c8fa9b0a,EN000021.tif,0,0,14598,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\a7163280-81ae-4f92-b7c2-da16c8fa9b0a,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000021.tif,þþKþþ
1,0,0,0,84,EN000022,EN000022,40112722-a5bb-4df3-92ce-e270dc35122e,EN000022.tif,0,0,35654,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\40112722-a5bb-4df3-92ce-e270dc35122e,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000022.tif,þþKþþ
1,0,0,0,85,EN000023,EN000023,1129a4a3-9565-4f81-b076-1c907de25627,EN000023.tif,0,0,6078,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\1129a4a3-9565-4f81-b076-1c907de25627,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000023.tif,þþKþþ
1,0,0,0,86,EN000024,EN000024,f96a5f13-4b4c-4479-8ee1-1bf728f7e7ce,EN000024.tif,0,0,22117,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\f96a5f13-4b4c-4479-8ee1-1bf728f7e7ce,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000024.tif,þþKþþ
1,0,0,0,87,EN000025,EN000025,fa1e5a1a-5cfe-4b67-aa14-b0950bf601fc,EN000025.tif,0,0,13746,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\fa1e5a1a-5cfe-4b67-aa14-b0950bf601fc,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000025.tif,þþKþþ
1,0,0,0,88,EN000026,EN000026,92fa356c-b578-4af1-85f5-88163bd85792,EN000026.tif,0,0,4795,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\92fa356c-b578-4af1-85f5-88163bd85792,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000026.tif,þþKþþ
1,0,0,0,89,EN000027,EN000027,2d8d299e-d868-4302-9ae7-60ae93bd59be,EN000027.tif,0,0,7702,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\2d8d299e-d868-4302-9ae7-60ae93bd59be,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000027.tif,þþKþþ
1,0,0,0,90,EN000028,EN000028,7bb81026-5f61-4b8c-a349-12c15a758360,EN000028.tif,0,0,8543,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\7bb81026-5f61-4b8c-a349-12c15a758360,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000028.tif,þþKþþ
1,0,0,0,91,EN000029,EN000029,fdb494c3-cec9-4237-8d89-e148c491837b,EN000029.tif,0,0,22703,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\fdb494c3-cec9-4237-8d89-e148c491837b,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000029.tif,þþKþþ
0,0,0,0,92,EN000029,EN000029_01,be80dfc9-2477-4688-a5a4-fe2a30a528fe,EN000029_01.tif,1,0,24568,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\be80dfc9-2477-4688-a5a4-fe2a30a528fe,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000029_01.tif,þþKþþ
0,0,0,0,93,EN000029,EN000029_02,f24f1e38-0c27-4bfc-a3e7-57331e081760,EN000029_02.tif,2,0,1060,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\f24f1e38-0c27-4bfc-a3e7-57331e081760,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000029_02.tif,þþKþþ
1,0,0,0,94,EN000030,EN000030,52b378e3-5c6e-4536-9ea6-bfc9fd94efc2,EN000030.tif,0,0,59534,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\52b378e3-5c6e-4536-9ea6-bfc9fd94efc2,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000030.tif,þþKþþ
0,0,0,0,95,EN000030,EN000030_01,2b11049e-1a59-4f40-b2f8-206cbc2eba43,EN000030_01.tif,1,0,32562,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\2b11049e-1a59-4f40-b2f8-206cbc2eba43,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000030_01.tif,þþKþþ
1,0,0,0,96,EN000031,EN000031,f0d3c802-334c-4541-87eb-059cfa81a008,EN000031.tif,0,0,15013,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\f0d3c802-334c-4541-87eb-059cfa81a008,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000031.tif,þþKþþ
1,0,0,0,97,EN000032,EN000032,64fe5468-7b16-46a0-91bd-e5fdb80a93ea,EN000032.tif,0,0,4045,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\64fe5468-7b16-46a0-91bd-e5fdb80a93ea,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000032.tif,þþKþþ
1,0,0,0,98,EN000033,EN000033,6bd6cff9-1cfc-48ef-9854-7b2d7dca320e,EN000033.tif,0,0,8922,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\6bd6cff9-1cfc-48ef-9854-7b2d7dca320e,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000033.tif,þþKþþ
1,0,0,0,99,EN000034,EN000034,d80fe217-42ba-454a-bd51-09650a425e2b,EN000034.tif,0,0,6821,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\d80fe217-42ba-454a-bd51-09650a425e2b,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000034.tif,þþKþþ
1,0,0,0,100,EN000035,EN000035,8765c376-e8a3-4f30-af4e-6d9d979fb22e,EN000035.tif,0,0,11977,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\8765c376-e8a3-4f30-af4e-6d9d979fb22e,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000035.tif,þþKþþ</string>.Value
			Dim expectedReducedFileWith80Rows As String = <string>1,0,0,0,1,AS000001,AS000001,09ea36a7-6e04-45e5-8a89-79545589cb6f,AS000001.tif,0,0,35852,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\09ea36a7-6e04-45e5-8a89-79545589cb6f,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000001.tif,þþKþþ
1,0,0,0,2,AS000002,AS000002,684dabe5-1625-49fa-a5b3-7bf5c5106c71,AS000002.tif,0,0,256498,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\684dabe5-1625-49fa-a5b3-7bf5c5106c71,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002.tif,þþKþþ
0,0,0,0,3,AS000002,AS000002_001,60254207-6cb3-49fd-8db8-ae278c00a1b7,AS000002_001.tif,1,0,87746,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\60254207-6cb3-49fd-8db8-ae278c00a1b7,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_001.tif,þþKþþ
0,0,0,0,4,AS000002,AS000002_002,c1840300-999f-4a78-ad71-2aba69cfdbb3,AS000002_002.tif,2,0,84251,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\c1840300-999f-4a78-ad71-2aba69cfdbb3,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_002.tif,þþKþþ
0,0,0,0,5,AS000002,AS000002_003,fbf4dc1d-ac8b-4616-9831-8131e8bd67ce,AS000002_003.tif,3,0,85756,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\fbf4dc1d-ac8b-4616-9831-8131e8bd67ce,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_003.tif,þþKþþ
0,0,0,0,6,AS000002,AS000002_004,1388ce19-2b7c-4d84-9e0f-c854c1736e9f,AS000002_004.tif,4,0,88604,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\1388ce19-2b7c-4d84-9e0f-c854c1736e9f,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_004.tif,þþKþþ
0,0,0,0,7,AS000002,AS000002_005,567847ff-0265-4a5b-8bd0-70e291e27107,AS000002_005.tif,5,0,89120,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\567847ff-0265-4a5b-8bd0-70e291e27107,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_005.tif,þþKþþ
0,0,0,0,8,AS000002,AS000002_006,602c01b1-ddca-4d07-9aea-bcd135e929ae,AS000002_006.tif,6,0,90408,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\602c01b1-ddca-4d07-9aea-bcd135e929ae,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_006.tif,þþKþþ
0,0,0,0,9,AS000002,AS000002_007,d6b7144f-5b7f-4ca5-bb4b-7fdd655bd91d,AS000002_007.tif,7,0,85319,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\d6b7144f-5b7f-4ca5-bb4b-7fdd655bd91d,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_007.tif,þþKþþ
0,0,0,0,10,AS000002,AS000002_008,d963c346-e072-4069-95c9-4139476f0e81,AS000002_008.tif,8,0,88129,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\d963c346-e072-4069-95c9-4139476f0e81,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_008.tif,þþKþþ
0,0,0,0,11,AS000002,AS000002_009,338a1d90-e2c7-42ad-bcb8-c766a5fe9f81,AS000002_009.tif,9,0,88843,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\338a1d90-e2c7-42ad-bcb8-c766a5fe9f81,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_009.tif,þþKþþ
0,0,0,0,12,AS000002,AS000002_010,4891011a-dee1-4451-b6ce-597278b9e0fd,AS000002_010.tif,10,0,89422,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\4891011a-dee1-4451-b6ce-597278b9e0fd,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_010.tif,þþKþþ
0,0,0,0,13,AS000002,AS000002_011,4d54ad62-9889-4013-9972-47cd27b66901,AS000002_011.tif,11,0,85894,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\4d54ad62-9889-4013-9972-47cd27b66901,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_011.tif,þþKþþ
0,0,0,0,14,AS000002,AS000002_012,4e1f27f2-8d6c-43c4-a585-0f5eccc2c85e,AS000002_012.tif,12,0,87147,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\4e1f27f2-8d6c-43c4-a585-0f5eccc2c85e,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_012.tif,þþKþþ
0,0,0,0,15,AS000002,AS000002_013,564e2e34-4aa4-4d34-9b34-1860422ca2dc,AS000002_013.tif,13,0,88158,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\564e2e34-4aa4-4d34-9b34-1860422ca2dc,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_013.tif,þþKþþ
0,0,0,0,16,AS000002,AS000002_014,5a0b726c-2c7c-4fa9-9a84-5180319c7d59,AS000002_014.tif,14,0,23346,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\5a0b726c-2c7c-4fa9-9a84-5180319c7d59,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_014.tif,þþKþþ
0,0,0,0,17,AS000002,AS000002_015,b771cf85-751c-4d00-8b59-b3cbf8ac660e,AS000002_015.tif,15,0,167358,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\b771cf85-751c-4d00-8b59-b3cbf8ac660e,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_015.tif,þþKþþ
0,0,0,0,18,AS000002,AS000002_016,f0027451-4ea5-4cc9-aa38-922ce258c875,AS000002_016.tif,16,0,26464,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\f0027451-4ea5-4cc9-aa38-922ce258c875,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_016.tif,þþKþþ
0,0,0,0,19,AS000002,AS000002_017,cdd54394-a97f-4cbe-b57f-a58c9d8da9a9,AS000002_017.tif,17,0,26710,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\cdd54394-a97f-4cbe-b57f-a58c9d8da9a9,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_017.tif,þþKþþ
0,0,0,0,20,AS000002,AS000002_018,a50cb03c-9e83-455b-a523-16951dbb1ddb,AS000002_018.tif,18,0,28295,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\a50cb03c-9e83-455b-a523-16951dbb1ddb,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_018.tif,þþKþþ
0,0,0,0,21,AS000002,AS000002_019,93d89b1e-abe1-4ccd-9368-9c0eff5f5e44,AS000002_019.tif,19,0,28593,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\93d89b1e-abe1-4ccd-9368-9c0eff5f5e44,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_019.tif,þþKþþ
0,0,0,0,22,AS000002,AS000002_020,a910184b-1cfc-426c-ac56-c6e53348ff1b,AS000002_020.tif,20,0,28740,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\a910184b-1cfc-426c-ac56-c6e53348ff1b,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_020.tif,þþKþþ
0,0,0,0,23,AS000002,AS000002_021,f188f158-4564-4433-bf9b-9e47493d9ba6,AS000002_021.tif,21,0,29386,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\f188f158-4564-4433-bf9b-9e47493d9ba6,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_021.tif,þþKþþ
0,0,0,0,24,AS000002,AS000002_022,f60cd03c-e142-437a-a596-26282879660f,AS000002_022.tif,22,0,29364,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\f60cd03c-e142-437a-a596-26282879660f,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_022.tif,þþKþþ
0,0,0,0,25,AS000002,AS000002_023,cd615a44-7b91-4d13-9b26-7fab4d2ccf00,AS000002_023.tif,23,0,29678,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\cd615a44-7b91-4d13-9b26-7fab4d2ccf00,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_023.tif,þþKþþ
0,0,0,0,26,AS000002,AS000002_024,e4a3ffb5-1221-48e3-b833-6c8eb310bee5,AS000002_024.tif,24,0,31349,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\e4a3ffb5-1221-48e3-b833-6c8eb310bee5,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_024.tif,þþKþþ
0,0,0,0,27,AS000002,AS000002_025,8765f161-b000-40ad-a3e9-424fd54144d3,AS000002_025.tif,25,0,30698,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\8765f161-b000-40ad-a3e9-424fd54144d3,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_025.tif,þþKþþ
0,0,0,0,28,AS000002,AS000002_026,c627719a-7a19-45c4-b103-8f7cd546f58e,AS000002_026.tif,26,0,30142,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\c627719a-7a19-45c4-b103-8f7cd546f58e,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_026.tif,þþKþþ
0,0,0,0,29,AS000002,AS000002_027,6b60ffbc-865e-4980-a44f-5c6a7e04478c,AS000002_027.tif,27,0,28973,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\6b60ffbc-865e-4980-a44f-5c6a7e04478c,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_027.tif,þþKþþ
0,0,0,0,30,AS000002,AS000002_028,7b49d6ca-ce35-4ab3-8448-dbecb559693d,AS000002_028.tif,28,0,29180,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\7b49d6ca-ce35-4ab3-8448-dbecb559693d,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_028.tif,þþKþþ
0,0,0,0,31,AS000002,AS000002_029,48c2aba0-bcb2-428e-aa33-71cce0f88ca3,AS000002_029.tif,29,0,7538,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\48c2aba0-bcb2-428e-aa33-71cce0f88ca3,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000002_029.tif,þþKþþ
1,0,0,0,32,AS000003,AS000003,8c937e0c-33ba-4203-97d2-8d9d9682d147,AS000003.tif,0,0,63186,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\8c937e0c-33ba-4203-97d2-8d9d9682d147,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000003.tif,þþKþþ
0,0,0,0,33,AS000003,AS000003_01,4c8c7e2b-8f2b-477e-ba74-71482e18a548,AS000003_01.tif,1,0,537377,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\4c8c7e2b-8f2b-477e-ba74-71482e18a548,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000003_01.tif,þþKþþ
0,0,0,0,34,AS000003,AS000003_02,80867237-a72e-4c2a-8aae-c1b2ae8c549e,AS000003_02.tif,2,0,138412,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\80867237-a72e-4c2a-8aae-c1b2ae8c549e,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000003_02.tif,þþKþþ
0,0,0,0,35,AS000003,AS000003_03,d8ffbeca-5dcc-48ce-8e06-46ff1bd65541,AS000003_03.tif,3,0,87122,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\d8ffbeca-5dcc-48ce-8e06-46ff1bd65541,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000003_03.tif,þþKþþ
1,0,0,0,36,AS000004,AS000004,fae7699b-4121-4758-a9d1-b078db7acbf6,AS000004.tif,0,0,20277,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\fae7699b-4121-4758-a9d1-b078db7acbf6,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000004.tif,þþKþþ
0,0,0,0,37,AS000004,AS000004_01,e4f11a66-98ec-4ce1-8387-d233424fe18c,AS000004_01.tif,1,0,22472,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\e4f11a66-98ec-4ce1-8387-d233424fe18c,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000004_01.tif,þþKþþ
1,0,0,0,38,AS000005,AS000005,945d9abc-a535-42b0-b21c-570725cac188,AS000005.tif,0,0,35421,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\945d9abc-a535-42b0-b21c-570725cac188,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000005.tif,þþKþþ
0,0,0,0,39,AS000005,AS000005_01,e4ce8922-e0d8-4398-82a7-30bcc97f41b0,AS000005_01.tif,1,0,15036,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\e4ce8922-e0d8-4398-82a7-30bcc97f41b0,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000005_01.tif,þþKþþ
0,0,0,0,40,AS000005,AS000005_02,3bb60589-a25c-43cd-9d50-be63e231095c,AS000005_02.tif,2,0,55065,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\3bb60589-a25c-43cd-9d50-be63e231095c,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000005_02.tif,þþKþþ
0,0,0,0,41,AS000005,AS000005_03,f4977a8f-3c00-4ed4-8370-2ffeba4ea03d,AS000005_03.tif,3,0,8124,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\f4977a8f-3c00-4ed4-8370-2ffeba4ea03d,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\AS000005_03.tif,þþKþþ
1,0,0,0,42,EN000001,EN000001,7c91e888-1fe8-4a7e-b904-4ff48f181173,EN000001.tif,0,0,32539,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\7c91e888-1fe8-4a7e-b904-4ff48f181173,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000001.tif,þþKþþ
0,0,0,0,43,EN000001,EN000001_01,4f26e3f3-4993-431a-a8dd-4ab8d08d1d60,EN000001_01.tif,1,0,45266,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\4f26e3f3-4993-431a-a8dd-4ab8d08d1d60,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000001_01.tif,þþKþþ
0,0,0,0,44,EN000001,EN000001_02,75e265ad-6580-416e-a3a2-6579d388676e,EN000001_02.tif,2,0,55522,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\75e265ad-6580-416e-a3a2-6579d388676e,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000001_02.tif,þþKþþ
0,0,0,0,45,EN000001,EN000001_03,0431dc32-0633-4cb5-ad14-566f199f0476,EN000001_03.tif,3,0,56934,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\0431dc32-0633-4cb5-ad14-566f199f0476,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000001_03.tif,þþKþþ
0,0,0,0,46,EN000001,EN000001_04,c7954bab-c978-483d-8392-7af3c4699fb0,EN000001_04.tif,4,0,52800,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\c7954bab-c978-483d-8392-7af3c4699fb0,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000001_04.tif,þþKþþ
0,0,0,0,47,EN000001,EN000001_05,939aa5c3-54a5-4976-b950-8ce76cfe7bdb,EN000001_05.tif,5,0,35386,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\939aa5c3-54a5-4976-b950-8ce76cfe7bdb,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000001_05.tif,þþKþþ
1,0,0,0,48,EN000002,EN000002,6534c151-f9e1-43ae-a5be-726f1d4a4998,EN000002.tif,0,0,39912,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\6534c151-f9e1-43ae-a5be-726f1d4a4998,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000002.tif,þþKþþ
0,0,0,0,49,EN000002,EN000002_01,c3098e1a-42eb-4f12-afbb-c83ace90e42b,EN000002_01.tif,1,0,43989,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\c3098e1a-42eb-4f12-afbb-c83ace90e42b,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000002_01.tif,þþKþþ
0,0,0,0,50,EN000002,EN000002_02,e26630a7-379d-455d-9e1f-7faad14b282a,EN000002_02.tif,2,0,41384,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\e26630a7-379d-455d-9e1f-7faad14b282a,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000002_02.tif,þþKþþ
0,0,0,0,51,EN000002,EN000002_03,d21f20a3-9f9f-48a6-ae46-c9e7e3fa26b9,EN000002_03.tif,3,0,37265,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\d21f20a3-9f9f-48a6-ae46-c9e7e3fa26b9,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000002_03.tif,þþKþþ
1,0,0,0,52,EN000003,EN000003,2ab6498f-552b-4258-b33a-a2a230d35256,EN000003.tif,0,0,43106,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\2ab6498f-552b-4258-b33a-a2a230d35256,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000003.tif,þþKþþ
0,0,0,0,53,EN000003,EN000003_01,31d0c30a-c2ff-458d-aeb3-28c4161f8e05,EN000003_01.tif,1,0,58084,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\31d0c30a-c2ff-458d-aeb3-28c4161f8e05,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000003_01.tif,þþKþþ
0,0,0,0,54,EN000003,EN000003_02,19186002-74ac-413a-a949-bf7e05f0e3c6,EN000003_02.tif,2,0,6022,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\19186002-74ac-413a-a949-bf7e05f0e3c6,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000003_02.tif,þþKþþ
1,0,0,0,55,EN000004,EN000004,694fe524-938a-4ee8-9a31-19a57e0e1c58,EN000004.tif,0,0,43114,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\694fe524-938a-4ee8-9a31-19a57e0e1c58,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000004.tif,þþKþþ
0,0,0,0,56,EN000004,EN000004_01,fb59d6fa-cd3e-4a0c-a269-3679f9dcca0e,EN000004_01.tif,1,0,58084,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\fb59d6fa-cd3e-4a0c-a269-3679f9dcca0e,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000004_01.tif,þþKþþ
0,0,0,0,57,EN000004,EN000004_02,81ca5ff3-219b-49a1-99b3-331cb452ec5e,EN000004_02.tif,2,0,6022,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\81ca5ff3-219b-49a1-99b3-331cb452ec5e,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000004_02.tif,þþKþþ
1,0,0,0,58,EN000005,EN000005,1de2940b-9f61-4ccf-9d6b-2e0a04109689,EN000005.tif,0,0,5331,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\1de2940b-9f61-4ccf-9d6b-2e0a04109689,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000005.tif,þþKþþ
1,0,0,0,59,EN000006,EN000006,6d139a06-2ae3-4788-9e65-6f122f27ad4c,EN000006.tif,0,0,4765,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\6d139a06-2ae3-4788-9e65-6f122f27ad4c,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000006.tif,þþKþþ
1,0,0,0,60,EN000007,EN000007,b4e5480a-8538-4534-84f4-6804d963d15b,EN000007.tif,0,0,23585,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\b4e5480a-8538-4534-84f4-6804d963d15b,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000007.tif,þþKþþ
1,0,0,0,61,EN000008,EN000008,98045c12-8e56-4e23-b00d-36cd9f558d42,EN000008.tif,0,0,34341,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\98045c12-8e56-4e23-b00d-36cd9f558d42,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000008.tif,þþKþþ
1,0,0,0,62,EN000009,EN000009,800a9721-3445-43d0-8ac8-fe6aad7fcb51,EN000009.tif,0,0,18595,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\800a9721-3445-43d0-8ac8-fe6aad7fcb51,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000009.tif,þþKþþ
1,0,0,0,63,EN000010,EN000010,058cbcb1-bc4e-4c0d-b5ce-33bfee08775f,EN000010.tif,0,0,5980,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\058cbcb1-bc4e-4c0d-b5ce-33bfee08775f,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000010.tif,þþKþþ
1,0,0,0,64,EN000011,EN000011,c6b67844-9c67-430d-9418-d5a5958cbc81,EN000011.tif,0,0,4954,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\c6b67844-9c67-430d-9418-d5a5958cbc81,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000011.tif,þþKþþ
1,0,0,0,65,EN000012,EN000012,5dbf3691-194a-45e9-bc6a-994638347459,EN000012.tif,0,0,6644,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\5dbf3691-194a-45e9-bc6a-994638347459,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000012.tif,þþKþþ
1,0,0,0,66,EN000013,EN000013,681c6b76-2942-4ecc-8ba6-fb4c1dbdbb1e,EN000013.tif,0,0,11482,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\681c6b76-2942-4ecc-8ba6-fb4c1dbdbb1e,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000013.tif,þþKþþ
1,0,0,0,67,EN000014,EN000014,d9162157-22d4-49e8-8268-ed3699f58ac0,EN000014.tif,0,0,45830,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\d9162157-22d4-49e8-8268-ed3699f58ac0,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000014.tif,þþKþþ
0,0,0,0,68,EN000014,EN000014_01,ba2095ce-b07d-4843-b82f-1daebb79ed6a,EN000014_01.tif,1,0,1474,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\ba2095ce-b07d-4843-b82f-1daebb79ed6a,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000014_01.tif,þþKþþ
1,0,0,0,69,EN000015,EN000015,4c2f860e-8f74-4c61-897e-90ec62578652,EN000015.tif,0,0,44286,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\4c2f860e-8f74-4c61-897e-90ec62578652,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000015.tif,þþKþþ
1,0,0,0,70,EN000016,EN000016,c52da0b7-cbb2-4e57-bcbf-a4d168d144bc,EN000016.tif,0,0,46507,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\c52da0b7-cbb2-4e57-bcbf-a4d168d144bc,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000016.tif,þþKþþ
0,0,0,0,71,EN000016,EN000016_01,13f002cf-553e-4faf-b678-030d23a9a85c,EN000016_01.tif,1,0,70123,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\13f002cf-553e-4faf-b678-030d23a9a85c,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000016_01.tif,þþKþþ
0,0,0,0,72,EN000016,EN000016_02,7854750b-8f14-4490-9149-8dcfc05893cf,EN000016_02.tif,2,0,51746,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\7854750b-8f14-4490-9149-8dcfc05893cf,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000016_02.tif,þþKþþ
1,0,0,0,73,EN000017,EN000017,de2c471a-2eb3-442f-829b-e3481eeee46f,EN000017.tif,0,0,52518,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\de2c471a-2eb3-442f-829b-e3481eeee46f,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000017.tif,þþKþþ
0,0,0,0,74,EN000017,EN000017_01,93dd5cb2-1c1e-4be5-a15a-7029f4094367,EN000017_01.tif,1,0,79364,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\93dd5cb2-1c1e-4be5-a15a-7029f4094367,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000017_01.tif,þþKþþ
0,0,0,0,75,EN000017,EN000017_02,df396946-c0f6-4c44-9622-3aeef8b3e4df,EN000017_02.tif,2,0,58376,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\df396946-c0f6-4c44-9622-3aeef8b3e4df,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000017_02.tif,þþKþþ
1,0,0,0,76,EN000018,EN000018,2658007a-fd1d-47ce-928b-17239a229511,EN000018.tif,0,0,20921,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\2658007a-fd1d-47ce-928b-17239a229511,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000018.tif,þþKþþ
0,0,0,0,77,EN000018,EN000018_01,e8d2a1e4-091f-4d20-8307-c1193f66cd6c,EN000018_01.tif,1,0,26544,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\e8d2a1e4-091f-4d20-8307-c1193f66cd6c,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000018_01.tif,þþKþþ
0,0,0,0,78,EN000018,EN000018_02,fed5df43-4715-4ef1-b83e-e565ef187c82,EN000018_02.tif,2,0,26168,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\fed5df43-4715-4ef1-b83e-e565ef187c82,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000018_02.tif,þþKþþ
0,0,0,0,79,EN000018,EN000018_03,4a5503a6-838c-477b-be28-134206a7c0cd,EN000018_03.tif,3,0,22676,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\4a5503a6-838c-477b-be28-134206a7c0cd,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000018_03.tif,þþKþþ
0,0,0,0,80,EN000018,EN000018_04,3986ff37-cba2-4054-9b1b-7827a8de4c72,EN000018_04.tif,4,0,18039,\\localhost\files\EDDS1015054\RV_55f36d7b-4eb9-4f7f-aba2-b9a7b6b460a3\3986ff37-cba2-4054-9b1b-7827a8de4c72,\\192.168.14.18\SvP_ExportII\VOL01\IMG001\EN000018_04.tif,þþKþþ
</string>.Value

			'Fixing Up line endings
			originalBatchSizeWith100Images = originalBatchSizeWith100Images.Replace(Chr(10), vbCrLf)
			expectedReducedFileWith80Rows = expectedReducedFileWith80Rows.Replace(Chr(10), vbCrLf)

			Dim bulkImporter As MockForLoweBatchSizeBulkImageFileImporter = New MockForLoweBatchSizeBulkImageFileImporter(78, originalBatchSizeWith100Images, _args, _controller, _guid, False, False, New MockBulkImportManagerWebExceptions(True))
			bulkImporter.MockLowerBatchSizeAndRetry(100)
			Assert.AreEqual(expectedReducedFileWith80Rows, bulkImporter._outPutFromStringWriter.ToString())
		End Sub

#End Region
	End Class
End Namespace

