
using kCura.WinEDDS.Api;
using kCura.WinEDDS.Core.Import.Tasks;

namespace kCura.WinEDDS.Core.Import
{
	public class ImportBatchJob : IImportBatchJob
	{
		private readonly IImportNativesTask _importNativesTask;
		private readonly IImportFoldersTask _importFoldersTask;
		private readonly IImportPrepareMetadataTask _importCreateMetadataTask;
		private readonly IImportMetadataTask _importMetadataTask;

		public ImportBatchJob(IImportNativesTask importNativesTask, IImportFoldersTask importFoldersTask, IImportPrepareMetadataTask importCreateMetadataTask,
			IImportMetadataTask importMetadataTask)
		{
			_importNativesTask = importNativesTask;
			_importFoldersTask = importFoldersTask;
			_importCreateMetadataTask = importCreateMetadataTask;
			_importMetadataTask = importMetadataTask;
		}

		public void Run(ImportBatchContext batchContext)
		{
			// TODO:
			foreach (ArtifactFieldCollection record in batchContext.ArtifactFields)
			{
				UploadNatives(record);
				CreateFolderStructure();
				MetaDocument metadataDoc = CreateMetadata();
				UploadMetadata(metadataDoc);
			}
		}

		private void UploadMetadata(MetaDocument metadataDoc)
		{
			_importMetadataTask.Execute(metadataDoc);
		}

		private MetaDocument CreateMetadata()
		{
			return _importCreateMetadataTask.Execute();
		}

		private void CreateFolderStructure()
		{
			_importFoldersTask.Execute();
		}

		private void UploadNatives(ArtifactFieldCollection record)
		{
			_importNativesTask.Execute(record);
		}

		
		
	}
}
