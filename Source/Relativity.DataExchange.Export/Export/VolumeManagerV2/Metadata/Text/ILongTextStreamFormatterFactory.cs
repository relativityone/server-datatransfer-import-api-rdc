namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text
{
	using System.IO;

	using kCura.WinEDDS.Exporters;

	public interface ILongTextStreamFormatterFactory
	{
		ILongTextStreamFormatter Create(TextReader source);
	}
}