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

	[TestFixture(true)]
	[TestFixture(false)]
	public class KeplerCodeManagerTests : KeplerServiceTestBase
	{
		private const string EmptySingleChoiceFieldName = "EmptySingleChoiceField";
		private const string PopulatedSingleChoiceFieldName = "PopulatedSingleChoiceField";
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
			parentId = await WorkspaceHelper.ReadRootArtifactId(this.TestParameters, this.TestParameters.WorkspaceId).ConfigureAwait(false);

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

		[Test]
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

		[Test]
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

		[Test]
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

		[Test]
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

		[Test]
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

		[Test]
		public void SholdRetrieveAllCodesOfType()
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

		[Test]
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

		[Test]
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