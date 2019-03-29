namespace Relativity.Export.VolumeManagerV2.Repository
{
	using System.Collections.Generic;
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	using Relativity.Export.VolumeManagerV2.Metadata.Text;

	public interface ILongTextBuilder
	{
		IList<LongText> CreateLongText(ObjectExportInfo artifact, CancellationToken cancellationToken);
	}
}