// ----------------------------------------------------------------------------
// <copyright file="AssemblySetup.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration
{
	using System;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;

	/// <summary>
	/// Represents a global assembly-wide setup routine that's guaranteed to be executed before ANY NUnit test.
	/// </summary>
	[SetUpFixture]
	public static class AssemblySetup
	{
		/// <summary>
		/// Not each test in this Assembly requires Relativity instance, so <see cref="IntegrationTestParameters"/>
		/// are created only when it is necessary.
		/// </summary>
		private static Lazy<IntegrationTestParameters> testParametersLazy;

		/// <summary>
		/// Gets the test parameters used by all integration tests within the current assembly.
		/// </summary>
		/// <value>
		/// The <see cref="IntegrationTestParameters"/> instance.
		/// </value>
		public static IntegrationTestParameters TestParameters => testParametersLazy.Value;

		/// <summary>
		/// The main setup method.
		/// </summary>
		[OneTimeSetUp]
		public static void Setup()
		{
			testParametersLazy = new Lazy<IntegrationTestParameters>(CreateTestParameters);
		}

		/// <summary>
		/// The main teardown method.
		/// </summary>
		[OneTimeTearDown]
		public static void TearDown()
		{
			if (testParametersLazy.IsValueCreated)
			{
				IntegrationTestHelper.Destroy(TestParameters);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			category: "Microsoft.Reliability",
			checkId: "CA2000:Dispose objects before losing scope",
			Justification = "https://devblogs.microsoft.com/pfxteam/do-i-need-to-dispose-of-tasks/")]
		private static IntegrationTestParameters CreateTestParameters() =>
			CreateTestParametersAsync().GetAwaiter().GetResult();

		private static async Task<IntegrationTestParameters> CreateTestParametersAsync()
		{
			var parameters = IntegrationTestHelper.Create();

			if (parameters.PerformAdditionalWorkspaceSetup)
			{
				await FieldHelper.EnsureWellKnownFieldsAsync(parameters).ConfigureAwait(false);
			}

			return parameters;
		}
	}
}