' -----------------------------------------------------------------------------------------------------
' <copyright file="BulkImportManagerTests.cs" company="Relativity ODA LLC">
'   © Relativity All Rights Reserved.
' </copyright>
' -----------------------------------------------------------------------------------------------------

Imports kCura.EDDS.WebAPI.BulkImportManagerBase
Imports NUnit.Framework
Imports BulkImportManager = kCura.WinEDDS.Service.BulkImportManager

Namespace Relativity.DataExchange.Import.NUnit

	<TestFixture>
	Public Class BulkImportManagerTests

		'pulled straight from the sql timeout exception
		Private Const TimeoutMessage As String = "Error: Timeout expired.  The timeout period elapsed prior to completion of the operation or the server is not responding."
#Region "Members"
		Private _keyPathExistsAlready As Boolean
		Private _keyValExistsAlready As Boolean
		Private _settings As ImageLoadInfo
#End Region

#Region " Setup "

		<SetUp()> Public Sub SetUp()
			kCura.WinEDDS.Config.ProgrammaticServiceURL = "https://r1.kcura.com/RelativityWebAPI/"
			_keyPathExistsAlready = RegKeyHelper.SubKeyPathExists(RegKeyHelper.RelativityKeyPath)
			_keyValExistsAlready = False
			If _keyPathExistsAlready = True Then
				_keyValExistsAlready = RegKeyHelper.SubKeyExists(RegKeyHelper.RelativityKeyPath, RegKeyHelper.RelativityServiceURLKey)
			End If

			If _keyValExistsAlready = False Then
				RegKeyHelper.CreateKeyWithValueOnPath(Not _keyPathExistsAlready, RegKeyHelper.RelativityKeyPath, RegKeyHelper.RelativityServiceURLKey, RegKeyHelper.RelativityDefaultServiceURL)
			End If

			_settings = New ImageLoadInfo With {
	 .BulkFileName = "",
	 .UploadFullText = False,
	 .Overlay = OverwriteType.Both,
	 .DestinationFolderArtifactID = 0,
	 .Repository = "",
	 .UseBulkDataImport = False,
	 .RunID = "0",
	 .KeyFieldArtifactID = 0
	 }
		End Sub

		<TearDown()> Public Sub TakeDown()
			If _keyValExistsAlready = False Then
				RegKeyHelper.RemoveKeyPath(RegKeyHelper.RelativityKeyPath)
			End If
		End Sub
#End Region

#Region " Should throw Bulk SQL exceptions "
		<Test()>
		Public Sub Image_Import_Failure_Throws_Exception()
			Using manager As New MockBulkImportManager(True)
				Assert.Throws(Of BulkImportManager.BulkImportSqlException)(Sub() manager.BulkImportImage(0, _settings, False))
			End Using
		End Sub

		<Test()>
		Public Sub CheckResultsForException_should_throw_BulkImportSqlException()
			Using manager As New MockBulkImportManager(True)
				Dim results = New MassImportResults()
				results.ExceptionDetail = New SoapExceptionDetail()
				results.ExceptionDetail.ExceptionMessage = "Some message --nothing specific-- rest of message"
				Assert.Throws(Of BulkImportManager.BulkImportSqlException)(Sub() manager.CheckResultsForExceptionPublic(results))
			End Using
		End Sub

		<Test()>
		Public Sub CheckResultsForException_should_throw_BulkImportSqlTimeoutException()
			Using manager As New MockBulkImportManager(True)
				Dim results = New MassImportResults()
				results.ExceptionDetail = New SoapExceptionDetail()
				results.ExceptionDetail.ExceptionMessage = "Some message Timeout expired rest of message"
				Assert.Throws(Of BulkImportManager.BulkImportSqlTimeoutException)(Sub() manager.CheckResultsForExceptionPublic(results))
			End Using
		End Sub

		<Test()>
		Public Sub CheckResultsForException_should_throw_InsufficientPermissionsForImportException()
			Using manager As New MockBulkImportManager(True)
				Dim results = New MassImportResults()
				results.ExceptionDetail = New SoapExceptionDetail()
				results.ExceptionDetail.ExceptionMessage = "Some message ##InsufficientPermissionsForImportException## rest of message"
				Assert.Throws(Of BulkImportManager.InsufficientPermissionsForImportException)(Sub() manager.CheckResultsForExceptionPublic(results))
			End Using
		End Sub

		<Test()>
		Public Sub CheckResultsForException_should_add_message_for_stack_overflow()
			Using manager As New MockBulkImportManager(True)
				Dim results = New MassImportResults()
				results.ExceptionDetail = New SoapExceptionDetail()
				results.ExceptionDetail.ExceptionMessage = "Some message Server stack limit has been reached rest of message"
				Dim exception = Assert.Throws(Of BulkImportManager.BulkImportSqlException)(Sub() manager.CheckResultsForExceptionPublic(results))
				Assert.That(exception.Message.Contains("Try to import less fields"))
			End Using
		End Sub

		<Test()>
		Public Sub CheckResultsForException_should_not_throw_when_exception_details_are_empty()
			Using manager As New MockBulkImportManager(True)
				Dim results = New MassImportResults()
				manager.CheckResultsForExceptionPublic(results)
			End Using
		End Sub

		<Test()>
		Public Sub Production_Image_Import_Failure_Throws_Exception()
			Using manager As New MockBulkImportManager(True)
				Assert.Throws(Of BulkImportManager.BulkImportSqlException)(Sub() manager.BulkImportProductionImage(0, _settings, 0, False))
			End Using
		End Sub

		<Test()>
		Public Sub Native_Import_Failure_Throws_Exception()
			Using manager As New MockBulkImportManager(True)
				Assert.Throws(Of BulkImportManager.BulkImportSqlException)(Sub() manager.BulkImportNative(0, Nothing, False, False))
			End Using
		End Sub

		<Test()>
		Public Sub Object_Import_Failure_Throws_Exception()
			Using manager As New MockBulkImportManager(True)
				Assert.Throws(Of BulkImportManager.BulkImportSqlException)(Sub() manager.BulkImportObjects(0, Nothing, False))
			End Using
		End Sub
#End Region

#Region " Should throw Bulk SQL Timeout exceptions "
		<Test()>
		Public Sub Image_Import_Timeout_Throws_Timeout_Exception()
			Using manager As New MockBulkImportManager(TimeoutMessage)
				Assert.Throws(Of BulkImportManager.BulkImportSqlTimeoutException)(Sub() manager.BulkImportImage(0, _settings, False))
			End Using
		End Sub

		<Test()>
		Public Sub Production_Image_Import_Timeout_Throws_Timeout_Exception()
			Using manager As New MockBulkImportManager(TimeoutMessage)
				Assert.Throws(Of BulkImportManager.BulkImportSqlTimeoutException)(Sub() manager.BulkImportProductionImage(0, _settings, 0, False))
			End Using
		End Sub

		<Test()>
		Public Sub Native_Import_Timeout_Throws_Timeout_Exception()
			Using manager As New MockBulkImportManager(TimeoutMessage)
				Assert.Throws(Of BulkImportManager.BulkImportSqlTimeoutException)(Sub() manager.BulkImportNative(0, Nothing, False, False))
			End Using
		End Sub

		<Test()>
		Public Sub Object_Import_Timeout_Throws_Timeout_Exception()
			Using manager As New MockBulkImportManager(TimeoutMessage)
				Assert.Throws(Of BulkImportManager.BulkImportSqlTimeoutException)(Sub() manager.BulkImportObjects(0, Nothing, False))
			End Using
		End Sub
#End Region

#Region " Shouldn't throw exceptions "
		<Test()> Public Sub Image_Import_Worked_No_Exception()
			Using manager As New MockBulkImportManager(False)
				Dim retval As MassImportResults = manager.BulkImportImage(0, _settings, False)
				Assert.AreEqual(Nothing, retval.ExceptionDetail)
			End Using
		End Sub

		<Test()> Public Sub Production_Image_Import_Worked_No_Exception()
			Using manager As New MockBulkImportManager(False)
				Dim retval As MassImportResults = manager.BulkImportProductionImage(0, _settings, 0, False)
				Assert.AreEqual(Nothing, retval.ExceptionDetail)
			End Using
		End Sub

		<Test()> Public Sub Native_Import_Worked_No_Exception()
			Using manager As New MockBulkImportManager(False)
				Dim retval As MassImportResults = manager.BulkImportNative(0, Nothing, False, False)
				Assert.AreEqual(Nothing, retval.ExceptionDetail)
			End Using
		End Sub

		<Test()> Public Sub Object_Import_Worked_No_Exception()
			Using manager As New MockBulkImportManager(False)
				Dim retval As MassImportResults = manager.BulkImportObjects(0, Nothing, False)
				Assert.AreEqual(Nothing, retval.ExceptionDetail)
			End Using
		End Sub
#End Region


#Region " Mock Classes "

		''' <summary>
		''' Mock BulkImportManager for testing when the webservice throws bulk SQL Exceptions
		''' </summary>
		Public Class MockBulkImportManager
			Inherits BulkImportManager

			Public Property ErrorMessage As SoapExceptionDetail

			Public Sub New(ByVal throwsException As Boolean)
				MyBase.New(Nothing, Nothing)
				If throwsException Then
					Me.ErrorMessage = New SoapExceptionDetail()
				Else
					Me.ErrorMessage = Nothing
				End If
			End Sub

			Public Sub New(ByVal exceptionMessage As String)
				MyBase.New(Nothing, Nothing)
				If exceptionMessage IsNot Nothing Then
					Me.ErrorMessage = New SoapExceptionDetail()
					Me.ErrorMessage.ExceptionMessage = exceptionMessage
				Else
					Me.ErrorMessage = Nothing
				End If
			End Sub
			Public Sub CheckResultsForExceptionPublic(ByVal results As MassImportResults)
				CheckResultsForException(results)
			End Sub

			Protected Overrides Function InvokeBulkImportImage(ByVal appID As Integer, ByVal settings As ImageLoadInfo, ByVal inRepository As Boolean) As MassImportResults
				Dim retval As New MassImportResults
				retval.ExceptionDetail = Me.ErrorMessage
				Return retval
			End Function

			Protected Overrides Function InvokeBulkImportNative(ByVal appID As Integer, ByVal settings As NativeLoadInfo, ByVal inRepository As Boolean, ByVal includeExtractedTextEncoding As Boolean) As MassImportResults
				Dim retval As New MassImportResults
				retval.ExceptionDetail = Me.ErrorMessage
				Return retval
			End Function

			Protected Overrides Function InvokeBulkImportObjects(ByVal appID As Integer, ByVal settings As ObjectLoadInfo, ByVal inRepository As Boolean) As MassImportResults
				Dim retval As New MassImportResults
				retval.ExceptionDetail = Me.ErrorMessage
				Return retval
			End Function

			Protected Overrides Function InvokeBulkImportProductionImage(ByVal appID As Integer, ByVal settings As ImageLoadInfo, ByVal productionKeyFieldArtifactID As Integer, ByVal inRepository As Boolean) As MassImportResults
				Dim retval As New MassImportResults
				retval.ExceptionDetail = Me.ErrorMessage
				Return retval
			End Function

		End Class

#End Region

	End Class
End Namespace