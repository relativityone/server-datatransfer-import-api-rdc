// ----------------------------------------------------------------------------
// <copyright file="LoadTestsAssemblySetup.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.LoadTests
{
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Import.NUnit.Integration;

	[SetUpFixture]
	public class LoadTestsAssemblySetup
	{
		[OneTimeSetUp]
		public Task SetupAsync()
		{
			return AssemblySetup.SetupAsync();
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			AssemblySetup.TearDown();
		}
	}
}