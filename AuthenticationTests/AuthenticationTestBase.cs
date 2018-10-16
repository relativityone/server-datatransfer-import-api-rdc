using System;
using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI.IntegrationTests.Helpers;

namespace kCura.Relativity.ImportAPI.IntegrationTests.AuthenticationTests
{
	public class AuthenticationTestBase
	{
		protected const string CONTROL_NUMBER_COLUMN_NAME = "Control Number";

		public void SetupJobSettings(ImportSettingsBase settings)
		{
			settings.CaseArtifactId = Utils.GetWorkspaceId();

			settings.SelectedIdentifierFieldName = CONTROL_NUMBER_COLUMN_NAME;

			settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;
			settings.CopyFilesToDocumentRepository = true;
			settings.DisableExtractedTextEncodingCheck = true;
			settings.DisableUserSecurityCheck = true;
			settings.ExtractedTextFieldContainsFilePath = false;
			settings.MaximumErrorCount = int.MaxValue - 1;
			settings.StartRecordNumber = 0;
			settings.Billable = false;
			settings.LoadImportedFullTextFromServer = false;
			settings.MoveDocumentsInAppendOverlayMode = false;
		}

		public void ImportIntoWorkspace<T>(T job, Action<T> setupJobAction) where T : IImportNotifier, IImportBulkArtifactJob
		{
			ImportApiTestErrorHandler.Subscribe(job);
			setupJobAction(job);

			// act
			job.Execute();
		}
	}
}
