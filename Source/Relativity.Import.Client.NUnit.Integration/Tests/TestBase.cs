using System;
using System.Collections.Generic;
using kCura.NUnit.Integration;
using kCura.Relativity.Client;
using kCura.Relativity.ImportAPI.IntegrationTests.Services;
using NUnit.Framework;
using Platform.Keywords.Connection;
using Platform.Keywords.RSAPI;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Tests
{
	public class TestBase
	{
		private static int _objectTypeUniqueSuffix;
		private readonly List<int> _createdArtifacts = new List<int>();

		// The custom object type under test.
		protected const string TRANSFER_OBJECT_TYPE_NAME = "Transfer";
		protected const string TRANSFER_FIELD_DESCRIPTION = "Description";
		protected const string TRANSFER_FIELD_NAME = "Name";
		protected const string TRANSFER_FIELD_DETAIL_ID = "TransferDetailId";
		protected const string TRANSFER_FIELD_DATASOURCEID = "TransferDataSourceId";

		// To test single-object fields.
		protected const string TRANSFERDETAIL_OBJECT_TYPE_NAME = "TransferDetail";
		protected const string TRANSFERDETAIL_FIELD_NAME = "Name";

		// To test multiple-object fields.
		protected const string TRANSFERDATASOURCE_OBJECT_TYPE_NAME = "TransferDataSource";
		protected const string TRANSFERDATASOURCE_FIELD_NAME = "Name";

		protected int WorkspaceId { get; private set; }
		protected string DocumentIdentifierColumnName { get; private set; }
		protected int TransferDataSourceArtifactTypeId { get; set; }
		protected int TransferDataSourceDescriptorArtifactTypeId { get; set; }
		protected int TransferDetailArtifactTypeId { get; set; }
		protected int TransferDetailDescriptorArtifactTypeId { get; set; }
		protected int TransferArtifactTypeId { get; set; }
		protected int TransferIdentifierFieldId { get; set; }
		protected int ObjectTypeUniqueSuffix => _objectTypeUniqueSuffix;

		[OneTimeSetUp]
		public virtual void OneTimeSetUp()
		{
			CreateWorkspace();
			DocumentIdentifierColumnName = FieldService.GetDocumentIdentifierFieldName(WorkspaceId);
			_objectTypeUniqueSuffix++;
			this.OnOneTimeSetUp();
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			DeleteWorkspace();
			this.OnOneTimeTearDown();
		}

		[SetUp]
		public void Setup()
		{
			this.OnSetup();
		}

		[TearDown]
		public void TearDown()
		{
			// Note: this removes the artifacts from the supplied list.
			ObjectService.DeleteObjects(this.WorkspaceId, this._createdArtifacts);
			this.OnTearDown();
		}

		protected virtual void OnOneTimeSetUp()
		{
		}

		protected virtual void OnOneTimeTearDown()
		{
		}

		protected virtual void OnSetup()
		{
		}

		protected virtual void OnTearDown()
		{
		}

		protected void CreateTransferDetailObjectType()
		{
			string transferDetailObjectType = $"{TRANSFERDETAIL_OBJECT_TYPE_NAME}-{this.ObjectTypeUniqueSuffix}";
			int transferDetailDescriptorTypeId = ObjectTypeService.CreateObjectType(this.WorkspaceId, transferDetailObjectType);
			this.TransferDetailDescriptorArtifactTypeId =
				ObjectTypeService.QueryObjectTypeDescriptorId(this.WorkspaceId, transferDetailDescriptorTypeId);
			this.TransferDetailArtifactTypeId = ObjectService.QueryArtifactTypeId(this.WorkspaceId, transferDetailObjectType);
		}

		protected void CreateTransferDataSourceObjectType()
		{
			// This is a many-to-many relationship.
			string transferDataSourceObjectType = $"{TRANSFERDATASOURCE_OBJECT_TYPE_NAME}-{this.ObjectTypeUniqueSuffix}";
			int transferDataSourceDescriptorTypeId = ObjectTypeService.CreateObjectType(this.WorkspaceId, transferDataSourceObjectType);
			this.TransferDataSourceDescriptorArtifactTypeId =
				ObjectTypeService.QueryObjectTypeDescriptorId(this.WorkspaceId, transferDataSourceDescriptorTypeId);
			this.TransferDataSourceArtifactTypeId = ObjectService.QueryArtifactTypeId(this.WorkspaceId, transferDataSourceObjectType);
		}

		protected void CreateTransferObjectType()
		{
			string transferObjectType = $"{TRANSFER_OBJECT_TYPE_NAME}-{this.ObjectTypeUniqueSuffix}";
			int transferDescriptorTypeId = ObjectTypeService.CreateObjectType(this.WorkspaceId, transferObjectType);
			int transferWorkspaceObjectTypeId =
				ObjectTypeService.QueryObjectTypeDescriptorId(this.WorkspaceId, transferDescriptorTypeId);
			FieldService.CreateSingleObjectField(
				this.WorkspaceId,
				transferWorkspaceObjectTypeId,
				this.TransferDetailDescriptorArtifactTypeId,
				TRANSFER_FIELD_DETAIL_ID);
			FieldService.CreateMultipleObjectField(
				this.WorkspaceId,
				transferWorkspaceObjectTypeId,
				this.TransferDataSourceDescriptorArtifactTypeId,
				TRANSFER_FIELD_DATASOURCEID);
			this.TransferArtifactTypeId = ObjectService.QueryArtifactTypeId(this.WorkspaceId, transferObjectType);
			this.TransferIdentifierFieldId = FieldService.GetIdentifierFieldId(this.WorkspaceId, transferObjectType);
		}

		protected void CreateSingleObjectInstance(string name)
		{
			Dictionary<string, object> fields = new Dictionary<string, object>
			{
				{ TRANSFERDETAIL_FIELD_NAME, name }
			};

			int artifactId = ObjectService.CreateObject(
				this.WorkspaceId,
				this.TransferDetailArtifactTypeId,
				fields);
			this._createdArtifacts.Add(artifactId);
		}

		protected void CreateMultipleObjectInstance(string name)
		{
			Dictionary<string, object> fields = new Dictionary<string, object>
			{
				{ TRANSFERDATASOURCE_FIELD_NAME, name }
			};

			int artifactId = ObjectService.CreateObject(
				this.WorkspaceId,
				this.TransferDataSourceArtifactTypeId,
				fields);
			this._createdArtifacts.Add(artifactId);
		}

		private void CreateWorkspace()
		{
			using (IRSAPIClient rsapiClient = ServiceFactory.GetProxy<IRSAPIClient>(SharedTestVariables.ADMIN_USERNAME,
				SharedTestVariables.DEFAULT_PASSWORD))
			{
				string now = DateTime.Now.ToString("MM-dd HH.mm.ss.fff");
				WorkspaceId =
					WorkspaceHelpers.CreateWorkspace(rsapiClient, $"Import API test workspace ({now})", "Relativity Starter Template");
				WorkspaceHelpers.MarkTestWorkspaceAsUsed(WorkspaceId);
			}
		}

		private void DeleteWorkspace()
		{
			if (WorkspaceId > 0)
			{
				WorkspaceHelpers.DeleteTestWorkspace(WorkspaceId);
			}
		}
	}
}
