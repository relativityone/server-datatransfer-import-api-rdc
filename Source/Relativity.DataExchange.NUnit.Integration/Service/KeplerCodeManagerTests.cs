// <copyright file="KeplerCodeManagerTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Integration.Service
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Threading.Tasks;
	using global::NUnit.Framework;

	using kCura.WinEDDS.Api;
	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Replacement;

	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.Testing.Identification;

	[TestFixture(true)]
	[TestFixture(false)]
	[Feature.DataTransfer.ImportApi]
	public class KeplerCodeManagerTests : KeplerServiceTestBase
	{
		private const string EmptySingleChoiceFieldName = "EmptySingleChoiceField";
		private const string PopulatedSingleChoiceFieldName = "PopulatedSingleChoiceField";
		private const string NonExistingChoiceName = "NotExitingChoiceName";
		private const int NonExistingCodeTypeId = -1;
		private const int NonExistingParentId = -1;
		private const int ChoicesCount = 10;

		private int emptySingleChoiceFieldId;
		private int populatedSingleChoiceFieldId;
		private int parentId;

		private kCura.EDDS.WebAPI.DocumentManagerBase.Field emptySingleChoiceFieldCodeType;
		private kCura.EDDS.WebAPI.DocumentManagerBase.Field populatedSingleChoiceFieldCodeType;
		private List<string> uniqueValues;

		public KeplerCodeManagerTests(bool useKepler)
			: base(useKepler)
		{
		}

		private int PopulatedSingleChoiceFieldCodeTypeId
		{
			get
			{
				if (this.populatedSingleChoiceFieldCodeType == null)
				{
					this.ReadCodeTypes();
				}

				return this.populatedSingleChoiceFieldCodeType.CodeTypeID.GetValueOrDefault();
			}
		}

		private ArtifactField PopulatedSingleChoiceFieldCodeType
		{
			get
			{
				if (this.populatedSingleChoiceFieldCodeType == null)
				{
					this.ReadCodeTypes();
				}

				return new ArtifactField(this.populatedSingleChoiceFieldCodeType);
			}
		}

		private int EmptySingleChoiceFieldCodeTypeId
		{
			get
			{
				if (this.emptySingleChoiceFieldCodeType == null)
				{
					this.ReadCodeTypes();
				}

				return this.emptySingleChoiceFieldCodeType.CodeTypeID.GetValueOrDefault();
			}
		}

		[OneTimeSetUp]
		public async Task OneTimeSetUpAsync()
		{
			this.parentId = await WorkspaceHelper.ReadRootArtifactId(this.TestParameters, this.TestParameters.WorkspaceId).ConfigureAwait(false);

			this.emptySingleChoiceFieldId = await FieldHelper.CreateSingleChoiceFieldAsync(
				                                    this.TestParameters,
				                                    EmptySingleChoiceFieldName,
				                                    (int)ArtifactType.Document,
				                                    isOpenToAssociations: false).ConfigureAwait(false);

			this.populatedSingleChoiceFieldId = await FieldHelper.CreateSingleChoiceFieldAsync(
				                              this.TestParameters,
				                              PopulatedSingleChoiceFieldName,
				                              (int)ArtifactType.Document,
				                              isOpenToAssociations: false).ConfigureAwait(false);

			var choicesValueSource = ChoicesValueSource.CreateSingleChoiceSource(ChoicesCount);
			await ChoiceHelper.ImportValuesIntoChoiceAsync(
				this.TestParameters,
				this.populatedSingleChoiceFieldId,
				choicesValueSource).ConfigureAwait(false);
			this.uniqueValues = choicesValueSource.UniqueValues;
		}

		[OneTimeTearDown]
		public async Task OneTimeTearDownAsync()
		{
			await FieldHelper.DeleteFieldAsync(this.TestParameters, this.populatedSingleChoiceFieldId).ConfigureAwait(false);
			await FieldHelper.DeleteFieldAsync(this.TestParameters, this.emptySingleChoiceFieldId).ConfigureAwait(false);
		}

		[IdentifiedTest("6a239075-617b-4c76-97e9-fb28f6effeff")]
		public void ShouldCreate()
		{
			// arrange
			string choiceValue = $"Choice Value {(this.UseKepler ? "Kepler" : "WebApi")}";

			using (ICodeManager sut = ManagerFactory.CreateCodeManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var code = new kCura.EDDS.WebAPI.CodeManagerBase.Code()
				{
				   Name = choiceValue,
				   Order = 2000,
				   ParentArtifactID = this.parentId,
				   CodeType = this.EmptySingleChoiceFieldCodeTypeId,
				};

				// act
				var result = sut.Create(this.TestParameters.WorkspaceId, code);

				// assert
				Assert.That((int)result, Is.GreaterThan(0));
			}
		}

		[IdentifiedTest("64beffce-bc1c-4233-8bde-cc2ce4acfa84")]
		public void ShouldNotCreateCodeThatAlreadyExists()
		{
			// arrange
			string choiceValue = $"Duplicated choice value {(this.UseKepler ? "Kepler" : "WebApi")}";

			using (ICodeManager sut = ManagerFactory.CreateCodeManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var code = new kCura.EDDS.WebAPI.CodeManagerBase.Code()
					           {
						           Name = choiceValue,
						           Order = 2000,
						           ParentArtifactID = this.parentId,
						           CodeType = this.EmptySingleChoiceFieldCodeTypeId,
					           };

				var secondCode = new kCura.EDDS.WebAPI.CodeManagerBase.Code()
					           {
						           Name = choiceValue,
						           Order = 2000,
						           ParentArtifactID = this.parentId,
						           CodeType = this.EmptySingleChoiceFieldCodeTypeId,
					           };

				// act
				var resultCode = sut.Create(this.TestParameters.WorkspaceId, code);
				var resultSecondCode = sut.Create(this.TestParameters.WorkspaceId, secondCode);

				// assert
				Assert.That((int)resultCode, Is.EqualTo((int)resultSecondCode));
			}
		}

		[IdentifiedTest("f04bf873-d9f4-48c1-a96a-5c40cdfe8ff2")]
		public void ShouldNotCreateWhenParametersAreInvalid()
		{
			// arrange
			string choiceValue = $"Choice Value {(this.UseKepler ? "Kepler" : "WebApi")}";

			using (ICodeManager sut = ManagerFactory.CreateCodeManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				var code = new kCura.EDDS.WebAPI.CodeManagerBase.Code()
					           {
						           Name = choiceValue,
						           Order = 2000,
						           ParentArtifactID = this.parentId,
						           CodeType = this.EmptySingleChoiceFieldCodeTypeId,
					           };

				Assert.That(() => sut.Create(NonExistingWorkspaceId, code), this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));

				code = new kCura.EDDS.WebAPI.CodeManagerBase.Code()
					           {
						           Name = choiceValue,
						           Order = 2000,
						           ParentArtifactID = NonExistingParentId,
						           CodeType = this.EmptySingleChoiceFieldCodeTypeId,
					           };

				var result = sut.Create(this.TestParameters.WorkspaceId, code);

				Assert.That((string)result, Does.Contain("SqlException: The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_ArtifactAncestry_Artifact1\""));

				code = new kCura.EDDS.WebAPI.CodeManagerBase.Code()
					           {
						           Name = choiceValue,
						           Order = 2000,
						           ParentArtifactID = this.parentId,
						           CodeType = NonExistingCodeTypeId,
					           };

				result = sut.Create(this.TestParameters.WorkspaceId, code);

				Assert.That((string)result, Does.Contain("SqlException: The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Code_CodeType\""));
			}
		}

		[IdentifiedTest("cd750f8b-91e8-4014-8d4c-670ebf264970")]
		public void ShouldCreateNewCodeDTOProxy()
		{
			// arrange
			string choiceValue = "Choice Value";
			int order = 0;

			using (ICodeManager sut = ManagerFactory.CreateCodeManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				var code = sut.CreateNewCodeDTOProxy(this.EmptySingleChoiceFieldCodeTypeId, choiceValue, order, this.parentId);

				// assert
				Assert.That(code.CodeType, Is.EqualTo(this.EmptySingleChoiceFieldCodeTypeId));
				Assert.That(code.Name, Is.EqualTo(choiceValue));
				Assert.That(code.ParentArtifactID, Is.EqualTo(this.parentId));
				Assert.That(code.Order, Is.EqualTo(order));
			}
		}

		[IdentifiedTest("547362bf-caa2-4efd-8366-b485ecd1fb6c")]
		public void ShouldGetAllForHierarchical()
		{
			// arrange
			using (ICodeManager sut = ManagerFactory.CreateCodeManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				DataSet results = sut.GetAllForHierarchical(this.TestParameters.WorkspaceId, this.PopulatedSingleChoiceFieldCodeTypeId);

				// assert
				List<string> choices = results
					.Tables[0]
					.Rows
					.OfType<DataRow>()
					.Select(row => row.Field<string>(0))
					.ToList();

				List<string> expected = new List<string> { string.Empty };
				expected.AddRange(this.uniqueValues);
				Assert.That(choices, Is.EquivalentTo(expected));
			}
		}

		[IdentifiedTest("d4468ee3-842c-4cba-b7f6-b93d9a002cd7")]
		public void ShouldNotGetAllForHierarchicalWhenParametersAreInvalid()
		{
			// arrange
			using (ICodeManager sut = ManagerFactory.CreateCodeManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(() => sut.GetAllForHierarchical(NonExistingWorkspaceId, this.PopulatedSingleChoiceFieldCodeTypeId), this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));

				DataSet results = sut.GetAllForHierarchical(this.TestParameters.WorkspaceId, NonExistingCodeTypeId);
				List<string> choices = results
					.Tables[0]
					.Rows
					.OfType<DataRow>()
					.Select(row => row.Field<string>(0))
					.ToList();

				List<string> expected = new List<string> { string.Empty };
				Assert.That(choices, Is.EquivalentTo(expected));
			}
		}

		[IdentifiedTest("a68ff829-f020-4f4f-962e-5e4030e81e6e")]
		public async Task ShouldGetChoiceLimitForUI()
		{
			// arrange
			var expectedChoiceLimitForUI = await InstanceSettingsHelper.QueryInstanceSetting(
				                               this.TestParameters,
				                               "Relativity.Core",
				                               "ChoiceLimitForUI").ConfigureAwait(false);

			using (ICodeManager sut = ManagerFactory.CreateCodeManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				int choiceLimitForUI = sut.GetChoiceLimitForUI();

				// assert
				Assert.That(choiceLimitForUI, Is.EqualTo(Convert.ToInt32(expectedChoiceLimitForUI)));
			}
		}

		[IdentifiedTest("dcdd9a91-9c05-4713-a8ee-0360af915e6a")]
		public void ShouldReadID()
		{
			// arrange
			using (ICodeManager sut = ManagerFactory.CreateCodeManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				int result = sut.ReadID(this.TestParameters.WorkspaceId, this.parentId, this.PopulatedSingleChoiceFieldCodeTypeId, this.uniqueValues[0]);

				// assert
				Assert.That(result, Is.GreaterThan(0));
			}
		}

		[IdentifiedTest("d3be3d5b-62bb-4f67-af79-1932a2d56aea")]
		public void ShouldNotReadIDWhenParametersAreInvalid()
		{
			// arrange
			using (ICodeManager sut = ManagerFactory.CreateCodeManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(
					() => sut.ReadID(NonExistingWorkspaceId, this.parentId, this.PopulatedSingleChoiceFieldCodeTypeId, this.uniqueValues[0]),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));

				int result = sut.ReadID(this.TestParameters.WorkspaceId, NonExistingParentId, this.PopulatedSingleChoiceFieldCodeTypeId, this.uniqueValues[0]);
				Assert.That(result, Is.EqualTo(-1));

				result = sut.ReadID(this.TestParameters.WorkspaceId, this.parentId, NonExistingCodeTypeId, this.uniqueValues[0]);
				Assert.That(result, Is.EqualTo(-1));

				result = sut.ReadID(this.TestParameters.WorkspaceId, this.parentId, this.PopulatedSingleChoiceFieldCodeTypeId, NonExistingChoiceName);
				Assert.That(result, Is.EqualTo(-1));
			}
		}

		[IdentifiedTest("275bef10-4a29-4a86-b070-090ee53719b7")]
		public void ShouldRetrieveAllCodesOfType()
		{
			// arrange
			using (ICodeManager sut = ManagerFactory.CreateCodeManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				Relativity.DataExchange.Service.ChoiceInfo[] results = sut.RetrieveAllCodesOfType(
					this.TestParameters.WorkspaceId,
					this.PopulatedSingleChoiceFieldCodeTypeId);

				// assert
				Assert.That(results.Length, Is.EqualTo(ChoicesCount));
			}
		}

		[IdentifiedTest("f6298c0a-2449-4082-a83e-eb15475ed099")]
		public void ShouldNotRetrieveAllCodesOfTypeWhenParametersAreInvalid()
		{
			// arrange
			using (ICodeManager sut = ManagerFactory.CreateCodeManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(
					() => sut.RetrieveAllCodesOfType(NonExistingWorkspaceId, this.PopulatedSingleChoiceFieldCodeTypeId),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));

				Relativity.DataExchange.Service.ChoiceInfo[] results = sut.RetrieveAllCodesOfType(
					this.TestParameters.WorkspaceId, NonExistingCodeTypeId);

				Assert.That(results.Length, Is.EqualTo(0));
			}
		}

		[IdentifiedTest("0d7a8fd4-d29a-4300-9bde-15516471e3e7")]
		public void ShouldRetrieveCodeByNameAndTypeID()
		{
			// arrange
			using (ICodeManager sut = ManagerFactory.CreateCodeManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				Relativity.DataExchange.Service.ChoiceInfo result = sut.RetrieveCodeByNameAndTypeID(
					this.TestParameters.WorkspaceId,
					this.PopulatedSingleChoiceFieldCodeType,
					this.uniqueValues[0]);

				// assert
				Assert.That(result, Is.Not.Null);
			}
		}

		[IdentifiedTest("0d7a8fd4-d29a-4300-9bde-15516471e3e7")]
		public void ShouldNotRetrieveCodeByNameAndTypeIDWhenParametersAreInvalid()
		{
			// arrange
			using (ICodeManager sut = ManagerFactory.CreateCodeManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act  && assert
				Assert.That(
					() => sut.RetrieveCodeByNameAndTypeID(
						NonExistingWorkspaceId,
						this.PopulatedSingleChoiceFieldCodeType,
						this.uniqueValues[0]),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));

				Assert.That(
					() => sut.RetrieveCodeByNameAndTypeID(
						NonExistingWorkspaceId,
						null,
						this.uniqueValues[0]),
					Throws.Exception);

				Relativity.DataExchange.Service.ChoiceInfo result = sut.RetrieveCodeByNameAndTypeID(
					this.TestParameters.WorkspaceId,
					this.PopulatedSingleChoiceFieldCodeType,
					NonExistingChoiceName);

				Assert.That(result, Is.Null);
			}
		}

		[IdentifiedTest("4f917dff-c81c-41b0-8703-b29b70ea4bcb")]
		public void ShouldRetrieveCodesAndTypesForCase()
		{
			// arrange
			using (ICodeManager sut = ManagerFactory.CreateCodeManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				DataSet results = sut.RetrieveCodesAndTypesForCase(this.TestParameters.WorkspaceId);

				// assert
				DataRow codeType = results.Tables[1].Rows.OfType<DataRow>()
					.SingleOrDefault(row => row.Field<string>(1) == PopulatedSingleChoiceFieldName);

				Assert.That(codeType, Is.Not.Null);

				DataRow[] codes = results.Tables[0].Rows.OfType<DataRow>().Where(row => row.Field<int>(3) == this.PopulatedSingleChoiceFieldCodeTypeId)
					.ToArray();

				Assert.That(codes.Length, Is.EqualTo(ChoicesCount));
			}
		}

		[IdentifiedTest("5d065aa6-4e94-4ca4-8e6e-051908487688")]
		public void ShouldNotRetrieveCodesAndTypesForCaseWhenParametersAreInvalid()
		{
			// arrange
			using (ICodeManager sut = ManagerFactory.CreateCodeManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(() => sut.RetrieveCodesAndTypesForCase(NonExistingWorkspaceId), this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		private void ReadCodeTypes()
		{
			using (IFieldQuery fieldQuery = ManagerFactory.CreateFieldQuery(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				kCura.EDDS.WebAPI.DocumentManagerBase.Field[] fields = fieldQuery.RetrieveAllAsArray(
					this.TestParameters.WorkspaceId,
					(int)ArtifactType.Document);

				this.emptySingleChoiceFieldCodeType = fields.Single(field => field.DisplayName == EmptySingleChoiceFieldName);
				this.populatedSingleChoiceFieldCodeType = fields.Single(field => field.DisplayName == PopulatedSingleChoiceFieldName);
			}
		}
	}
}