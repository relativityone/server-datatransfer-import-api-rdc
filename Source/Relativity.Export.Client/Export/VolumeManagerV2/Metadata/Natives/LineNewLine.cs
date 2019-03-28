using System;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives
{
	public class LineNewLine
	{
		public void AddNewLine(DeferredEntry loadFileEntry)
		{
			loadFileEntry.AddStringEntry(Environment.NewLine);
		}
	}
}