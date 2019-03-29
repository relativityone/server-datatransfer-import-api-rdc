namespace Relativity.Export.VolumeManagerV2.Metadata.Natives
{
	using System;

	using kCura.WinEDDS.LoadFileEntry;

	public class LineNewLine
	{
		public void AddNewLine(DeferredEntry loadFileEntry)
		{
			loadFileEntry.AddStringEntry(Environment.NewLine);
		}
	}
}