// <copyright file="CollationEdgeCasesTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	[TestType.EdgeCase]
	[TestExecutionCategory.CI]
	public class CollationEdgeCasesTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		/// <summary>
		/// This test documents current behavior, but we should consider changing that in the future.
		/// In most cases duplicated names are detected on the client side, but some specific strings
		/// are considered different by the .NET code, while they are equal according to our default
		/// SQL collation. More details: https://einstein.kcura.com/x/CoB8Dg .
		/// </summary>
		/// <remarks>This test reproduces issue fixed in REL-558402.</remarks>
		[IdentifiedTest("6d252c5a-2087-4afd-8afc-6bab1d0e7be4")]
		[IgnoreIfVersionLowerThan(RelativityVersion.PrairieSmoke0)]
		public void ShouldThrowExceptionFromMassImportWhenControlNumbersAreDuplicated()
		{
			// ARRANGE
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			string[] controlNumbers =
			{
				"Somestring",
				"some😀string😀",
			};
			var dataSourceBuilder = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumbers);

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(dataSourceBuilder.Build());

			// ASSERT
			Assert.That(results.FatalException, Is.Not.Null);
			Assert.That(results.FatalException.Message, Contains.Substring("Cannot insert duplicate key row in object 'EDDSDBO.Document'"));
		}
	}
}
