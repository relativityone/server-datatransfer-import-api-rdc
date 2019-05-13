namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images.Lines
{
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;

	public class NoFullTextLoadFileEntry : IFullTextLoadFileEntry
	{
		public void WriteFullTextLine(ObjectExportInfo artifact, string batesNumber, int pageNumber, long pageOffset, IRetryableStreamWriter writer, CancellationToken token)
		{
		}

		public void Dispose()
		{
		}
	}
}