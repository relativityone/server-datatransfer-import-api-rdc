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

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification = "It is a NUnit requirement that setup class is not static.")]
	[SetUpFixture]
	public class LoadTestsAssemblySetup
	{
		[OneTimeSetUp]
		public static Task SetupAsync()
		{
			return AssemblySetup.SetupAsync();
		}

		[OneTimeTearDown]
		public static void TearDown()
		{
			AssemblySetup.TearDown();
		}
	}
}