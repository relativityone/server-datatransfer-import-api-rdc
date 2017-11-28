using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using kCura.WinEDDS.Core.Export.VolumeManagerV2;
using kCura.WinEDDS.Exceptions;
using kCura.WinEDDS.Exporters;
using NUnit.Framework;
using Polly;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2
{
	public class ABC
	{
		private int i = 0;
		private CancellationTokenSource _cancellationTokenSource;

		[Test]
		public void F()
		{
			var a = @"C:\a.qwerty";
			var b = Path.ChangeExtension(a, "zxc");
		}

		private void Action1(Context context, CancellationToken token)
		{
			if (i < 4)
			{
				_cancellationTokenSource.Cancel();
				i++;
				throw new FileWriteException(FileWriteException.DestinationFile.Image, null);
			}
		}

		public class Status : IStatus
		{
			public void WriteWarning(string warning)
			{
				
			}

			public void WriteError(string error)
			{
				throw new NotImplementedException();
			}

			public void WriteImgProgressError(ObjectExportInfo artifact, int imageIndex, Exception ex, string notes)
			{
				throw new NotImplementedException();
			}
		}
	}
}
