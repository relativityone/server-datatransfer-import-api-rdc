using kCura.WinEDDS.TApi;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public interface ITransferClientHandler
	{
		void Attach(TapiBridge tapiBridge);
		void Detach();
	}

	public class TransferClientHandler : ITransferClientHandler
	{
		private TapiBridge _tapiBridge;

		public void Attach(TapiBridge tapiBridge)
		{
			_tapiBridge = tapiBridge;
			_tapiBridge.TapiClientChanged += OnClientChanged;
		}

		private void OnClientChanged(object sender, TapiClientEventArgs e)
		{
			
		}

		public void Detach()
		{
			_tapiBridge.TapiClientChanged -= OnClientChanged;
		}
	}
}