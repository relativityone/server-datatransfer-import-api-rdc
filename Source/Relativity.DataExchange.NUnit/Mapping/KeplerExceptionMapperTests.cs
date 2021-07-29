// <copyright file="KeplerExceptionMapperTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Mapping
{
	using global::NUnit.Framework;

	using kCura.WinEDDS.Mapping;

	using Relativity.Services.Exceptions;

	[TestFixture]
	public class KeplerExceptionMapperTests
	{
		[Test]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "These messages are OK.")]
		public void KeplerExceptionMapperShouldMapServiceExceptionToSoapException()
		{
			// arrange
			const string TestErrorMessage = "Error during call BulkImportImageAsync. InnerExceptionType: Relativity.Core.Exception.InsufficientAccessControlListPermissions, InnerExceptionMessage: Insufficient Permissions! Please ask your Relativity Administrator to allow you import permission.";
			const string ExpectedDetailXml = "<detail><ExceptionType>Relativity.Core.Exception.InsufficientAccessControlListPermissions</ExceptionType><ExceptionMessage>Error: Insufficient Permissions! Please ask your Relativity Administrator to allow you import permission.</ExceptionMessage><ExceptionFullText>Error: Insufficient Permissions! Please ask your Relativity Administrator to allow you import permission.</ExceptionFullText></detail>";

			var serviceException = new ServiceException(TestErrorMessage);
			var exceptionMapper = new KeplerExceptionMapper();

			// act
			var soapException = exceptionMapper.Map(serviceException);

			// assert
			Assert.AreEqual(serviceException.Message, soapException.Message);
			Assert.AreEqual(serviceException, soapException.InnerException);
			Assert.AreEqual(soapException.Detail.OuterXml, ExpectedDetailXml);
		}
	}
}
