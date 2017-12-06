using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository
{
	public class EmptyLongTextBuilder : ILongTextBuilder
	{
		public IList<LongText> CreateLongText(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			return Enumerable.Empty<LongText>().ToList();
		}
	}
}