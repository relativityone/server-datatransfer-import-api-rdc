// ----------------------------------------------------------------------------
// <copyright file="OverlayIdentifierTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Threading.Tasks;
	using global::NUnit.Framework;
	using kCura.Relativity.DataReaderClient;
	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Services.LinkManager.Interfaces;
	using Relativity.Testing.Identification;

	[TestFixture]
	[TestExecutionCategory.CI]
	public class OverlayIdentifierTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		private const RelativityVersion MinSupportedVersion = RelativityVersion.Foxglove;
		private bool testsSkipped = false;

		private int createdObjectArtifactTypeId;
		private int documentKeyFieldId;
		private int objectKeyFieldId;
		private int documentTextFieldId;
		private int objectTextFieldId;
		private int artifactTypeId;

		[OneTimeSetUp]
		public async Task OneTimeSetUp()
		{
			testsSkipped = RelativityVersionChecker.VersionIsLowerThan(
							   this.TestParameters,
							   MinSupportedVersion);
			if (!this.testsSkipped)
			{
				await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document).ConfigureAwait(false); // Remove all Documents imported in AssemblySetup

				this.createdObjectArtifactTypeId = await this.CreateObjectInWorkspaceAsync().ConfigureAwait(false);

				await Task.WhenAll(
					   this.CreateTextFieldAsync(this.createdObjectArtifactTypeId, WellKnownFields.KeyFieldName).ContinueWith(task => this.objectKeyFieldId = task.Result),
					   this.CreateTextFieldAsync((int)ArtifactTypeID.Document, WellKnownFields.KeyFieldName).ContinueWith(task => this.documentKeyFieldId = task.Result),
					   this.CreateTextFieldAsync(this.createdObjectArtifactTypeId, WellKnownFields.TextFieldName).ContinueWith(task => this.objectTextFieldId = task.Result),
					   this.CreateTextFieldAsync((int)ArtifactTypeID.Document, WellKnownFields.TextFieldName).ContinueWith(task => this.documentTextFieldId = task.Result))
				   .ConfigureAwait(false);
			}
		}

		[TearDown]
		public async Task TearDown()
		{
			if (!testsSkipped)
			{
				await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, this.artifactTypeId).ConfigureAwait(false);
			}
		}

		[OneTimeTearDown]
		public async Task OneTimeTearDown()
		{
			if (!testsSkipped)
			{
				await Task.WhenAll(
						FieldHelper.DeleteFieldAsync(this.TestParameters, this.objectKeyFieldId),
						FieldHelper.DeleteFieldAsync(this.TestParameters, this.documentKeyFieldId),
						FieldHelper.DeleteFieldAsync(this.TestParameters, this.objectTextFieldId),
						FieldHelper.DeleteFieldAsync(this.TestParameters, this.documentTextFieldId))
					.ConfigureAwait(false);
			}
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("53174721-9360-4708-8639-1b25e104c9ab")]
		[Feature.DataTransfer.ImportApi.Configuration.Validation]
		[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
		[TestType.Error]
		public void ShouldNotOverlayControlNumber([Values(OverwriteModeEnum.AppendOverlay, OverwriteModeEnum.Overlay)] OverwriteModeEnum overwriteMode)
		{
			DocumentWithKeyFieldDto[] initialData =
			{
				new DocumentWithKeyFieldDto("11", "A"),
				new DocumentWithKeyFieldDto("12", "B"),
				new DocumentWithKeyFieldDto("13", "C"),
				new DocumentWithKeyFieldDto("14", "D"),
			};

			DocumentWithKeyFieldDto[] importData =
			{
				new DocumentWithKeyFieldDto("21", "A"),
				new DocumentWithKeyFieldDto("22", "B"),
				new DocumentWithKeyFieldDto("23", "C"),
				new DocumentWithKeyFieldDto("24", "D"),
			};

			// ARRANGE && ACT && ASSERT
			Assert.Throws<ImportSettingsException>(() => ArrangeAndActOverlayIndetifierTest(ArtifactType.Document, overwriteMode, initialData, importData));
		}

		[TestCaseSource(typeof(OverlayIdentifierTestCases), nameof(OverlayIdentifierTestCases.ShouldOverlayIdentifierTestCaseData))]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("1c955071-67b1-40aa-819f-9c1fdc3020c0")]
		[Feature.DataTransfer.ImportApi.Operations.ImportRDOs]
		[TestType.MainFlow]
		[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "caseName is needed to identify test case")]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "caseName is needed to identify test case")]
		public void ShouldOverlayIdentifier(
			string caseName,
			OverwriteModeEnum overwriteMode,
			DocumentWithKeyFieldDto[] initialData,
			DocumentWithKeyFieldDto[] importData,
			DocumentWithKeyFieldDto[] expectedData)
		{
			// ARRANGE && ACT
			ImportTestJobResult results = ArrangeAndActOverlayIndetifierTest(ArtifactType.ObjectType, overwriteMode, initialData, importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, importData?.Length ?? 0);
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(importData?.Length));

			string[] fieldsToValidate = new[] { WellKnownFields.ControlNumber, WellKnownFields.KeyFieldName };
			DocumentWithKeyFieldDto[] resultData = RdoHelper.QueryRelativityObjects(this.TestParameters, this.artifactTypeId, fieldsToValidate)
				.Select(ro => new DocumentWithKeyFieldDto((string)ro.FieldValues[0].Value, (string)ro.FieldValues[1].Value))
				.ToArray();

			CollectionAssert.AreEquivalent(expectedData.Select(d => d.ToString()), resultData.Select(d => d.ToString()));
		}

		[TestCaseSource(typeof(OverlayIdentifierTestCases), nameof(OverlayIdentifierTestCases.ShouldOverlayIdentifierWithErrorTestCaseData))]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("76377061-b274-4c22-abce-b0fa05aba143")]
		[Feature.DataTransfer.ImportApi.Operations.ImportRDOs]
		[TestType.Error]
		[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "caseName is needed to identify test case")]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "caseName is needed to identify test case")]
		public void ShouldOverlayIdentifierWithError(
			string caseName,
			OverwriteModeEnum overwriteMode,
			DocumentWithKeyFieldDto[] initialData,
			DocumentWithKeyFieldDto[] importData,
			DocumentWithKeyFieldDto[] expectedData,
			string errorMessage)
		{
			// ARRANGE && ACT
			ImportTestJobResult results = ArrangeAndActOverlayIndetifierTest(ArtifactType.ObjectType, overwriteMode, initialData, importData);

			// ASSERT
			Assert.That(results.ErrorRows.Count, Is.GreaterThan(0));
			ThenTheErrorRowsHaveCorrectMessage(results.ErrorRows, errorMessage);

			// verify if initial data didn't change
			string[] fieldsToValidate = new[] { WellKnownFields.ControlNumber, WellKnownFields.KeyFieldName };
			DocumentWithKeyFieldDto[] resultData = RdoHelper.QueryRelativityObjects(this.TestParameters, this.artifactTypeId, fieldsToValidate)
				.Select(ro => new DocumentWithKeyFieldDto((string)ro.FieldValues[0].Value, (string)ro.FieldValues[1].Value))
				.ToArray();

			CollectionAssert.AreEquivalent(expectedData.Select(d => d.ToString()), resultData.Select(d => d.ToString()));
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("4089f177-7332-40fc-9409-36fc4e9d00f9")]
		[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
		[Feature.DataTransfer.ImportApi.Operations.ImportRDOs]
		[TestType.MainFlow]
		public void ShouldOverlayWithoutIdentifier([Values(ArtifactType.Document, ArtifactType.ObjectType)] ArtifactType artifactType)
		{
			// ARRANGE
			this.artifactTypeId = GetArtifactTypeIdForTest(artifactType);
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings(this.artifactTypeId);

			// Prepare data for import under test
			DocumentWithKeyFieldDto[] initialData =
			{
				new DocumentWithKeyFieldDto("100010", "AAA"), new DocumentWithKeyFieldDto("100011", "BBB"),
				new DocumentWithKeyFieldDto("100012", "CCC"), new DocumentWithKeyFieldDto("100013", "DDD"),
				new DocumentWithKeyFieldDto("100014", "EEE"),
			};

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);
			this.JobExecutionContext.Execute(initialData);

			DocumentWithoutIdentifierDto[] importData =
			{
				new DocumentWithoutIdentifierDto("AAA", "text1"),
				new DocumentWithoutIdentifierDto("BBB", "text2"),
				new DocumentWithoutIdentifierDto("CCC", "text3"),
				new DocumentWithoutIdentifierDto("DDD", "text4"),
			};

			DocumentWithoutIdentifierDto[] expectedData =
			{
				new DocumentWithoutIdentifierDto("AAA", "text1"),
				new DocumentWithoutIdentifierDto("BBB", "text2"),
				new DocumentWithoutIdentifierDto("CCC", "text3"),
				new DocumentWithoutIdentifierDto("DDD", "text4"),
				new DocumentWithoutIdentifierDto("EEE", null),
			};

			settings.OverwriteMode = OverwriteModeEnum.Overlay;
			settings.IdentityFieldId = GetKeyFieldIdForTest(artifactType);
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, importData?.Length ?? 0);
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(importData?.Length));

			string[] fieldsToValidate = new[] { WellKnownFields.KeyFieldName, WellKnownFields.TextFieldName };

			DocumentWithoutIdentifierDto[] resultData = RdoHelper.QueryRelativityObjects(this.TestParameters, this.artifactTypeId, fieldsToValidate)
				.Select(ro => new DocumentWithoutIdentifierDto((string)ro.FieldValues[0].Value, (string)ro.FieldValues[1].Value))
				.ToArray();

			CollectionAssert.AreEquivalent(expectedData.Select(d => d.ToString()), resultData.Select(d => d.ToString()));
		}

		private ImportTestJobResult ArrangeAndActOverlayIndetifierTest(ArtifactType artifactType, OverwriteModeEnum overwriteMode, DocumentWithKeyFieldDto[] initialData, DocumentWithKeyFieldDto[] importData)
		{
			this.artifactTypeId = GetArtifactTypeIdForTest(artifactType);
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings(this.artifactTypeId);

			// Prepare data for import under test
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);
			this.JobExecutionContext.Execute(initialData);

			settings.OverwriteMode = overwriteMode;
			settings.IdentityFieldId = GetKeyFieldIdForTest(artifactType);
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			// ACT
			return this.JobExecutionContext.Execute(importData);
		}

		private int GetArtifactTypeIdForTest(ArtifactType artifactType)
		{
			return artifactType == ArtifactType.Document
				? (int)ArtifactTypeID.Document
				: this.createdObjectArtifactTypeId;
		}

		private async Task<int> CreateTextFieldAsync(int rdoArtifactTypeId, string fieldName)
		{
			return await FieldHelper.CreateFixedLengthTextFieldAsync(
				this.TestParameters,
				rdoArtifactTypeId,
				fieldName,
				false,
				length: 50)
				.ConfigureAwait(false);
		}

		private int GetKeyFieldIdForTest(ArtifactType artifactType)
		{
			int keyFieldId = artifactType == ArtifactType.Document
				? this.documentKeyFieldId
				: this.objectKeyFieldId;
			return keyFieldId;
		}

		private class DocumentWithoutIdentifierDto
		{
			public DocumentWithoutIdentifierDto(string keyField, string textField)
			{
				this.TextField = textField;
				this.KeyField = keyField;
			}

			[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Must be public for DataReader")]
			[System.ComponentModel.DisplayName(WellKnownFields.KeyFieldName)]
			public string KeyField { get; }

			[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Must be public for DataReader")]
			[System.ComponentModel.DisplayName(WellKnownFields.TextFieldName)]
			public string TextField { get; }

			public override string ToString()
			{
				return $"{KeyField}_{TextField}";
			}
		}
	}
}