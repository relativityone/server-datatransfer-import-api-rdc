// -----------------------------------------------------------------------------------------------------
// <copyright file="StreamStub.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.IO;

	public class StreamStub : MemoryStream
	{
		private readonly int _failOn;
		private int _counter;

		public StreamStub(int failOn)
		{
			this._failOn = failOn;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this._counter == this._failOn)
			{
				this._counter++;
				throw new IOException();
			}

			base.Write(buffer, offset, count);
			this._counter++;
		}
	}
}