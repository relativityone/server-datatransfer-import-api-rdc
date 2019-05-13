﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;
	using System.Threading;

	public static class WaitHandleExtensions
	{
		public static bool WaitOne(this WaitHandle handle, TimeSpan timeSpan, CancellationToken cancellationToken)
		{
			var n = WaitHandle.WaitAny(new[] {handle, cancellationToken.WaitHandle}, timeSpan);
			switch (n)
			{
				case WaitHandle.WaitTimeout:
					return false;
				case 0:
					return true;
				default:
					cancellationToken.ThrowIfCancellationRequested();
					return false; // never reached
			}
		}
	}
}