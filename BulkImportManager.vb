Imports NUnit.Framework

Namespace kCura.WinEDDS.NUnit.Service

	<TestFixture()> Public Class BulkImportManager

		'pulled straight from the sql timeout exception
		Private Const TimeoutMessage As String = "Error: Timeout expired.  The timeout period elapsed prior to completion of the operation or the server is not responding."
#Region "Members"
		Private _keyPathExistsAlready As Boolean
		Private _keyValExistsAlready As Boolean
		Private _settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo
#End Region

#Region " Setup "

		<SetUp()> Public Sub SetUp()
			_keyPathExistsAlready = RegKeyHelper.SubKeyPathExists(RegKeyHelper.RelativityKeyPath)
			_keyValExistsAlready = False
			If _keyPathExistsAlready = True Then
				_keyValExistsAlready = RegKeyHelper.SubKeyExists(RegKeyHelper.RelativityKeyPath, RegKeyHelper.RelativityServiceURLKey)
			End If

			If _keyValExistsAlready = False Then
				RegKeyHelper.CreateKeyWithValueOnPath(Not _keyPathExistsAlready, RegKeyHelper.RelativityKeyPath, RegKeyHelper.RelativityServiceURLKey, RegKeyHelper.RelativityDefaultServiceURL)
			End If

			_settings = New kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo With {
	 .BulkFileName = "",
	 .UploadFullText = False,
	 .Overlay = kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType.Both,
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
		<ExpectedException(GetType(WinEDDS.Service.BulkImportManager.BulkImportSqlException))>
		Public Sub Image_Import_Failure_Throws_Exception()
			Dim manager As New MockBulkImportManager(True)
			manager.BulkImportImage(0, _settings, False)
		End Sub

		<Test()>
		<ExpectedException(GetType(WinEDDS.Service.BulkImportManager.BulkImportSqlException))>
		Public Sub Production_Image_Import_Failure_Throws_Exception()
			Dim manager As New MockBulkImportManager(True)
			manager.BulkImportProductionImage(0, _settings, 0, False)
		End Sub

		<Test()>
		 <ExpectedException(GetType(WinEDDS.Service.BulkImportManager.BulkImportSqlException))>
		Public Sub Native_Import_Failure_Throws_Exception()
			Dim manager As New MockBulkImportManager(True)
			manager.BulkImportNative(0, Nothing, False, False, -1)
		End Sub

		<Test()>
		 <ExpectedException(GetType(WinEDDS.Service.BulkImportManager.BulkImportSqlException))>
		Public Sub Object_Import_Failure_Throws_Exception()
			Dim manager As New MockBulkImportManager(True)
			manager.BulkImportObjects(0, Nothing, False)
		End Sub
#End Region

#Region " Should throw Bulk SQL Timeout exceptions "
		<Test()>
		<ExpectedException(GetType(WinEDDS.Service.BulkImportManager.BulkImportSqlTimeoutException))>
		Public Sub Image_Import_Timeout_Throws_Timeout_Exception()
			Dim manager As New MockBulkImportManager(TimeoutMessage)
			manager.BulkImportImage(0, _settings, False)
		End Sub

		<Test()>
		<ExpectedException(GetType(WinEDDS.Service.BulkImportManager.BulkImportSqlTimeoutException))>
		Public Sub Production_Image_Import_Timeout_Throws_Timeout_Exception()
			Dim manager As New MockBulkImportManager(TimeoutMessage)
			manager.BulkImportProductionImage(0, _settings, 0, False)
		End Sub

		<Test()>
		 <ExpectedException(GetType(WinEDDS.Service.BulkImportManager.BulkImportSqlTimeoutException))>
		Public Sub Native_Import_Timeout_Throws_Timeout_Exception()
			Dim manager As New MockBulkImportManager(TimeoutMessage)
			manager.BulkImportNative(0, Nothing, False, False, -1)
		End Sub

		<Test()>
		 <ExpectedException(GetType(WinEDDS.Service.BulkImportManager.BulkImportSqlTimeoutException))>
		Public Sub Object_Import_Timeout_Throws_Timeout_Exception()
			Dim manager As New MockBulkImportManager(TimeoutMessage)
			manager.BulkImportObjects(0, Nothing, False)
		End Sub
#End Region

#Region " Shouldn't throw exceptions "
		<Test()> Public Sub Image_Import_Worked_No_Exception()
			Dim manager As New MockBulkImportManager(False)
			Dim retval As EDDS.WebAPI.BulkImportManagerBase.MassImportResults = manager.BulkImportImage(0, _settings, False)
			Assert.AreEqual(Nothing, retval.ExceptionDetail)
		End Sub

		<Test()> Public Sub Production_Image_Import_Worked_No_Exception()
			Dim manager As New MockBulkImportManager(False)
			Dim retval As EDDS.WebAPI.BulkImportManagerBase.MassImportResults = manager.BulkImportProductionImage(0, _settings, 0, False)
			Assert.AreEqual(Nothing, retval.ExceptionDetail)
		End Sub

		<Test()> Public Sub Native_Import_Worked_No_Exception()
			Dim manager As New MockBulkImportManager(False)
			Dim retval As EDDS.WebAPI.BulkImportManagerBase.MassImportResults = manager.BulkImportNative(0, Nothing, False, False, -1)
			Assert.AreEqual(Nothing, retval.ExceptionDetail)
		End Sub

		<Test()> Public Sub Object_Import_Worked_No_Exception()
			Dim manager As New MockBulkImportManager(False)
			Dim retval As EDDS.WebAPI.BulkImportManagerBase.MassImportResults = manager.BulkImportObjects(0, Nothing, False)
			Assert.AreEqual(Nothing, retval.ExceptionDetail)
		End Sub
#End Region


#Region " Mock Classes "

		''' <summary>
		''' Mock BulkImportManager for testing when the webservice throws bulk SQL Exceptions
		''' </summary>
		''' <remarks></remarks>
		Public Class MockBulkImportManager
			Inherits kCura.WinEDDS.Service.BulkImportManager

			Public Property ErrorMessage As EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail

			Public Sub New(ByVal throwsException As Boolean)
				MyBase.new(Nothing, Nothing)
				If throwsException Then
					Me.ErrorMessage = New EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail()
				Else
					Me.ErrorMessage = Nothing
				End If
			End Sub

			Public Sub New(ByVal exceptionMessage As String)
				MyBase.new(Nothing, Nothing)
				If exceptionMessage IsNot Nothing Then
					Me.ErrorMessage = New EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail()
					Me.ErrorMessage.ExceptionMessage = exceptionMessage
				Else
					Me.ErrorMessage = Nothing
				End If
			End Sub

			Protected Overrides Function InvokeBulkImportImage(ByVal appID As Integer, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo, ByVal inRepository As Boolean) As EDDS.WebAPI.BulkImportManagerBase.MassImportResults
				Dim retval As New EDDS.WebAPI.BulkImportManagerBase.MassImportResults
				retval.ExceptionDetail = Me.ErrorMessage
				Return retval
			End Function

			Protected Overrides Function InvokeBulkImportNative(ByVal appID As Integer, ByVal settings As EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo, ByVal inRepository As Boolean, ByVal includeExtractedTextEncoding As Boolean, ByVal rootFolderID As Int32) As EDDS.WebAPI.BulkImportManagerBase.MassImportResults
				Dim retval As New EDDS.WebAPI.BulkImportManagerBase.MassImportResults
				retval.ExceptionDetail = Me.ErrorMessage
				Return retval
			End Function

			Protected Overrides Function InvokeBulkImportObjects(ByVal appID As Integer, ByVal settings As EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo, ByVal inRepository As Boolean) As EDDS.WebAPI.BulkImportManagerBase.MassImportResults
				Dim retval As New EDDS.WebAPI.BulkImportManagerBase.MassImportResults
				retval.ExceptionDetail = Me.ErrorMessage
				Return retval
			End Function

			Protected Overrides Function InvokeBulkImportProductionImage(ByVal appID As Integer, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo, ByVal productionKeyFieldArtifactID As Integer, ByVal inRepository As Boolean) As EDDS.WebAPI.BulkImportManagerBase.MassImportResults
				Dim retval As New EDDS.WebAPI.BulkImportManagerBase.MassImportResults
				retval.ExceptionDetail = Me.ErrorMessage
				Return retval
			End Function

		End Class

#End Region

	End Class
End Namespace