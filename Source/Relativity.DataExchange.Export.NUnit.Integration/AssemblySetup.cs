// ----------------------------------------------------------------------------
// <copyright file="AssemblySetup.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using global::NUnit.Framework;

	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;

	/// <summary>
	/// Represents a global assembly-wide setup routine that's guaranteed to be executed before ANY NUnit test.
	/// </summary>
	[SetUpFixture]
	[Category(TestCategories.Export)]
	[Category(TestCategories.Integration)]
	public class AssemblySetup
	{
		/// <summary>
		/// Gets the test parameters used by all integration tests within the current assembly.
		/// </summary>
		/// <value>
		/// The <see cref="IntegrationTestParameters"/> instance.
		/// </value>
		public static IntegrationTestParameters TestParameters
		{
			get;
			private set;
		}

		[OneTimeSetUp]
		public static void Setup()
		{
			TestParameters = IntegrationTestHelper.Create();

			var importApi = new ImportAPI(
				TestParameters.RelativityUserName,
				TestParameters.RelativityPassword,
				TestParameters.RelativityWebApiUrl.ToString());

			ImportHelper.ImportDefaultTestData(importApi, TestParameters.WorkspaceId);
		}

		[OneTimeTearDown]
		public static void TearDown()
		{
			IntegrationTestHelper.Destroy(TestParameters);
		}
	}
}