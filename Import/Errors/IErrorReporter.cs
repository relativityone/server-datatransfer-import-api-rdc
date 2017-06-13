using System;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public interface IErrorReporter
	{
		event EventHandler<LineError> ErrorOccurred;
	}
}