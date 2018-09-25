using System;
using System.Collections.Generic;
using System.Linq;
using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI.IntegrationTests.Helpers;
using kCura.Relativity.ImportAPI.IntegrationTests.Tests;

namespace kCura.Relativity.ImportAPI.IntegrationTests
{
	public class TestBase
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
			job.OnComplete += JobOnOnComplete;
			job.OnFatalException += JobOnOnFatalException;
			setupJobAction(job);

			// act
			job.Execute();
		}

		private void JobOnOnComplete(JobReport jobreport)
		{
			if (jobreport.FatalException != null)
			{
				throw jobreport.FatalException;
			}

			if (jobreport.ErrorRowCount > 0)
			{
				IEnumerable<string> errors = jobreport.ErrorRows.Select(x => $"{x.Identifier} - {x.Message}");
				throw new ImportApiTestException(string.Join("\n", errors));
			}
		}

		private void JobOnOnFatalException(JobReport jobreport)
		{
			throw jobreport.FatalException;
		}
	}
}
