using System.Threading;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public class SafeIncrement
	{
		private int _current;

		public int GetNext()
		{
			return Interlocked.Increment(ref _current);
		}
	}
}