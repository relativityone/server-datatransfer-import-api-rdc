// <copyright file="ImportApiTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Threading.Tasks;
	using System.Web.Services.Protocols;

	using global::NUnit.Framework;
	using global::NUnit.Framework.Constraints;

	using kCura.Relativity.ImportAPI;
	using kCura.Relativity.ImportAPI.Data;
	using kCura.Relativity.ImportAPI.Enumeration;
	using kCura.WinEDDS.Exceptions;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.Testing.Identification;

	[TestFixture(false)]
	[TestFixture(true)]
	public class ImportApiTests
	{
		private const int DocumentArtifactTypeId = 10;

		private readonly bool useKepler;

		private IntegrationTestParameters testParameters;

		private ImportAPI sut;

		public ImportApiTests(bool useKepler)
		{
			this.useKepler = useKepler;
		}

		[IdentifiedTest("fd73c064-f91b-4741-9cf6-84dee1f4b5b8")]
		[Feature.DataTransfer.ImportApi.Authentication]
		public static void ShouldThrowAuthenticationErrorForInvalidCredentials()
		{
			// act & assert
			Assert.That(
				() => new ImportAPI("Username", "Wrong password"),
				Throws.Exception.InstanceOf<InvalidLoginException>());
		}

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			this.testParameters = AssemblySetup.TestParameters;
			Assume.That(this.testParameters.WorkspaceId, Is.Positive, "The test workspace must be created or specified in order to run this integration test.");

			AppSettings.Instance.UseKepler = this.useKepler;

			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls
																			 | SecurityProtocolType.Tls11
																			 | SecurityProtocolType.Tls12;
		}

		[SetUp]
		public void SetUp()
		{
			this.sut = new ImportAPI(
				this.testParameters.RelativityUserName,
				this.testParameters.RelativityPassword,
				this.testParameters.RelativityWebApiUrl.ToString());
		}

		[OneTimeTearDown]
		public Task OneTimeTearDown()
		{
			return ProductionHelper.DeleteAllProductionsAsync(this.testParameters);
		}

		[IdentifiedTest("101ec706-f7bd-4189-9ece-9d59ea1dfed3")]
		[Feature.DataTransfer.ImportApi.Operations.GetUploadableArtifactTypes]
		public void ShouldGetUploadableArtifactTypesForExistingWorkspace()
		{
			// act
			ArtifactType[] uploadableArtifacts = this.sut.GetUploadableArtifactTypes(this.testParameters.WorkspaceId)?.ToArray();

			// assert
			Assert.That(uploadableArtifacts, Is.Not.Null, "There should be some uploadable artifacts.");
			Assert.That(uploadableArtifacts, Is.Not.Empty, "There should be some uploadable artifacts.");

			var documentArtifact = uploadableArtifacts.SingleOrDefault(x => x.ID == DocumentArtifactTypeId);
			Assert.That(documentArtifact, Is.Not.Null, "Document artifact type should be present.");
			Assert.That(documentArtifact.Name, Is.EqualTo("Document"), "Document artifact type should have correct name.");
		}

		[IdentifiedTest("2c04e107-fcf1-4923-8182-0e2d5510ea12")]
		[Feature.DataTransfer.ImportApi.Operations.GetUploadableArtifactTypes]
		public void ShouldThrowExceptionWhenGetUploadableArtifactTypesForNonExistingWorkspace()
		{
			// arrange
			const int NonExistingWorkspaceId = 123456789;

			// act & assert
			Assert.That(
				() => this.sut.GetUploadableArtifactTypes(NonExistingWorkspaceId),
				this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
		}

		[IdentifiedTest("58c73e4c-8df7-407c-b915-a023b9b71959")]
		[Feature.DataTransfer.ImportApi.Operations.GetWorkspaceFields]
		public void ShouldGetWorkspaceFieldsForExistingWorkspaceAndArtifactType()
		{
			// act
			Field[] fields = this.sut.GetWorkspaceFields(this.testParameters.WorkspaceId, DocumentArtifactTypeId).ToArray();

			// assert
			Assert.That(fields, Is.Not.Null, "Document should have some fields.");
			Assert.That(fields, Is.Not.Empty, "Document should have some fields.");

			var controlNumberField = fields.SingleOrDefault(x => x.Name == WellKnownFields.ControlNumber);
			Assert.That(controlNumberField, Is.Not.Null, "Document should have control number field.");
			Assert.That(controlNumberField.FieldCategory, Is.EqualTo(FieldCategoryEnum.Identifier), "Control number should be an identifier.");
		}

		[IdentifiedTest("b8cc663e-6bb4-4886-a736-e401b6f38312")]
		[Feature.DataTransfer.ImportApi.Operations.GetWorkspaceFields]
		public void ShouldThrowExceptionWhenGetWorkspaceFieldsForNonExistingWorkspace()
		{
			// arrange
			const int NonExistingWorkspaceId = 123456789;

			// act & assert
			Assert.That(
				() => this.sut.GetWorkspaceFields(NonExistingWorkspaceId, DocumentArtifactTypeId),
				this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
		}

		[IdentifiedTest("d40d944b-850b-4183-9fd3-9d90298ad786")]
		[Feature.DataTransfer.ImportApi.Operations.GetWorkspaceFields]
		public void ShouldReturnEmptyCollectionWhenGetWorkspaceFieldsForNonExistingArtifactType()
		{
			// arrange
			const int NonExistingArtifactTypeId = 123456789;

			// act
			IEnumerable<Field> fields = this.sut.GetWorkspaceFields(this.testParameters.WorkspaceId, NonExistingArtifactTypeId);

			// assert
			Assert.That(fields, Is.Empty, "It should throw empty collection for non existing artifact type.");
		}

		[IdentifiedTest("f1dee0f9-a8b1-4c69-9842-5dfc1072569b")]
		[Feature.DataTransfer.ImportApi.Operations.GetWorkspaces]
		public async Task ShouldGetWorkspaces()
		{
			// arrange
			string expectedDefaultFileRepository = await WorkspaceHelper.GetDefaultFileRepositoryAsync(this.testParameters).ConfigureAwait(false);

			// act
			Workspace[] workspaces = this.sut.Workspaces().ToArray();

			// assert
			Assert.That(workspaces, Is.Not.Null, "There should be some workspaces.");
			Assert.That(workspaces, Is.Not.Empty, "There should be some workspaces.");

			var testWorkspace = workspaces.SingleOrDefault(x => x.ArtifactID == this.testParameters.WorkspaceId);
			Assert.That(testWorkspace, Is.Not.Null, "Should return test workspace");
			Assert.That(testWorkspace.Name, Is.EqualTo(this.testParameters.WorkspaceName), "Should return correct workspace name.");
			Assert.That(testWorkspace.DocumentPath, Is.EqualTo(expectedDefaultFileRepository), "DocumentPath should be correct.");
		}

		[IdentifiedTest("a15a5ff2-8ff0-43ca-839d-65f3db4ff94c")]
		[Feature.DataTransfer.ImportApi.Operations.GetProductionSets]
		public async Task ShouldGetProductionSetsAsync()
		{
			// arrange
			(string productionName, string batesPrefix)[] expectedProductions =
			{
				($"{Guid.NewGuid():N}", "PREFIX"),
				($"{Guid.NewGuid():N}", "PRE_"),
			};

			foreach (var (productionName, batesPrefix) in expectedProductions)
			{
				await ProductionHelper.CreateProductionAsync(this.testParameters, productionName, batesPrefix).ConfigureAwait(false);
			}

			// act
			ProductionSet[] actualProductions = this.sut.GetProductionSets(this.testParameters.WorkspaceId).ToArray();

			// assert
			var actualProductionNames = actualProductions.Select(x => x.Name);
			var expectedProductionNames = expectedProductions.Select(x => x.productionName);
			Assert.That(actualProductionNames, Is.SupersetOf(expectedProductionNames), "Should return all productions with correct names.");
		}

		[IdentifiedTest("61285e70-9054-4d62-baa2-433d4d6c5c0d")]
		[Feature.DataTransfer.ImportApi.Operations.GetProductionSets]
		public void ShouldThrowExceptionWhenGetProductionSetsForNonExistingWorkspace()
		{
			// arrange
			const int NonExistingWorkspaceId = 123456789;

			// act & assert
			Assert.That(
				() => this.sut.GetProductionSets(NonExistingWorkspaceId),
				this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
		}

		private IResolveConstraint GetExpectedExceptionConstraintForNonExistingWorkspace(int workspaceId)
		{
			string expectedExceptionMessage;
			if (this.useKepler)
			{
				expectedExceptionMessage =
					"Error during call PermissionCheckInterceptor." +
					" InnerExceptionType: Relativity.Core.Exception.InvalidAppArtifactID," +
					$" InnerExceptionMessage: Could not retrieve ApplicationID #{workspaceId}.";
			}
			else
			{
				expectedExceptionMessage = $"Could not retrieve ApplicationID #{workspaceId}.";
			}

			return Throws.Exception.InstanceOf<SoapException>()
				.With.Message.EqualTo(expectedExceptionMessage);
		}
	}
}
