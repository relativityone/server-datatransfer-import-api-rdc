// <copyright file="KeplerExceptionDetailTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Mapping
{
	using global::NUnit.Framework;

	using kCura.WinEDDS.Mapping;

	using Relativity.Services.Exceptions;

	[TestFixture]
	public class KeplerExceptionDetailTests
	{
		[TestCase("InnerExceptionType:InnerExceptionMessage:", "", "")]
		[TestCase("InnerExceptionType:,InnerExceptionMessage:", "", "")]
		[TestCase("InnerExceptionType:abc,InnerExceptionMessage:", "abc", "")]
		[TestCase("InnerExceptionType:,InnerExceptionMessage:abc", "", "Error: abc")]
		[TestCase("InnerExceptionMessage:InnerExceptionType:", "", "")]
		[TestCase("InnerExceptionMessage:,InnerExceptionType:", "", "")]
		[TestCase("InnerExceptionMessage:,InnerExceptionType:abc", "abc", "")]
		[TestCase("InnerExceptionMessage:abc,InnerExceptionType:", "", "Error: abc")]
		[TestCase(
			"Error during call BulkImportImageAsync. InnerExceptionType: Relativity.Core.Exception.InsufficientAccessControlListPermissions, InnerExceptionMessage: Insufficient Permissions! Please ask your Relativity Administrator to allow you import permission.",
			"Relativity.Core.Exception.InsufficientAccessControlListPermissions",
			"Error: Insufficient Permissions! Please ask your Relativity Administrator to allow you import permission.")]
		[TestCase(
			"Error during call BulkImportImageAsync. InnerExceptionMessage: Insufficient Permissions! Please ask your Relativity Administrator to allow you import permission., InnerExceptionType: Relativity.Core.Exception.InsufficientAccessControlListPermissions",
			"Relativity.Core.Exception.InsufficientAccessControlListPermissions",
			"Error: Insufficient Permissions! Please ask your Relativity Administrator to allow you import permission.")]
		[TestCase(
			"    Exception occurred.    InnerExceptionType:    System.Exception    ,    InnerExceptionMessage:    Test error!    ",
			"System.Exception",
			"Error: Test error!")]
		[TestCase(
			"    Exception occurred.    InnerExceptionMessage:    Test error!    ,    InnerExceptionType:    System.Exception    ",
			"System.Exception",
			"Error: Test error!")]
		public void KeplerExceptionDetailShouldExtractExceptionTypeAndMessage(string inputExceptionMessage, string expectedExceptionType, string expectedExceptionMessage)
		{
			var exceptionDetails = new KeplerExceptionDetail(new ServiceException(inputExceptionMessage));

			Assert.AreEqual(expectedExceptionType, exceptionDetails.ExceptionType);
			Assert.AreEqual(expectedExceptionMessage, exceptionDetails.ExceptionMessage);
			Assert.AreEqual(expectedExceptionMessage, exceptionDetails.ExceptionFullText);
		}

		[TestCase(null, "Relativity.Services.Exceptions.ServiceException", "Exception of type 'Relativity.Services.Exceptions.ServiceException' was thrown.")]
		[TestCase("", "Relativity.Services.Exceptions.ServiceException", "")]
		[TestCase("   ", "Relativity.Services.Exceptions.ServiceException", "")]
		[TestCase("Test", "Relativity.Services.Exceptions.ServiceException", "Test")]
		[TestCase(
			"Relativity.Services.Exceptions.ServiceException: No Content found in response. Response code: Unauthorized. ReasonPhrase: Unauthorized",
			"Relativity.Services.Exceptions.ServiceException",
			"Relativity.Services.Exceptions.ServiceException: No Content found in response. Response code: Unauthorized. ReasonPhrase: Unauthorized")]
		[TestCase(
			"Relativity.Services.Exceptions.ServiceException: Object reference not set to an instance of an object.",
			"Relativity.Services.Exceptions.ServiceException",
			"Relativity.Services.Exceptions.ServiceException: Object reference not set to an instance of an object.")]
		[TestCase("InnerExceptionType:", "Relativity.Services.Exceptions.ServiceException", "InnerExceptionType:")]
		[TestCase("InnerExceptionMessage:", "Relativity.Services.Exceptions.ServiceException", "InnerExceptionMessage:")]
		public void KeplerExceptionDetailAndErrorDetailKeysNotFoundInMessageShouldReturnOriginalTypeAndMessage(string inputExceptionMessage, string expectedExceptionType, string expectedExceptionMessage)
		{
			var exceptionDetails = new KeplerExceptionDetail(new ServiceException(inputExceptionMessage));

			Assert.AreEqual(expectedExceptionType, exceptionDetails.ExceptionType);
			Assert.AreEqual(expectedExceptionMessage, exceptionDetails.ExceptionMessage);
			Assert.AreEqual(expectedExceptionMessage, exceptionDetails.ExceptionFullText);
		}

		[Test]
		public void KeplerExceptionDetailAndExceptionIsNullShouldSetEmptyValues()
		{
			var exceptionDetails = new KeplerExceptionDetail(null);

			Assert.AreEqual(string.Empty, exceptionDetails.ExceptionType);
			Assert.AreEqual(string.Empty, exceptionDetails.ExceptionMessage);
			Assert.AreEqual(string.Empty, exceptionDetails.ExceptionFullText);
		}
	}
}
