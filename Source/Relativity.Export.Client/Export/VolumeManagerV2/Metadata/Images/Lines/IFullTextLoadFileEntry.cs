namespace Relativity.Export.VolumeManagerV2.Metadata.Images.Lines
{
	using System;
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	using Relativity.Export.VolumeManagerV2.Metadata.Writers;

	public interface IFullTextLoadFileEntry : IDisposable
	{
		void WriteFullTextLine(ObjectExportInfo artifact, string batesNumber, int pageNumber, long pageOffset, IRetryableStreamWriter writer, CancellationToken token);
	}
}