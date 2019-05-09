// -----------------------------------------------------------------------------------------------------
// <copyright file="HttpServiceExceptionTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System;
	using System.Net;

	using global::NUnit.Framework;

	[TestFixture]
	public class HttpServiceExceptionTests : SerializationTestsBase
	{
		[Test]
		public static void ShouldSerializeAndDeserializeTheObject()
		{
			// No parameters - just defaults.
			HttpServiceException expected = new HttpServiceException("The error message");
			HttpServiceException actual = BinarySerialize(expected) as HttpServiceException;
			Assert.That(actual, Is.Not.Null);
			ValidatePropertyValues(actual, HttpServiceException.DefaultHttpStatusCode, HttpServiceException.DefaultFatalValue, false);
			actual = SoapSerialize(expected) as HttpServiceException;
			Assert.That(actual, Is.Not.Null);
			ValidatePropertyValues(actual, HttpServiceException.DefaultHttpStatusCode, HttpServiceException.DefaultFatalValue, false);

			// No parameters - just the inner exception.
			expected = new HttpServiceException("The error message", new InvalidOperationException());
			actual = BinarySerialize(expected) as HttpServiceException;
			Assert.That(actual, Is.Not.Null);
			ValidatePropertyValues(actual, HttpServiceException.DefaultHttpStatusCode, HttpServiceException.DefaultFatalValue, true);
			actual = SoapSerialize(expected) as HttpServiceException;
			Assert.That(actual, Is.Not.Null);
			ValidatePropertyValues(actual, HttpServiceException.DefaultHttpStatusCode, HttpServiceException.DefaultFatalValue, true);

			// Parameters - just the inner exception and the fatal flag.
			expected = new HttpServiceException("The error message", new InvalidOperationException(), true);
			actual = BinarySerialize(expected) as HttpServiceException;
			Assert.That(actual, Is.Not.Null);
			ValidatePropertyValues(actual, HttpServiceException.DefaultHttpStatusCode, true, true);
			actual = SoapSerialize(expected) as HttpServiceException;
			Assert.That(actual, Is.Not.Null);
			ValidatePropertyValues(actual, HttpServiceException.DefaultHttpStatusCode, true, true);

			// Parameters - the inner exception, fatal flag, and HTTP status code.
			expected = new HttpServiceException("The error message", new InvalidOperationException(), HttpStatusCode.NotFound, true);
			actual = BinarySerialize(expected) as HttpServiceException;
			Assert.That(actual, Is.Not.Null);
			ValidatePropertyValues(actual, HttpStatusCode.NotFound, true, true);
			actual = SoapSerialize(expected) as HttpServiceException;
			Assert.That(actual, Is.Not.Null);
			ValidatePropertyValues(actual, HttpStatusCode.NotFound, true, true);
		}

		private static void ValidatePropertyValues(HttpServiceException actual, HttpStatusCode expectedStatusCode, bool expectedFatal, bool expectedInnerException)
		{
			Assert.That(actual, Is.Not.Null);
			Assert.That(actual.Fatal, Is.EqualTo(expectedFatal));
			Assert.That(actual.Message, Is.EqualTo("The error message"));
			Assert.That(actual.StatusCode, Is.EqualTo(expectedStatusCode));
			Assert.That(actual.InnerException, expectedInnerException ? Is.Not.Null : Is.Null);
		}
	}
}