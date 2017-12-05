using System;
using System.Threading;


namespace kCura.WinEDDS.TApi
{
	/// <summary>
	/// Robust IO reporter
	/// </summary>
	public interface IIoReporter
	{
		/// <summary>
		/// Get file length
		/// </summary>
		long GetFileLength(string fileName, int lineNumberInParentFile);


		/// <summary>
		/// Property to expose IoWarningPublisher
		/// </summary>
		IoWarningPublisher IOWarningPublisher { get; }

    }
}
