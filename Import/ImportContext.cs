using System;

namespace kCura.WinEDDS.Core.Import
{
	public class ImportContext
	{
		public LoadFile Settings { get; set; }

		public Guid JobRunId { get; set; }
		public string RunId => JobRunId.ToString().Replace("-", "_");
		public long TotalRecordCount { get; set; }
		public int? ParentFolderId { get; set; }
		
	}
}
