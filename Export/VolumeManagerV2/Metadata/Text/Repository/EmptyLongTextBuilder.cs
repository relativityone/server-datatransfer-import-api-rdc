using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository
{
	public class EmptyLongTextBuilder : ILongTextBuilder
	{
		public IEnumerable<LongText> CreateLongText(ObjectExportInfo artifact)
		{
			return Enumerable.Empty<LongText>();
		}
	}
}