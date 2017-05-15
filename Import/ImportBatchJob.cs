
using kCura.WinEDDS.Api;

namespace kCura.WinEDDS.Core.Import
{
	public class ImportBatchJob : IImportBatchJob
	{
		public void Run(ImportBatchContext batchContext)
		{

			foreach (ArtifactFieldCollection record in batchContext.ArtifactFields)
			{
				UploadNatives(record);
				CreateFolderStructure();
				CreateMetadata();
				UploadMetadata();
			}
		}

		private void UploadMetadata()
		{
			throw new System.NotImplementedException();
		}

		private void CreateMetadata()
		{
			throw new System.NotImplementedException();
		}

		private void CreateFolderStructure()
		{
			throw new System.NotImplementedException();
		}

		private void UploadNatives(ArtifactFieldCollection record)
		{
			throw new System.NotImplementedException();
		}

		
		
	}
}
