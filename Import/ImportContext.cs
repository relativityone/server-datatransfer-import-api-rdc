using System;

namespace kCura.WinEDDS.Core.Import
{
	public class ImportContext
	{
		public LoadFile Settings { get; set; }

		public Guid JobRunId { get; set; }
		public long TotalRecordCount { get; set; }
		public int? ParentFolderId { get; set; }
		
	}
}
