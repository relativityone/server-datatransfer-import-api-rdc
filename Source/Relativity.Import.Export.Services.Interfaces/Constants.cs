namespace Relativity.Import.Export.Services
{
	using System;

	public static class Constants
	{
		public const Int32 LIST_VIEW_LONG_TEXT_TRIM_LENGTH = 1000;
		public const string LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN = "#KCURA99DF2F0FEB88420388879F1282A55760#";

		public static Int32 LONG_TEXT_MAX_LENGTH_FOR_LIST = 1048576;
		public static Int32 EXPORT_CHUNK_MAX_TEXT_SIZE = 10485760;
		public static Int32 EXPORT_CHUNK_MAX_FILE_COUNT = 10000;
		public const Int32 ADMIN_WORKSPACE_ARTIFACTID = -1;

		/// <summary>
		/// The default field delimiter to use when no other value is set.
		/// </summary>
		/// <remarks>The document loader is configurable, but the image loader is not</remarks>
		public const string DEFAULT_FIELD_DELIMITER = "þþKþþ";

		/// <summary>
		/// The value used at the end of a line by the image loader.
		/// </summary>
		/// <remarks>The document loader is configurable, but the image loader is not</remarks>
		public const string ENDLINETERMSTRING = DEFAULT_FIELD_DELIMITER + Microsoft.VisualBasic.Constants.vbCrLf;
		public const string LICENSE_AGREEMENT_TEXT = "The programs included herein are subject to a restricted use license and can only be used in conjunction with this application.";
		public const string BRANDING_JOB_STATUS_DEFAULT = "Error";
		public const string BRANDING_JOB_STATUS_WAITING = "Waiting";
		public const string BRANDING_JOB_STATUS_PROCESSING = "Processing";
		public const string BRANDING_JOB_STATUS_PENDING_FINALIZATION = "Pending Finalization";
		public static System.Guid DOCUMENT_OBJECT_GUID = new System.Guid("15C36703-74EA-4FF8-9DFB-AD30ECE7530D");
		public static System.Guid DASHBOARD_OBJECT_GUID = new System.Guid("DE1B78FF-B4C3-4E10-B01F-7065BCFFA4B8");
		public const string SFU_CUSTOM_PERMISSION_ENABLED_TOGGLE_NAME = "Relativity.SingleFileUpload.Core.Toggles.ValidateSFUCustomPermissions";
		public const string ADD_DOCUMENT_CUSTOM_PERMISSION = "New Document";
		public static System.Guid DOCUMENT_FIELD_CONCEPTUAL_INDEX_GUID = new System.Guid("664FA808-6579-425C-929D-EAA2B096B2DA");
		public static System.Guid DOCUMENT_FIELD_CLASSIFICATION_INDEX_GUID = new System.Guid("307B785E-3267-479D-B2CF-6FF6DE9259C3");
		public static System.Guid DOCUMENT_FIELD_RELATIVITY_IMAGE_COUNT_GUID = new System.Guid("D726B2D9-4192-43DF-86EF-27D36560931A");
		public const string PERSISTENT_LISTS = "Lists";
		public const string PERSISTENT_LISTS_OBJECT_TYPE_NAME = "Lists";
		public const string ASSOCIATED_ARTIFACTTYPEID = "AssociatedArtifactTypeID";
		public const string Name = "Name";
		public const string Notes = "Notes";
		public const string CreatedBy = "SystemCreatedBy";
		public const string CreatedOn = "SystemCreatedOn";
		public const string CSRF_COOKIE_NAME = "CSRFHolder";
		public const string CSRF_HEADER_NAME = "X-CSRF-Header";
		public const string CSRF_SESSION_NAME = "_CSRFValidationToken";
		public const string CSRF_FIELD_NAME = "CSRFPreventField";
		public const string RELATIVITY_SCRIPT_LOGIN = "RelativityScriptLogin";
		public const string RULB_COOKIE_NAME = "RULB";
		public const string SHAREDSERVICES_CLIENT_ID = "SharedServices";
		public const string CENTRALIDENTITYSERVER_ENVIRONMENT_ID = "5354535F-5365-636F-6E64-456173746572";
		public const string VIEW_TYPE_DEVELOPER = "Developer";
		public const string VIEW_TYPE_ADMIN_UPDATE = "Admin Update";
		public const string VIEW_TYPE_ADMIN_UPDATE_DELETE = "Admin Update/Delete";
		public const string VIEW_TYPE_SEARCH = "Search";
		public const string QS_KEY_ARTIFACT_ID = "ArtifactID";
		public const string QS_KEY_ARTIFACT_TYPE_ID = "ArtifactTypeID";
		public const string QS_KEY_PARENT_ARTIFACT_ID = "ParentArtifactID";
	}
}