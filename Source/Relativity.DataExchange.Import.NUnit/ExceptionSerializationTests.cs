// -----------------------------------------------------------------------------------------------------
// <copyright file="ExceptionSerializationTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exceptions;

	using Relativity.DataExchange.Io;

	[TestFixture]
	public static class ExceptionSerializationTests
	{
		public static IEnumerable ExceptionTypeTestCases
		{
			get
			{
				Func<Exception, bool> validateNoOp = exception => true;
				yield return new TestCaseData(new kCura.WinEDDS.Credentials.CredentialsNotSetException(), validateNoOp);
				yield return new TestCaseData(new kCura.WinEDDS.Exceptions.CodeCreationFailedException("Message"), validateNoOp);
				yield return new TestCaseData(new kCura.WinEDDS.Exceptions.CredentialsNotSupportedException("Message"), validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Exceptions.FieldValueImportException(1, "Message", "additional"), validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Exceptions.FieldValueImportException(
						new InvalidOperationException(),
						1,
						"Message",
						"additional"),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Exceptions.FileWriteException(
						FileWriteException.DestinationFile.Generic,
						new InvalidOperationException()),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Exceptions.InvalidLoginException("Message"),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Exceptions.InvalidLoginException("Message", new InvalidOperationException()),
					validateNoOp);
				yield return new TestCaseData(new kCura.WinEDDS.Exceptions.WebApiException("Message"), validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Exceptions.WebApiException("Message", new InvalidOperationException()),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Exceptions.WebDownloadCorruptException("Message"),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.FileDownloader.DistributedReLoginException(),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.BulkLoadFileImporter.IdentityValueNotSetException(),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.BulkLoadFileImporter.ExtractedTextFileNotFoundException(), validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.BulkLoadFileImporter.ExtractedTextTooLargeException(),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.CodeValidator.CodeCreationException(true, "Message"),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.LoadFileBase.ExtractedTextTooLargeException(),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.LoadFileBase.IdentifierOverlapException("Message", "Message"),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.LoadFileBase.MissingFullTextFileException(1, 1),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.LoadFileBase.MissingUserException(1, 1, "Message"),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.LoadFileBase.CodeCreationException(1, 1, false, "Message"),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.LoadFileBase.ColumnCountMismatchException(1, 1, 1),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.LoadFileBase.DuplicateObjectReferenceException(1, 1, "Message"), validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.LoadFileBase.NonExistentParentException(1, 1, "Message"), validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.LoadFileBase.ParentObjectReferenceRequiredException(1, 1),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.LoadFileBase.DuplicateMulticodeValueException(1, 1, "Message"), validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.OpticonFileReader.InvalidLineFormatException(1, 1),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlException("Message"), validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlException(
						"Message",
						new InvalidOperationException()),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlException(
						new kCura.EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail()),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlException(
						new kCura.EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail
							{
								ExceptionMessage = "Message",
							}),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlException(
						new kCura.EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail
							{
								ExceptionMessage = string.Empty,
							}),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlTimeoutException("Message"), validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlTimeoutException(
						"Message",
						new InvalidOperationException()),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Service.BulkImportManager.InsufficientPermissionsForImportException(
						"Message"), validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Service.BulkImportManager.InsufficientPermissionsForImportException(
						"Message", new InvalidOperationException()), validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Service.BulkImportManager.InsufficientPermissionsForImportException(
						new kCura.EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail()), validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Service.BulkImportManager.InsufficientPermissionsForImportException(
						new kCura.EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail
						{ ExceptionMessage = "Message" }), validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Service.BulkImportManager.InsufficientPermissionsForImportException(
						new kCura.EDDS.WebAPI.BulkImportManagerBase.SoapExceptionDetail { ExceptionMessage = string.Empty }),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Service.ExportManager.InsufficientPermissionsForExportException("Message"),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Service.ExportManager.InsufficientPermissionsForExportException(
						"Message", new InvalidOperationException()), validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.Service.FileIO.CustomException("Message", new InvalidOperationException()),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.SettingsFactoryBase.InvalidCredentialsException(),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.SettingsFactoryBase.InvalidCredentialsException("Message"),
					validateNoOp);
				yield return new TestCaseData(
					new kCura.WinEDDS.SettingsFactoryBase.InvalidCredentialsException(
						"Message",
						new InvalidOperationException()),
					validateNoOp);
				yield return new TestCaseData(new FileTypeIdentifyException(), validateNoOp);
				yield return new TestCaseData(
					new FileTypeIdentifyException("Message"),
					validateNoOp);
				yield return new TestCaseData(
					new FileTypeIdentifyException("Message", new InvalidOperationException()),
					validateNoOp);
				yield return new TestCaseData(
					new FileTypeIdentifyException("Message", new InvalidOperationException(), FileTypeIdentifyError.Io),
					validateNoOp);
				yield return new TestCaseData(
					new ImportCredentialException("Message", "username", "url"),
					validateNoOp);
				yield return new TestCaseData(new ImportSettingsException("setting"), validateNoOp);
				yield return new TestCaseData(new ImportSettingsException("setting", "additionalInfo"), validateNoOp);
				yield return new TestCaseData(
					new ImportSettingsConflictException("setting", "conflictingSettings", "Message"), validateNoOp);
			}
		}

		[Test]
		[TestCaseSource(nameof(ExceptionTypeTestCases))]
		public static void ItShouldSerializeAndDeserializeTheException(Exception exception, Func<Exception, bool> validate)
		{
			if (exception == null)
			{
				throw new ArgumentNullException(nameof(exception));
			}

			if (validate == null)
			{
				throw new ArgumentNullException(nameof(validate));
			}

			IFormatter formatter = new BinaryFormatter();
			using (MemoryStream stream = new MemoryStream())
			{
				formatter.Serialize(stream, exception);
				stream.Seek(0, SeekOrigin.Begin);
				Exception deserializedException = (Exception)formatter.Deserialize(stream);
				Assert.IsNotNull(deserializedException);
				Assert.That(validate(deserializedException), Is.True);
			}
		}
	}
}