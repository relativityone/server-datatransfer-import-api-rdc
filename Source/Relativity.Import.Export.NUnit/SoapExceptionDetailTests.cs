// -----------------------------------------------------------------------------------------------------
// <copyright file="SoapExceptionDetailTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using global::NUnit.Framework;

	using Relativity.Import.Export.Service;

	[TestFixture]
	public class SoapExceptionDetailTests : SerializationTestsBase
	{
		[Test]
		public static void ShouldSerializeAndDeserializeTheObject()
		{
			SoapExceptionDetail expected = new SoapExceptionDetail
				                               {
					                               Details = new[] { "One", "Two" },
					                               ExceptionFullText = "Exception full text",
					                               ExceptionMessage = "Exception message",
					                               ExceptionTrace = "Exception trace",
					                               ExceptionType = "Exception type"
				                               };
			SoapExceptionDetail actual = BinarySerialize(expected) as SoapExceptionDetail;
			Assert.That(actual, Is.Not.Null);
			ValidatePropertyValues(actual);
			actual = SoapSerialize(expected) as SoapExceptionDetail;
			Assert.That(actual, Is.Not.Null);
			ValidatePropertyValues(actual);
		}

		private static void ValidatePropertyValues(SoapExceptionDetail actual)
		{
			Assert.That(actual.Details, Is.Not.Null);
			Assert.That(actual.Details.Length, Is.EqualTo(2));
			Assert.That(actual.Details[0], Is.EqualTo("One"));
			Assert.That(actual.Details[1], Is.EqualTo("Two"));
			Assert.That(actual.ExceptionFullText, Is.EqualTo("Exception full text"));
			Assert.That(actual.ExceptionMessage, Is.EqualTo("Exception message"));
			Assert.That(actual.ExceptionTrace, Is.EqualTo("Exception trace"));
			Assert.That(actual.ExceptionType, Is.EqualTo("Exception type"));
		}
	}
}