// ----------------------------------------------------------------------------
// <copyright file="AssemblySetup.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;

	[SetUpFixture]
	public class AssemblySetup
	{
		public static IntegrationTestParameters TestParameters
		{
			get;
			private set;
		}

		[OneTimeSetUp]
		public void Setup()
		{
			TestParameters = IntegrationTestHelper.Create();
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			IntegrationTestHelper.Destroy(TestParameters);
		}
	}
}