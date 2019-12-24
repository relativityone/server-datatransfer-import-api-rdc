namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers
{
	using System;
	using System.Threading.Tasks;

	using kCura.WinEDDS.Exporters;

	public interface IErrorFileWriter : IDisposable
	{
		void Write(ErrorFileWriter.ExportFileType type, ObjectExportInfo documentInfo, string fileLocation, string errorText);
		Task WriteAsync(ErrorFileWriter.ExportFileType type, ObjectExportInfo documentInfo, string fileLocation, string errorText);
	}
}