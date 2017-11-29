using System;
using System.IO;
using System.Threading;
using Relativity.Transfer;

namespace kCura.WinEDDS.TApi
{
	/// <summary>
	/// </summary>
	public class DownloadTapiBridge : TapiBridge
	{
		/// <summary>
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="log"></param>
		/// <param name="token"></param>
		public DownloadTapiBridge(TapiBridgeParameters parameters, ITransferLog log, CancellationToken token) : base(parameters, TransferDirection.Download, log, token)
		{
		}

		/// <summary>
		/// </summary>
		/// <param name="sourceFile"></param>
		/// <param name="targetFileName"></param>
		/// <param name="order"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		protected override TransferPath CreateTransferPath(string sourceFile, string targetFileName, int order)
		{
			var fileInfo = new FileInfo(targetFileName);
			var transferPath = new TransferPath
			{
				SourcePath = sourceFile,
				TargetPath = fileInfo.Directory.FullName,
				TargetFileName = fileInfo.Name,
				Order = order
			};
			return transferPath;
		}
	}
}