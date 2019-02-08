using System;
using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using Moq;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Download
{
	public class ExportTapyBridgeFactoryStub : IExportTapiBridgeFactory
	{
		private Dictionary<RelativityFileShareSettings, Mock<IDownloadTapiBridge>> _tapiBridges;

		public ExportTapyBridgeFactoryStub()
		{
			_tapiBridges = new Dictionary<RelativityFileShareSettings, Mock<IDownloadTapiBridge>>();
		}

		public IDownloadTapiBridge CreateForLongText(CancellationToken token)
		{
			throw new NotImplementedException();
		}

		public IDownloadTapiBridge CreateForFiles(RelativityFileShareSettings fileshareSettings, CancellationToken token)
		{
			_tapiBridges[fileshareSettings] = new Mock<IDownloadTapiBridge>();
			return _tapiBridges[fileshareSettings].Object;
		}

		public void VerifyBridges(Action<Mock<IDownloadTapiBridge>> verifyAction)
		{
			foreach (var tapiBridge in _tapiBridges.Values)
			{
				verifyAction(tapiBridge);
			}
		}
	}
}