namespace Relativity.Export.VolumeManagerV2.Metadata.Writers
{
	using System;

	public interface IErrorFileWriter : IDisposable
	{
		void Write(ErrorFileWriter.ExportFileType type, string recordIdentifier, string fileLocation, string errorText);
	}
}