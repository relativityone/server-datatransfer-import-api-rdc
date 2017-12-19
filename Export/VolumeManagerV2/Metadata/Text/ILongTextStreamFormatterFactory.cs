using System.IO;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public interface ILongTextStreamFormatterFactory
	{
		ILongTextStreamFormatter Create(TextReader source);
	}
}