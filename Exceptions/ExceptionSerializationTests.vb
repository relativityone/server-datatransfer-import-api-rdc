Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary
Imports kCura.WinEDDS.Exceptions

Imports NUnit.Framework

Namespace kCura.WinEDDS.NUnit

	<TestFixture()>
	Public Class ExceptionSerializationTests

		<Test>
		Public Sub ItShouldSerializeAndDeserializeTheWinEddsException()
			SerializeAndDeserialize(new kCura.WinEDDS.Credentials.CredentialsNotSetException())
			SerializeAndDeserialize(new kCura.WinEDDS.Exceptions.CodeCreationFailedException("Message"))
			SerializeAndDeserialize(new kCura.WinEDDS.Exceptions.CredentialsNotSupportedException("Message"))
	        SerializeAndDeserialize(new kCura.WinEDDS.Exceptions.FieldValueImportException(1, "Message", "additional"))
			SerializeAndDeserialize(new kCura.WinEDDS.Exceptions.FileWriteException(New FileWriteException.DestinationFile(),  new InvalidOperationException()))
	        SerializeAndDeserialize(new kCura.WinEDDS.Exceptions.InvalidLoginException("Message"))
	        SerializeAndDeserialize(new kCura.WinEDDS.Exceptions.InvalidPackageException("Message"))
	        SerializeAndDeserialize(new kCura.WinEDDS.RelativityVersionMismatchException("Message"))
	        SerializeAndDeserialize(new kCura.WinEDDS.Exceptions.WebApiConnectionFailureException("Message"))
	        SerializeAndDeserialize(new kCura.WinEDDS.Exceptions.WebDownloadCorruptException("Message"))
	        SerializeAndDeserialize(new kCura.WinEDDS.FileDownloader.DistributedReLoginException())
	        SerializeAndDeserialize(new kCura.WinEDDS.BulkLoadFileImporter.IdentityValueNotSetException())
	        SerializeAndDeserialize(new kCura.WinEDDS.BulkLoadFileImporter.ExtractedTextFileNotFoundException())
	        SerializeAndDeserialize(new kCura.WinEDDS.BulkLoadFileImporter.ExtractedTextTooLargeException())
	        SerializeAndDeserialize(new kCura.WinEDDS.CodeValidator.CodeCreationException(true, "Message"))
	        SerializeAndDeserialize(new kCura.WinEDDS.LoadFileBase.ExtractedTextTooLargeException())
	        SerializeAndDeserialize(new kCura.WinEDDS.LoadFileBase.IdentifierOverlapException("Message", "Message"))
	        SerializeAndDeserialize(new kCura.WinEDDS.LoadFileBase.MissingFullTextFileException(1, 1))
	        SerializeAndDeserialize(new kCura.WinEDDS.LoadFileBase.MissingUserException(1, 1, "Message"))
	        SerializeAndDeserialize(new kCura.WinEDDS.LoadFileBase.CodeCreationException(1, 1, false, "Message"))
	        SerializeAndDeserialize(new kCura.WinEDDS.LoadFileBase.ColumnCountMismatchException(1, 1, 1))
	        SerializeAndDeserialize(new kCura.WinEDDS.LoadFileBase.DuplicateObjectReferenceException(1, 1, "Message"))
	        SerializeAndDeserialize(new kCura.WinEDDS.LoadFileBase.NonExistentParentException(1, 1, "Message"))
	        SerializeAndDeserialize(new kCura.WinEDDS.LoadFileBase.ParentObjectReferenceRequiredException(1, 1))
	        SerializeAndDeserialize(new kCura.WinEDDS.LoadFileBase.BcpPathAccessException("Message"))
	        SerializeAndDeserialize(new kCura.WinEDDS.LoadFileBase.DuplicateMulticodeValueException(1, 1, "Message"))
	        SerializeAndDeserialize(new kCura.WinEDDS.OpticonFileReader.InvalidLineFormatException(1, 1))
	        SerializeAndDeserialize(new kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlException(new kCura.EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail()))
	        SerializeAndDeserialize(new kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlTimeoutException("Message"))
	        SerializeAndDeserialize(new kCura.WinEDDS.Service.BulkImportManager.InsufficientPermissionsForImportException("Message"))
	        SerializeAndDeserialize(new kCura.WinEDDS.Service.ExportManager.InsufficientPermissionsForExportException("Message"))
	        SerializeAndDeserialize(new kCura.WinEDDS.Service.FileIO.CustomException("Message", new InvalidOperationException()))
	        SerializeAndDeserialize(new kCura.WinEDDS.SettingsFactoryBase.InvalidCredentialsException("Message", new InvalidOperationException()))
		End Sub	

		Private Sub SerializeAndDeserialize(ByVal exception As System.Exception)
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