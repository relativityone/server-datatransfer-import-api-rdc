using System;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public interface IErrorFileWriter : IDisposable
	{
		void Write(ErrorFileWriter.ExportFileType type, string recordIdentifier, string fileLocation, string errorText);
	}
}