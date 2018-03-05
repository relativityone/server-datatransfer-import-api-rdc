using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public interface IFileshareCredentialsService
	{
		AsperaCredential GetCredentialsForFileshare(Uri fileUri);
	}
}
