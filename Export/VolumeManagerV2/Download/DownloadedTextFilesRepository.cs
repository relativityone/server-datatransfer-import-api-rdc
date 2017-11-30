using System.Collections.Generic;
using System.Linq;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class DownloadedTextFilesRepository
	{
		private readonly IList<TextExportRequest> _textExportRequests;

		public DownloadedTextFilesRepository()
		{
			_textExportRequests = new List<TextExportRequest>();
		}

		public void AddTextExportLocation(TextExportRequest exportRequest)
		{
			_textExportRequests.Add(exportRequest);
		}

		public bool IsTextFileDownloaded(int artifactId, int fieldArtifactId)
		{
			return _textExportRequests.Any(x => x.FieldArtifactId == fieldArtifactId && x.ArtifactId == artifactId);
		}

		public string GetTextFileLocation(int artifactId, int fieldArtifactId)
		{
			return _textExportRequests.First(x => x.FieldArtifactId == fieldArtifactId && x.ArtifactId == artifactId).DestinationLocation;
		}
	}
}