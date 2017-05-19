
using System.Collections.Specialized;
using kCura.WinEDDS.Api;

namespace kCura.WinEDDS.Core.Import
{
	public interface IImportMetadata
	{
		IArtifactReader ArtifactReader { get; }

		NameValueCollection ProcessedDocIdentifiers { get; set; }

		string PrepareFieldsAndExtractIdentityValue(FileMetadata fileMetadata);
	}
}
