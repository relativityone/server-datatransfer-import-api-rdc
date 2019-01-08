Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary
Imports kCura.WinEDDS.Exceptions

Imports NUnit.Framework

Namespace kCura.WinEDDS.NUnit

	<TestFixture()>
	Public Class ExceptionSerializationTests

		Public Shared ReadOnly Iterator Property TestCases() As IEnumerable
			Get
				Yield New kCura.WinEDDS.Credentials.CredentialsNotSetException()
				Yield New kCura.WinEDDS.Exceptions.CodeCreationFailedException("Message")
				Yield New kCura.WinEDDS.Exceptions.CredentialsNotSupportedException("Message")
				Yield New kCura.WinEDDS.Exceptions.FieldValueImportException(1, "Message", "additional")
				Yield New kCura.WinEDDS.Exceptions.FieldValueImportException(New InvalidOperationException(), 1, "Message", "additional")
				Yield New kCura.WinEDDS.Exceptions.FileWriteException(New FileWriteException.DestinationFile(), New InvalidOperationException())
				Yield New kCura.WinEDDS.Exceptions.InvalidLoginException("Message")
				Yield New kCura.WinEDDS.Exceptions.InvalidLoginException("Message", New InvalidOperationException())
				Yield New kCura.WinEDDS.Exceptions.InvalidPackageException("Message")
				Yield New kCura.WinEDDS.Exceptions.InvalidPackageException("Message", New InvalidOperationException())
				Yield New kCura.WinEDDS.RelativityVersionMismatchException("Message", "RelativityVersion", "ClientVersion")
				Yield new kCura.WinEDDS.Exceptions.WebApiException("Message")
				Yield new kCura.WinEDDS.Exceptions.WebApiException("Message", New InvalidOperationException())
		        Yield new kCura.WinEDDS.Exceptions.WebDownloadCorruptException("Message")
		        Yield new kCura.WinEDDS.FileDownloader.DistributedReLoginException()
		        Yield new kCura.WinEDDS.BulkLoadFileImporter.IdentityValueNotSetException()
		        Yield new kCura.WinEDDS.BulkLoadFileImporter.ExtractedTextFileNotFoundException()
		        Yield new kCura.WinEDDS.BulkLoadFileImporter.ExtractedTextTooLargeException()
		        Yield new kCura.WinEDDS.CodeValidator.CodeCreationException(True, "Message")
		        Yield new kCura.WinEDDS.LoadFileBase.ExtractedTextTooLargeException()
		        Yield new kCura.WinEDDS.LoadFileBase.IdentifierOverlapException("Message", "Message")
		        Yield new kCura.WinEDDS.LoadFileBase.MissingFullTextFileException(1, 1)
		        Yield new kCura.WinEDDS.LoadFileBase.MissingUserException(1, 1, "Message")
		        Yield new kCura.WinEDDS.LoadFileBase.CodeCreationException(1, 1, False, "Message")
		        Yield new kCura.WinEDDS.LoadFileBase.ColumnCountMismatchException(1, 1, 1)
		        Yield new kCura.WinEDDS.LoadFileBase.DuplicateObjectReferenceException(1, 1, "Message")
		        Yield new kCura.WinEDDS.LoadFileBase.NonExistentParentException(1, 1, "Message")
		        Yield new kCura.WinEDDS.LoadFileBase.ParentObjectReferenceRequiredException(1, 1)
		        Yield new kCura.WinEDDS.LoadFileBase.BcpPathAccessException("Message")
		        Yield new kCura.WinEDDS.LoadFileBase.DuplicateMulticodeValueException(1, 1, "Message")
		        Yield new kCura.WinEDDS.OpticonFileReader.InvalidLineFormatException(1, 1)
				Yield new kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlException("Message")
				Yield new kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlException("Message", New InvalidOperationException())
				Yield new kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlException(New kCura.EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail())
				Yield new kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlException(New kCura.EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail() With { .ExceptionMessage = "Message" } )
				Yield new kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlException(New kCura.EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail() With { .ExceptionMessage = "" } )
		        Yield new kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlTimeoutException("Message")
				Yield new kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlTimeoutException("Message", New InvalidOperationException())
		        Yield new kCura.WinEDDS.Service.BulkImportManager.InsufficientPermissionsForImportException("Message")
				Yield new kCura.WinEDDS.Service.BulkImportManager.InsufficientPermissionsForImportException("Message", New InvalidOperationException())
				Yield new kCura.WinEDDS.Service.BulkImportManager.InsufficientPermissionsForImportException(New kCura.EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail())
				Yield new kCura.WinEDDS.Service.BulkImportManager.InsufficientPermissionsForImportException(New kCura.EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail() With { .ExceptionMessage = "Message" } )
				Yield new kCura.WinEDDS.Service.BulkImportManager.InsufficientPermissionsForImportException(New kCura.EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail() With { .ExceptionMessage = "" } )
		        Yield new kCura.WinEDDS.Service.ExportManager.InsufficientPermissionsForExportException("Message")
				Yield new kCura.WinEDDS.Service.ExportManager.InsufficientPermissionsForExportException("Message", New InvalidOperationException())
		        Yield new kCura.WinEDDS.Service.FileIO.CustomException("Message", New InvalidOperationException())
				Yield new kCura.WinEDDS.SettingsFactoryBase.InvalidCredentialsException()
				Yield new kCura.WinEDDS.SettingsFactoryBase.InvalidCredentialsException("Message")
		        Yield new kCura.WinEDDS.SettingsFactoryBase.InvalidCredentialsException("Message", New InvalidOperationException())
			End Get
		End Property
	
		<Test>
		<TestCaseSource(nameof(TestCases))>
		Public Sub ItShouldSerializeAndDeserializeTheWinEddsException(ByVal exception As Exception)
			Dim formatter As IFormatter = New BinaryFormatter()
			Using stream As MemoryStream = New MemoryStream()
				formatter.Serialize(stream, exception)
				stream.Seek(0, SeekOrigin.Begin)
				Dim deserializedException As Exception = CType(formatter.Deserialize(stream), Exception)
				Assert.IsNotNull(deserializedException)
			End Using
		End Sub
	End Class
End Namespace
