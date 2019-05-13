namespace Relativity.DataExchange.Export.VolumeManagerV2.Repository
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;	

	public class EmptyLongTextBuilder : ILongTextBuilder
	{
		public IList<LongText> CreateLongText(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			return Enumerable.Empty<LongText>().ToList();
		}
	}
}