namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System.Threading;

	public class SafeIncrement
	{
		private int _current;

		public int GetNext()
		{
			return Interlocked.Increment(ref _current);
		}
	}
}