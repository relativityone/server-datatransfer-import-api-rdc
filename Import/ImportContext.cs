using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.WinEDDS.Core.Import
{
	public class ImportContext
	{
		public LoadFile Args { get; set; }
		public int Timezoneoffset { get; set; }
		public bool DoRetryLogic { get; set; }
		public bool AutoDetect { get; set; }

		public bool InitializeArtifactReader { get; set; }
		public Guid JobRunId { get; set; }
		public long TotalRecordCount { get; set; }

		public int? ParentFolderId { get; set; }
	}
}
