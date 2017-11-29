using System;
using System.Threading;
using Relativity.Transfer;

namespace kCura.WinEDDS.TApi
{
	/// <summary>
	/// </summary>
	public sealed class UploadTapiBridge : TapiBridge
	{
		/// <summary>
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="log"></param>
		/// <param name="token"></param>
		public UploadTapiBridge(TapiBridgeParameters parameters, ITransferLog log, CancellationToken token) : base(parameters, TransferDirection.Upload, log, token)
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
			return new TransferPath
			{
				SourcePath = sourceFile,
				TargetPath =
					parameters.SortIntoVolumes
						? pathManager.GetNextTargetPath(TargetPath)
						: TargetPath,
				TargetFileName = targetFileName,
				Order = order
			};
		}
	}
}