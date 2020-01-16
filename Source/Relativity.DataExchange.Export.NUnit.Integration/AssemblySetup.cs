// ----------------------------------------------------------------------------
// <copyright file="AssemblySetup.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using global::NUnit.Framework;

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

			if (TestParameters.SkipIntegrationTests)
			{
				return;
			}

			var controlNumbers = ImportHelper.ImportDefaultTestData(TestParameters);

			// we are importing images only for the first document
			ImportHelper.ImportImagesForDocuments(TestParameters, controlNumbers.GetRange(0, 1));
		}

		[OneTimeTearDown]
		public static void TearDown()
		{
			IntegrationTestHelper.Destroy(TestParameters);
		}
	}
}