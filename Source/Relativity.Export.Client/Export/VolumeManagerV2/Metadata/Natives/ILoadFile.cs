namespace Relativity.Export.VolumeManagerV2.Metadata.Natives
{
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	public interface ILoadFile
	{
		void Create(ObjectExportInfo[] artifacts, CancellationToken cancellationToken);
	}
}