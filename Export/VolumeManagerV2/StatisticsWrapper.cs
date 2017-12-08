using kCura.WinEDDS.TApi;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	/// <summary>
	///     TODO better name
	/// </summary>
	public class StatisticsWrapper
	{
		private TapiBridge _filesBridge;
		private TapiBridge _textBridge;

		public void AttachToTapiBridgeForFiles(TapiBridge tapiBridge)
		{
			_filesBridge = tapiBridge;
			_filesBridge.TapiErrorMessage += OnFileErrorMessage;
			_filesBridge.TapiWarningMessage += OnFileWarningMessage;
			_filesBridge.TapiStatusMessage += OnFileStatusMessage;
			_filesBridge.TapiProgress += OnFileProgress;
		}

		private void OnFileProgress(object sender, TapiProgressEventArgs e)
		{
			
		}

		private void OnFileStatusMessage(object sender, TapiMessageEventArgs e)
		{
			
		}

		private void OnFileWarningMessage(object sender, TapiMessageEventArgs e)
		{
			
		}

		private void OnFileErrorMessage(object sender, TapiMessageEventArgs e)
		{
			
		}

		public void AttachToTapiBridgeForLongText(TapiBridge tapiBridge)
		{
			_textBridge = tapiBridge;
		}

		public void DetachAll()
		{
			_filesBridge.TapiErrorMessage -= OnFileErrorMessage;
			_filesBridge.TapiWarningMessage -= OnFileWarningMessage;
			_filesBridge.TapiStatusMessage -= OnFileStatusMessage;
			_filesBridge.TapiProgress -= OnFileProgress;
		}
	}
}