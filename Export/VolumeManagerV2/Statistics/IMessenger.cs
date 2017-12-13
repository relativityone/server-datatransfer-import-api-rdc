namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public interface IMessenger
	{
		void CreatingImageLoadFileMetadata();
		void CreatingLoadFileMetadata();
		void StartingRollupImages();
		void PreparingBatchForExport();
		void ValidatingBatch();
		void RestoringAfterCancel();
		void BatchCompleted();
		void DownloadingBatch();
		void FilesDownloadCompleted();
		void StateRestored();
	}
}