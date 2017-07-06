
using System;

namespace kCura.WinEDDS.Core.Import
{
	public interface IImportExceptionHandlerExec
	{
		void TryCatchExecNonFatal(Action executeAction, Action finalizeAction = null);

		void TryCatchExec(Action executeAction, Action finalizeAction = null);

		T TryCatchExec<T>(Func<T> executeAction, T defaultRetValue = default(T), Action finalizeAction = null,
			bool rethrowFatal = false);

		void IgnoreOnExceptionExec<TException>(Action action, Func<bool> condition = null);
	}
}
