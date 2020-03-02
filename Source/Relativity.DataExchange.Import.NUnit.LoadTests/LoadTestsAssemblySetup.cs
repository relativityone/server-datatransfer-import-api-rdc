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
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification = "It is a NUnit requirement that setup class is not static.")]
	public class LoadTestsAssemblySetup
	{
		[OneTimeSetUp]
		public static async Task Setup()
		{
			await AssemblySetup.SetupAsync().ConfigureAwait(false);
		}

		[OneTimeTearDown]
		public static void TearDown()
		{
			AssemblySetup.TearDown();
		}
	}
}