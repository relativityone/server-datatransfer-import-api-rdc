using System;

namespace Relativity.Import.Export.Services
{
	public class Constants
	{
		public const Int32 LIST_VIEW_LONG_TEXT_TRIM_LENGTH = 1000;
		public const string LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN = "#KCURA99DF2F0FEB88420388879F1282A55760#";

		public static Int32 LONG_TEXT_MAX_LENGTH_FOR_LIST = 1048576;
		public static Int32 EXPORT_CHUNK_MAX_TEXT_SIZE = 10485760;
		public static Int32 EXPORT_CHUNK_MAX_FILE_COUNT = 10000;

		public const Int32 ADMIN_WORKSPACE_ARTIFACTID = -1;

		/// <summary>
		/// 		''' The default field delimiter to use when no other value is set.
		/// 		''' </summary>
		/// 		''' <remarks>The document loader is configurable, but the image loader is not</remarks>
		public const string DEFAULT_FIELD_DELIMITER = "þþKþþ";

		/// <summary>
		/// 		''' The value used at the end of a line by the image loader.
		/// 		''' </summary>
		/// 		''' <remarks>The document loader is configurable, but the image loader is not</remarks>
		public const string ENDLINETERMSTRING = DEFAULT_FIELD_DELIMITER + Microsoft.VisualBasic.Constants.vbCrLf;

		public const string LICENSE_AGREEMENT_TEXT = "The programs included herein are subject to a restricted use license and can only be used in conjunction with this application.";

		public const string BRANDING_JOB_STATUS_DEFAULT = "Error";
		public const string BRANDING_JOB_STATUS_WAITING = "Waiting";
		public const string BRANDING_JOB_STATUS_PROCESSING = "Processing";
		public const string BRANDING_JOB_STATUS_PENDING_FINALIZATION = "Pending Finalization";

		public static System.Guid DOCUMENT_OBJECT_GUID = new System.Guid("15C36703-74EA-4FF8-9DFB-AD30ECE7530D");
		public static System.Guid DASHBOARD_OBJECT_GUID = new System.Guid("DE1B78FF-B4C3-4E10-B01F-7065BCFFA4B8");


		public const string BINDERS_APPLICATION_GUID = "62AB9904-2F5E-442E-9D5E-259636EAE79C";

		public const string SINGLE_FILE_UPLOAD_APPLICATION_GUID = "1738CEB6-9546-44A7-8B9B-E64C88E47320";
		public const string TRANSCRIPTS_APPLICATION_GUID = "D2AA60D8-B04B-4AE4-A8AB-372492374736";
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

		public class RelativityApplicationsGUIDs
		{
			public static readonly Guid ANALYTICS = new Guid(ANALYTICS_APPLICATION_GUID_STRING);
			public static readonly Guid ANALYTICS_CORE = new Guid(ANALYTICS_CORE_APPLICATION_GUID_STRING);
			public static readonly Guid ANALYTICS_ACTIVE_LEARNING = new Guid(ANALYTICS_ACTIVE_LEARNING_APPLICATION_GUID_STRING);
			public static readonly Guid ASSISTED_REVIEW = new Guid(ASSISTED_REVIEW_APPLICATION_GUID_STRING);
			public static readonly Guid BINDERS = new Guid(BINDERS_APPLICATION_GUID_STRING);
			public static readonly Guid COLLECTIONS = new Guid(COLLECTIONS_APPLICATION_GUID_STRING);
			public static readonly Guid CREATE_AND_MAP_FIELD_CATALOG = new Guid(CREATE_AND_MAP_FIELD_CATALOG_GUID_STRING);
			public static readonly Guid DATAGRID_AUDIT = new Guid(DATAGRID_AUDIT_APPLICATION_GUID_STRING);
			public static readonly Guid DATAGRID_CORE = new Guid(DATAGRID_CORE_APPLICATION_GUID_STRING);
			public static readonly Guid DATAGRID_MIGRATION = new Guid(DATAGRID_MIGRATION_APPLICATION_GUID_STRING);
			public static readonly Guid DOCUMENT_VIEWER = new Guid(DOCUMENT_VIEWER_APPLICATION_GUID_STRING);
			public static readonly Guid ECA_AND_INVESTIGATION = new Guid(ECA_AND_INVESTIGATION_GUID_STRING);
			public static readonly Guid IMAGING = new Guid(IMAGING_APPLICATION_GUID_STRING);
			public static readonly Guid LIST = new Guid(LIST_APPLICATION_GUID_STRING);
			public static readonly Guid LIQUID_FORMS = new Guid(LIQUID_FORMS_APPLICATION_GUID_STRING);
			public static readonly Guid OCR = new Guid(OCR_APPLICATION_GUID_STRING);
			public static readonly Guid PERFORMANCE_DASHBOARD = new Guid(PERFORMANCE_DASHBOARD_APPLICATION_GUID_STRING);
			public static readonly Guid PORTAL = new Guid(PORTAL_APPLICATION_GUID_STRING);
			public static readonly Guid PRODUCTION = new Guid(PRODUCTION_APPLICATION_GUID_STRING);
			public static readonly Guid RELATIVITY = new Guid(RELATIVITY_GUID);
			public static readonly Guid RELATIVITY_INTEGRATION_POINTS = new Guid(RELATIVITY_INTEGRATION_POINTS_GUID_STRING);
			public static readonly Guid RELATIVITY_LEGAL_HOLD = new Guid(RELATIVITY_LEGAL_HOLD_GUID_STRING);
			public static readonly Guid RELATIVITY_LIST_PAGE = new Guid(RELATIVITY_LIST_PAGE_GUID_STRING);
			public static readonly Guid RELATIVITY_SERVICES = new Guid(RELATIVITY_SERVICES_GUID_STRING);
			public static readonly Guid SEARCH_TERM_REPORT = new Guid(SEARCH_TERM_REPORT_APPLICATION_GUID_STRING);
			public static readonly Guid SINGLE_FILE_UPLOAD = new Guid(SINGLE_FILE_UPLOAD_APPLICATION_GUID);
			public static readonly Guid STAGING_EXPLORER = new Guid(STAGING_EXPLORER_GUID_STRING);
			public static readonly Guid TELEMETRY = new Guid(TELEMETRY_APPLICATION_GUID_STIRNG);
			public static readonly Guid TRANSFORM_SET = new Guid(TRANSFORM_SET_APPLICATION_GUID_STRING);
			public static readonly Guid TRANSCRIPTS = new Guid(TRANSCRIPTS_APPLICATION_GUID);
		}

		public const string ANALYTICS_APPLICATION_GUID_STRING = "51B4E374-EF1B-43A0-B5A8-E2841AC3EFE1";
		public const string ANALYTICS_CORE_APPLICATION_GUID_STRING = "62284ADD-91F5-4F35-A582-BBCFA439AD8C";
		public const string ANALYTICS_ACTIVE_LEARNING_APPLICATION_GUID_STRING = "FBA7CD7D-2388-4F39-BD0A-8F92B626CBC9";
		public const string ASSISTED_REVIEW_APPLICATION_GUID_STRING = "81CEB2F0-747A-4E8B-AAD5-7C40D864D96D";
		public const string BINDERS_APPLICATION_GUID_STRING = "62AB9904-2F5E-442E-9D5E-259636EAE79C";
		public const string COLLECTIONS_APPLICATION_GUID_STRING = "CC9356DE-C1F9-4E3B-85E1-58890C5D7B66";
		public const string CREATE_AND_MAP_FIELD_CATALOG_GUID_STRING = "770536E6-56B7-4E70-8304-14208DFA84AC";
		public const string DATAGRID_AUDIT_APPLICATION_GUID_STRING = "293c50ad-7b6d-45d0-9121-7f3826e1cca5";
		public const string DATAGRID_CORE_APPLICATION_GUID_STRING = "6a8c2341-6888-44da-b1a4-5bdce0d1a383";
		public const string DATAGRID_MIGRATION_APPLICATION_GUID_STRING = "684B10BB-3B12-4BB1-83E9-A56A7D6CA67F";
		public const string DOCUMENT_VIEWER_APPLICATION_GUID_STRING = "5725CAB5-EE63-4155-B227-C74CC9E26A76";
		public const string ECA_AND_INVESTIGATION_GUID_STRING = "7457250B-DED5-4C5C-8F36-ED371F94EA93";
		public const string IMAGING_APPLICATION_GUID_STRING = "C9E4322E-6BD8-4A37-AE9E-C3C9BE31776B";
		public const string LIST_APPLICATION_GUID_STRING = "E5FDDDF9-B55B-454C-8D96-B8285D0DE2BE";
		public const string LIQUID_FORMS_APPLICATION_GUID_STRING = "FDFEEECF-449D-4C86-8C10-B062F58020C5";
		public const string OCR_APPLICATION_GUID_STRING = "8354D537-689A-4DDE-B057-5EF2FE4DBA2B";
		public const string PERFORMANCE_DASHBOARD_APPLICATION_GUID_STRING = "60a1d0a3-2797-4fb3-a260-614cbfd3fa0d";
		public const string PORTAL_APPLICATION_GUID_STRING = "5CC9766B-A006-4F11-BC1E-7B1099986E12";
		public const string PRODUCTION_APPLICATION_GUID_STRING = "51B19AB2-3D45-406C-A85E-F98C01B033EC";
		public const string RELATIVITY_GUID = "BD10A60D-B8EC-4928-84EE-6FC4F30D9612";
		public const string RELATIVITY_INTEGRATION_POINTS_GUID_STRING = "DCF6E9D1-22B6-4DA3-98F6-41381E93C30C";
		public const string RELATIVITY_LEGAL_HOLD_GUID_STRING = "98F31698-90A0-4EAD-87E3-DAC723FED2A6";
		public const string RELATIVITY_LIST_PAGE_GUID_STRING = "2ff16b11-a4ca-4f02-8bbb-1f07f23fe713";
		public const string RELATIVITY_SERVICES_GUID_STRING = "FEAEE623-044B-419D-9FDA-02FCC65EB71B";
		public const string SEARCH_TERM_REPORT_APPLICATION_GUID_STRING = "F807AC5A-6F0C-4EF3-9C7D-DB2BAE51A5F4";
		public const string STAGING_EXPLORER_GUID_STRING = "11395906-173E-43E8-926C-BEF995A4B0B2";
		public const string TELEMETRY_APPLICATION_GUID_STIRNG = "5F5CB82F-8981-4269-92BD-060034DF3648";
		public const string TRANSFORM_SET_APPLICATION_GUID_STRING = "71058546-35A7-4812-B7E5-7494C3D672B6";
		public const string QS_KEY_ARTIFACT_ID = "ArtifactID";
		public const string QS_KEY_ARTIFACT_TYPE_ID = "ArtifactTypeID";
		public const string QS_KEY_PARENT_ARTIFACT_ID = "ParentArtifactID";

		public class Export
		{
			/// <summary>
			/// 		''' Used to pick out the COALESCED text field from export precedence
			/// 		''' </summary>
			/// 		''' <remarks>Spaces are here to ensure that there will never be a a sql column name conflict, because we strip all punctuation out of non-generated fields' column names for query results</remarks>
			public const string TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME = "Text Precedence";
			public const string TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME = "KCURA FULL TEXT SOURCE";
			public const string TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_INDEX_AVF_COLUMN_NAME = "KCURA FULL TEXT SOURCE INDEX";
			public const string TEXT_PRECEDENCE_AWARE_TEXT_SIZE = "KCURA FULL TEXT SIZE";
			public const string BEGIN_BATES_COLUMN_NAME = "Begin Bates";
		}
		public class RegistryKeys
		{
			public const string HomeRealmKey = "HRDHint";
		}
	}
}
