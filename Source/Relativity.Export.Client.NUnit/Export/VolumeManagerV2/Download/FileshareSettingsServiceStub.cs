using System;
using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Download
{
	public class FileshareSettingsServiceStub : IFileshareSettingsService
	{
		private readonly List<RelativityFileShareSettings> _fileshares;

		public FileshareSettingsServiceStub(IEnumerable<string> fileshares)
		{
			_fileshares = fileshares.Select(f => new RelativityFileShareSettings(f, new AsperaCredential())).ToList();
		}

		public RelativityFileShareSettings GetSettingsForFileshare(string fileUri)
		{
			return _fileshares.FirstOrDefault(fs => fs.FileshareUri.IsBaseOf(new Uri(fileUri)));
		}
	}
}