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

	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2;
	using Relativity.DataExchange.Process;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;
	using Relativity.Transfer;

	/// <summary>
	/// Defines static helper methods to create common mocked objects.
	/// </summary>
	internal static class MockObjectFactory
	{
		public static Mock<ILog> CreateMockLogger()
		{
			return new Mock<ILog>();
		}

		public static Mock<IAppSettings> CreateMockAppSettings()
		{
			return new Mock<IAppSettings>();
		}

		public static Mock<IFileShareSettingsService> CreateMockFileShareSettingsService()
		{
			return new Mock<IFileShareSettingsService>();
		}

		public static Mock<IProcessEventWriter> CreateMockProcessEventWriter()
		{
			return new Mock<IProcessEventWriter>();
		}

		public static Mock<IProcessErrorWriter> CreateMockProcessErrorWriter()
		{
			return new Mock<IProcessErrorWriter>();
		}

		public static Mock<TapiObjectService> CreateMockTapiObjectService()
		{
			// The understanding is - accept all default methods and replace specific methods with a mock.
			return new Mock<TapiObjectService> { CallBase = true };
		}

		public static Mock<IUserNotification> CreateMockUserNotification()
		{
			return new Mock<IUserNotification>();
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