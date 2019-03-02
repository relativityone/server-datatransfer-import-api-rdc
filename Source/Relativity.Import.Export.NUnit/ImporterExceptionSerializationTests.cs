// -----------------------------------------------------------------------------------------------------
// <copyright file="ImporterExceptionSerializationTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;

	using global::NUnit.Framework;

	using Relativity.Import.Export.Io;

	[TestFixture]
	public static class ImporterExceptionSerializationTests
	{
		private const int TestRowNumber = 99;
		private const int TestColumnNumber = 999;
		private const string TestFieldName = "Test-Field-Name";
		private const int TestLength = 555;
		private const string TestInnerExceptionMessage = "test inner exception";

		public static IEnumerable ExceptionTypeTestCases
		{
			get
			{
				Action<Exception> validateNoOp = exception => { };
				Action<Exception> validateBooleanException = ValidateBooleanException;
				Action<Exception> validateCellException = ValidateCellException;
				Action<Exception> validateDateException = ValidateDateException;
				Action<Exception> validateDecimalException = ValidateDecimalException;
				Action<Exception> validateObjectNameException = ValidateObjectNameException;
				Action<Exception> validateStringWithoutFieldInfoException = ValidateStringWithoutFieldInfoException;
				Action<Exception> validateStringWithFieldInfoException = ValidateStringWithFieldInfoException;
				Exception innerException = new InvalidOperationException(TestInnerExceptionMessage);
				yield return new TestCaseData(new BooleanImporterException(), validateNoOp);
				yield return new TestCaseData(new BooleanImporterException("a"), validateNoOp);
				yield return new TestCaseData(new BooleanImporterException("a", innerException), validateNoOp);
				yield return new TestCaseData(
					new BooleanImporterException(TestRowNumber, TestColumnNumber, innerException),
					validateBooleanException);
				yield return new TestCaseData(new CellImporterException(), validateNoOp);
				yield return new TestCaseData(new CellImporterException("a"), validateNoOp);
				yield return new TestCaseData(new CellImporterException("a", innerException), validateNoOp);
				yield return new TestCaseData(
					new CellImporterException(TestRowNumber, TestColumnNumber, innerException),
					validateCellException);
				yield return new TestCaseData(new DateImporterException(), validateNoOp);
				yield return new TestCaseData(new DateImporterException("a"), validateNoOp);
				yield return new TestCaseData(new DateImporterException("a", innerException), validateNoOp);
				yield return new TestCaseData(
					new DateImporterException(TestRowNumber, TestColumnNumber, innerException),
					validateDateException);
				yield return new TestCaseData(new DecimalImporterException(), validateNoOp);
				yield return new TestCaseData(new DecimalImporterException("a"), validateNoOp);
				yield return new TestCaseData(new DecimalImporterException("a", innerException), validateNoOp);
				yield return new TestCaseData(
					new DecimalImporterException(TestRowNumber, TestColumnNumber, innerException),
					validateDecimalException);
				yield return new TestCaseData(new ObjectNameImporterException(), validateNoOp);
				yield return new TestCaseData(new ObjectNameImporterException("a"), validateNoOp);
				yield return new TestCaseData(new ObjectNameImporterException("a", innerException), validateNoOp);
				yield return new TestCaseData(
					new ObjectNameImporterException(TestRowNumber, TestColumnNumber, TestLength, TestFieldName),
					validateObjectNameException);
				yield return new TestCaseData(new StringImporterException(), validateNoOp);
				yield return new TestCaseData(new StringImporterException("a"), validateNoOp);
				yield return new TestCaseData(new StringImporterException("a", innerException), validateNoOp);
				yield return new TestCaseData(
					new StringImporterException(TestRowNumber, TestColumnNumber, TestLength),
					validateStringWithoutFieldInfoException);
				yield return new TestCaseData(
					new StringImporterException(TestRowNumber, TestColumnNumber, TestLength, TestFieldName),
					validateStringWithFieldInfoException);
			}
		}

		[Test]
		[TestCaseSource(nameof(ExceptionTypeTestCases))]
		public static void ItShouldSerializeAndDeserializeTheImporterException(Exception exception, Action<Exception> validate)
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
				validate(deserializedException);
			}
		}

		private static void ValidateBooleanException(Exception exception)
		{
			Assert.That(exception, Is.TypeOf<BooleanImporterException>());
			ValidateImporterException(
				exception as BooleanImporterException,
				Relativity.Import.Export.Resources.Strings.BooleanImporterErrorAdditionalInfo);
		}

		private static void ValidateCellException(Exception exception)
		{
			Assert.That(exception, Is.TypeOf<CellImporterException>());
			ValidateImporterException(
				exception as CellImporterException,
				Relativity.Import.Export.Resources.Strings.CellImporterErrorAdditionalInfo);
		}

		private static void ValidateDateException(Exception exception)
		{
			Assert.That(exception, Is.TypeOf<DateImporterException>());
			ValidateImporterException(
				exception as DateImporterException,
				Relativity.Import.Export.Resources.Strings.DateImporterErrorAdditionalInfo);
		}

		private static void ValidateDecimalException(Exception exception)
		{
			Assert.That(exception, Is.TypeOf<DecimalImporterException>());
			ValidateImporterException(
				exception as DecimalImporterException,
				Relativity.Import.Export.Resources.Strings.DecimalImporterErrorAdditionalInfo);
		}

		private static void ValidateObjectNameException(Exception exception)
		{
			Assert.That(exception, Is.TypeOf<ObjectNameImporterException>());
			string expectedAdditionalInfoMessage = ObjectNameImporterException.CreateAdditionalInfoMessage(
				TestLength,
				TestFieldName);
			string expectedErrorMessage = ImporterException.GetErrorMessage(
				TestRowNumber,
				TestFieldName,
				expectedAdditionalInfoMessage);
			ValidateImporterException(
				exception as ObjectNameImporterException,
				expectedErrorMessage,
				expectedAdditionalInfoMessage,
				TestRowNumber,
				TestColumnNumber,
				TestFieldName);
		}

		private static void ValidateStringWithoutFieldInfoException(Exception exception)
		{
			Assert.That(exception, Is.TypeOf<StringImporterException>());
			string expectedAdditionalInfoMessage = StringImporterException.GetAdditionalInfoMessage(TestLength);
			string expectedErrorMessage = ImporterException.GetErrorMessage(
				TestRowNumber,
				TestColumnNumber,
				expectedAdditionalInfoMessage);
			ValidateImporterException(
				exception as StringImporterException,
				expectedErrorMessage,
				expectedAdditionalInfoMessage);
		}

		private static void ValidateStringWithFieldInfoException(Exception exception)
		{
			Assert.That(exception, Is.TypeOf<StringImporterException>());
			string expectedAdditionalInfoMessage =
				StringImporterException.GetAdditionalInfoMessage(TestLength, TestFieldName);
			string expectedErrorMessage = ImporterException.GetErrorMessage(
				TestRowNumber,
				TestFieldName,
				expectedAdditionalInfoMessage);
			ValidateImporterException(
				exception as StringImporterException,
				expectedErrorMessage,
				expectedAdditionalInfoMessage,
				TestRowNumber,
				TestColumnNumber,
				TestFieldName);
		}

		private static void ValidateImporterException(
			ImporterException exception,
			string expectedAdditionalInfo)
		{
			string expectedErrorMessage = ImporterException.GetErrorMessage(
				TestRowNumber,
				TestColumnNumber,
				expectedAdditionalInfo);
			ValidateImporterException(exception, expectedErrorMessage, expectedAdditionalInfo);
		}

		private static void ValidateImporterException(
			ImporterException exception,
			string expectedErrorMessage,
			string expectedAdditionalInfo,
			long expectedRowNumber = TestRowNumber,
			int expectedColumnNumber = TestColumnNumber,
			string expectedFieldName = null)
		{
			Assert.That(exception, Is.Not.Null);
			Assert.That(exception.Message, Is.EqualTo(expectedErrorMessage));
			ValidateProperties(
				exception,
				expectedAdditionalInfo,
				expectedRowNumber,
				expectedColumnNumber,
				expectedFieldName);
		}

		private static void ValidateProperties(
			ImporterException exception,
			string expectedAdditionalInfo,
			long expectedRowNumber,
			int expectedColumnNumber,
			string expectedFieldName)
		{
			Assert.That(exception.AdditionalInfo, Is.EqualTo(expectedAdditionalInfo));
			Assert.That(exception.Column, Is.EqualTo(expectedColumnNumber));
			Assert.That(exception.FieldName, Is.EqualTo(expectedFieldName));
			Assert.That(exception.Row, Is.EqualTo(expectedRowNumber));
		}
	}
}