// -----------------------------------------------------------------------------------------------------
// <copyright file="MockObjectFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Defines static helper methods to create common mocked objects.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using System.Collections.Generic;

	using Moq;

	using Relativity.DataExchange.Transfer;
	using Relativity.Transfer;

	/// <summary>
	/// Defines static helper methods to create common mocked objects.
	/// </summary>
	internal static class MockObjectFactory
	{
		public static Mock<TapiObjectService> CreateMockTapiObjectService()
		{
			// The understanding is - accept all default methods and replace specific methods with a mock.
			return new Mock<TapiObjectService> { CallBase = true };
		}

		public static Mock<ITapiFileStorageSearchResults> CreateMockEmptyTapiFileStorageSearchResults(bool cloudInstance)
		{
			Mock<ITapiFileStorageSearchResults> mockFileStorageSearchResults = new Mock<ITapiFileStorageSearchResults>();
			mockFileStorageSearchResults.SetupGet(x => x.CloudInstance).Returns(cloudInstance);
			mockFileStorageSearchResults.SetupGet(x => x.FileShares).Returns(new List<RelativityFileShare>());
			mockFileStorageSearchResults.SetupGet(x => x.InvalidFileShares).Returns(new List<RelativityFileShare>());
			return mockFileStorageSearchResults;
		}

		public static Mock<ITapiFileStorageSearchResults> CreateMockInvalidTapiFileStorageSearchResults(bool cloudInstance)
		{
			Mock<ITapiFileStorageSearchResults> mockFileStorageSearchResults = new Mock<ITapiFileStorageSearchResults>();
			mockFileStorageSearchResults.SetupGet(x => x.CloudInstance).Returns(cloudInstance);
			mockFileStorageSearchResults.SetupGet(x => x.FileShares).Returns(new List<RelativityFileShare>());
			mockFileStorageSearchResults.SetupGet(x => x.InvalidFileShares).Returns(
				new List<RelativityFileShare>
					{
						new RelativityFileShare(
							1,
							"invalid",
							@"\\files",
							new ResourceServerType(1, "invalid"),
							null,
							cloudInstance),
					});
			return mockFileStorageSearchResults;
		}
	}
}