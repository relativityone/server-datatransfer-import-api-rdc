using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository
{
	public interface ILongTextBuilder
	{
		IList<LongText> CreateLongText(ObjectExportInfo artifact, CancellationToken cancellationToken);
	}
}