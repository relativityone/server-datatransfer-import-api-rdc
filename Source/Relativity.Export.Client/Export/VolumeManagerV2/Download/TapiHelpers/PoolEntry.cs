namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public class PoolEntry
	{
		public IDownloadTapiBridge Bridge { get; }
		public bool InUse { get; set; }
		public bool Connected { get; set; }

		public PoolEntry(IDownloadTapiBridge bridge)
		{
			Bridge = bridge;
		}
	}
}