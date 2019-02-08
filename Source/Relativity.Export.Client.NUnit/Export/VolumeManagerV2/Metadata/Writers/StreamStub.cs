using System.IO;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Writers
{
	public class StreamStub : MemoryStream
	{
		private int _counter;
		private readonly int _failOn;

		public StreamStub(int failOn)
		{
			_failOn = failOn;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (_counter == _failOn)
			{
				_counter++;
				throw new IOException();
			}

			base.Write(buffer, offset, count);
			_counter++;
		}
	}
}