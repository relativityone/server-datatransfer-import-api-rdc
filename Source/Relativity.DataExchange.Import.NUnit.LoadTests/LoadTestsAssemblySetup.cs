// ----------------------------------------------------------------------------
// <copyright file="LoadTestsAssemblySetup.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.LoadTests
{
	using global::NUnit.Framework;

	using Relativity.DataExchange.Import.NUnit.Integration;

	[SetUpFixture]
	public class LoadTestsAssemblySetup
	{
		private readonly AssemblySetup _assemblySetup = new AssemblySetup();

		[OneTimeSetUp]
		public void Setup()
		{
			this._assemblySetup.Setup();
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			this._assemblySetup.TearDown();
		}
	}
}