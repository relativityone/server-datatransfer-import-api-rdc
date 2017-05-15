using System.Collections.Generic;
using kCura.WinEDDS.Api;

namespace kCura.WinEDDS.Core.Import
{
	public class ImportBatchContext
	{
		public List<ArtifactFieldCollection> ArtifactFields { get; private set; }

		public ImportBatchContext()
		{
			ArtifactFields = new List<ArtifactFieldCollection>();
		}
	}
}
