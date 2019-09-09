namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers
{
	using System;

	using kCura.WinEDDS.Exporters;

	public interface IErrorFileWriter : IDisposable
	{
		void Write(ErrorFileWriter.ExportFileType type, ObjectExportInfo documentInfo, string fileLocation, string errorText);
	}
}