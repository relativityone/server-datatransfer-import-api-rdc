using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository
{
	public interface ILongTextBuilder
	{
		IList<LongText> CreateLongText(ObjectExportInfo artifact, CancellationToken cancellationToken);
	}
}