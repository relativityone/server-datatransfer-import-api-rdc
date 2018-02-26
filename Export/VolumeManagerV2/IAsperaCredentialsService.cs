using System;
using System.Collections.Generic;
using System.Linq;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public interface IAsperaCredentialsService
	{
		Credential GetAsperaCredentialsForFileshare(Uri fileUri);
	}

	public class AsperaCredentialsServiceMock : IAsperaCredentialsService
	{

		private readonly Dictionary<Uri, Credential> _uris = new Dictionary<Uri, Credential>
		{
			{ new Uri("\\\\files1.il1ddftasmfs001.kcura.corp\\T002\\files\\"), new Credential() },
			{ new Uri("\\\\files1.il1ddftasmfs002.kcura.corp\\T002\\files\\"), new Credential() },
			{ new Uri("\\\\files1.il1ddftasmfs003.kcura.corp\\T002\\files\\"), new Credential() },
		};

		public Credential GetAsperaCredentialsForFileshare(Uri fileUri)
		{
			return _uris.First(uriCredential => uriCredential.Key.IsBaseOf(fileUri)).Value;
		}
	}
}
